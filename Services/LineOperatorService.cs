using System.Linq.Expressions;
using DEPOTCONTAINER.Models.DTOs;
using DEPOTCONTAINER.Models.Entities;
using DEPOTCONTAINER.Repositories.Interfaces;
using DEPOTCONTAINER.Services.Interfaces;

namespace DEPOTCONTAINER.Services;

/// <summary>
/// Service cho LineOperator.
/// </summary>
public class LineOperatorService : BaseService, ILineOperatorService
{
    private readonly IUnitOfWork _uow;

    public LineOperatorService(IUnitOfWork uow) => _uow = uow;

    public async Task<ApiResponse<PagedResult<LineOperatorDto>>> GetPagedAsync(QueryParameters parameters, CancellationToken cancellationToken = default)
    {
        Expression<Func<LineOperator, bool>>? predicate = null;
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var search = parameters.SearchTerm.Trim().ToLower();
            predicate = l => l.OwnerCode.ToLower().Contains(search)
                         || l.Name.ToLower().Contains(search);
        }

        Func<IQueryable<LineOperator>, IOrderedQueryable<LineOperator>>? orderBy = parameters.SortBy?.ToLower() switch
        {
            "code" => q => parameters.SortDescending ? q.OrderByDescending(l => l.OwnerCode) : q.OrderBy(l => l.OwnerCode),
            "name" => q => parameters.SortDescending ? q.OrderByDescending(l => l.Name) : q.OrderBy(l => l.Name),
            _ => q => q.OrderBy(l => l.OwnerCode)
        };

        var paged = await _uow.LineOperators.GetPagedAsync(
            parameters.PageNumber, parameters.PageSize, predicate, orderBy, cancellationToken);

        var items = paged.Items.Select(LineOperatorDto.FromEntity).ToList();
        var result = new PagedResult<LineOperatorDto>
        {
            Items = items,
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
        return Success(result);
    }

    public async Task<ApiResponse<LineOperatorDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _uow.LineOperators.GetByIdAsync(id, cancellationToken);
        if (entity == null) return Failure<LineOperatorDto>($"Không tìm thấy Line Operator Id={id}");
        return Success(LineOperatorDto.FromEntity(entity));
    }

    public async Task<ApiResponse<LineOperatorDto>> CreateAsync(LineOperatorDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await _uow.LineOperators.GetByOwnerCodeAsync(dto.OwnerCode, cancellationToken);
        if (existing != null)
            return Failure<LineOperatorDto>($"Owner Code '{dto.OwnerCode}' đã tồn tại");

        var entity = new LineOperator
        {
            OwnerCode = dto.OwnerCode.ToUpper(),
            Name = dto.Name,
            TaxCode = dto.TaxCode,
            Address = dto.Address,
            Phone = dto.Phone,
            Email = dto.Email,
            Note = dto.Note
        };
        await _uow.LineOperators.AddAsync(entity, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return Success(LineOperatorDto.FromEntity(entity), "Tạo Line Operator thành công");
    }

    public async Task<ApiResponse<LineOperatorDto>> UpdateAsync(int id, LineOperatorDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _uow.LineOperators.GetByIdAsync(id, cancellationToken);
        if (entity == null) return Failure<LineOperatorDto>($"Không tìm thấy Line Operator Id={id}");

        entity.OwnerCode = dto.OwnerCode.ToUpper();
        entity.Name = dto.Name;
        entity.TaxCode = dto.TaxCode;
        entity.Address = dto.Address;
        entity.Phone = dto.Phone;
        entity.Email = dto.Email;
        entity.Note = dto.Note;

        _uow.LineOperators.Update(entity);
        await _uow.SaveChangesAsync(cancellationToken);
        return Success(LineOperatorDto.FromEntity(entity), "Cập nhật thành công");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _uow.LineOperators.GetByIdAsync(id, cancellationToken);
        if (entity == null) return Failure<bool>($"Không tìm thấy Line Operator Id={id}");

        _uow.LineOperators.Remove(entity);
        await _uow.SaveChangesAsync(cancellationToken);
        return Success(true, "Xóa thành công");
    }
}

/// <summary>
/// Service cho Customer.
/// </summary>
public class CustomerService : BaseService, ICustomerService
{
    private readonly IUnitOfWork _uow;

    public CustomerService(IUnitOfWork uow) => _uow = uow;

    public async Task<ApiResponse<PagedResult<CustomerDto>>> GetPagedAsync(QueryParameters parameters, CancellationToken cancellationToken = default)
    {
        Expression<Func<Customer, bool>>? predicate = null;
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var search = parameters.SearchTerm.Trim().ToLower();
            predicate = c => c.TaxCode.ToLower().Contains(search) || c.Name.ToLower().Contains(search);
        }

        Func<IQueryable<Customer>, IOrderedQueryable<Customer>>? orderBy = parameters.SortBy?.ToLower() switch
        {
            "name" => q => parameters.SortDescending ? q.OrderByDescending(c => c.Name) : q.OrderBy(c => c.Name),
            "tax" => q => parameters.SortDescending ? q.OrderByDescending(c => c.TaxCode) : q.OrderBy(c => c.TaxCode),
            _ => q => q.OrderBy(c => c.Name)
        };

        var paged = await _uow.Customers.GetPagedAsync(
            parameters.PageNumber, parameters.PageSize, predicate, orderBy, cancellationToken);

        var items = paged.Items.Select(CustomerDto.FromEntity).ToList();
        var result = new PagedResult<CustomerDto>
        {
            Items = items,
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
        return Success(result);
    }

    public async Task<ApiResponse<CustomerDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _uow.Customers.GetByIdAsync(id, cancellationToken);
        if (entity == null) return Failure<CustomerDto>($"Không tìm thấy Customer Id={id}");
        return Success(CustomerDto.FromEntity(entity));
    }

    public async Task<ApiResponse<CustomerDto>> CreateAsync(CustomerDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await _uow.Customers.GetByTaxCodeAsync(dto.TaxCode, cancellationToken);
        if (existing != null)
            return Failure<CustomerDto>($"MST '{dto.TaxCode}' đã tồn tại");

        var entity = new Customer
        {
            TaxCode = dto.TaxCode,
            Name = dto.Name,
            Address = dto.Address,
            Phone = dto.Phone,
            ContactPerson = dto.ContactPerson,
            Note = dto.Note
        };
        await _uow.Customers.AddAsync(entity, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return Success(CustomerDto.FromEntity(entity), "Tạo Customer thành công");
    }

    public async Task<ApiResponse<CustomerDto>> UpdateAsync(int id, CustomerDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _uow.Customers.GetByIdAsync(id, cancellationToken);
        if (entity == null) return Failure<CustomerDto>($"Không tìm thấy Customer Id={id}");

        entity.TaxCode = dto.TaxCode;
        entity.Name = dto.Name;
        entity.Address = dto.Address;
        entity.Phone = dto.Phone;
        entity.ContactPerson = dto.ContactPerson;
        entity.Note = dto.Note;

        _uow.Customers.Update(entity);
        await _uow.SaveChangesAsync(cancellationToken);
        return Success(CustomerDto.FromEntity(entity), "Cập nhật thành công");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _uow.Customers.GetByIdAsync(id, cancellationToken);
        if (entity == null) return Failure<bool>($"Không tìm thấy Customer Id={id}");

        _uow.Customers.Remove(entity);
        await _uow.SaveChangesAsync(cancellationToken);
        return Success(true, "Xóa thành công");
    }
}