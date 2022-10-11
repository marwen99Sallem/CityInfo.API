using CityInfo.Models;

namespace CityInfo;
    public class CititesDataStore
    {
    public List<CityDto> Cities { get; set; }

    public static  CititesDataStore Current { get; }=new CititesDataStore();

    public CititesDataStore()
    {
        Cities = new List<CityDto>() {
         new CityDto()
        {
            Id = 1,
            Name = "NYC",
            description = "the one with the big park"
        },
        new CityDto()
        {
            Id = 2,
            Name = "Antwerp",
            description = "the one with the cathedral"
        },
            new CityDto()
        {
            Id = 3,
            Name = "Paris",
            description = "the one with eiffel tour"
        }};

       




    }
    }

