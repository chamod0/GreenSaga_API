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
    public class TimeLineController : ControllerBase
    {
        private readonly AppDbContext _authContext;


        public TimeLineController(AppDbContext appDbContext)
        {

            _authContext = appDbContext;
        }
        [HttpPost("timeLineCreate")]
        public async Task<IActionResult> timeLineCreate([FromBody] TimeLineBox timeLineObj)
        {
            if (timeLineObj == null)
                return BadRequest();
            //check user name
     
            await _authContext.TimeLineBox.AddAsync(timeLineObj);
            await _authContext.SaveChangesAsync();
            return Ok(new
            {
                Message = "Project Create Sucess!"
            });

        }
        [HttpGet("getTimeLineForSupervisor/{id?}/{userID}")]
        public async Task<ActionResult<TimeLineBox>> getTimeLineForSupervisor(int? id, int userID)
        {
            // var projects = _projectService.GetCultivationProjects().Where(p => p.Id == id);
            if (id is null)
            {
                return BadRequest("can't pass null values");
            }

            else if (id == 0)
            {
                return Ok(await _authContext.TimeLineBox.Where(p => p.UserID == userID).ToListAsync());
            }
            else
            {

                // var project = _projectService.GetCultivationProjects().Where(p => p.Id == id);
                var project = await _authContext.TimeLineBox.Where(p => p.Id == id && p.UserID == userID).ToListAsync();

                return Ok(project);
            }

        }
        [HttpGet("getTimeLineForFarmer/{ProjectId?}/{userID}")]
        public async Task<ActionResult<TimeLineBox>> getTimeLineForFarmer(int? ProjectId, int userID)
        {
            // var projects = _projectService.GetCultivationProjects().Where(p => p.Id == id);
            if (ProjectId is null)
            {
                return BadRequest("can't pass null values");
            }

            //else if (ProjectId == 0)
            //{
            //    return Ok(await _authContext.TimeLineBox.Where(p => p.ProjectId == ProjectId).ToListAsync());
            //}
            else
            {

                // var project = _projectService.GetCultivationProjects().Where(p => p.Id == id);
                var project = await _authContext.TimeLineBox.Where(p => p.ProjectId == ProjectId ).ToListAsync();

                return Ok(project);
            }

        }
        [HttpDelete]
        [Route(("deleteTimeLine/{id:int}"))]
        public async Task<IActionResult> deleteTimeLine([FromRoute] int id)
        {
            try
            {
                var timeLineBoxDelete = await _authContext.TimeLineBox.FirstOrDefaultAsync(p => p.Id == id);

                if (timeLineBoxDelete != null)
                {
                    _authContext.Remove(timeLineBoxDelete);
                    await _authContext.SaveChangesAsync();
                    return Ok(timeLineBoxDelete);
                }

                return NotFound($"Projects with Id = {id} not found");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }
    }
}
