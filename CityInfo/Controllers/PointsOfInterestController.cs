using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CityInfo.Models;
using CityInfo;
using Microsoft.AspNetCore.JsonPatch;
using CityInfo.Services;
using AutoMapper;
using CityInfo.Entities;

namespace CityInfo.Controllers;

[ApiController]
[Route("api/cities/{cityId}/pointsofinterest")]
public class PointsOfInterestController : ControllerBase
{



    private readonly ILogger<PointsOfInterestController> _logger;
    private readonly IMailService _mailService;
    private readonly IMapper _mapper;
    private readonly ICityInfoRepository _cityInfoRepository;

    //constructor injection
    public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, ICityInfoRepository cityInfoRepository, IMapper mapper)
    {
        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
        _mailService = mailService
            ?? throw new ArgumentNullException(nameof(mailService));
        _mapper = mapper
             ?? throw new ArgumentNullException(nameof(mapper));
        _cityInfoRepository = cityInfoRepository
            ?? throw new ArgumentNullException(nameof(cityInfoRepository));

    }



    [HttpGet]
    public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> getPointsOfInterest(int cityId)
    {

        if (!await _cityInfoRepository.checkCityExistsAsync(cityId))
        {

            _logger.LogInformation($"city with city id {cityId} is not found");
            return NotFound();
        }


        var pointsOfInterest = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);

        return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterest));


    }

    [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
    public async Task<ActionResult<PointOfInterestDto>> getPointOfInterest(int cityId, int pointOfInterestId)
    {



        if (!await _cityInfoRepository.checkCityExistsAsync(cityId))
            return NotFound();

        var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
        if (pointOfInterest == null)
            return NotFound();

        return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
    }


    [HttpPost]
    public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(
        int CityId,
        PointsOfInterestForCreationDto pointOfInterest)
    {

        if (!await _cityInfoRepository.checkCityExistsAsync(CityId))
        {
            return NotFound();
        }

        var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

        await _cityInfoRepository.AddPointOfInterestForCityAsync(CityId, finalPointOfInterest);
        await _cityInfoRepository.SaveChangesAsync(); // it persists data in the dataBase

        var createdPointOfInterestToReturn = _mapper.Map<PointOfInterestDto>(finalPointOfInterest);
        return CreatedAtRoute("GetPointOfInterest",
         new
         {
             cityId = CityId,
             pointOfInterestId = createdPointOfInterestToReturn.Id
         },
         createdPointOfInterestToReturn);



    }



    [HttpPut("{pointOfInterestId}")]
    public async Task<ActionResult> UpdatePointofInterest(
        int cityId,
        int pointOfInterestId,
        PointsOfInterestForUpdatingDto pointOfInterest
        )
    {

       if(! await _cityInfoRepository.checkCityExistsAsync(cityId))
        return NotFound();

        var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
        if(pointOfInterestEntity == null) return NotFound();
        _mapper.Map(pointOfInterest, pointOfInterestEntity); //overrides the values in pointOfInterestEntity with the ones from pointOfInterest 

       await _cityInfoRepository.SaveChangesAsync();

        return NoContent();



    }
    //partially updating a ressource allow partial updates
    [HttpPatch("{pointofinterestid}")]
    public  async Task<ActionResult> PartiallyUpdatePointOfInterest(
        int cityId, int pointOfInterestId,
        JsonPatchDocument<PointsOfInterestForUpdatingDto> patchDocument)
    {

        if (!await _cityInfoRepository.checkCityExistsAsync(cityId))
            return NotFound();

        var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId,pointOfInterestId);

        if (pointOfInterestEntity == null)
            return NotFound();

        PointsOfInterestForUpdatingDto pointOfInterestToPatch = _mapper.Map<PointsOfInterestForUpdatingDto>(pointOfInterestEntity);

        patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        if (!TryValidateModel(pointOfInterestToPatch))
            return BadRequest(ModelState);

        _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);

        await _cityInfoRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{pointofinterestid}")]
    public async Task<ActionResult> DeletePointOfInterest(
        int cityId,
        int pointOfInterestId)
    {

        if (!await _cityInfoRepository.checkCityExistsAsync(cityId))
            return NotFound();

        var pointOfInterestEntity =await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId,pointOfInterestId);

        if (pointOfInterestEntity == null)
            return NotFound();
      

         _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);
        await _cityInfoRepository.SaveChangesAsync();
        _mailService.Send("Point of interest deleted",
            $"Point  of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted");
        return NoContent();
    }


}
