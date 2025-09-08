# 🛒 Website Thương mại điện tử - ASP.NET MVC
## 🏆 Giới thiệu hê thống
Dự án **Website bán hàng** được xây dựng bằng **ASP.NET MVC** với mục tiêu cung cấp một nền tảng thương mại điện tử với các chức năng cơ bản. Người dùng có thể tìm kiếm sản phẩm, thêm vào giỏ hàng và thanh toán online. Quản trị viên có thể quản lý sản phẩmm, đơn hàng, thống kê doanh thu và khách hàng.
## 🚀 Các tính năng nổi bật dạt được của dự án
- Thêm sản phẩm vào giỏ hàng
- Tìm kiểm sản phẩm theo tên, danh mục
- Lọc sản phảm theo giá
- Xem chi tiết sản phẩm
- Thanh toán Online bằng VNPay và Momo
- Đăng nhập, đăng ký, đăng nhập bằng Google
- Xác thực, phân quyền
- Quản trị viên quản lý sản phẩm, đơn hàng...
- Thống kê doanh thu
## 🛠️ Công nghệ sử dụng
- Framework: ASP.Net MVC, Entity Framework
- UI: HTML, CSS, Bootstrap, JavaScript
- Database: SQL Server
- Authentication: ASP.Net Identity
## ⚙️ Hướng dẫn cài dặt
1. Clone dự án: git clone https://github.com/thanhdanhk2004/Shopping.git
2. Mở solution bằng Visual Studio
3. Cấu hình lại chuỗi kết nối đến database của bạn trong file appsettings.json
- "ConnectionStrings": {
    "ConnectedDb": "Data Source=your_server;Initial Catalog=EShopping;Integrated Security=True;Trust Server Certificate=True"
  },
4. Cài đặt các gói cần thiết
- Microsoft.EntityFrameworkCore (9.0.5) (gói này dùng dể sử dụng Entity Framework Core)
- Microsoft.EntityFrameworkCore.SqlServer (9.0.5) (gói này dùng để làm việc với SQL Server)
- Microsoft.EntityFrameworkCore.Tools (9.0.5) (gói này dùng để làm việc với Entity Framework Core)
- Newtonsoft.Json (13.0.3) (gói này dùng để làm việc với JSON)
- Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.0) (gói này dùng  cung cấp Identity dể quản lý user, role, password...)
- RestSharp (112.1.0) (gói này làm việc với API)
## 👨‍💻 Tác giả: Lê Thanh Dân
- 📧 Email: thanhdanhk2004@gmail.com
- 💼 LinkedIn: https://www.linkedin.com/in/d%C3%A2n-l%C3%AA-thanh-77b229361/
