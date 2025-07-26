using AdsPlatformsFinder.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdSpacesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdsPlatformsController : ControllerBase
    {
        private readonly Node _root;

        public AdsPlatformsController(Node root)
        {
            _root = root;
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

                return Ok(_root);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("load")]
        public async Task<IActionResult> loadAdsPlatforms(string path)
        {
            if (path == null || path.Length == 0)
            {
                return BadRequest("not correct path");
            }

            try
            {
                var pathParts = path.Split("/").Where(x => x != "").ToList();

                return Ok(_root.GetTitles(pathParts));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}