namespace CityInfo.Models;
    public class CityDto
    {
    public int Id { get; set; }
    public string Name { get; set; }=String.Empty;
    public string? description { get; set; }
    public int NumberOfPointsOfInterest {
        get
        { return PointsOfInterest.Count();
        }
    }

    public ICollection<PointOfInterestDto> PointsOfInterest { get; set; }=new List  <PointOfInterestDto>();

    
}

