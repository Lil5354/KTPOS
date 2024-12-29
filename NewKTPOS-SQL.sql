USE MASTER
GO
DROP DATABASE IF EXISTS KTPOS
GO
CREATE DATABASE KTPOS
GO
USE KTPOS
GO
CREATE TABLE ACCOUNT (
	ID		   INT IDENTITY PRIMARY KEY,
    IDSTAFF    NVARCHAR(10) UNIQUE NOT NULL,
    FULLNAME   NVARCHAR(50) NOT NULL,                        -- Tên đầy đủ
    EMAIL      NVARCHAR(50) ,			                   -- Email (duy nhất)
    PHONE      NVARCHAR(11) NOT NULL,                        -- Số điện thoại
    DOB        DATE NOT NULL CHECK (DOB <= GETDATE()),        -- Ngày sinh (phải nhỏ hơn hoặc bằng ngày hiện tại)
    [PASSWORD] NVARCHAR(50) NOT NULL CHECK(LEN([PASSWORD]) >= 6) DEFAULT 'ktpos123', -- Mật khẩu
    [ROLE]     NVARCHAR(20) CHECK([ROLE] IN ('Staff', 'Manager')), -- Vai trò
    VISIBLE    INT NOT NULL DEFAULT 1                        -- Trạng thái hiển thị (0: ẩn, 1: hiển thị)
);
--Trigger sinh IDSTAFF theo KT+ thứ tự nhân viên vào làm
GO
CREATE TABLE [TABLE] (
    ID         INT IDENTITY PRIMARY KEY,   -- ID bàn
    FNAME      NVARCHAR(50) UNIQUE,        -- Tên bàn
    STATUS     INT NOT NULL DEFAULT 1,     -- 1: AVAILABLE/ 0: UNAVAILABLE
    CAPACITY   TINYINT NOT NULL DEFAULT 4,     -- SỐ Lượng người tối đa
    VISIBLE    INT NOT NULL DEFAULT 1      -- 0: FALSE / 1: TRUE
);
GO
CREATE TABLE ITEM (
    ID          INT IDENTITY PRIMARY KEY,
    FNAME       NVARCHAR(50) NOT NULL,
    CATEGORY	NVARCHAR(10) CHECK(CATEGORY IN ('Food', 'Drink')) NOT NULL,
    PRICE       FLOAT NOT NULL CHECK(PRICE > 0),
    VISIBLE     INT NOT NULL DEFAULT 1,
	SALESFLAG	BIT NOT NULL DEFAULT 0,    -- Đánh dấu bán ít (0: không, 1: có)
    DISCOUNTRATE FLOAT CHECK(DISCOUNTRATE BETWEEN 0 AND 80) DEFAULT 0, --% giảm giá
);
GO
CREATE TABLE BILL (
    ID          INT IDENTITY PRIMARY KEY,			-- ID hóa đơn
    DATEPAYMENT DATE NOT NULL DEFAULT GETDATE(),	-- Ngày thanh toán
    IDTABLE     INT NULL,						-- ID bàn
    IDSTAFF     NVARCHAR(10) NOT NULL,				-- Mã nhân viên (khóa ngoại từ ACCOUNT)
	BILLTYPE    BIT NOT NULL DEFAULT 1,             -- Loại bill: 0 (Mang đi), 1 (Tại bàn)
	CHKIN_TIME  DATETIME NOT NULL DEFAULT GETDATE(),
    CHKOUT_TIME DATETIME NULL,
    DURATION    AS DATEDIFF(MINUTE, CHKIN_TIME, CHKOUT_TIME), -- Thời gian khách ngồi tính theo phút
    STATUS      INT NOT NULL DEFAULT 0,				-- Trạng thái thanh toán (0: chưa thanh toán, 1: đã thanh toán)
	CONSTRAINT CK_BILL_BILLTYPE_IDTABLE CHECK (BILLTYPE = 1 OR (BILLTYPE = 0 AND IDTABLE IS NULL)), -- Ràng buộc khi takeaway thì id table = null
    CONSTRAINT FK_BILL_TABLE FOREIGN KEY (IDTABLE) REFERENCES [TABLE](ID),
    CONSTRAINT FK_BILL_ACCOUNT FOREIGN KEY (IDSTAFF) REFERENCES ACCOUNT(IDSTAFF)
);
GO
CREATE TABLE BILLINF (
    ID      INT IDENTITY PRIMARY KEY,      -- ID hóa đơn chi tiết
    IDBILL  INT NOT NULL,                  -- ID hóa đơn (khóa ngoại từ BILL)
    IDFD    INT NOT NULL,                  -- ID món (khóa ngoại từ ITEM)
    COUNT   INT NOT NULL DEFAULT 1,        -- Số lượng món
    CONSTRAINT FK_BILLINF_BILL FOREIGN KEY (IDBILL) REFERENCES BILL(ID),
    CONSTRAINT FK_BILLINF_ITEM FOREIGN KEY (IDFD) REFERENCES ITEM(ID)
);
CREATE TABLE TAG (
    ID          INT IDENTITY PRIMARY KEY, -- ID Tag
    TAGNAME     NVARCHAR(50) UNIQUE CHECK(TAGNAME IN ('Best Seller', 'New Arrival', 'Regular', 'Seasonal', 'Limited Edition'))NOT NULL -- Tên Tag (e.g., 'Best Seller', 'New Arrival')
);

-- bảng liên kết ITEM_TAG để quản lý mối quan hệ N-N giữa ITEM và TAG
CREATE TABLE ITEM_TAG (
    ID          INT IDENTITY PRIMARY KEY,
    IDITEM      INT NOT NULL, -- ID Item
    IDTAG       INT NOT NULL, -- ID Tag
    CONSTRAINT FK_ITEMTAG_ITEM FOREIGN KEY (IDITEM) REFERENCES ITEM(ID),
    CONSTRAINT FK_ITEMTAG_TAG FOREIGN KEY (IDTAG) REFERENCES TAG(ID),
    CONSTRAINT UQ_ITEMTAG UNIQUE (IDITEM, IDTAG) -- Đảm bảo mỗi cặp Item-Tag là duy nhất
);
GO
-- Bước 4: Thêm dữ liệu ví dụ cho bảng TAG
INSERT INTO TAG (TAGNAME)
VALUES ('Best Seller'), ('New Arrival'), ('Regular'), ('Seasonal'), ('Limited Edition');
GO
CREATE TRIGGER trg_InsertIDStaff
ON ACCOUNT
INSTEAD OF INSERT
AS
BEGIN
    DECLARE @NewID INT;
    DECLARE @NewIDStaff NVARCHAR(10);

    -- Lấy ID lớn nhất trong bảng ACCOUNT, nếu không có dữ liệu thì khởi tạo là 1
    SELECT @NewID = ISNULL(MAX(CAST(SUBSTRING(IDSTAFF, 3, LEN(IDSTAFF)) AS INT)), 0) + 1
    FROM ACCOUNT;

    -- Tạo IDSTAFF theo định dạng KT + ID
    SET @NewIDStaff = 'KT' + RIGHT('000' + CAST(@NewID AS NVARCHAR(3)), 3);

    -- Thực hiện chèn dữ liệu với IDSTAFF tự động sinh ra
    INSERT INTO ACCOUNT (IDSTAFF, FULLNAME, EMAIL, PHONE, DOB, [PASSWORD], [ROLE], VISIBLE)
    SELECT @NewIDStaff, FULLNAME, EMAIL, PHONE, DOB, [PASSWORD], [ROLE], VISIBLE
    FROM INSERTED;
END;
GO
INSERT INTO ACCOUNT (FULLNAME, EMAIL, PHONE, DOB, [PASSWORD], [ROLE], VISIBLE) VALUES 
    (N'Võ Đăng Khoa',			'khoavd2809@gmail.com',	'0843019548', '2004-09-28', 'khoavo123',		'Manager', 1)
INSERT INTO ACCOUNT (FULLNAME, EMAIL, PHONE, DOB, [PASSWORD], [ROLE], VISIBLE) VALUES 
	(N'Dương Thị Thanh Thảo',	'thaott26@gmail.com',	'0902234567', '2003-06-26', 'pupu123',			'Manager', 1)
INSERT INTO ACCOUNT (FULLNAME, EMAIL, PHONE, DOB, [PASSWORD], [ROLE], VISIBLE) VALUES 
	(N'Hoàng Văn Thiên',		'hvt2003@gmail.com',	'0903234567', '2003-05-15', 'chillguy1',		'Staff', 1)
INSERT INTO ACCOUNT (FULLNAME, EMAIL, PHONE, DOB, [PASSWORD], [ROLE], VISIBLE) VALUES 
    (N'Lê Thiện Nhân',			'nhanle@gmail.com',		'0904234567', '2001-12-12', 'cuchuoi2xu',		'Staff', 1)
INSERT INTO ACCOUNT (FULLNAME, EMAIL, PHONE, DOB, [PASSWORD], [ROLE], VISIBLE) VALUES 
    (N'Từ Tuấn Sang',			'tsang@gmail.com',		'0905234567', '2005-03-15', 'tsang123',			'Staff', 1)
INSERT INTO ACCOUNT (FULLNAME, EMAIL, PHONE, DOB, [PASSWORD], [ROLE], VISIBLE) VALUES 
    (N'Nguyễn Thành Đạt',		'dathphong@gmail.com',	'0906234567', '2007-11-20', 'hoangtusitinh',	'Staff', 1)
INSERT INTO ACCOUNT (FULLNAME, EMAIL, PHONE, DOB, [PASSWORD], [ROLE], VISIBLE) VALUES 
    (N'Nguyễn Giang Gia Huy',	'huybo@gmail.com',		'0907234567', '2006-08-05', 'huybo123',			'Staff', 1)
GO
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
GO
-- Insert data into ITEM
INSERT INTO ITEM (FNAME, CATEGORY, PRICE, VISIBLE, SALESFLAG, DISCOUNTRATE) 
VALUES 
('Spring Rolls', 'Food', 35000, 1, 0, 0),
('Beef Pho', 'Food', 60000, 1, 0, 0),
('Flan Cake', 'Food', 20000, 1, 1, 15),
('Iced Coffee', 'Drink', 25000, 1, 1, 10),
('Beer', 'Drink', 30000, 1, 1, 5),
('Fried Rice', 'Food', 50000, 1, 1, 6),
('Salad', 'Food', 45000, 1, 0, 0),
('Baguette', 'Food', 15000, 1, 0, 0),
('Orange Juice', 'Drink', 20000, 1, 0, 0),
('Watermelon Smoothie', 'Drink', 25000, 1, 1, 20);
GO
-- Insert data into Bill
INSERT INTO BILL (IDTABLE, IDSTAFF, CHKIN_TIME, CHKOUT_TIME, STATUS, BILLTYPE) 
VALUES 
(1, 'KT001', '2024-12-20 09:00', '2024-12-20 10:00', 1, 1), -- Dine-In
(2, 'KT002', '2024-12-20 10:15', '2024-12-20 11:15', 0, 1), -- Dine-In
(3, 'KT003', '2024-12-20 11:30', '2024-12-20 12:30', 1, 1), -- Dine-In
(NULL, 'KT004', '2024-12-20 12:45', '2024-12-20 13:45', 0, 0), -- Take Away
(5, 'KT001', '2024-12-20 14:00', '2024-12-20 15:00', 1, 1), -- Dine-In
(NULL, 'KT002', '2024-12-20 15:15', '2024-12-20 16:15', 0, 0), -- Take Away
(7, 'KT003', '2024-12-20 16:30', '2024-12-20 17:30', 1, 1), -- Dine-In
(NULL, 'KT004', '2024-12-20 17:45', '2024-12-20 18:45', 0, 0), -- Take Away
(9, 'KT001', '2024-12-20 19:00', '2024-12-20 20:00', 1, 1), -- Dine-In
(NULL, 'KT002', '2024-12-20 20:15', '2024-12-20 21:15', 0, 0); -- Take Away
GO
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
(9, 10, 1);
GO
--Thông tin nhân viên 
SELECT IDSTAFF, FULLNAME, EMAIL, PHONE, DOB, [ROLE], VISIBLE 
FROM ACCOUNT;
--DS BÀN
SELECT ID, FNAME, STATUS, CAPACITY, VISIBLE 
FROM [TABLE];
--DS MÓN ĂN KÈM PHÂN LOẠI 
SELECT 
    ITEM.FNAME AS Food_Name, 
    CATEGORY AS Category_Name, 
    ITEM.PRICE
FROM ITEM

--HÓA ĐƠN
SELECT 
    ACCOUNT.FULLNAME AS Staff_Name, 
    ITEM.FNAME AS Food_Item, 
    BILLINF.COUNT AS Quantity, 
    [TABLE].FNAME AS Table_Name,
	BILL.DURATION AS DURATION,
    BILL.DATEPAYMENT AS Payment_Date, 
    ITEM.PRICE * BILLINF.COUNT AS Total_Price
FROM BILL
JOIN ACCOUNT ON BILL.IDSTAFF = ACCOUNT.IDSTAFF
JOIN BILLINF ON BILL.ID = BILLINF.IDBILL
JOIN ITEM ON BILLINF.IDFD = ITEM.ID
JOIN [TABLE] ON BILL.IDTABLE = [TABLE].ID
WHERE BILL.STATUS = 1; -- Assuming 1 means paid*/

SELECT 
    T.FNAME AS TableName,
    T.CAPACITY,
    T.STATUS,
    ISNULL(SUM(BI.COUNT * I.PRICE), 0) AS [TOTAL PRICE]
FROM [TABLE] T
LEFT JOIN BILL B ON T.ID = B.IDTABLE AND B.STATUS = 1
LEFT JOIN BILLINF BI ON B.ID = BI.IDBILL
LEFT JOIN ITEM I ON BI.IDFD = I.ID
WHERE T.VISIBLE = 1
GROUP BY T.FNAME, T.CAPACITY, T.STATUS
SELECT TAGNAME FROM TAG 
SELECT I.FNAME AS [ITEM NAME], MAX(CASE WHEN T.TAGNAME = 'Best Seller' THEN 1 ELSE 0 END) AS TAG_CHECKBOX, I.SALESFLAG                              
FROM ITEM I LEFT JOIN ITEM_TAG IT ON I.ID = IT.IDITEM LEFT JOIN TAG T ON IT.IDTAG = T.ID GROUP BY I.FNAME, I.SALESFLAG               
ORDER BY I.FNAME;

SELECT 
    ID AS Bill_ID,
    CASE 
        WHEN BILLTYPE = 1 THEN 'Dine-In'
        ELSE 'Take Away'
    END AS Bill_Type,
    CHKIN_TIME AS Check_In_Time,
    CHKOUT_TIME AS Check_Out_Time,
    DURATION AS Duration_Minutes,
    CASE 
        WHEN STATUS = 1 THEN 'Paid'
        ELSE 'Unpaid'
    END AS Payment_Status
FROM BILL;