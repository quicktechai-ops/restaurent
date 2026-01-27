using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/[controller]")]
[Authorize(Roles = "CompanyAdmin,User")]
public class UploadController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public UploadController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpPost("image")]
    public async Task<ActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file uploaded" });

        // Validate file type
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
            return BadRequest(new { message = "Invalid file type. Only JPEG, PNG, GIF, and WebP are allowed." });

        // Max 5MB
        if (file.Length > 5 * 1024 * 1024)
            return BadRequest(new { message = "File size must be less than 5MB" });

        try
        {
            var apiKey = _configuration["ImgBB:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                return StatusCode(500, new { message = "ImgBB API key not configured" });

            // Convert file to base64
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var base64Image = Convert.ToBase64String(memoryStream.ToArray());

            // Upload to ImgBB
            var formContent = new MultipartFormDataContent();
            formContent.Add(new StringContent(apiKey), "key");
            formContent.Add(new StringContent(base64Image), "image");

            var response = await _httpClient.PostAsync("https://api.imgbb.com/1/upload", formContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return StatusCode(500, new { message = "Failed to upload image to CDN", details = responseContent });

            var jsonDoc = JsonDocument.Parse(responseContent);
            var imageUrl = jsonDoc.RootElement.GetProperty("data").GetProperty("url").GetString();
            var thumbnailUrl = jsonDoc.RootElement.GetProperty("data").GetProperty("thumb").GetProperty("url").GetString();
            var deleteUrl = jsonDoc.RootElement.GetProperty("data").GetProperty("delete_url").GetString();

            return Ok(new 
            { 
                url = imageUrl,
                thumbnail = thumbnailUrl,
                deleteUrl = deleteUrl,
                message = "Image uploaded successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error uploading image", details = ex.Message });
        }
    }
}
