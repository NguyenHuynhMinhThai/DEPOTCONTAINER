-- =====================================================================
-- Script khởi tạo database cho hệ thống Depot Container
-- Chạy tự động bởi Docker khi volume MySQL trống (lần đầu tiên)
-- =====================================================================

-- Đảm bảo database depotdb tồn tại (MYSQL_DATABASE đã tạo sẵn, nhưng tạo lại cho chắc)
CREATE DATABASE IF NOT EXISTS depotdb
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE depotdb;

-- Cấp quyền cho user app (backup cho root)
GRANT ALL PRIVILEGES ON depotdb.* TO 'depot_user'@'%';
FLUSH PRIVILEGES;

-- =====================================================================
-- Lưu ý: Toàn bộ schema + seed data sẽ được EF Core tự động tạo
-- thông qua EnsureCreated() + DbSeeder trong Program.cs.
-- File này chỉ cần thiết để:
--   1. Đảm bảo database tồn tại
--   2. Cấp quyền cho user app
-- =====================================================================