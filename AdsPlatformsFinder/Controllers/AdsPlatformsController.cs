using AdsPlatformsFinder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AdSpacesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdsPlatformsController : ControllerBase
    {
        private readonly Node _root;
        private readonly IMemoryCache _cache;

        public AdsPlatformsController(Node root, IMemoryCache cache)
        {
            _root = root;
            _cache = cache;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadAdsPlatforms(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            try
            {
                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);
                var lines = (await reader.ReadToEndAsync()).Split("\n").Select(x => x.Trim('\r')).ToArray();
                for (int i = 0; i < lines.Length; i++)
                {
                    var location = lines[i].Split(":")[0];
                    var paths = lines[i].Split(":")[1].Split(",").Where(x => x != "").ToList();

                    for (int k = 0; k < paths.Count; k++)
                    {
                        var path = paths[k].Split("/").Where(x => x != "").ToList();
                        _root.Processing(location, path);
                    }
                }

                _cache.Remove("ads_platforms_*");
                return Ok(_root);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("load")]
        public async Task<IActionResult> loadAdsPlatforms(string path)
        {
            if (path == null || path.Length == 0)
            {
                return BadRequest("not correct path");
            }

            try
            {
                var pathParts = path.Split("/").Where(x => x != "").ToList();
                var cacheKey = $"ads_platforms_{string.Join("_", pathParts)}";

                if (!_cache.TryGetValue(cacheKey, out List<string>? titles))
                {
                    titles = _root.GetTitles(pathParts);
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));
                    _cache.Set(cacheKey, titles, cacheEntryOptions);
                }

                return Ok(titles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}