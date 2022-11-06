using AutoMapper;

namespace CityInfo.Profiles;
public class PointOfInterestProfile : Profile
{
    public PointOfInterestProfile()
    {
        CreateMap<Entities.PointOfInterest,Models.PointOfInterestDto>();
        CreateMap <Models.PointsOfInterestForCreationDto, Entities.PointOfInterest>();
        CreateMap<Models.PointsOfInterestForUpdatingDto, Entities.PointOfInterest>();
        CreateMap<Entities.PointOfInterest,Models.PointsOfInterestForUpdatingDto>();
      
    }

}
