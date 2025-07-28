using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Shapy.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Shapy.Infrastructure.Persistence;

namespace Shapy.API.Controllers
{
    [ApiController]
    [Route("api/project")]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly RepositoryContext _context;

        public ProjectController(RepositoryContext context)
        {
            _context = context;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? User.FindFirst("sub")?.Value
                   ?? User.FindFirst("userId")?.Value
                   ?? throw new UnauthorizedAccessException("User ID not found in token");
        }

        // GET: api/project
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProject()
        {
            var userId = GetCurrentUserId();

            return await _context.Project
                .Where(p => p.UserId == userId)
                .Include(p => p.User)
                .ToListAsync();
        }

        // GET: api/project/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(string id)
        {
            var userId = GetCurrentUserId();

            var project = await _context.Project
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        // GET: api/project/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ProjectListResponseDto>> GetProjectByUserId(string userId, [FromQuery] int skip = 0,
        [FromQuery] int limit = 10)
        {

            if (limit <= 0 || skip < 0)
            {
                return BadRequest("Invalid pagination parameters.");
            }

            var Project = await _context.Project
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .Skip(skip * limit)
                .Take(limit)
                .OrderByDescending(p => p.UpdatedAt)
                .ToListAsync();

            return new ProjectListResponseDto { Data = Project, NextPage = Project.Count == limit ? skip + 1 : null};
        }

        // POST: api/project
        [HttpPost]
        public async Task<ActionResult<Project>> CreateProject(CreateProjectDto createProjectDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();

            var project = new Project
            {
                Name = createProjectDto.Name,
                UserId = userId, // Use userId from JWT token
                Json = createProjectDto.Json,
                Height = createProjectDto.Height,
                Width = createProjectDto.Width,
                ThumbnailUrl = createProjectDto.ThumbnailUrl,
                IsTemplate = createProjectDto.IsTemplate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Project.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }

        // PUT: api/project/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(string id, UpdateProjectDto updateProjectDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();

            var project = await _context.Project
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (project == null)
            {
                return NotFound();
            }

            // Update properties
            project.Name = updateProjectDto.Name;
            project.Json = updateProjectDto.Json;
            project.Height = updateProjectDto.Height;
            project.Width = updateProjectDto.Width;
            project.ThumbnailUrl = updateProjectDto.ThumbnailUrl;
            project.IsTemplate = updateProjectDto.IsTemplate;
            project.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id, userId))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // PATCH: api/project/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchProject(string id, PatchProjectDto patchProjectDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();

            var project = await _context.Project
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (project == null)
            {
                return NotFound();
            }

            // Update only the properties that were provided
            if (patchProjectDto.Name != null)
            {
                project.Name = patchProjectDto.Name;
            }

            if (patchProjectDto.Json != null)
            {
                project.Json = patchProjectDto.Json;
            }

            if (patchProjectDto.Height.HasValue)
            {
                project.Height = patchProjectDto.Height.Value;
            }

            if (patchProjectDto.Width.HasValue)
            {
                project.Width = patchProjectDto.Width.Value;
            }

            if (patchProjectDto.ThumbnailUrl != null)
            {
                project.ThumbnailUrl = patchProjectDto.ThumbnailUrl;
            }

            if (patchProjectDto.IsTemplate.HasValue)
            {
                project.IsTemplate = patchProjectDto.IsTemplate.Value;
            }

            // Always update the UpdatedAt timestamp
            project.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id, userId))
                {
                    return NotFound();
                }
                throw;
            }

            return Ok();
        }

        // DELETE: api/project/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(string id)
        {
            var userId = GetCurrentUserId();

            var project = await _context.Project
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (project == null)
            {
                return NotFound();
            }

            _context.Project.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjectExists(string id, string userId)
        {
            return _context.Project.Any(e => e.Id == id && e.UserId == userId);
        }
    }

    // DTOs for request/response
    public class CreateProjectDto
    {
        [Required]
        public required string Name { get; set; }

        [Required(AllowEmptyStrings = true)]
        public required string Json { get; set; }

        [Required]
        public int Height { get; set; }

        [Required]
        public int Width { get; set; }

        public string? ThumbnailUrl { get; set; }

        public bool? IsTemplate { get; set; }
    }

    public class UpdateProjectDto
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Json { get; set; }

        [Required]
        public int Height { get; set; }

        [Required]
        public int Width { get; set; }

        public string? ThumbnailUrl { get; set; }

        public bool? IsTemplate { get; set; }
    }

    public class PatchProjectDto
    {
        public string? Name { get; set; }

        public string? Json { get; set; }

        public int? Height { get; set; }

        public int? Width { get; set; }

        public string? ThumbnailUrl { get; set; }

        public bool? IsTemplate { get; set; }
    }

    public class ProjectListResponseDto
    {
        public required List<Project> Data { get; set; }
        public long? NextPage { get; set; }
        
    }
}