using CityInfo.Models;

namespace CityInfo;
public class CitiesDataStore
{
    public List<CityDto> Cities { get; set; }

    public static CitiesDataStore Current { get; } = new CitiesDataStore();

    public CitiesDataStore()
    {
        Cities = new List<CityDto>() {
         new CityDto()
        {
            Id = 1,
            Name = "NYC",
            description = "the one with the big park",
            PointsOfInterest=new List<PointOfInterestDto>()
            {
                new PointOfInterestDto()
                {
                    Id=1,
                    Name="Central park",
                    Description="a nice park"
                },
                                
                new PointOfInterestDto()
                {
                    Id=2,
                    Name="Empire State Building",
                    Description="very tall building"
                }
            }
        },
        new CityDto()
        {
            Id = 2,
            Name = "Antwerp",
            description = "the one with the cathedral",
            PointsOfInterest=new List<PointOfInterestDto>()
            {
                new PointOfInterestDto()
                {
                    Id=1,
                    Name="a cathedral",
                    Description="a nice worship place"
                },

            
            }
        },
            new CityDto()
        {
            Id = 3,
            Name = "Paris",
            description = "the one with eiffel tour",
            PointsOfInterest=new List<PointOfInterestDto>()
            {
                new PointOfInterestDto()
                {
                    Id=1,
                    Name="eiffel tower",
                    Description="a tall metallic tower"
                },

                new PointOfInterestDto()
                {
                    Id=2,
                    Name="champs elyssé",
                    Description="very big street"
                }
            }
        }};






    }
}

