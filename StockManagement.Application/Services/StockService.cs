using StockManagement.Application.Contracts;
using StockManagement.Application.Interfaces;
using StockManagement.Domain.Entities;
using StockManagement.Domain.Enums;

namespace StockManagement.Application.Services;

public class StockService : IStockService
{
    private readonly IProductRepository _productRepository;
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly IAlertRepository _alertRepository;

    public StockService(
        IProductRepository productRepository,
        IStockMovementRepository stockMovementRepository,
        IAlertRepository alertRepository)
    {
        _productRepository = productRepository;
        _stockMovementRepository = stockMovementRepository;
        _alertRepository = alertRepository;
    }

    public async Task<IReadOnlyList<StockMovementDto>> GetMovementsByProductIdAsync(Guid productId)
    {
        var movements = await _stockMovementRepository.GetByProductIdAsync(productId);
        return movements.Select(ToDto).ToArray();
    }

    public async Task<StockMovementDto> AddMovementAsync(Guid productId, StockMovementType type, decimal quantite, Guid userId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product is null)
            throw new InvalidOperationException("Produit introuvable.");

        if (quantite <= 0)
            throw new InvalidOperationException("Quantité invalide.");

        var signedDelta = type == StockMovementType.Entree ? quantite : -quantite;
        var newQuantity = product.Quantite + signedDelta;
        if (newQuantity < 0)
            throw new InvalidOperationException("Quantité en sortie invalide (stock insuffisant).");

        product.Quantite = newQuantity;
        await _productRepository.UpdateAsync(product);

        var movement = new StockMovement
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Type = type,
            Quantite = quantite,
            UserId = userId,
            DateUtc = DateTime.UtcNow
        };

        await _stockMovementRepository.AddAsync(movement);

        // Génération d'alertes de stock faible (simple au démarrage : on journalise à chaque mouvement).
        if (product.Quantite < product.SeuilMin)
        {
            var message =
                $"Stock faible: {product.Nom} (Réf: {product.Reference}). Niveau: {product.Quantite} {product.Unite}, seuil: {product.SeuilMin} {product.Unite}.";

            var alert = new Alert
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Type = AlertType.StockFaible,
                Message = message,
                DateUtc = DateTime.UtcNow
            };

            await _alertRepository.AddAsync(alert);
        }

        return new StockMovementDto(
            movement.Id,
            movement.ProductId,
            movement.Type,
            movement.Quantite,
            movement.DateUtc,
            movement.UserId);
    }

    public async Task<IReadOnlyList<LowStockProductDto>> GetLowStockProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        var low = products
            .Where(p => p.Quantite < p.SeuilMin)
            .Select(p => new LowStockProductDto(p.Id, p.Nom, p.Reference, p.Quantite, p.SeuilMin, p.Localisation))
            .ToArray();

        return low;
    }

    private static StockMovementDto ToDto(StockMovement m) =>
        new(
            m.Id,
            m.ProductId,
            m.Type,
            m.Quantite,
            m.DateUtc,
            m.UserId);
}

