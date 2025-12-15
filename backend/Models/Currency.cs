using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

[Table("currencies")]
public class Currency
{
    [Key]
    [Column("currency_code")]
    [MaxLength(3)]
    public string CurrencyCode { get; set; } = string.Empty;

    [Column("name")]
    [MaxLength(50)]
    public string? Name { get; set; }

    [Column("symbol")]
    [MaxLength(10)]
    public string? Symbol { get; set; }

    [Column("decimal_places")]
    public short DecimalPlaces { get; set; }

    [Column("is_default")]
    public bool IsDefault { get; set; } = false;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;
}
