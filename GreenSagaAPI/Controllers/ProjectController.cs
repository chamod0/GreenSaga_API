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
             

                return Ok(result);
            }


        }
        [HttpGet("getProjectForSupervisor/{id?}/{userID}")]
        public async Task<ActionResult<cultivationProjects>> getProjectForSupervisor(int? id, int userID)
        {
          
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

                
                var project = await _authContext.Projects.Where(p => p.Id == id && p.SupervisorID == userID).ToListAsync();

                return Ok(project);
            }


        }
        [HttpDelete]
        [Route(("deleteProjects/{id:int}"))]
        public async Task<IActionResult> DeleteProjects([FromRoute]int id)
        {
            try
            {
                var projectDelete = await _authContext.Projects.FirstOrDefaultAsync(p => p.Id == id);

                if (projectDelete != null)
                {
                    _authContext.Remove(projectDelete);
                    await _authContext.SaveChangesAsync();
                    return Ok(projectDelete);
                }

                  return NotFound($"Projects with Id = {id} not found");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }
        [HttpPut]
        [Route(("updateProjects/{id:int}"))]
        public async Task<IActionResult> UpdateProjects([FromRoute] int id, [FromBody] cultivationProjects cultivationProjects )
        {
            try
            {
                var projectUpdate = await _authContext.Projects.FirstOrDefaultAsync(p => p.Id == id);

                if (projectUpdate != null)

                {
                    projectUpdate.ProjectName = cultivationProjects.ProjectName;
                    projectUpdate.Description = cultivationProjects.Description;
                    projectUpdate.SupervisorID = cultivationProjects.SupervisorID;
            
                    await _authContext.SaveChangesAsync();
                    return Ok(projectUpdate);
                }

                return NotFound($"Projects with Id = {id} not found");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }
        [HttpGet("getProjectSearch/{id?}/{userID}/{searchString}")]
        public async Task<IActionResult> getProjectSearch(int? id, int userID,string searchString)
        {

            if (id is null)
            {
                return BadRequest("can't pass null values");
            }

            else 
            {
                var sqlQuery = "SELECT Projects.Id ,ProjectCode  ,ProjectName,Description,Createby ,UserID ,SupervisorID ,users.FirstName +' '+users.LastName as SupervisorName,ModifyAt ,ModifyBy ,DeleteAt ,DeleteBy ,Status ,CreateAt   " +
                    "FROM  Projects   INNER JOIN users ON users.ID = Projects.SupervisorID " +
                    " WHERE UserID = @p0 and ProjectName LIKE  @p1";
                var result = _authContext.ProjecwithSupervisor.FromSqlRaw(sqlQuery, userID, "%" + searchString + "%" ).ToList();
                return Ok(result);
            }
       


        }
    }
}


