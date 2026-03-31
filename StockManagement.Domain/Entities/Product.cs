namespace StockManagement.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }

    public string Nom { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;

    public Guid CategorieId { get; set; }
    public Category? Categorie { get; set; }

    // Quantité stockée (ex: tonnes, pièces, litres...).
    public decimal Quantite { get; set; }
    public string Unite { get; set; } = string.Empty;
    public string Localisation { get; set; } = string.Empty;

    public decimal SeuilMin { get; set; }
}

