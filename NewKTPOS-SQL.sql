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
CREATE TABLE [F&BCATEGORY] (
    ID          INT IDENTITY PRIMARY KEY,
    FNAME       NVARCHAR(50) UNIQUE NOT NULL, 
    VISIBLE     INT NOT NULL DEFAULT 1
);
GO
CREATE TABLE ITEM (
    ID          INT IDENTITY PRIMARY KEY,
    FNAME       NVARCHAR(50) NOT NULL,
    IDCATEGORY  INT NOT NULL,
    PRICE       FLOAT NOT NULL CHECK(PRICE > 0),
    VISIBLE     INT NOT NULL DEFAULT 1,
	SALEGROUP   NVARCHAR(20) CHECK(SALEGROUP IN ('Best Seller', 'New Arrival', 'Regular', 'Seasonal', 'Limited Edition'))DEFAULT 'Regular',
	SALESFLAG	BIT NOT NULL DEFAULT 0,    -- Đánh dấu bán ít (0: không, 1: có)
    DISCOUNTRATE FLOAT CHECK(DISCOUNTRATE BETWEEN 0 AND 100) DEFAULT 0, --% giảm giá
	CONSTRAINT UQ_ITEM_NAME_CATEGORY UNIQUE (FNAME, IDCATEGORY),
	CONSTRAINT FK_ITEM_CATEGORY FOREIGN KEY (IDCATEGORY) REFERENCES [F&BCATEGORY](ID) 
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
('Table 10', 1, 6, 1),
('Table VIP', 1, 10, 1);
GO
-- Insert data into [F&BCATEGORY]
INSERT INTO [F&BCATEGORY] (FNAME, VISIBLE)
VALUES
('Appetizers', 1),
('Main Dishes', 1),
('Desserts', 1),
('Drinks', 1),
('Alcohol', 1),
('Vegetarian', 1),
('Cakes', 1),
('Snacks', 1),
('Juices', 1),
('Smoothies', 1);
GO
-- Insert data into ITEM
INSERT INTO ITEM (FNAME, IDCATEGORY, PRICE, VISIBLE, SALEGROUP, SALESFLAG, DISCOUNTRATE) 
VALUES 
('Spring Rolls', 1, 35000, 1, 'Best Seller', 0, 0),
('Beef Pho', 2, 60000, 1, 'New Arrival', 0, 0),
('Flan Cake', 3, 20000, 1, 'Regular', 1, 15),
('Iced Coffee', 4, 25000, 1, 'Regular', 1, 10),
('Beer', 5, 30000, 1, 'Limited Edition', 1, 5),
('Fried Rice', 2, 50000, 1, 'Seasonal', 1, 6),
('Salad', 1, 45000, 1, 'Best Seller', 0, 0),
('Baguette', 3, 15000, 1, 'Regular', 0, 0),
('Orange Juice', 4, 20000, 1, 'Best Seller', 0, 0),
('Watermelon Smoothie', 4, 25000, 1, 'Regular', 1, 20);
GO
-- Insert data into Bill
INSERT INTO BILL (IDTABLE, IDSTAFF, CHKIN_TIME, CHKOUT_TIME, STATUS, BILLTYPE) 
VALUES 
(1, 'KT001', '2024-12-20 09:00', '2024-12-20 10:00', 0, 1), -- Dine-In
(2, 'KT002', '2024-12-20 10:15', '2024-12-20 11:15', 0, 1), -- Dine-In
(3, 'KT003', '2024-12-20 11:30', '2024-12-20 12:30', 0, 1), -- Dine-In
(NULL, 'KT004', '2024-12-20 12:45', '2024-12-20 13:45', 0, 0), -- Take Away
(5, 'KT001', '2024-12-20 14:00', '2024-12-20 15:00', 0, 1), -- Dine-In
(NULL, 'KT002', '2024-12-20 15:15', '2024-12-20 16:15', 0, 0), -- Take Away
(7, 'KT003', '2024-12-20 16:30', '2024-12-20 17:30', 0, 1), -- Dine-In
(NULL, 'KT004', '2024-12-20 17:45', '2024-12-20 18:45', 0, 0), -- Take Away
(9, 'KT001', '2024-12-20 19:00', '2024-12-20 20:00', 0, 1), -- Dine-In
(NULL, 'KT002', '2024-12-20 20:15', '2024-12-20 21:15', 0, 0); -- Take Away
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
    [F&BCATEGORY].FNAME AS Category_Name, 
    ITEM.SALEGROUP AS Sale_Group,
    ITEM.PRICE
FROM ITEM
JOIN [F&BCATEGORY] ON ITEM.IDCATEGORY = [F&BCATEGORY].ID;
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
    i.FNAME as NAME,
    bi.COUNT as QTY,
    i.PRICE as PRICE,
    i.ID as ID
FROM BILLINF bi
JOIN ITEM i ON bi.IDFD = i.ID
WHERE bi.IDBILL = 2;