USE MASTER
GO
ALTER DATABASE KTPOS SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO
DROP DATABASE IF EXISTS KTPOS
GO
CREATE DATABASE KTPOS
GO
USE KTPOS
GO
-- Table for customer management
CREATE TABLE CUSTOMER (
	ID         INT IDENTITY PRIMARY KEY,
	FULLNAME   NVARCHAR(50) NOT NULL,
	PHONE      NVARCHAR(11) UNIQUE NOT NULL,
	GENDER     NVARCHAR(10) CHECK(GENDER IN ('Male', 'Female', 'Other')) NOT NULL,
	HOMETOWN   NVARCHAR(50) 
);
GO
CREATE TABLE ACCOUNT (
	ID		   INT IDENTITY PRIMARY KEY,
	IDSTAFF    NVARCHAR(10) UNIQUE NOT NULL,
	FULLNAME   NVARCHAR(50) NOT NULL,
	EMAIL      NVARCHAR(50),
	PHONE      NVARCHAR(11) NOT NULL,
	DOB        DATE NOT NULL CHECK (DOB <= GETDATE()),
	[PASSWORD] NVARCHAR(50) NOT NULL CHECK(LEN([PASSWORD]) >= 6) DEFAULT 'ktpos123',
	[ROLE]     NVARCHAR(20) CHECK([ROLE] IN ('Staff', 'Manager')),
	STATUS	   INT NOT NULL DEFAULT 1
);
GO
-- Trigger for auto-generating IDSTAFF
CREATE TRIGGER trg_InsertIDStaff
ON ACCOUNT
INSTEAD OF INSERT
AS
BEGIN
	DECLARE @NewID INT;
	DECLARE @NewIDStaff NVARCHAR(10);

	SELECT @NewID = ISNULL(MAX(CAST(SUBSTRING(IDSTAFF, 3, LEN(IDSTAFF)) AS INT)), 0) + 1
	FROM ACCOUNT;

	SET @NewIDStaff = 'KT' + RIGHT('000' + CAST(@NewID AS NVARCHAR(3)), 3);

	INSERT INTO ACCOUNT (IDSTAFF, FULLNAME, EMAIL, PHONE, DOB, [PASSWORD], [ROLE], STATUS)
	SELECT @NewIDStaff, FULLNAME, EMAIL, PHONE, DOB, [PASSWORD], [ROLE], STATUS
	FROM INSERTED;
END;
GO

-- Table for table management
CREATE TABLE [TABLE] (
	ID         INT IDENTITY PRIMARY KEY,
	FNAME      NVARCHAR(50) UNIQUE,
	STATUS     INT NOT NULL DEFAULT 1,
	CAPACITY   TINYINT NOT NULL DEFAULT 4,
	VISIBLE    INT NOT NULL DEFAULT 1
);
GO
-- Table for promotions management
CREATE TABLE PROMOTION (
    ID          INT IDENTITY PRIMARY KEY,
    FNAME        NVARCHAR(100) NOT NULL,
    [DESCRIPTION] NVARCHAR(255),
    DISCOUNT    FLOAT CHECK(DISCOUNT BETWEEN 0 AND 100) NOT NULL,
    [START_DATE]DATE NOT NULL,
    END_DATE    DATE NOT NULL,  -- Fix for CHECK constraint
    STATUS      INT NOT NULL DEFAULT 1,
    APPLY_TO    NVARCHAR(20) CHECK(APPLY_TO IN ('Item', 'Bill')) NOT NULL,
	CONSTRAINT CHK_END_DATE_AFTER_START_DATE CHECK (END_DATE > [START_DATE])
);
GO
-- Table for item management
CREATE TABLE ITEM (
	ID          INT IDENTITY PRIMARY KEY,
	FNAME       NVARCHAR(50) NOT NULL,
	CATEGORY	NVARCHAR(10) CHECK(CATEGORY IN ('Food', 'Drink')) NOT NULL,
	PRICE       FLOAT NOT NULL CHECK(PRICE > 0),
	VISIBLE     INT NOT NULL DEFAULT 1,
);
GO

-- Table for bill management
CREATE TABLE BILL (
	ID          INT IDENTITY PRIMARY KEY,
	IDTABLE     INT NULL,
	IDSTAFF     NVARCHAR(10) NOT NULL,
	IDCUSTOMER  INT NULL,
	BILLTYPE    BIT NOT NULL DEFAULT 1,
	CHKIN_TIME  DATETIME NOT NULL DEFAULT GETDATE(),
	CHKOUT_TIME DATETIME NULL, --= Payment time
	DURATION    AS DATEDIFF(MINUTE, CHKIN_TIME, CHKOUT_TIME),
	TOTAL		DECIMAL(10,0) NULL,
	STATUS      INT NOT NULL DEFAULT 0,
	CONSTRAINT CK_BILL_BILLTYPE_IDTABLE CHECK (BILLTYPE = 1 OR (BILLTYPE = 0 AND IDTABLE IS NULL)),
	CONSTRAINT FK_BILL_TABLE FOREIGN KEY (IDTABLE) REFERENCES [TABLE](ID),
	CONSTRAINT FK_BILL_ACCOUNT FOREIGN KEY (IDSTAFF) REFERENCES ACCOUNT(IDSTAFF),
	CONSTRAINT FK_BILL_CUSTOMER FOREIGN KEY (IDCUSTOMER) REFERENCES CUSTOMER(ID)
);
GO
-- Table for bill details
CREATE TABLE BILLINF (
	ID      INT IDENTITY PRIMARY KEY,
	IDBILL  INT NOT NULL,
	IDFD    INT NOT NULL,
	COUNT   INT NOT NULL DEFAULT 1,
	CONSTRAINT FK_BILLINF_BILL FOREIGN KEY (IDBILL) REFERENCES BILL(ID),
	CONSTRAINT FK_BILLINF_ITEM FOREIGN KEY (IDFD) REFERENCES ITEM(ID)
);
GO
-- Table for tag management
CREATE TABLE TAG (
	ID          INT IDENTITY PRIMARY KEY,
	TAGNAME     NVARCHAR(50) UNIQUE CHECK(TAGNAME IN ('Best Seller', 'New Arrival', 'Regular', 'Seasonal', 'Limited Edition')) NOT NULL
);
GO
-- Table for item-tag relationship
CREATE TABLE ITEM_TAG (
	ID          INT IDENTITY PRIMARY KEY,
	IDITEM      INT NOT NULL,
	IDTAG       INT NOT NULL,
	CONSTRAINT FK_ITEMTAG_ITEM FOREIGN KEY (IDITEM) REFERENCES ITEM(ID),
	CONSTRAINT FK_ITEMTAG_TAG FOREIGN KEY (IDTAG) REFERENCES TAG(ID),
	CONSTRAINT UQ_ITEMTAG UNIQUE (IDITEM, IDTAG)
);
GO
-- Insert sample tags
INSERT INTO TAG (TAGNAME)
VALUES ('Best Seller'), ('New Arrival'), ('Regular'), ('Seasonal'), ('Limited Edition');

GO
CREATE TABLE ITEM_PROMOTION (
	ID          INT IDENTITY PRIMARY KEY,
	IDPROMOTION INT NOT NULL,
	IDITEM      INT NOT NULL,
	CONSTRAINT FK_PROMOTIONITEM_PROMOTION FOREIGN KEY (IDPROMOTION) REFERENCES PROMOTION(ID),
	CONSTRAINT FK_PROMOTIONITEM_ITEM FOREIGN KEY (IDITEM) REFERENCES ITEM(ID),
	CONSTRAINT UQ_PROMOTIONITEM UNIQUE (IDPROMOTION, IDITEM),
);
GO
-- Link promotions to bills
CREATE TABLE BILL_PROMOTION (
	ID          INT IDENTITY PRIMARY KEY,
	IDBILL      INT NOT NULL,
	IDPROMOTION INT NOT NULL,
	CONSTRAINT FK_BILLPROMOTION_BILL FOREIGN KEY (IDBILL) REFERENCES BILL(ID),
	CONSTRAINT FK_BILLPROMOTION_PROMOTION FOREIGN KEY (IDPROMOTION) REFERENCES PROMOTION(ID),
	CONSTRAINT UQ_BILLPROMOTION UNIQUE (IDBILL, IDPROMOTION),
);
GO --TRIGGER TỰ ĐỘNG THÊM BILL ID VÀO BILL_PROMOTION NẾU NGÀY TẠO BILL TRÙNG KHỚP VỚI NGÀY GIẢM GIÁ THEO BILL
CREATE OR ALTER TRIGGER trg_ApplyPromotion
ON BILL
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Áp dụng khuyến mãi theo hóa đơn dựa vào ngày của bill
    INSERT INTO BILL_PROMOTION (IDBILL, IDPROMOTION)
    SELECT i.ID, p.ID
    FROM inserted i
    JOIN PROMOTION p ON p.APPLY_TO = 'Bill'
    WHERE p.STATUS = 1
    AND i.CHKIN_TIME BETWEEN p.[START_DATE] AND p.END_DATE
    AND NOT EXISTS (
        SELECT 1 
        FROM BILL_PROMOTION bp
        WHERE bp.IDBILL = i.ID 
        AND bp.IDPROMOTION = p.ID
    );
END;
GO --TRIGGER TỰ ĐỘNG TÍNH TOTAL AMOUNT (BAO GỒM KHUYẾN MÃI HOẶC KHÔNG)
CREATE OR ALTER TRIGGER TR_Calculate_Bill_Total
ON BILLINF
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Tạo bảng tạm chứa các Bill ID cần cập nhật
    DECLARE @AffectedBills TABLE (BillID int);
    
    -- Thu thập Bill ID từ các thao tác INSERT, UPDATE, DELETE
    INSERT INTO @AffectedBills (BillID)
    SELECT IDBILL FROM inserted
    UNION
    SELECT IDBILL FROM deleted;
    
    -- Cập nhật tổng tiền cho các hóa đơn bị ảnh hưởng
    UPDATE b
    SET b.TOTAL = (
        -- Tính tổng tiền sau khi trừ cả hai loại giảm giá
        SELECT CAST(
            -- Tổng tiền gốc
            (SELECT SUM(bi.COUNT * i.PRICE)
             FROM BILLINF bi
             JOIN ITEM i ON bi.IDFD = i.ID
             WHERE bi.IDBILL = b.ID) -
            
            -- Trừ tiền giảm giá món
            CASE 
                WHEN EXISTS (
                    SELECT 1 
                    FROM BILLINF bi2
                    JOIN ITEM_PROMOTION ip ON bi2.IDFD = ip.IDITEM
                    JOIN PROMOTION p ON ip.IDPROMOTION = p.ID
                    WHERE bi2.IDBILL = b.ID 
                    --AND (b.CHKOUT_TIME IS NULL OR CAST(b.CHKOUT_TIME AS DATE) BETWEEN p.[START_DATE] AND p.END_DATE)
                    AND (CAST(b.CHKIN_TIME AS DATE) BETWEEN p.[START_DATE] AND p.END_DATE)
                ) 
                THEN 
                    (SELECT SUM(bi.COUNT * i.PRICE)
                     FROM BILLINF bi
                     JOIN ITEM i ON bi.IDFD = i.ID
                     WHERE bi.IDBILL = b.ID) *
                    (SELECT TOP 1 p.DISCOUNT/100.0
                     FROM PROMOTION p 
                     JOIN ITEM_PROMOTION ip ON p.ID = ip.IDPROMOTION
                     JOIN BILLINF bi ON ip.IDITEM = bi.IDFD
                     WHERE bi.IDBILL = b.ID
                   --  AND (b.CHKOUT_TIME IS NULL OR CAST(b.CHKOUT_TIME AS DATE) BETWEEN p.[START_DATE] AND p.END_DATE)
                     AND (CAST(b.CHKIN_TIME AS DATE) BETWEEN p.[START_DATE] AND p.END_DATE)
                     ORDER BY p.DISCOUNT DESC)
                ELSE 0
            END -
            
            -- Trừ tiền giảm giá hóa đơn
            CASE 
                WHEN EXISTS (
                    SELECT 1 
                    FROM BILL_PROMOTION bp
                    JOIN PROMOTION p ON bp.IDPROMOTION = p.ID
                    WHERE bp.IDBILL = b.ID
                  --  AND (b.CHKOUT_TIME IS NULL OR CAST(b.CHKOUT_TIME AS DATE) BETWEEN p.[START_DATE] AND p.END_DATE)
                    AND (CAST(b.CHKIN_TIME AS DATE) BETWEEN p.[START_DATE] AND p.END_DATE)
                ) 
                THEN 
                    (SELECT SUM(bi.COUNT * i.PRICE)
                     FROM BILLINF bi
                     JOIN ITEM i ON bi.IDFD = i.ID
                     WHERE bi.IDBILL = b.ID) *
                    (SELECT TOP 1 p.DISCOUNT/100.0
                     FROM PROMOTION p 
                     JOIN BILL_PROMOTION bp ON p.ID = bp.IDPROMOTION
                     WHERE bp.IDBILL = b.ID
                   --  AND (b.CHKOUT_TIME IS NULL OR CAST(b.CHKOUT_TIME AS DATE) BETWEEN p.[START_DATE] AND p.END_DATE)
                     AND (CAST(b.CHKIN_TIME AS DATE) BETWEEN p.[START_DATE] AND p.END_DATE)
                     ORDER BY p.DISCOUNT DESC)
                ELSE 0
            END
        AS decimal(10,0))
    )
    FROM BILL b
    JOIN @AffectedBills ab ON b.ID = ab.BillID;
END;
GO
CREATE TRIGGER trg_UpdateTableStatus
ON BILL
AFTER UPDATE
AS
BEGIN
    -- Khi BILL.STATUS = 1, đảm bảo TABLE.STATUS = 1
    IF EXISTS (
        SELECT 1
        FROM INSERTED i
        JOIN [TABLE] t ON i.IDTABLE = t.ID
        WHERE i.STATUS = 1 AND t.STATUS <> 1
    )
    BEGIN
        UPDATE [TABLE]
        SET STATUS = 1
        FROM [TABLE] t
        JOIN INSERTED i ON t.ID = i.IDTABLE
        WHERE i.STATUS = 1;
    END
    -- Khi BILL.STATUS = 0, đảm bảo TABLE.STATUS = 0
    IF EXISTS (
        SELECT 1
        FROM INSERTED i
        JOIN [TABLE] t ON i.IDTABLE = t.ID
        WHERE i.STATUS = 0 AND t.STATUS <> 0
    )
    BEGIN
        UPDATE [TABLE]
        SET STATUS = 0
        FROM [TABLE] t
        JOIN INSERTED i ON t.ID = i.IDTABLE
        WHERE i.STATUS = 0;
    END
END;
GO
INSERT INTO ACCOUNT (FULLNAME, EMAIL, PHONE, DOB, [PASSWORD], [ROLE], STATUS) VALUES 
    (N'Võ Đăng Khoa',			'khoavd2809@gmail.com',	'0843019548', '2004-09-28', 'khoavo123',		'Manager', 1)
INSERT INTO ACCOUNT (FULLNAME, EMAIL, PHONE, DOB, [PASSWORD], [ROLE], STATUS) VALUES 
	(N'Dương Thị Thanh Thảo',	'thaott26@gmail.com',	'0902234567', '2003-06-26', 'pupu123',			'Manager', 1)
INSERT INTO ACCOUNT (FULLNAME, EMAIL, PHONE, DOB, [PASSWORD], [ROLE], STATUS) VALUES 
	(N'Hoàng Văn Thiên',		'hvt2003@gmail.com',	'0903234567', '2003-05-15', 'chillguy1',		'Staff', 1)
INSERT INTO ACCOUNT (FULLNAME, EMAIL, PHONE, DOB, [PASSWORD], [ROLE], STATUS) VALUES 
    (N'Lê Thiện Nhân',			'nhanle@gmail.com',		'0904234567', '2001-12-12', 'cuchuoi2xu',		'Staff', 1)
INSERT INTO ACCOUNT (FULLNAME, EMAIL, PHONE, DOB, [PASSWORD], [ROLE], STATUS) VALUES 
    (N'Từ Tuấn Sang',			'tsang@gmail.com',		'0905234567', '2005-03-15', 'tsang123',			'Staff', 1)
INSERT INTO ACCOUNT (FULLNAME, EMAIL, PHONE, DOB, [PASSWORD], [ROLE], STATUS) VALUES 
    (N'Nguyễn Thành Đạt',		'dathphong@gmail.com',	'0906234567', '2007-11-20', 'hoangtusitinh',	'Staff', 1)
INSERT INTO ACCOUNT (FULLNAME, EMAIL, PHONE, DOB, [PASSWORD], [ROLE], STATUS) VALUES 
    (N'Nguyễn Giang Gia Huy',	'huybo@gmail.com',		'0907234567', '2006-08-05', 'huybo123',			'Staff', 1)
GO
INSERT INTO CUSTOMER (FULLNAME, PHONE, GENDER, HOMETOWN)
VALUES 
('Nguyen Van F', '0956789012', 'Male', N'Hà Nội'),
('Le Thi G', '0967890123', 'Female', N'Đà Nẵng'),
('Tran Van H', '0978901234', 'Male', N'TP Hồ Chí Minh'),
('Pham Thi I', '0989012345', 'Female', N'Huế'),
('Hoang Van J', '0990123456', 'Male', N'Cần Thơ'),
('Nguyen Thi K', '0911234567', 'Female', N'Hà Nội'),
('Tran Van L', '0922345678', 'Male', N'Bắc Ninh'),
('Pham Thi M', '0933456789', 'Female', N'Hải Phòng'),
('Hoang Van N', '0944567890', 'Male', N'Vũng Tàu'),
('Le Thi O', '0955678901', 'Female', N'Quảng Nam');
INSERT INTO PROMOTION (FNAME, [DESCRIPTION], DISCOUNT, [START_DATE], END_DATE, STATUS, APPLY_TO)
VALUES 
('New Year Discount', 'Discount for all items on New Year', 20, '2024-11-01', '2025-12-31', 1, 'Item'),
('Merry Christmas', '10% off on all bills during holidays', 10, '2024-11-01', '2025-12-31', 1, 'Bill'),
('Seasonal Fruits', 'Special price for seasonal drinks', 15, '2024-11-01', '2025-12-31', 1, 'Item');
INSERT INTO [TABLE] (FNAME, STATUS, CAPACITY, VISIBLE) 
VALUES
('Table 1', 1, 4, 1),
('Table 2', 1, 4, 1),
('Table 3', 0, 4, 1),
('Table 4', 1, 4, 1),
('Table 5', 1, 6, 1),
('Table 6', 1, 4, 1),
('Table 7', 0, 4, 1),
('Table 8', 1, 2, 1),
('Table 9', 1, 4, 1),
('Table VIP', 1, 10, 1);
INSERT INTO BILL (IDTABLE, IDSTAFF, CHKIN_TIME, CHKOUT_TIME, STATUS, BILLTYPE, IDCUSTOMER) 
VALUES 
(1, 'KT001', '2024-12-31 09:00', '2024-12-31 10:00', 1, 1, 1),
(3, 'KT003', '2024-12-20 11:30', NULL, 0, 1, 3),
(NULL, 'KT004', '2024-12-31 12:45', '2024-12-31 13:45', 1, 0, 4),
(5, 'KT001', '2024-12-24 14:00', '2024-12-24 16:00', 1, 1, 5),
(NULL, 'KT002', '2024-11-01 15:15', '2024-11-01 16:15', 1, 0, 6),
(7, 'KT003', '2024-12-31 16:30', '2024-12-31 17:30', 1, 1, 7),
(NULL, 'KT004', '2024-12-24 17:45', NULL, 0, 0, 8),
(9, 'KT001', '2024-12-20 19:00', '2024-12-20 20:00', 1, 1, 9),
(NULL, 'KT002', '2024-12-31 20:15', '2024-12-31 21:15', 1, 0, 10);
INSERT INTO ITEM (FNAME, CATEGORY, PRICE, VISIBLE) 
VALUES 
('Spring Rolls', 'Food', 35000, 1),
('Beef Pho', 'Food', 60000, 1),
('Flan Cake', 'Food', 20000, 1),
('Iced Coffee', 'Drink', 25000, 1),
('Beer', 'Drink', 30000, 1),
('Fried Rice', 'Food', 50000, 1),
('Salad', 'Food', 45000, 1),
('Baguette', 'Food', 15000, 1),
('Orange Juice', 'Drink', 20000, 1),
('Watermelon Smoothie', 'Drink', 25000, 1);

INSERT INTO ITEM_PROMOTION (IDPROMOTION, IDITEM)
VALUES
-- Promotion 1: "New Year Discount" applies to all items
(1, 1), -- Spring Rolls
(1, 2), -- Beef Pho
(1, 3), -- Flan Cake
(1, 4), -- Iced Coffee
(1, 5), -- Beer
(1, 6), -- Fried Rice

-- Promotion 3: "Seasonal Fruits" applies to seasonal drinks
(3, 7), -- Orange Juice
(3, 10); -- Watermelon Smoothie

INSERT INTO ITEM_TAG (IDITEM, IDTAG)
VALUES
-- Spring Rolls (Regular)
(1, 3),
-- Beef Pho (Best Seller, Regular)
(2, 1),
(2, 3),
-- Flan Cake (Best Seller, Seasonal)
(3, 1),
(3, 4),
-- Iced Coffee (Best Seller, Regular)
(4, 1),
(4, 3),
-- Beer (Best Seller)
(5, 1),
-- Fried Rice (New Arrival, Regular)
(6, 2),
(6, 3),
-- Salad ( Regular)
(7, 3),
-- Baguette (Regular)
(8, 3),
-- Orange Juice (Regular, Seasonal)
(9, 3),
(9, 4),
-- Watermelon Smoothie (Seasonal, Limited Edition)
(10, 4),
(10, 5);
GO 
-- Insert data into BILLINF
INSERT INTO BILLINF (IDBILL, IDFD, COUNT)
VALUES 
(1, 1, 2),
(1, 2, 1),
(2, 3, 3),
(3, 4, 1),
(4, 5, 2),
(5, 6, 1),
(6, 7, 3),
(7, 8, 2),
(8, 9, 4),
(9, 10, 1),
(10, 1, 2),  -- 2 phần Spring Rolls
(10, 4, 1),  -- 1 phần Iced Coffee
(10, 7, 1);
---------------------------------
SELECT 
    B.ID,
    A.FULLNAME AS CashierName,
    B.TOTAL AS TotalAmount,
    B.IDTABLE AS TableID,
    B.CHKIN_TIME AS CheckInTime,
    B.CHKOUT_TIME AS CheckOutTime,
    C.FULLNAME AS CustomerName
FROM BILL B
LEFT JOIN ACCOUNT A ON B.IDSTAFF = A.IDSTAFF
LEFT JOIN CUSTOMER C ON B.IDCUSTOMER = C.ID;
-----------------------------------------------------------------------------------
WITH BillItems AS (
    SELECT 
        b.ID AS BillID,
        SUM(bi.COUNT * i.PRICE) AS SubTotal
    FROM BILL b
    JOIN BILLINF bi ON b.ID = bi.IDBILL
    JOIN ITEM i ON bi.IDFD = i.ID
    GROUP BY b.ID
),
ItemDiscounts AS (
    SELECT DISTINCT
        b.ID AS BillID,
        'Item Discount' as DiscountType
    FROM BILL b
    JOIN BILLINF bi ON b.ID = bi.IDBILL
    JOIN ITEM i ON bi.IDFD = i.ID
    JOIN ITEM_PROMOTION ip ON i.ID = ip.IDITEM
    JOIN PROMOTION p ON ip.IDPROMOTION = p.ID
    WHERE ( b.CHKOUT_TIME IS NULL OR CAST(b.CHKOUT_TIME AS DATE) BETWEEN p.[START_DATE] AND p.END_DATE)
    AND (CAST(b.CHKIN_TIME AS DATE) BETWEEN p.[START_DATE] AND p.END_DATE)
),
BillDiscounts AS (
    SELECT DISTINCT
        b.ID AS BillID,
        'Bill Discount' as DiscountType
    FROM BILL b
    JOIN BILL_PROMOTION bp ON b.ID = bp.IDBILL
    JOIN PROMOTION p ON bp.IDPROMOTION = p.ID
    WHERE ( b.CHKOUT_TIME IS NULL OR CAST(b.CHKOUT_TIME AS DATE) BETWEEN p.[START_DATE] AND p.END_DATE)
    AND (CAST(b.CHKIN_TIME AS DATE) BETWEEN p.[START_DATE] AND p.END_DATE)
),
CombinedDiscounts AS (
    SELECT BillID, DiscountType FROM ItemDiscounts
    UNION
    SELECT BillID, DiscountType FROM BillDiscounts
),
FinalDiscounts AS (
    SELECT 
        BillID,
        CASE 
            WHEN COUNT(*) > 1 THEN N'Cả hai'
            ELSE MAX(DiscountType)
        END AS DiscountType
    FROM CombinedDiscounts
    GROUP BY BillID
)
SELECT 
    a.FULLNAME AS N'Thu ngân',
    b.ID AS N'Mã hóa đơn',
    FORMAT(b.CHKIN_TIME, 'dd/MM/yyyy HH:mm') AS N'Thời gian tạo',
    FORMAT(b.CHKOUT_TIME, 'dd/MM/yyyy HH:mm') AS N'Thời gian thanh toán',
    CASE 
        WHEN b.STATUS = 1 THEN N'Đã thanh toán'
        ELSE N'Chưa thanh toán'
    END AS N'Trạng thái',
    c.FULLNAME AS N'Tên khách hàng',
    CASE
        WHEN fd.DiscountType = 'Item Discount' THEN N'Giảm giá món'
        WHEN fd.DiscountType = 'Bill Discount' THEN N'Giảm giá hóa đơn'
        WHEN fd.DiscountType = N'Cả hai' THEN N'Giảm giá món & hóa đơn'
        ELSE N'Không giảm giá'
    END AS N'Loại giảm giá',
    CAST(bi.SubTotal AS decimal(10,0)) AS N'Tổng tiền gốc',
    -- Tính tiền giảm giá món
    CAST(
        CASE 
            WHEN EXISTS (
                SELECT 1 FROM ItemDiscounts id WHERE id.BillID = b.ID
            ) THEN bi.SubTotal * (
                SELECT TOP 1 p.DISCOUNT/100.0
                FROM PROMOTION p 
                JOIN ITEM_PROMOTION ip ON p.ID = ip.IDPROMOTION
                JOIN BILLINF bif ON ip.IDITEM = bif.IDFD
                WHERE bif.IDBILL = b.ID AND (b.CHKOUT_TIME IS NULL OR CAST(b.CHKOUT_TIME AS DATE) BETWEEN p.[START_DATE] AND p.END_DATE) AND (CAST(b.CHKIN_TIME AS DATE) BETWEEN p.[START_DATE] AND p.END_DATE)
                ORDER BY p.DISCOUNT DESC
            )
            ELSE 0
        END 
    AS decimal(10,0)) AS N'Tiền giảm giá món',
    -- Tính tiền giảm giá hóa đơn
    CAST(
        CASE 
            WHEN EXISTS (
                SELECT 1 FROM BillDiscounts bd WHERE bd.BillID = b.ID
            ) THEN bi.SubTotal * (
                SELECT TOP 1 p.DISCOUNT/100.0
                FROM PROMOTION p 
                JOIN BILL_PROMOTION bp ON p.ID = bp.IDPROMOTION
                WHERE bp.IDBILL = b.ID
                AND (b.CHKOUT_TIME IS NULL OR CAST(b.CHKOUT_TIME AS DATE) BETWEEN p.[START_DATE] AND p.END_DATE)
                AND (CAST(b.CHKIN_TIME AS DATE) BETWEEN p.[START_DATE] AND p.END_DATE)
                ORDER BY p.DISCOUNT DESC
            )
            ELSE 0
        END 
    AS decimal(10,0)) AS N'Tiền giảm giá hóa đơn',
    -- Tính tổng tiền cuối
    CAST(
        bi.SubTotal - 
        CASE 
            WHEN EXISTS (
                SELECT 1 FROM ItemDiscounts id WHERE id.BillID = b.ID
            ) THEN bi.SubTotal * (
                SELECT TOP 1 p.DISCOUNT/100.0
                FROM PROMOTION p 
                JOIN ITEM_PROMOTION ip ON p.ID = ip.IDPROMOTION
                JOIN BILLINF bif ON ip.IDITEM = bif.IDFD
                WHERE bif.IDBILL = b.ID
                AND (b.CHKOUT_TIME IS NULL OR CAST(b.CHKOUT_TIME AS DATE) BETWEEN p.[START_DATE] AND p.END_DATE)
                AND (CAST(b.CHKIN_TIME AS DATE) BETWEEN p.[START_DATE] AND p.END_DATE)
                ORDER BY p.DISCOUNT DESC
            )
            ELSE 0
        END -
        CASE 
            WHEN EXISTS (
                SELECT 1 FROM BillDiscounts bd WHERE bd.BillID = b.ID
            ) THEN bi.SubTotal * (
                SELECT TOP 1 p.DISCOUNT/100.0
                FROM PROMOTION p 
                JOIN BILL_PROMOTION bp ON p.ID = bp.IDPROMOTION
                WHERE bp.IDBILL = b.ID
                AND (b.CHKOUT_TIME IS NULL OR CAST(b.CHKOUT_TIME AS DATE) BETWEEN p.[START_DATE] AND p.END_DATE)
                AND (CAST(b.CHKIN_TIME AS DATE) BETWEEN p.[START_DATE] AND p.END_DATE)
                ORDER BY p.DISCOUNT DESC
            )
            ELSE 0
        END 
    AS decimal(10,0)) AS N'Tổng tiền sau giảm giá',
    CASE 
        WHEN b.BILLTYPE = 1 THEN N'Tại quán'
        ELSE	N'Mang về'
    END AS N'Loại hóa đơn',
    t.FNAME AS N'Bàn'
FROM BILL b
JOIN ACCOUNT a ON b.IDSTAFF = a.IDSTAFF
LEFT JOIN CUSTOMER c ON b.IDCUSTOMER = c.ID
LEFT JOIN [TABLE] t ON b.IDTABLE = t.ID
JOIN BillItems bi ON b.ID = bi.BillID
LEFT JOIN FinalDiscounts fd ON b.ID = fd.BillID
ORDER BY b.ID ASC;
-------------------------------
GO
CREATE PROCEDURE sp_CalculateBillDetails
    @IDBILL INT
AS
BEGIN
    -- Calculate item details with discounts
    WITH ItemDiscounts AS (
        SELECT 
            ip.IDITEM,
            MAX(p.DISCOUNT) as MaxDiscount
        FROM ITEM_PROMOTION ip
        JOIN PROMOTION p ON ip.IDPROMOTION = p.ID
        JOIN BILL b ON b.ID = @IDBILL
        WHERE (b.CHKOUT_TIME IS NULL OR CAST(b.CHKOUT_TIME AS DATE) BETWEEN p.[START_DATE] AND p.END_DATE)
        AND (CAST(b.CHKIN_TIME AS DATE) BETWEEN p.[START_DATE] AND p.END_DATE)
        GROUP BY ip.IDITEM
    ),
    BillTotals AS (
        SELECT 
            @IDBILL AS BillID,
            SUM(bi.COUNT * i.PRICE) AS SubTotal,
            SUM(bi.COUNT * (i.PRICE * ISNULL(id.MaxDiscount/100.0, 0))) AS ItemDiscount,
            (
                SELECT TOP 1 p.DISCOUNT
                FROM BILL_PROMOTION bp
                JOIN PROMOTION p ON bp.IDPROMOTION = p.ID
                WHERE bp.IDBILL = @IDBILL
                AND EXISTS (
                    SELECT 1 FROM BILL b 
                    WHERE b.ID = @IDBILL 
                    AND (b.CHKOUT_TIME IS NULL OR CAST(b.CHKOUT_TIME AS DATE) BETWEEN p.[START_DATE] AND p.END_DATE)
                    AND (CAST(b.CHKIN_TIME AS DATE) BETWEEN p.[START_DATE] AND p.END_DATE)
                )
                ORDER BY p.DISCOUNT DESC
            ) AS BillDiscountPercentage
        FROM BILLINF bi
        JOIN ITEM i ON bi.IDFD = i.ID
        LEFT JOIN ItemDiscounts id ON i.ID = id.IDITEM
        WHERE bi.IDBILL = @IDBILL
        GROUP BY bi.IDBILL
    )
    SELECT 
        i.FNAME AS mathang,
        CAST(i.PRICE AS decimal(10,0)) AS gia,
        CAST(ISNULL(i.PRICE * id.MaxDiscount/100.0, 0) AS decimal(10,0)) AS giammon,
        bi.COUNT AS sl,
        CAST(bi.COUNT * (i.PRICE - ISNULL(i.PRICE * id.MaxDiscount/100.0, 0)) AS decimal(10,0)) AS sotien,
        CAST(bt.SubTotal AS decimal(10,0)) AS subtotal,
        CAST(bt.ItemDiscount + (ISNULL(bt.BillDiscountPercentage, 0)/100.0 * (bt.SubTotal - bt.ItemDiscount)) AS decimal(10,0)) AS totaldiscount,
        CAST(bt.SubTotal - (bt.ItemDiscount + (ISNULL(bt.BillDiscountPercentage, 0)/100.0 * (bt.SubTotal - bt.ItemDiscount))) AS decimal(10,0)) AS total
    FROM BILLINF bi
    JOIN ITEM i ON bi.IDFD = i.ID
    LEFT JOIN ItemDiscounts id ON i.ID = id.IDITEM
    CROSS JOIN BillTotals bt
    WHERE bi.IDBILL = @IDBILL;
END;
go
DECLARE @BillID INT = 1; -- Thay đổi ID bill cần test

-- Check dữ liệu bill
SELECT b.*, t.FNAME as TableName, a.FULLNAME as StaffName
FROM BILL b
LEFT JOIN [TABLE] t ON b.IDTABLE = t.ID 
LEFT JOIN ACCOUNT a ON b.IDSTAFF = a.IDSTAFF
WHERE b.ID = @BillID;

-- Check chi tiết bill
EXEC sp_CalculateBillDetails @IDBILL = @BillID;

-- Check giảm giá áp dụng
SELECT p.*
FROM PROMOTION p
JOIN ITEM_PROMOTION ip ON p.ID = ip.IDPROMOTION
JOIN BILLINF bi ON ip.IDITEM = bi.IDFD
WHERE bi.IDBILL = @BillID;

SELECT p.*
FROM PROMOTION p
JOIN BILL_PROMOTION bp ON p.ID = bp.IDPROMOTION 
WHERE bp.IDBILL = @BillID;

--------------------------------------------------------------------------
SELECT 
    i.FNAME AS ITEM_NAME,
    i.CATEGORY,
    i.PRICE,
    ISNULL(SUM(bi.COUNT), 0) AS QTY,  -- Tổng số lượng đã bán
    STRING_AGG(t.TAGNAME, ', ') AS TAG  -- Nối tên các tag liên quan
FROM 
    ITEM i
LEFT JOIN 
    BILLINF bi ON i.ID = bi.IDFD
LEFT JOIN 
    ITEM_TAG it ON i.ID = it.IDITEM
LEFT JOIN 
    TAG t ON it.IDTAG = t.ID
GROUP BY 
    i.FNAME, i.CATEGORY, i.PRICE
ORDER BY 
    i.FNAME;
-----
SELECT I.FNAME AS [ITEM NAME], I.CATEGORY,  I.PRICE, ISNULL(SUM(bi.COUNT), 0) AS QTY, MAX(CASE WHEN T.TAGNAME = 'Regular' THEN 1 ELSE 0 END) AS TAG
FROM ITEM I LEFT JOIN BILLINF bi ON i.ID = bi.IDFD LEFT JOIN ITEM_TAG IT ON I.ID = IT.IDITEM LEFT JOIN TAG T ON IT.IDTAG = T.ID GROUP BY I.FNAME, I.CATEGORY,I.PRICE ORDER BY I.FNAME; 

---------------

SELECT IDSTAFF FROM ACCOUNT where EMAIL = 'hvt2003@gmail.com'

DECLARE @CheckTime DATETIME = '2024-12-31 10:00:00';

SELECT *
FROM PROMOTION
WHERE @CheckTime BETWEEN [START_DATE] AND END_DATE;

SELECT 
    IP.IDITEM AS ItemID,
    P.FNAME AS PromotionName,
    P.DISCOUNT AS Discount,
    P.[START_DATE],
    P.END_DATE
FROM 
    ITEM_PROMOTION IP
INNER JOIN 
    PROMOTION P ON IP.IDPROMOTION = P.ID
WHERE 
    GETDATE() BETWEEN P.[START_DATE] AND P.END_DATE 
ORDER BY 
    P.DISCOUNT, P.FNAME, IP.IDITEM;

SELECT  
    IP.IDITEM AS ITEMID,
    P.DISCOUNT AS DISCOUNT,
    P.[START_DATE],
    P.END_DATE
FROM 
    ITEM_PROMOTION IP
INNER JOIN 
    PROMOTION P ON IP.IDPROMOTION = P.ID
WHERE 
    GETDATE() BETWEEN P.[START_DATE] AND P.END_DATE 
ORDER BY 
    P.DISCOUNT DESC, P.FNAME, IP.IDITEM;


SELECT DISCOUNT FROM PROMOTION WHERE GETDATE() BETWEEN [START_DATE] AND END_DATE AND APPLY_TO = 'Bill' ORDER BY DISCOUNT DESC;
	

SELECT DISTINCT
    I.ID AS ItemID,
    ISNULL(P.DISCOUNT, 0) AS Discount, -- Gán mức giảm giá = 0 nếu không có giảm giá
    P.FNAME AS PromotionName,
    P.[START_DATE],
    P.END_DATE
FROM 
    ITEM I -- Giả sử bảng ITEMS chứa danh sách tất cả món ăn
LEFT JOIN 
    ITEM_PROMOTION IP ON I.ID = IP.IDITEM
LEFT JOIN 
    PROMOTION P ON IP.IDPROMOTION = P.ID
ORDER BY 
    ISNULL(P.DISCOUNT, 0) DESC; -- Sắp xếp giảm dần theo Discount (NULL = 0)


SELECT  
    DISTINCT I.ID AS ID ,
    I.FNAME AS FNAME,
    I.CATEGORY AS CATEGORY,
    I.PRICE AS PRICE,
    ISNULL(P.DISCOUNT, 0) AS DISCOUNT, 
    P.FNAME AS PromotionName,
    P.[START_DATE] AS StartDate,
    P.END_DATE AS EndDate
FROM 
    ITEM I
LEFT JOIN 
    ITEM_PROMOTION IP ON I.ID = IP.IDITEM
LEFT JOIN 
    PROMOTION P ON IP.IDPROMOTION = P.ID
WHERE 
	CATEGORY = 'Drink'
ORDER BY 
    ISNULL(P.DISCOUNT, 0) DESC;

SELECT  DISTINCT 
	I.ID AS ID ,
    I.FNAME AS FNAME,
    I.CATEGORY AS CATEGORY,
    I.PRICE AS PRICE,
    ISNULL(P.DISCOUNT, 0) AS DISCOUNT
FROM 
    ITEM I
LEFT JOIN 
    ITEM_PROMOTION IP ON I.ID = IP.IDITEM
LEFT JOIN 
    PROMOTION P ON IP.IDPROMOTION = P.ID
ORDER BY 
    ISNULL(P.DISCOUNT, 0) DESC;

SELECT DISTINCT 
    I.ID AS ItemID,
    I.FNAME AS ItemName,
    I.CATEGORY AS Category,
    I.PRICE AS Price,
    ISNULL(P.DISCOUNT, 0) AS Discount, 
    P.FNAME AS PromotionName,
    P.[START_DATE] AS StartDate,
    P.END_DATE AS EndDate
FROM 
    ITEM I
LEFT JOIN 
    ITEM_PROMOTION IP ON I.ID = IP.IDITEM
LEFT JOIN 
    PROMOTION P ON IP.IDPROMOTION = P.ID
JOIN 
    ITEM_TAG IT ON I.ID = IT.IDITEM
JOIN 
    TAG T ON IT.IDTAG = T.ID AND T.TAGNAME = 'Best Seller'
ORDER BY 
    ISNULL(P.DISCOUNT, 0) DESC; 

	select * from BILL