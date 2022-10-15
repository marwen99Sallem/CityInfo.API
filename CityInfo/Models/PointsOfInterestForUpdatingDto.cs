using System.ComponentModel.DataAnnotations;

namespace CityInfo.Models;
public class PointsOfInterestForUpdatingDto
{

    [Required(ErrorMessage = "ceff field is too long.")]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }
}
    
