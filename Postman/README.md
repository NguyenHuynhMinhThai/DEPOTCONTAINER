# DEPOTCONTAINER - Postman API Testing

Bộ file Postman Collection + Environment để test toàn bộ API của hệ thống **DEPOT Container**.

## 📁 Cấu trúc file

```
Postman/
├── DEPOTCONTAINER.postman_collection.json   # Collection chính (tất cả API)
└── DEPOTCONTAINER.postman_environment.json  # Environment với biến sẵn
```

## 🚀 Hướng dẫn sử dụng

### 1. Chuẩn bị

Đảm bảo API đang chạy. Mặc định project dùng port `5007` (xem `Properties/launchSettings.json`):

```bash
dotnet run
```

Sau khi chạy, API sẵn sàng tại:
- HTTP: `http://localhost:5007`
- HTTPS: `https://localhost:7140`
- Swagger UI: `http://localhost:5007/swagger`

### 2. Import vào Postman

**Cách 1: Import file JSON**
1. Mở Postman → Click **Import** (góc trên bên trái).
2. Kéo thả cả 2 file `*.json` vào cửa sổ Import, hoặc click **Upload Files**.
3. Postman sẽ tự động nhận diện Collection và Environment.

**Cách 2: Paste raw JSON**
1. Mở Postman → **Import** → **Raw text**.
2. Paste nội dung từng file JSON và nhấn **Import**.

### 3. Chọn Environment

Ở góc trên bên phải Postman, chọn **Environment** = `DEPOTCONTAINER - Local`.

## 📂 Cấu trúc Collection

Collection được nhóm thành **7 thư mục** theo route:

| # | Thư mục | Base Route | Chức năng |
|---|--------|-----------|----------|
| 1 | Containers | `/api/containers` | CRUD container + Validate + Assign Location |
| 2 | Blocks | `/api/blocks` | CRUD block + Layout + Generate |
| 3 | Line Operators | `/api/lineoperators` | CRUD hãng khai thác (CMA, MSC...) |
| 4 | Customers | `/api/customers` | CRUD khách nhận container |
| 5 | Movements | `/api/movements` | Lịch sử container vào/ra bãi |
| 6 | Release Orders | `/api/release-orders` | Lệnh giao container + Execute |
| 7 | Health & Swagger | `/swagger`, `/` | Kiểm tra API & docs |

## 🔧 Các biến Environment quan trọng

| Biến | Mặc định | Mô tả |
|------|---------|-------|
| `baseUrl` | `http://localhost:5007` | URL gốc của API |
| `baseUrlHttps` | `https://localhost:7140` | URL HTTPS (nếu dùng profile https) |
| `apiPrefix` | `/api` | Prefix của tất cả API |
| `containerId` | `1` | Id mẫu cho test container |
| `containerNumber` | `CMAU1234567` | Số container mẫu |
| `blockId` | `1` | Id block mẫu |
| `bayId`, `rowId`, `tierId` | `1` | Id vị trí mẫu |
| `lineOperatorId` | `1` | Id Line Operator mẫu |
| `customerId` | `1` | Id Customer mẫu |
| `movementId` | `1` | Id movement mẫu |
| `releaseOrderId` | `1` | Id release order m�u |
| `pageNumber`, `pageSize` | `1`, `10` | Tham số phân trang |

## 📋 Response thống nhất

Mọi API đều trả về cấu trúc `ApiResponse<T>`:

```json
{
  "success": true,
  "message": "Thành công",
  "data": { ... },
  "errors": []
}
```

Khi lỗi:
```json
{
  "success": false,
  "message": "Validation failed",
  "errors": ["ContainerNumber is invalid", "Size must be 20, 40 or 45"]
}
```

## 🔢 Enum Reference (truyền theo số)

### ContainerType (ký tự ASCII)
| Value | Code | Tên |
|-------|------|-----|
| 85 | 'U' | Dry |
| 82 | 'R' | Reefer |
| 83 | 'S' | OpenTop |
| 70 | 'F' | FlatRack |
| 66 | 'B' | Bunker |
| 86 | 'V' | Ventilated |
| 90 | 'Z' | Specialized |

### ContainerSize
| Value | Size |
|-------|------|
| 20 | 20ft |
| 40 | 40ft |
| 45 | 45ft |

### ContainerCondition
| Value | Tên |
|-------|-----|
| 0 | Normal |
| 1 | SlightlyDamaged |
| 2 | SeverelyDamaged |

### ContainerCategory
| Value | Tên |
|-------|-----|
| 0 | CategoryA (mới, tốt) |
| 1 | CategoryB (đã qua sử dụng) |
| 2 | CategoryC (cần sửa chữa) |

### BlockType
| Value | Tên |
|-------|-----|
| 0 | Physical (có Bay/Row/Tier) |
| 1 | Virtual (block ảo) |

### MovementType
| Value | Tên |
|-------|-----|
| 0 | In (vào bãi) |
| 1 | Out (ra khỏi bãi) |
| 2 | Internal (di chuyển nội bộ) |

### ReleaseOrderStatus
| Value | Tên |
|-------|-----|
| 0 | New |
| 1 | InProgress |
| 2 | Completed |
| 3 | Cancelled |
| 4 | Expired |

## 📦 Dữ liệu mẫu có sẵn (sau khi chạy lần đầu)

Sau khi start project lần đầu, `DbSeeder` sẽ tự động insert:

- **4 Line Operators**: CMA CGM, MSC, HMM, Maersk
- **3 Customers**: Hòa Phát, Thành Công, Logistics VN
- **3 Blocks**:
  - Khu A (Container 20ft) - 4 bays × 3 rows × 4 tiers
  - Khu B (Container 40ft) - 4 bays × 3 rows × 4 tiers
  - V-01 (Block ảo cho hàng hư hỏng)
- **2 Containers mẫu**: `CMAU1234567` (Dry 20ft), `MSCU7654321` (Reefer 40ft)

> Sau khi API chạy, bạn có thể gọi ngay `GET /api/containers` để thấy data mẫu.

## � Workflow test mẫu (End-to-End)

Để test toàn bộ luồng nghiệp vụ, làm theo thứ tự sau:

1. **Tạo Line Operator** → `POST /api/lineoperators` (lấy `id` trả về)
2. **Tạo Customer** → `POST /api/customers` (lấy `id` trả về)
3. **Tạo Block** → `POST /api/blocks` (lấy `id` trả về)
4. **Generate Block Layout** → `POST /api/blocks/{id}/generate-layout`
5. **Tạo Container** → `POST /api/containers` (lấy `id` trả về)
6. **Validate Container Number** → `GET /api/containers/validate/{number}`
7. **Assign Location** → `POST /api/containers/{id}/assign-location`
8. **Tạo Movement (In)** → `POST /api/movements`
9. **Tạo Release Order** → `POST /api/release-orders`
10. **Execute Release** → `POST /api/release-orders/{id}/execute`
11. **Kiểm tra lịch sử** → `GET /api/movements/by-container/{id}`

## 🐛 Troubleshooting

| Vấn đề | Giải pháp |
|--------|-----------|
| `Connection refused` | Đảm bảo `dotnet run` đang chạy và đúng port `5007` |
| 404 Not Found | Kiểm tra route đã đúng chưa; xem Swagger `/swagger` để verify |
| Database error | Kiểm tra MySQL đang chạy và connection string trong `appsettings.json` |
| Enum không hợp lệ | Truyền theo **số** (int), không phải string. Xem bảng Enum ở trên |

## 📚 Tài liệu liên quan

- Swagger UI: `http://localhost:5007/swagger`
- File hướng dẫn dự án: `../README.md` và `../HUONG_DAN_CODE_VA_QUAN_HE.txt`
- Controllers: `../Controllers/Api/`
- DTOs: `../Models/DTOs/`
