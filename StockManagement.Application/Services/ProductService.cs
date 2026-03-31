using StockManagement.Application.Contracts;
using StockManagement.Application.Interfaces;
using StockManagement.Domain.Entities;

namespace StockManagement.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(ToDto).ToArray();
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product is null ? null : ToDto(product);
    }

    public async Task<ProductDto> CreateAsync(ProductCreateRequest request)
    {
        if (!await _categoryRepository.ExistsAsync(request.CategorieId))
            throw new InvalidOperationException("Catégorie introuvable.");

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Nom = request.Nom,
            Reference = request.Reference,
            CategorieId = request.CategorieId,
            Quantite = request.Quantite,
            Unite = request.Unite,
            Localisation = request.Localisation,
            SeuilMin = request.SeuilMin
        };

        await _productRepository.AddAsync(product);
        return ToDto(product);
    }

    public async Task<bool> UpdateAsync(Guid id, ProductUpdateRequest request)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product is null) return false;

        if (!await _categoryRepository.ExistsAsync(request.CategorieId))
            throw new InvalidOperationException("Catégorie introuvable.");

        product.Nom = request.Nom;
        product.Reference = request.Reference;
        product.CategorieId = request.CategorieId;
        product.Quantite = request.Quantite;
        product.Unite = request.Unite;
        product.Localisation = request.Localisation;
        product.SeuilMin = request.SeuilMin;

        await _productRepository.UpdateAsync(product);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product is null) return false;

        await _productRepository.DeleteAsync(product);
        return true;
    }

    private static ProductDto ToDto(Product p) =>
        new(
            p.Id,
            p.Nom,
            p.Reference,
            p.CategorieId,
            p.Quantite,
            p.Unite,
            p.Localisation,
            p.SeuilMin);
}

