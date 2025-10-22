CREATE DATABASE QUANLYNGUOIDUNG

USE QUANLYNGUOIDUNG

-- Bảng Users (Nguoi Dung)
CREATE TABLE USERS (
    MaND INT PRIMARY KEY IDENTITY(1,1),         -- Mã người dùng (PK) - Dùng INT IDENTITY cho tiện
    LoaiUser NVARCHAR(50) NOT NULL,             -- Loại người dùng (Admin, Nguoichoi)
    Email NVARCHAR(100) NOT NULL,				-- Email
	UserName NVARCHAR(100),						-- Username
    SDT NVARCHAR(20),							-- Số điện thoại
    MatKhauHash NVARCHAR(255) NOT NULL,         -- Nên lưu Hash mật khẩu, không lưu trực tiếp
    NgaySinh DATE NOT NULL,                     -- Ngày sinh
    NgayTao DATETIME2 DEFAULT GETDATE()         -- Ngày tạo tài khoản
);

-- Kiểm tra dữ liệu trong bảng USERS
SELECT * FROM USERS
