using CityInfo.DbContexts;
using CityInfo.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.Services
{
    //the place where we define persistence logic
    //the consumer has no interest in knowing how icityinforepository is implemented
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _cityInfoContext;

        public CityInfoRepository(CityInfoContext cityInfoContext)
        {
            _cityInfoContext = cityInfoContext ?? throw new ArgumentNullException(nameof(cityInfoContext));
        }

        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _cityInfoContext.Cities.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name,string? searchQuery,int pageSize,int pageNumber)
        {
           
           
            //collection to start from
            var collection=_cityInfoContext.Cities as IQueryable<City>;

            if(!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
               collection=collection.Where(c => c.Name == name);
            }
            if(!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection=collection.Where(a=>a.Name.Contains(searchQuery) 
                ||  (a.Description!=null && a.Description.Contains(searchQuery) ));
            }

            var totalItemCount = await collection.CountAsync();

            var metaData = new PaginationMetadata(totalItemCount, pageSize, pageNumber) ;
           var collectionToReturn=  await collection.OrderBy(c => c.Name)
                .Skip(pageSize*(pageNumber-1))
                .Take(pageSize)
                .ToListAsync();

            return (collectionToReturn, metaData);
        }


        public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId, false);
            if (city != null)
                city.PointsOfInterest.Add(pointOfInterest); //this will make sure that the foreign key persists to the cityId when persisting
            //the last add method is not an async call because it doesn't do an I/O operation
        }
        public async Task<City?> GetCityAsync(
          int cityid
          , bool includePointOfInterest)
        {
            if (includePointOfInterest)
            {
                return await _cityInfoContext.Cities.Include(c => c.PointsOfInterest)
                    .Where(c => c.Id == cityid).FirstOrDefaultAsync();
            }

            return await _cityInfoContext.Cities
                .Where(c => c.Id == cityid).FirstOrDefaultAsync();
        }

        public Task<bool> checkCityExistsAsync(int cityId)
        {
            return _cityInfoContext.Cities.AnyAsync(c => c.Id == cityId);
        }


        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(
            int cityId,
            int pointOfInterestId)
        {
            return await _cityInfoContext.PointsOfInterest.Where(p =>

                p.Id == pointOfInterestId && cityId == p.CityId).FirstOrDefaultAsync();
        }


        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
        {
            return await _cityInfoContext.PointsOfInterest.Where(p => p.CityId == cityId).ToListAsync();
        }


        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {


            _cityInfoContext.PointsOfInterest.Remove(pointOfInterest);
        }


        public async Task<bool> SaveChangesAsync()
        {
            return (await _cityInfoContext.SaveChangesAsync() >= 0);

        }

        public async Task<bool> CityNameMatchesCityId(string? cityName, int cityId)
        {
            return await _cityInfoContext.Cities.AnyAsync(c => c.Id == cityId && c.Name == cityName);
        }
    }
}
