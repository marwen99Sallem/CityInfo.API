using CityInfo.Entities;

namespace CityInfo.Services;
//this is contract
public interface ICityInfoRepository
{
    Task<IEnumerable<City>> GetCitiesAsync();
    Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, int pageSize, int pageNumber);


    Task<City?> GetCityAsync(int cityid, bool includePointOfInterest);

    Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);

    Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId);
    Task<bool> checkCityExistsAsync(int cityId);

    Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest);

    void DeletePointOfInterest(PointOfInterest pointOfInterest);// delete is not an I/O operation it's an in-memory operation that's why we don't need it to be async

    Task<bool> CityNameMatchesCityId(string? cityName, int cityId);
    Task<bool> SaveChangesAsync();


}
