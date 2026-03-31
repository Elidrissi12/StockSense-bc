using StockManagement.Application.Contracts;

namespace StockManagement.Application.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetAllAsync();
    Task<ProductDto?> GetByIdAsync(Guid id);
    Task<ProductDto> CreateAsync(ProductCreateRequest request);
    Task<bool> UpdateAsync(Guid id, ProductUpdateRequest request);
    Task<bool> DeleteAsync(Guid id);
}

