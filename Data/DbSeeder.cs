using DEPOTCONTAINER.Data;
using DEPOTCONTAINER.Models.Entities;
using DEPOTCONTAINER.Models.Enums;
using DEPOTCONTAINER.Repositories.Interfaces;
using DEPOTCONTAINER.Validators;
using Microsoft.EntityFrameworkCore;

namespace DEPOTCONTAINER.Data;

/// <summary>
/// Seed dữ liệu mẫu cho database.
/// Giúp người mới có thể chạy thử ngay.
/// </summary>
public static class DbSeeder
{
    public static void Seed(DepotDbContext context)
    {
        // Seed Line Operators
        if (!context.LineOperators.Any())
        {
            var operators = new[]
            {
                new LineOperator { OwnerCode = "CMA", Name = "CMA CGM", TaxCode = "0301234567", Address = "Tân Cảng Sài Gòn", Phone = "028-1234-5678" },
                new LineOperator { OwnerCode = "MSC", Name = "Mediterranean Shipping Company", TaxCode = "0302345678", Address = "Bình Thạnh, TP.HCM", Phone = "028-2345-6789" },
                new LineOperator { OwnerCode = "HMM", Name = "Hyundai Merchant Marine", TaxCode = "0303456789", Address = "Quận 1, TP.HCM", Phone = "028-3456-7890" },
                new LineOperator { OwnerCode = "MAE", Name = "Maersk Line", TaxCode = "0304567890", Address = "Thủ Đức, TP.HCM", Phone = "028-4567-8901" }
            };
            context.LineOperators.AddRange(operators);
            context.SaveChanges();
        }

        // Seed Customers
        if (!context.Customers.Any())
        {
            var customers = new[]
            {
                new Customer { TaxCode = "0301123456", Name = "Cty TNHH Vận Tải Hòa Phát", Address = "Quận 7, TP.HCM", Phone = "028-9876-5432", ContactPerson = "Nguyễn Văn A" },
                new Customer { TaxCode = "0302234567", Name = "Cty CP XNK Thành Công", Address = "Bình Dương", Phone = "0274-123-456", ContactPerson = "Trần Thị B" },
                new Customer { TaxCode = "0303345678", Name = "Cty CP Logistics Việt Nam", Address = "Hà Nội", Phone = "024-555-6666", ContactPerson = "Lê Văn C" }
            };
            context.Customers.AddRange(customers);
            context.SaveChanges();
        }

        // Seed Blocks
        if (!context.Blocks.Any())
        {
            var blocks = new[]
            {
                new Block
                {
                    Code = "A", Name = "Khu A - Container 20ft",
                    BlockType = BlockType.Physical,
                    MaxBays = 4, MaxRows = 3, MaxTiers = 4,
                    MaxContainerSize = ContainerSize.Size20,
                    Description = "Khu chứa container 20ft",
                    IsActive = true
                },
                new Block
                {
                    Code = "B", Name = "Khu B - Container 40ft",
                    BlockType = BlockType.Physical,
                    MaxBays = 4, MaxRows = 3, MaxTiers = 4,
                    MaxContainerSize = ContainerSize.Size40,
                    Description = "Khu chứa container 40ft",
                    IsActive = true
                },
                new Block
                {
                    Code = "V-01", Name = "Block ảo - Hàng hư hỏng",
                    BlockType = BlockType.Virtual,
                    Description = "Block ảo dùng cho container hư hỏng cần sửa chữa",
                    IsActive = true
                }
            };
            context.Blocks.AddRange(blocks);
            context.SaveChanges();

            // Sinh Bay/Row/Tier cho các Physical blocks
            foreach (var block in blocks.Where(b => b.BlockType == BlockType.Physical))
            {
                DEPOTCONTAINER.Factories.BayFactory.CreateBaysForBlock(block, block.MaxBays!.Value, block.MaxRows!.Value, block.MaxTiers!.Value)
                    .ForEach(b => context.Bays.Add(b));
                context.SaveChanges();

                foreach (var bay in context.Bays.Where(b => b.BlockId == block.Id).ToList())
                {
                    for (int r = 1; r <= block.MaxRows!.Value; r++)
                    {
                        var row = new Row { BayId = bay.Id, RowNumber = r, MaxTiers = block.MaxTiers!.Value };
                        context.Rows.Add(row);
                        context.SaveChanges();

                        for (int t = 1; t <= block.MaxTiers!.Value; t++)
                        {
                            context.Tiers.Add(new Tier { RowId = row.Id, TierNumber = t, IsOccupied = false });
                        }
                    }
                }
                context.SaveChanges();
            }
        }

        // Seed Containers mẫu (số container theo ISO 6346)
        if (!context.Containers.Any())
        {
            var operators = context.LineOperators.ToList();
            // Số container mẫu: CMAU1234567 - check digit tính theo Modulo 11
            var cma = operators.FirstOrDefault(o => o.OwnerCode == "CMA");
            var msc = operators.FirstOrDefault(o => o.OwnerCode == "MSC");

            var containers = new[]
            {
                new Container
                {
                    ContainerNumber = "CMAU1234567",
                    ContainerType = ContainerType.Dry,
                    IsoCode = "22G1",
                    Size = ContainerSize.Size20,
                    MaxWeight = 30480m,
                    TareWeight = 2230m,
                    ManufactureDate = DateTime.UtcNow.AddYears(-3),
                    LineOperatorId = cma?.Id,
                    Condition = ContainerCondition.Normal,
                    Category = ContainerCategory.CategoryA,
                    IsInYard = false
                },
                new Container
                {
                    ContainerNumber = "MSCU7654321",
                    ContainerType = ContainerType.Reefer,
                    IsoCode = "45R1",
                    Size = ContainerSize.Size40,
                    MaxWeight = 32500m,
                    TareWeight = 4500m,
                    ManufactureDate = DateTime.UtcNow.AddYears(-2),
                    LineOperatorId = msc?.Id,
                    Condition = ContainerCondition.Normal,
                    Category = ContainerCategory.CategoryA,
                    IsInYard = false
                }
            };

            // Validate trước khi insert
            foreach (var c in containers)
            {
                var (isValid, err) = ContainerNumberValidator.ValidateWithMessage(c.ContainerNumber);
                if (isValid)
                    context.Containers.Add(c);
                else
                    Console.WriteLine($"[Seeder] Bỏ qua container không hợp lệ: {c.ContainerNumber} - {err}");
            }
            context.SaveChanges();
        }

        Console.WriteLine($"[Seeder] Đã seed: {context.LineOperators.Count()} LineOperators, {context.Customers.Count()} Customers, " +
                          $"{context.Blocks.Count()} Blocks, {context.Containers.Count()} Containers");
    }
}