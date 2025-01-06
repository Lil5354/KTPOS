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
	FNAME       NVARCHAR(50) UNIQUE NOT NULL,
	CATEGORY	NVARCHAR(10) CHECK(CATEGORY IN ('Food', 'Drink')) NOT NULL,
	PRICE       FLOAT NOT NULL CHECK(PRICE > 0),
	VISIBLE     INT NOT NULL DEFAULT 1,
	CONSTRAINT UQ_ITEM UNIQUE (FNAME, CATEGORY)
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
                    WHERE bi2.IDBILL = b.ID AND P.STATUS = 1
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
                     WHERE bi.IDBILL = b.ID AND P.STATUS = 1
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
                    WHERE bp.IDBILL = b.ID AND P.STATUS = 1
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
                     WHERE bp.IDBILL = b.ID AND P.STATUS = 1
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
AFTER UPDATE, INSERT
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
CREATE PROCEDURE ManageItemTag
    @OperationType INT,
    @ItemName NVARCHAR(50),
    @TagName NVARCHAR(50)
AS
BEGIN
    -- Biến lưu ID của ITEM và TAG
    DECLARE @ItemId INT, @TagId INT;

    -- Tìm ID của ITEM và TAG
    SELECT @ItemId = ID FROM ITEM WHERE FNAME = @ItemName;
    SELECT @TagId = ID FROM TAG WHERE TAGNAME = @TagName;
    -- Thực thi theo loại OperationType
    IF @OperationType = 1
    BEGIN
      INSERT INTO ITEM_TAG (IDITEM, IDTAG)
      VALUES (@ItemId, @TagId);
	END
    ELSE IF @OperationType = 2
    BEGIN
        DELETE FROM ITEM_TAG
        WHERE IDITEM = @ItemId AND IDTAG = @TagId;
    END
END;
GO
CREATE PROCEDURE ManageItemPromotion
    @OperationType INT, -- 1: Add, 2: Remove
    @ItemName NVARCHAR(255), -- Name of the ITEM
    @PromotionName NVARCHAR(255) -- Name of the PROMOTION
AS
BEGIN
    -- Declare variables to hold IDs
    DECLARE @ItemID INT, @PromotionID INT;
    -- Get the ID of the ITEM based on the name
    SELECT @ItemID = ID FROM ITEM WHERE FNAME = @ItemName;
    -- Get the ID of the PROMOTION based on the name
    SELECT @PromotionID = ID FROM PROMOTION WHERE FNAME = @PromotionName;
    -- Perform operations based on @OperationType
    IF @OperationType = 1
    BEGIN
        -- Insert into ITEM_PROMOTION
        INSERT INTO ITEM_PROMOTION (IDPROMOTION, IDITEM)
        VALUES (@PromotionID, @ItemID);
    END
    ELSE IF @OperationType = 2
    BEGIN
        DELETE FROM ITEM_PROMOTION
        WHERE IDPROMOTION = @PromotionID AND IDITEM = @ItemID;
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
INSERT INTO ACCOUNT (FULLNAME, EMAIL, PHONE, DOB, [PASSWORD], [ROLE], STATUS) VALUES 
    (N'Nguyễn Hà Vĩnh Khang', 'fabulous@gmail.com',		'0905245667', '2004-03-15', 'vkang123',			'Staff', 1)
INSERT INTO ACCOUNT (FULLNAME, EMAIL, PHONE, DOB, [PASSWORD], [ROLE], STATUS) VALUES 
    (N'Nguyễn Huy Khang',		'khangmeme@gmail.com',	'0901235467', '2004-11-20', 'ongtrummeme',		'Staff', 1)
INSERT INTO ACCOUNT (FULLNAME, EMAIL, PHONE, DOB, [PASSWORD], [ROLE], STATUS) VALUES 
    (N'Võ Quốc Nhật',	'ciyh@gmail.com','09072147867',					'2004-08-05',				'ciyh14@',			'Staff', 1)
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
('New Year Discount', 'Discount for all items on New Year', 20, '2024-12-31', '2025-01-30', 1, 'ITEM'),
('Merry Christmas', '10% off on all bills during holidays', 10, '2024-12-24', '2025-12-26', 1, 'BILL'),
('Seasonal Fruits', 'Special price for seasonal drinks', 15, '2024-11-01', '2024-12-31', 1, 'ITEM');
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
(NULL, 'KT004', '2024-12-31 12:45', '2024-12-31 13:45', 1, 0, 4),
(5, 'KT001', '2024-12-24 14:00', '2024-12-24 16:00', 1, 1, 5),
(NULL, 'KT002', '2024-11-01 15:15', '2024-11-01 16:15', 1, 0, 6),
(7, 'KT003', '2024-12-31 16:30', '2024-12-31 17:30', 1, 1, 7),
(9, 'KT001', '2024-12-20 19:00', '2024-12-20 20:00', 1, 1, 9),
(NULL, 'KT002', '2024-12-31 20:15', '2024-12-31 21:15', 1, 0, 10),
(1, 'KT001', '2023-01-10 12:00', '2023-01-10 13:30', 1, 1, NULL),
(2, 'KT002', '2023-02-15 18:45', '2023-02-15 19:50', 1, 1, 2),
(3, 'KT003', '2023-03-20 08:30', '2023-03-20 09:45', 1, 1, 3),
(4, 'KT004', '2023-04-05 14:15', '2023-04-05 15:45', 1, 1, NULL),
(5, 'KT005', '2024-05-25 19:00', '2024-05-25 20:20', 1, 1, 5),
(6, 'KT006', '2024-06-12 10:00', '2024-06-12 11:30', 1,1, NULL),
(7, 'KT001', '2024-07-18 16:00', '2024-07-18 17:15', 1, 1, 7),
(8, 'KT002', '2024-08-22 09:30', '2024-08-22 10:45', 1, 1, 8),
(9, 'KT003', '2024-09-14 13:00', '2024-09-14 14:30', 1, 1, 9),
(10, 'KT004', '2024-10-08 11:30', '2024-10-08 12:45', 1, 1, NULL),
(1, 'KT005', '2024-11-23 08:15', '2024-11-23 09:20', 1, 1, 3),
(2, 'KT006', '2024-12-31 17:00', '2024-12-31 18:30', 1, 1, 4),
(7, 'KT001', '2024-01-12 13:30', '2024-01-12 15:00', 1, 1, NULL),
(8, 'KT002', '2026-02-20 10:15', '2026-02-20 11:45', 1, 1, 5),
(9, 'KT003', '2026-03-18 17:30', '2026-03-18 19:00', 1, 1, 7),
(10, 'KT004', '2026-04-25 12:00', '2026-04-25 13:30', 1, 1, NULL),
(7, 'KT005', '2026-05-05 19:15', '2026-05-05 20:45', 1, 1, 8),
(1, 'KT001', '2021-02-14 10:30', '2021-02-14 12:00', 1, 1, 1),--25
(NULL, 'KT002', '2021-05-10 14:00', '2021-05-10 15:30', 1, 0, NULL),
(3, 'KT003', '2021-08-25 17:00', '2021-08-25 18:30', 1, 1, 2),
(NULL, 'KT004', '2021-11-30 19:45', '2021-11-30 21:15', 1, 0, 2),
(NULL, 'KT001', '2021-01-15 11:00', '2021-01-15 12:30', 1, 0, 6),
(2, 'KT002', '2021-04-10 14:00', '2021-04-10 15:30', 1, 1, 7),
(NULL, 'KT003', '2021-07-20 17:15', '2021-07-20 18:45', 1, 0, NULL),
(4, 'KT004', '2021-10-05 19:00', '2021-10-05 20:30', 1, 1, 9),
(NULL, 'KT005', '2021-12-18 12:00', '2021-12-18 13:30', 1, 0, 3),
(5, 'KT004', '2021-01-05 19:00', '2021-01-05 20:30', 1, 1, 2),
(4, 'KT004', '2021-09-05 19:00', '2021-09-05 20:30', 1, 1, 2),
(1, 'KT004', '2021-02-05 19:00', '2021-02-05 20:30', 1, 1, 9),--35
(5, 'KT001', '2022-03-05 11:15', '2022-03-05 12:45', 1, 1, 2),--36
(NULL, 'KT002', '2022-06-18 09:30', '2022-06-18 11:00', 1, 0, NULL),
(7, 'KT003', '2022-09-12 16:00', '2022-09-12 17:30', 1, 1, 4),
(NULL, 'KT004', '2022-12-20 20:30', '2022-12-20 22:00', 1, 0, NULL),
(1, 'KT006', '2022-02-22 10:00', '2022-02-22 11:30', 1, 1, NULL),
(NULL, 'KT007', '2022-05-14 13:00', '2022-05-14 14:45', 1, 0, 3),
(3, 'KT005', '2022-08-08 18:00', '2022-08-08 19:30', 1, 1, 7),
(NULL, 'KT004', '2022-11-11 20:15', '2022-11-11 21:45', 1, 0, NULL),
(5, 'KT003', '2022-12-30 15:00', '2022-12-30 16:30', 1, 1, 5);--44
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
(1, 8), -- Baguette
(1, 9), -- Orange Juice
(1, 10), -- Watermelon Smoothie

-- Promotion 3: "Seasonal Fruits" applies to seasonal drinks
(3, 9), -- Orange Juice
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
(2, 2, 1),
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
(10, 7, 1),
(11, 1, 3),
(12, 2, 1),
(13, 3, 2),
(14, 4, 4),
(15, 5, 2),
(16, 6, 3),
(17, 7, 1),
(18, 8, 2),
(19, 9, 1),
(20, 10, 3),
(21, 1, 2),
(22, 2, 4),
(23, 3, 3),
(24, 4, 1),
(21, 5, 2),
(20, 6, 1),
(25, 1, 3),
(26, 2, 2),
(27, 3, 1),
(28, 4, 4),
(29, 5, 2),
(30, 6, 3),
(31, 7, 1),
(32, 8, 5),
(26, 2, 2),
(27, 3, 1),
(28, 4, 4),
(29, 5, 2),
(33, 6, 3),
(34, 7, 1),
(35, 8, 5),
(27, 3, 1),
(28, 4, 4),
(29, 5, 2),
(36, 1, 2),
(36, 3, 3),
(37, 2, 1),
(37, 4, 4),
(38, 5, 2),
(38, 6, 1),
(39, 7, 3),
(39, 8, 2),
(40, 9, 4),
(40, 10, 1),
(41, 1, 5),
(41, 3, 2),
(42, 2, 3),
(42, 4, 2),
(43, 5, 4),
(43, 6, 3),
(40, 7, 1),
(45, 4, 2),
(44, 5, 4),
(45, 6, 3),
(44, 7, 1),
(42, 8, 5);
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
--------------------------------------------------------------------------
--------------------
SELECT 
   B.ID,
   B.CHKIN_TIME AS [DATE],
   C.FULLNAME AS [CUSTOMER],
   CASE WHEN B.BILLTYPE = 1 THEN 'Dine-In' ELSE 'Take away' END AS [TYPE],
   (SELECT SUM(bi.COUNT * i.PRICE)
    FROM BILLINF bi
    JOIN ITEM i ON bi.IDFD = i.ID 
    WHERE bi.IDBILL = B.ID) AS TOTAL,
   (SELECT SUM(bi.COUNT * i.PRICE)
    FROM BILLINF bi
    JOIN ITEM i ON bi.IDFD = i.ID 
    WHERE bi.IDBILL = B.ID) - B.TOTAL AS DISCOUNT,
   B.TOTAL AS PAYMENT,
   (SELECT SUM(TOTAL) FROM BILL) AS REVENUE
FROM BILL B
LEFT JOIN ACCOUNT A ON B.IDSTAFF = A.IDSTAFF 
LEFT JOIN CUSTOMER C ON B.IDCUSTOMER = C.ID
GROUP BY 
   B.ID, 
   B.CHKIN_TIME, 
   C.FULLNAME, 
   B.BILLTYPE, 
   B.TOTAL;

DECLARE @StartDate DATE = '2024-01-01';
DECLARE @EndDate DATE = '2025-12-31';

WITH BillDetails AS (
    SELECT 
        YEAR(B.CHKIN_TIME) AS [Year],
        SUM(bi.COUNT * i.PRICE) AS TOTAL_SALES,
        SUM(bi.COUNT * i.PRICE) - B.TOTAL AS DISCOUNT,
        B.TOTAL AS PAYMENT
    FROM BILL B
    JOIN BILLINF bi ON B.ID = bi.IDBILL
    JOIN ITEM i ON bi.IDFD = i.ID
    WHERE 
        B.CHKIN_TIME BETWEEN @StartDate AND @EndDate
    GROUP BY 
        YEAR(B.CHKIN_TIME), B.TOTAL
)
SELECT 
    [Year],
    SUM(TOTAL_SALES) AS TOTAL_SALES,
    SUM(DISCOUNT) AS TOTAL_DISCOUNT,
    SUM(PAYMENT) AS FINAL_REVENUE
FROM BillDetails
GROUP BY [Year]
ORDER BY [Year];
   --
   WITH BillDetails AS (
    SELECT 
        B.ID,
        FORMAT(B.CHKIN_TIME, 'yyyy-MM') AS [MONTH],
        SUM(bi.COUNT * i.PRICE) AS TOTAL_SALES,
        SUM(bi.COUNT * i.PRICE) - B.TOTAL AS DISCOUNT,
        B.TOTAL AS PAYMENT
    FROM BILL B
    JOIN BILLINF bi ON B.ID = bi.IDBILL
    JOIN ITEM i ON bi.IDFD = i.ID
    GROUP BY B.ID, FORMAT(B.CHKIN_TIME, 'yyyy-MM'), B.TOTAL
)
SELECT 
    [MONTH],
    SUM(TOTAL_SALES) AS TOTAL_SALES,
    SUM(DISCOUNT) AS TOTAL_DISCOUNT,
    SUM(PAYMENT) AS FINAL_REVENUE
FROM BillDetails
GROUP BY [MONTH]
ORDER BY [MONTH];
SELECT 
    B.CHKIN_TIME AS [DATE],
    C.FULLNAME AS [CUSTOMER],
    CASE WHEN B.BILLTYPE = 1 THEN 'Dine-In' ELSE 'Take away' END AS [TYPE],
    (SELECT SUM(bi.COUNT * i.PRICE)
     FROM BILLINF bi
     JOIN ITEM i ON bi.IDFD = i.ID 
     WHERE bi.IDBILL = B.ID) AS TOTAL,
    (SELECT SUM(bi.COUNT * i.PRICE)
     FROM BILLINF bi
     JOIN ITEM i ON bi.IDFD = i.ID 
     WHERE bi.IDBILL = B.ID) - B.TOTAL AS DISCOUNT,
    B.TOTAL AS PAYMENT
FROM BILL B
LEFT JOIN ACCOUNT A ON B.IDSTAFF = A.IDSTAFF 
LEFT JOIN CUSTOMER C ON B.IDCUSTOMER = C.ID
WHERE B.CHKIN_TIME BETWEEN @STARTDATE AND @ENDDATE;

SELECT 
    C.HOMETOWN,
    COUNT(DISTINCT C.ID) AS TotalCustomers,
    COUNT(DISTINCT B.ID) AS TotalVisits,
    SUM(B.TOTAL) AS TotalSpent
FROM CUSTOMER C
JOIN BILL B ON C.ID = B.IDCUSTOMER
WHERE 
    B.STATUS = 1 
    AND C.HOMETOWN IS NOT NULL
GROUP BY 
    C.HOMETOWN
ORDER BY 
    TotalCustomers DESC, TotalVisits DESC;

SELECT * FROM ACCOUNT


SELECT  DISTINCT I.ID AS ID , I.FNAME AS FNAME, I.CATEGORY AS CATEGORY, I.PRICE AS PRICE, ISNULL(P.DISCOUNT, 0) AS DISCOUNT FROM ITEM I LEFT JOIN ITEM_PROMOTION IP ON I.ID = IP.IDITEM LEFT JOIN PROMOTION P ON IP.IDPROMOTION = P.ID ORDER BY ISNULL(P.DISCOUNT, 0) DESC;

select * from ITEM


