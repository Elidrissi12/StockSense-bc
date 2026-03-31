using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Application.Contracts;
using StockManagement.Application.Interfaces;

namespace StockManagement.API.Controllers;

[ApiController]
[Route("api/files")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly IFileStorageService _fileStorageService;

    public FilesController(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    [HttpPost("upload")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<ActionResult<StoredFileMetadataDto>> Upload([FromForm] IFormFile file, [FromForm] string tag = "general")
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "Aucun fichier reçu." });

        await using var ms = new MemoryStream();
        await file.CopyToAsync(ms);

        try
        {
            var created = await _fileStorageService.SaveAsync(new StoredFileCreateRequest(
                file.FileName,
                file.ContentType,
                ms.ToArray(),
                tag,
                GetCurrentUserIdOrNull()));

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StoredFileMetadataDto>> GetById([FromRoute] Guid id)
    {
        var metadata = await _fileStorageService.GetMetadataAsync(id);
        if (metadata is null) return NotFound();
        return Ok(metadata);
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download([FromRoute] Guid id)
    {
        var file = await _fileStorageService.DownloadAsync(id);
        if (file is null) return NotFound();
        return File(file.Data, file.ContentType, file.FileName);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Responsable")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var deleted = await _fileStorageService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    private Guid? GetCurrentUserIdOrNull()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(raw, out var userId) ? userId : null;
    }
}

