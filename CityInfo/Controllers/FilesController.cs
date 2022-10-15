using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.Controllers;

[Route("api/files")]
[ApiController]
public class FilesController : ControllerBase
{

    private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

    public FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
    {
        _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider
            ?? throw new ArgumentNullException(
                nameof(fileExtensionContentTypeProvider));

    }

    [HttpGet("{fileId}")]
    public ActionResult GetFile(string fileId)
{
        var pathToFile = "asp.net core tutorial- YouTube.html";

        //check whether file exists
        if(!System.IO.File.Exists(pathToFile))
        {
            return NotFound();
        }

        if(!_fileExtensionContentTypeProvider.TryGetContentType(pathToFile,out var contentType))
        {
            contentType = "application/octet-stream";
        }

        var bytes = System.IO.File.ReadAllBytes(pathToFile);

        return File(bytes,contentType,Path.GetFileName(pathToFile)) ;

}
    }

