# PRN232 LMS Lab 1

Hệ thống API quản lý học tập cho bài Lab 1 môn PRN232, xây dựng theo kiến trúc 3 lớp và tách rõ 4 nhóm model: `Entity`, `Business`, `Request`, `Response`.

## Tổng quan

Dự án mô phỏng một hệ thống quản lý sinh viên, học phần, học kỳ và đăng ký môn học. Dữ liệu trả về đều đi qua `Response` model và được bọc trong `ApiResponse<T>` để đồng nhất format phản hồi.

## Công nghệ sử dụng

| Thành phần | Công nghệ |
|---|---|
| Runtime | .NET 9 |
| Web API | ASP.NET Core Web API |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Tài liệu API | Swagger / OpenAPI |
| Triển khai | Docker Compose |

## Kiến trúc dự án

- `PRN232.LMS.API`: Controller, Swagger, cấu hình khởi động
- `PRN232.LMS.Services`: Business logic, `Business` model, `Request` và `Response` model
- `PRN232.LMS.Repositories`: `Entity`, `DbContext`, Repository
- `docker-compose.yml`: chạy API và SQL Server bằng Docker

## Quy ước 4 model

### Entity
Được dùng cho EF Core và tầng Repository. Repository chỉ làm việc với `Entity`.

### Business
Là model trung gian ở tầng nghiệp vụ, dùng để xử lý dữ liệu trước khi trả về client.

### Request
Dùng cho dữ liệu đầu vào từ client khi tạo hoặc cập nhật dữ liệu.

### Response
Dùng cho dữ liệu trả về client. Controller không trả trực tiếp `Entity`.

## Tính năng đã hỗ trợ

- CRUD cho `Students`, `Courses`, `Semesters`, `Enrollments`, `Subjects`
- Phân trang (`page`, `size`)
- Tìm kiếm (`search`)
- Sắp xếp (`sort`)
- Mở rộng dữ liệu liên quan (`expand`)
- Chọn trường trả về (`fields`)
- Trả dữ liệu theo chuẩn `ApiResponse<T>`

## Cấu trúc thư mục chính

```text
PRN232.LMS.API/
PRN232.LMS.Repositories/
PRN232.LMS.Services/
README.md
docker-compose.yml
```

Trong `PRN232.LMS.Services`:

```text
BusinessModels/
Models/
	Requests/
	Responses/
```

## Chạy dự án local

```powershell
dotnet restore
dotnet build PRN232.LMS.sln
dotnet run --project PRN232.LMS.API --launch-profile http
```

Swagger khi chạy local:

```text
http://localhost:5260/
```

## Chạy bằng Docker

```powershell
docker compose up --build -d
```

Swagger khi chạy bằng Docker:

```text
http://localhost:5000/
```

## API tiêu biểu

- `GET /api/students`
- `GET /api/courses`
- `GET /api/enrollments`
- `GET /api/semesters`
- `GET /api/subjects`
- `GET /api/students/{id}/enrollments`
- `GET /api/students/{id}/courses`
- `GET /api/courses/{id}/subjects`

## Ghi chú

- Database được tạo và seed tự động khi ứng dụng khởi động.
- Nếu build lỗi do file bị khóa, hãy tắt tiến trình `dotnet run` đang chạy rồi build lại.
- Nếu cần chụp ảnh báo cáo, nên mở Swagger ở chế độ local hoặc Docker rồi dùng các endpoint `GET` để minh họa.
