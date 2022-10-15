﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CityInfo.Models;
using CityInfo;
using Microsoft.AspNetCore.JsonPatch;

namespace CityInfo.Controllers;

[ApiController]
[Route("api/cities/{cityId}/pointsofinterest")]
public class PointsOfInterestController : ControllerBase
{

    IEnumerable<PointOfInterestDto> l = new List<PointOfInterestDto>();


    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestDto>> getPointsOfInterest(int cityId)
    {

        var city = CitiesDataStore.Current.Cities.FirstOrDefault(elt => elt.Id == cityId);

        if (city == null) return NotFound();


        return Ok(city.PointsOfInterest);

    }

    [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
    public ActionResult<PointOfInterestDto> getPointOfInterest(int cityId, int pointOfInterestId)
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(elt => elt.Id == cityId);

        if (city == null) return NotFound();

        var pointOfInterest = city.PointsOfInterest.FirstOrDefault(pt => pt.Id == pointOfInterestId);

        if (pointOfInterest == null) return NotFound();

        return Ok(pointOfInterest);
    }


    [HttpPost]
    public ActionResult<PointOfInterestDto> CreatePointOfInterest(
        int CityId,
        PointsOfInterestForCreationDto pointOfInterest)
    {


        var city = CitiesDataStore.Current.Cities.FirstOrDefault(elt => elt.Id == CityId);

        if (city == null) return NotFound();

        var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(
            c => c.PointsOfInterest).Max(p => p.Id);

        var finalPointOfInterest = new PointOfInterestDto
        {
            Id = ++maxPointOfInterestId,
            Name = pointOfInterest.Name,
            Description = pointOfInterest.Description,
        };

        city.PointsOfInterest.Add(finalPointOfInterest);


        return CreatedAtRoute("GetPointOfInterest",
            new
            {
                cityId = CityId,
                pointOfInterestId = finalPointOfInterest.Id
            },
            finalPointOfInterest);

    }


    [HttpPut("{pointOfInterestId}")]
    public ActionResult UpdatePointofInterest(
        int cityId,
        int pointOfInterestId,
        PointsOfInterestForUpdatingDto pointOfInterest
        )
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(elt => elt.Id == cityId);
        if (city == null) return NotFound();

        var pointOfInterestToUpdate = city.PointsOfInterest.FirstOrDefault(elt => elt.Id == pointOfInterestId);
        if (pointOfInterestToUpdate == null) return NotFound();

        pointOfInterestToUpdate.Name = pointOfInterest.Name;
        pointOfInterestToUpdate.Description = pointOfInterest.Description;
        return NoContent();


    }
    //partially updating a ressource allow partial updates
    [HttpPatch("{pointofinterestid}")]
    public ActionResult PartiallyUpdatePointOfInterest(
        int cityId, int pointOfInterestId,
        JsonPatchDocument<PointsOfInterestForUpdatingDto> patchDocument)
    {

        var city = CitiesDataStore.Current.Cities.FirstOrDefault(elt => elt.Id == cityId);
        if (city == null) return NotFound();

        var pointOfInterestToUpdate = city.PointsOfInterest.FirstOrDefault(elt => elt.Id == pointOfInterestId);
        if (pointOfInterestToUpdate == null) return NotFound();

        var pointOfInterestToPatch = new PointsOfInterestForUpdatingDto()
        {
            Name = pointOfInterestToUpdate.Name,
            Description = pointOfInterestToUpdate.Description
        };
        patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (!TryValidateModel(pointOfInterestToPatch)) return BadRequest(ModelState);

        pointOfInterestToUpdate.Name = pointOfInterestToPatch.Name;
        pointOfInterestToUpdate.Description = pointOfInterestToPatch.Description;

        return NoContent();

    }

    [HttpDelete("{pointofinterestid}")]
    public ActionResult DeletePointOfInterest(
        int cityId,
        int pointOfInterestId)
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(elt => elt.Id == cityId);
        if (city == null) return NotFound();

        var pointOfInterestToDelete = city.PointsOfInterest.FirstOrDefault(elt => elt.Id == pointOfInterestId);
        if (pointOfInterestToDelete == null) return NotFound();

        city.PointsOfInterest.Remove(pointOfInterestToDelete);
        return NoContent();
    }


}