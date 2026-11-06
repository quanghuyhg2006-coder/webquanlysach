# Quản lý sách theo thể loại

Ứng dụng Windows Forms dùng Entity Framework Core Code First và SQL Server, tổ chức thành ba tầng:

- `BookManagement.GUI`: giao diện WinForms.
- `BookManagement.BLL`: kiểm tra dữ liệu và nghiệp vụ.
- `BookManagement.DAL`: Entity Framework, truy vấn và migration.
- `BookManagement.Entity`: entity `Sach` và `TheLoai` dùng chung.
