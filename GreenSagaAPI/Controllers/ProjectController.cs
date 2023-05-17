using GreenSagaAPI.Context;
using GreenSagaAPI.Models;
using GreenSagaAPI.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
            //check user name
            if (await CheckProjectNameExistAsync(projectObj.ProjectName))
                return BadRequest(new { Message = projectObj.ProjectName + " Project Name Already Exist! " });
            await _authContext.Projects.AddAsync(projectObj);
            await _authContext.SaveChangesAsync();
            return Ok(new
            {
                Message = "Project Create Sucess!"
            });

        }
        private Task<bool> CheckProjectNameExistAsync(string projectName)
             => _authContext.Projects.AnyAsync(x => x.ProjectName == projectName);

        [HttpGet("getProject/{id?}/{userID}")]
        public async Task<IActionResult> project(int? id,int userID)
        {
           // var projects = _projectService.GetCultivationProjects().Where(p => p.Id == id);
            if (id is null)
            {
                return BadRequest("can't pass null values");
            }

            else if (id == 0)
            {
                var sqlQuery = "SELECT Projects.Id ,ProjectCode  ,ProjectName,Description,Createby ,UserID ,SupervisorID ,users.FirstName +' '+users.LastName as SupervisorName,ModifyAt ,ModifyBy ,DeleteAt ,DeleteBy ,Status ,CreateAt   FROM  Projects   INNER JOIN users ON users.ID = Projects.SupervisorID  WHERE UserID = @p0 ";
                var result = _authContext.ProjecwithSupervisor.FromSqlRaw(sqlQuery, userID).ToList();
                return Ok(result);
            }
            else
            {
                var sqlQuery = "SELECT Projects.Id ,ProjectCode  ,ProjectName,Description,Createby ,UserID ,SupervisorID ,users.FirstName +' '+users.LastName as SupervisorName,ModifyAt ,ModifyBy ,DeleteAt ,DeleteBy ,Status ,CreateAt   FROM  Projects   INNER JOIN users ON users.ID = Projects.SupervisorID  WHERE UserID = @p0 AND Projects.Id = @p1";

                var result = _authContext.ProjecwithSupervisor.FromSqlRaw(sqlQuery, userID, id).ToList();
                // var project = _projectService.GetCultivationProjects().Where(p => p.Id == id);
             //   var project = await _authContext.Projects.Where(p => p.Id == id && p.UserID == userID).ToListAsync();

                return Ok(result);
            }


        }
        [HttpGet("getProjectForSupervisor/{id?}/{userID}")]
        public async Task<ActionResult<cultivationProjects>> getProjectForSupervisor(int? id, int userID)
        {
            // var projects = _projectService.GetCultivationProjects().Where(p => p.Id == id);
            if (id is null)
            {
                return BadRequest("can't pass null values");
            }

            else if (id == 0)
            {
                return Ok(await _authContext.Projects.Where(p => p.SupervisorID == userID).ToListAsync());
            }
            else
            {

                // var project = _projectService.GetCultivationProjects().Where(p => p.Id == id);
                var project = await _authContext.Projects.Where(p => p.Id == id && p.SupervisorID == userID).ToListAsync();

                return Ok(project);
            }


        }
    }
}


