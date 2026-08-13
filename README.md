# HỆ THỐNG QUẢN LÝ DEPOT CONTAINER

Hệ thống quản lý depot container cho Tân Cảng Sài Gòn - xây dựng bằng **.NET 10 + Razor Pages + MySQL**.

## 🎯 Mục tiêu dự án

Xây dựng hệ thống quản lý depot giúp:
1. **Quản lý bãi container** (Block / Bay / Row / Tier)
2. **Quản lý thông tin container** (validate theo ISO 6346 - Modulo 11)
3. **Quản lý nhập/xuất container** (vòng đời container)
4. **Quản lý lệnh giao container** (từ Line Operator)
5. **Quản lý hãng khai thác & khách hàng**

## 🛠️ Tech Stack

- **Backend**: .NET 10 (ASP.NET Core)
- **Frontend**: Razor Pages + Bootstrap 5 + Bootstrap Icons
- **ORM**: Entity Framework Core 9 + Pomelo MySQL
- **Database**: MySQL 8.0+
- **API Docs**: Swagger UI
- **Architecture**: 3-Tier (Controller/Service/Repository)

## 📁 Cấu trúc dự án (3-Tier Architecture)

```
DEPOTCONTAINER/
├── Controllers/Api/      # API Controllers (REST endpoints)
├── Pages/                  # Razor Pages (UI server-side)
├── Services/               # Business logic layer
│   ├── Interfaces/         # Service contracts
├── Repositories/           # Data access layer
│   ├── Interfaces/         # Repository contracts
├── Models/
│   ├── Entities/           # Database entities
│   ├── DTOs/               # Data Transfer Objects
│   └── Enums/              # Enum definitions
├── Data/                   # DbContext + Seeder
├── Validators/             # Container Number Validator (Modulo 11)
├── Extensions/             # Extension Methods (Generic, LINQ, Lambda)
├── Factories/              # Factory Pattern (BayFactory, ContainerFactory)
└── Singletons/             # Singleton Pattern (ConfigManager, Cache)
```

## 🚀 Cách chạy

### 1. Yêu cầu môi trường
- .NET 10 SDK
- MySQL 8.0+ (đang chạy ở `localhost:3306`)

### 2. Cấu hình database
Sửa `appsettings.json` → `ConnectionStrings:DefaultConnection` nếu cần:
```json
"DefaultConnection": "server=localhost;port=3306;database=depotdb;user=root;password=YOUR_PASSWORD"
```

### 3. Chạy ứng dụng
```bash
cd DEPOTCONTAINER
dotnet restore
dotnet run
```

App sẽ tự động:
- Tạo database `depotdb` (nếu chưa có)
- Tạo các bảng (EF Core `EnsureCreated`)
- Seed dữ liệu mẫu (Line Operators, Customers, Blocks, Containers)

### 4. Truy cập
- **Trang chủ (Razor Pages)**: https://localhost:7xxx/
- **Swagger UI (API docs)**: https://localhost:7xxx/swagger
- **API endpoints**: `/api/containers`, `/api/blocks`, ...

### 5. Chạy bằng Docker Compose (khuyến nghị)

Toàn bộ hệ thống (MySQL + .NET app) được đóng gói trong Docker Compose:

```bash
cd DEPOTCONTAINER
docker compose up -d          # Khởi động (MySQL + .NET app)
docker compose logs -f app    # Xem logs của app
docker compose down           # Dừng
docker compose down -v        # Dừng + xóa data
```

**Sau khi chạy xong, truy cập:**
- 🌐 **Trang chủ (UI)**: http://localhost:8080/
- 📚 **Swagger UI**: http://localhost:8080/swagger
- 🗄️ **MySQL**: `localhost:3307` (user=`root`, password=`root123`, db=`depotdb`)

**Cấu hình trong `docker-compose.yml`:**
- MySQL 8.0 chạy port `3307` (host) → `3306` (container)
- .NET 10 SDK app chạy port `8080`
- Volume `depot-mysql-data` lưu data MySQL persistent
- App tự động chờ MySQL healthy trước khi start
- App tự động restore NuGet + tạo schema + seed data

## 📚 Pattern & Kỹ thuật áp dụng

### Design Patterns
- ✅ **3-Tier Architecture**: Controllers → Services → Repositories
- ✅ **Repository Pattern**: Generic Repository + Specific Repositories
- ✅ **Unit of Work Pattern**: `IUnitOfWork` quản lý transaction
- ✅ **Factory Pattern**: `BayFactory` (bay lẻ=20ft, bay chẵn=40ft), `ContainerFactory`
- ✅ **Singleton Pattern**: `DepotConfigManager`, `InMemoryCache`

### SOLID Principles
- ✅ **Single Responsibility**: Mỗi class có 1 trách nhiệm duy nhất
- ✅ **Open/Closed**: Dễ mở rộng mà không sửa code cũ
- ✅ **Liskov Substitution**: BaseEntity được kế thừa bởi mọi entity
- ✅ **Interface Segregation**: `IGenericRepository` + `IContainerRepository` (tách biệt)
- ✅ **Dependency Inversion**: Services phụ thuộc `IUnitOfWork`, không phụ thuộc implementation

### C# Features
- ✅ **Generic**: `IGenericRepository<T>`, `GenericFactory<T>`, `ApiResponse<T>`, `PagedResult<T>`
- ✅ **Partial class**: (có thể áp dụng thêm cho từng module)
- ✅ **Extension Methods**: `EnumerableExtensions`, `BaseEntityExtensions`, `StringExtensions`
- ✅ **Delegate**: `Action<string>` (logging), `Func<T, R>` (location builder, orderBy)
- ✅ **Predicate**: qua `Expression<Func<T, bool>>`
- ✅ **Lambda expression**: trong LINQ queries, factory delegates
- ✅ **LINQ**: filter, sort, project (`.Where()`, `.OrderBy()`, `.Select()`, `.Any()`)
- ✅ **Dependency Injection**: tất cả services qua `builder.Services.AddScoped<>()` trong `Program.cs`

### ASP.NET Core Features
- ✅ **ServiceCollection**: `Program.cs` đăng ký tất cả DI
- ✅ **Configuration**: `appsettings.json` + Environment Variables
- ✅ **Middleware**: custom request logging + Swagger
- ✅ **Controllers**: API Controllers + Razor Pages
- ✅ **Entity Framework Core**: Code First + Fluent API configuration

## 🌐 API Endpoints

### Containers
- `GET /api/containers` - Lấy danh sách (có phân trang, search, sort)
- `GET /api/containers/{id}` - Lấy theo ID
- `GET /api/containers/by-number/{number}` - Lấy theo số container
- `GET /api/containers/validate/{number}` - Validate số container (Modulo 11)
- `POST /api/containers` - Tạo mới
- `PUT /api/containers/{id}` - Cập nhật
- `DELETE /api/containers/{id}` - Xóa (soft delete)
- `POST /api/containers/{id}/assign-location` - Gán vị trí trong bãi

### Blocks (Bãi)
- `GET /api/blocks` - Danh sách block
- `GET /api/blocks/{id}/layout` - Layout Bay/Row/Tier của block
- `POST /api/blocks/{id}/generate-layout` - Tự động sinh layout

### Movements (Vận chuyển)
- `GET /api/movements` - Danh sách
- `POST /api/movements` - Ghi nhận IN/OUT

### Release Orders (Lệnh giao)
- `GET /api/release-orders` - Danh sách
- `POST /api/release-orders` - Tạo lệnh
- `POST /api/release-orders/{id}/execute` - Thực hiện giao container

### Line Operators & Customers
- `GET /api/lineoperators`, `POST /api/lineoperators`, ...
- `GET /api/customers`, `POST /api/customers`, ...

## 🗄️ Database Schema

```
LineOperators (Hãng khai thác)
   ├─ OwnerCode (3 chữ cái, unique)
   └─ Name, TaxCode, Address, ...

Customers (Khách hàng nhận container)
   └─ TaxCode (MST, unique), Name, ...

Blocks (Bãi container)
   ├─ Code (unique), Name
   ├─ BlockType (Physical / Virtual)
   └─ MaxBays, MaxRows, MaxTiers, MaxContainerSize
       │
       └─ Bays (bay lẻ=20ft, bay chẵn=40ft)
            │
            └─ Rows
                 │
                 └─ Tiers (vị trí xếp container)

Containers
   ├─ ContainerNumber (11 ký tự theo ISO 6346 - check Modulo 11)
   ├─ ContainerType (U/R/S/F...), Size (20/40/45), IsoCode
   ├─ MaxWeight, TareWeight, ManufactureDate
   ├─ CurrentBlockId/BayId/RowId/TierId
   └─ Condition (Normal/SlightlyDamaged/SeverelyDamaged)
       │
       └─ ContainerMovements (vòng đời IN/OUT)

ReleaseOrders (Lệnh giao container)
   ├─ OrderNumber, ValidUntil, ExportVessel, ExportDate
   ├─ LineOperatorId, CustomerId, Status
   └─ ReleaseOrderDetails (số lượng container mỗi loại cần giao)
```

## 🔍 Container Number Validation (ISO 6346)

Số container 11 ký tự theo cấu trúc:
```
[CMA][U][123456][7]
  ↓    ↓    ↓       ↓
Owner Type  Serial  Check Digit
 Code Code (6 số)  (Modulo 11)
```

Xem chi tiết thuật toán tại `Validators/ContainerNumberValidator.cs`.

## 📝 License

Dự án intern - Tân Cảng Sài Gòn.# DEPOTCONTAINER
