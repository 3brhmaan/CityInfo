﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers;

[Route("api/v{version:apiVersion}/files")]
[ApiController]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

    public FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
    {
        _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider;
    }

    [HttpGet("{fileId}")]
    public ActionResult GetFile(string fileId)
    {
        var pathToFile = "TestFile.txt";

        if (!System.IO.File.Exists(pathToFile))
            return NotFound();

        if (!_fileExtensionContentTypeProvider.TryGetContentType(
                pathToFile, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        var bytes = System.IO.File.ReadAllBytes(pathToFile);

        return File(bytes, contentType , Path.GetFileName(pathToFile));
    }

    [HttpPost]
    public async Task<ActionResult> CreateFile(IFormFile file)
    {
        if (file.Length == 0 || file.Length > 20971520 || file.ContentType != "application.pdf")
            return BadRequest("No file or an invalid one has been inputted.");

        var path = Path.Combine(
            Directory.GetCurrentDirectory(),
            $"Uploaded_file_{Guid.NewGuid()}.pdf");

        using (var stream = new FileStream(path , FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Ok("Your file has been uploaded successfully");
    }
}