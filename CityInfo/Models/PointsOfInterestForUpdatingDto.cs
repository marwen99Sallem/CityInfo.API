using System.ComponentModel.DataAnnotations;

namespace CityInfo.Models;
public class PointsOfInterestForUpdatingDto
{

    [Required(ErrorMessage = "name field is required")]
    [MaxLength(50,ErrorMessage = "ceff field is too long.")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }
}
    
