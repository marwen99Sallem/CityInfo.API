namespace CityInfo.Models;
   

/// <summary>
/// A DTO for a city without points of interest
/// </summary>
public class CityWithoutPointsOfInterestDto
{
    /// <summary>
    /// the id of the city 
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// the name of the city 
    /// </summary>
    public string Name { get; set; }=String.Empty;
    /// <summary>
    /// the description of the city
    /// </summary>
    public string? description { get; set; }
}

