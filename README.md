🧩 QuanLyNguoiDungApp (TCP Client–Server)

Ứng dụng quản lý người dùng viết bằng C# WinForms, SQL Server, và TCP Socket.
Ứng dụng được phát triển từ bản chạy local (Exercise 2.2), mở rộng sang mô hình Client–Server sử dụng TCP Socket và đa luồng (multi-thread) để quản lý nhiều kết nối đồng thời.

---

## 👥 Thành viên nhóm

1. 23520498 - Võ Duy Hiếu
2. 23520837 - Trương Tùng Lâm
3. 23521269 - Quách Trọng Hải Quân
4. 24520759 - Nguyễn Nhan Quốc Khang
1. Mục tiêu & Yêu cầu kỹ thuật

## 🎯 Mục tiêu & Yêu cầu kỹ thuật

1. Tạo ứng dụng WinForms có các chức năng:

  - Đăng ký

  - Đăng nhập

  - Xem thông tin người dùng (Profile)

  - Đăng xuất

2. Sử dụng SQL Server để lưu trữ thông tin người dùng.

3. Sử dụng TCP Socket để giao tiếp giữa Client và Server.

4. Server đa luồng (multiple thread) — có thể xử lý nhiều kết nối đồng thời.

5. Quản lý phiên làm việc (session) bằng token sinh ngẫu nhiên, có thời hạn hiệu lực.
   - Token hiệu lực 1 phút (mục đích kiểm tra, có thể tùy chỉnh).

7. Khi token hết hạn → Client tự động đăng xuất và yêu cầu đăng nhập lại.

## ⚙️ Kiến trúc hệ thống

🖥 Server – QuanLyServer

- Nghe trên cổng TCP 5050.

- Mỗi khi có client kết nối mới → tạo luồng riêng để xử lý.

- Quản lý token trong bộ nhớ:
```c#
Dictionary<string, (string email, DateTime expires)>
```
- Cung cấp các API TCP:

  - register – Đăng ký tài khoản mới.

  - login – Đăng nhập, sinh token, lưu thời gian hết hạn.

  - getinfo – Trả về thông tin người dùng theo token hợp lệ.

💻 Client – QuanLyNguoiDungApp

- Giao diện WinForms UI gồm 3 form:

  - LoginForm

  - SignupForm

  - ProfileForm

- Gửi và nhận dữ liệu JSON đến server qua TCP.

- Lưu token và thời gian hết hạn tại file user_token.txt.

- Tự động kiểm tra token mỗi 10 giây bằng System.Windows.Forms.Timer.

## 🔧 Hướng dẫn cài đặt

1. Cài đặt **SQL Server** và **SQL Server Management Studio (SSMS)**.
2. Cài đặt **Visual Studio" phiên bản 2016 trở về sau.  
3. Tạo database:
   ```sql
   CREATE DATABASE QUANLYNGUOIDUNG;

   USE QUANLYNGUOIDUNG;

   CREATE TABLE USERS (
       MaND INT PRIMARY KEY IDENTITY(1,1),
       LoaiUser NVARCHAR(50) NOT NULL,
       Email NVARCHAR(100) NOT NULL,
       UserName NVARCHAR(100) NOT NULL,
       SDT NVARCHAR(20),
       MatKhauHash NVARCHAR(255) NOT NULL,
       NgaySinh DATE NOT NULL,
       NgayTao DATETIME2 DEFAULT GETDATE()
   );

4. Cấu hình kết nối (Client và Server)
Trong App.config của cả hai project:
```
<connectionStrings>
  <add name="UserDb"
       connectionString="Server=TENMAYCUABAN;Database=QUANLYNGUOIDUNG;Trusted_Connection=True;User Id=sa;Password=matkhau;"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

⚠️ Lưu ý: Thay TENMAYCUABAN, User Id, Password theo cấu hình SQL Server cá nhân.

5. Mở solution QuanLyNguoiDungApp.sln trong Visual Studio và tiến hành chạy server và client.


## ▶️ Hướng dẫn chạy chương trình

🖥 1️⃣ Chạy Server

- Mở project QuanLyServer

- Nhấn Start hoặc Ctrl + F5

- Console hiển thị:
```cmd
Server started on port 5050
Waiting for clients...
```

💻 2️⃣ Chạy Client

- Mở project QuanLyNguoiDungApp

- Nhấn Start → giao diện Login xuất hiện

## 🧩 Các chức năng

🔐 Đăng ký

- Nhập đầy đủ thông tin bắt buộc (Email, Username, Ngày sinh, Mật khẩu, Xác nhận mật khẩu).

- Kiểm tra hợp lệ: email, username, mật khẩu, số điện thoại, ngày sinh (tuổi ≥ 12).

- Mật khẩu được hash bằng SHA-512 trước khi gửi lên server.

- Nếu hợp lệ → server lưu vào DB → client chuyển sang màn hình Profile.

🔑 Đăng nhập

- Nhập Email + Mật khẩu.

- Gửi email + passwordHash lên server.

- Nếu hợp lệ → server trả về token hợp lệ (hiệu lực 1 phút).

- Token được lưu trong file user_token.txt.

🧾 Profile

- Gửi yêu cầu getinfo kèm token.

- Nếu token hợp lệ → hiển thị:

  - Email
  
  - Username
  
  - Số điện thoại
  
  - Ngày sinh
  
  - Loại người dùng
  
- Nếu token hết hạn → thông báo “Phiên đăng nhập đã hết hạn” và quay lại màn hình Login.

🚪 Đăng xuất

- Xóa user_token.txt.

- Quay về form Login.

## 🧰 Công nghệ sử dụng

- Giao diện:	C# WinForms (.NET Framework 4.7.2)
- Giao tiếp:	TCP Socket
- Cơ sở dữ liệu:	SQL Server
- Hash mật khẩu: Hàm SHA-512
- Token session:	Guid + DateTime
- Đa luồng Server:	Thread / ThreadPool
