using GreenSagaAPI.Context;
using GreenSagaAPI.Models;
using GreenSagaAPI.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GreenSagaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly AppDbContext _authContext;

        private projectService _projectService;

   
        public ProjectController(AppDbContext appDbContext)
        {

            _authContext = appDbContext;
        }

        [HttpPost("projectCreate")]
        public async Task<IActionResult> CreateProject([FromBody] cultivationProjects projectObj)
        {
            if (projectObj == null)
                return BadRequest();

            await _authContext.Projects.AddAsync(projectObj);
            await _authContext.SaveChangesAsync();
            return Ok(new
            {
                Message = "Project Create Sucess!"
            });

        }


        [HttpGet("project/{id?}/{userID}")]
        public async Task<ActionResult<cultivationProjects>> project(int? id,int userID)
        {
           // var projects = _projectService.GetCultivationProjects().Where(p => p.Id == id);
            if (id is null)
            {
                return BadRequest("can't pass null values");
            }

            else if (id == 0)
            {
                return Ok( await _authContext.Projects.Where(p => p.UserID == userID).ToListAsync());
            }
            else
            {
      
               // var project = _projectService.GetCultivationProjects().Where(p => p.Id == id);
                var project = await _authContext.Projects.Where(p => p.Id == id && p.UserID == userID).ToListAsync();

                return Ok(project);
            }


        }
    }
}


