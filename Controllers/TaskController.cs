using Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Bson;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class TaskController : ControllerBase
    {
        
        
        private IMongoCollection<Korisnik> korisnikCollection;
        private IMongoCollection<Team> teamCollection;
        public TeamMakerContext Context {get;set;}
        public TaskController(TeamMakerContext context){
                Context = context;
                DataProvider dp = new DataProvider();
                korisnikCollection = dp.ConnectToMongo<Korisnik>("korisnik");
                teamCollection = dp.ConnectToMongo<Team>("team");
        }
        
        // [HttpGet]
        // [Route("GetTasks/{teamID}")]
        // public ActionResult GetTasks([FromRoute] int teamID)
        // {
        //     try
        //     {
              
        //         var username = User.FindFirstValue(ClaimTypes.Name);
            
        //         var team = Context.Timovi.Where(t => (t.ID == teamID) && (t.Leader.Username == username))
        //                                 .FirstOrDefault();
                                       
        //         if(team == null)
        //             return BadRequest("Team ne postoji");
          
                
        //         var tasks = Context.Taskovi  .Include(t => t.Korisnik)
        //                                     .Include(t => t.Team)
        //                                     .Where(t => t.Team == team)
        //                                     .Select(t => new {
        //                                         ID = t.ID,
        //                                         Ime = t.Ime,
        //                                         Opis = t.Opis,
        //                                         Status = t.Status,                                               
        //                                         Korisnik = t.Korisnik == null ? null : new {
        //                                                                                         ID = t.Korisnik.ID,
        //                                                                                         Ime = t.Ime
        //                                                                                     }
                                            
        //                                     })
        //                                 .ToList();

        //         return Ok(tasks);
                
        //     }
        //     catch(Exception e)
        //     {
        //         return BadRequest(e.Message);
        //     }
        // }


        // [HttpGet]
        // [Route("GetTasksWithStatus/{teamID}/{status}")]
        // public ActionResult GetTasks([FromRoute] int teamID, [FromRoute] int status)
        // {
        //     try
        //     {
               
        //         var username = User.FindFirstValue(ClaimTypes.Name);
        //         //neg!!!!
        //         var team = Context.Timovi.Where(t => (t.ID == teamID) && (t.Leader.Username == username))
        //                                 .FirstOrDefault();
                                    
        //         if(team == null)
        //             return BadRequest("Team ne postoji");
         
                
        //         var tasks = Context.Taskovi .Include(t => t.Team)
        //                                     .Where(t => (t.Team == team) && (t.Status==status))
        //                                     .Select(t => new {
        //                                         ID = t.ID,
        //                                         Ime = t.Ime,
        //                                         Opis = t.Opis,
        //                                         Status = t.Status,                                               
                                                                                    
        //                                     })
        //                                 .ToList();

        //         return Ok(tasks);
                
        //     }
        //     catch(Exception e)
        //     {
        //         return BadRequest(e.Message);
        //     }
        // }


        // [HttpPost]
        // [Route("CreateTask/{teamID}")]
        // public async Task<ActionResult> CreateTask([FromBody] Models.Task task, [FromRoute] int teamID)
        // {
        //     try
        //     {    
              

        //         var username = User.FindFirstValue(ClaimTypes.Name);
        //         var team = Context.Timovi.Include(t => t.Korisnici).Where(t => (t.ID == teamID) && (t.Leader.Username == username)).FirstOrDefault();

        //         if(team == null)
        //             return BadRequest("Team ne postoji");

        //         task.Team = team;
        //         task.Korisnik = null;

        //         Context.Taskovi.Add(task);
        //         await Context.SaveChangesAsync();

        //         return Ok(task);
        //     }
        //     catch(Exception e)
        //     {
        //         return BadRequest(e.Message);
        //     }
        // }

       
        // [HttpPut]
        // [Route("TakeTask/{taskID}")]
        // public ActionResult TakeTask([FromRoute] int taskID)
        // {
        //     var username = User.FindFirstValue(ClaimTypes.Name);

        //     try
        //     {
        //         var korisnik = Context.Korisnici.Where(k => k.Username == username).Include(k => k.Teams).FirstOrDefault();
                
                
        //         var task = Context.Taskovi.Where(t => t.ID == taskID).Include(t => t.Team).ThenInclude(t => t.Korisnici).Include(t => t.Sprint).FirstOrDefault();
                
        //         bool flag = true;
        //         foreach(var team in korisnik.Teams)
        //         {                   
        //             if(team.ID == task.Team.ID)
        //                 flag = false;

        //         }

        //         if(task == null || task.Status != 1 || flag || task.Sprint.Status == 1)
        //             return BadRequest("Greska");
                
        //         if(!task.Team.Korisnici.Contains(korisnik))
        //             return BadRequest("Greska");


        //         task.Status++;
        //         task.Korisnik = korisnik;

        //         Context.SaveChanges();
        //         return Ok("Task je preuzet");
        //     }
        //     catch(Exception e)
        //     {
        //         return BadRequest(e.Message);
        //     }
        // }

        // [HttpPut]
        // [Route("SendForReview/{taskID}")]
        // public ActionResult SendForReview([FromRoute] int taskID)
        // {
        //     var username = User.FindFirstValue(ClaimTypes.Name);

        //     try
        //     {
            
        //         var korisnik = Context.Korisnici.Where(k => k.Username == username).Include(k => k.Teams).FirstOrDefault();
                
                
        //         var task = Context.Taskovi.Where(t => t.ID == taskID).Include(t => t.Team).Include(t => t.Sprint).FirstOrDefault();
                
        //         bool flag = true;
        //         foreach(var t in korisnik.Taskovi)
        //         {                   
        //             if(t.ID == task.ID)
        //                 flag = false;
        //         }

        //         if(task == null || task.Status != 2 || flag || task.Sprint.Status == 1)
        //             return BadRequest("Greska");
                
        //         if(!task.Team.Korisnici.Contains(korisnik))
        //             return BadRequest("Greska");
                    
        //         task.Status++;

        //         Context.SaveChanges();
        //         return Ok("Task je preuzet");
        //     }
        //     catch(Exception e)
        //     {
        //         return BadRequest(e.Message);
        //     }
        // }
    
       
        // [HttpPut]
        // [Route("ApproveTask/{taskID}")]
        // public ActionResult ApproveTask([FromRoute] int taskID)
        // {

        //     var username = User.FindFirstValue(ClaimTypes.Name);

        //     try
        //     {
            
        //         var leader = Context.Korisnici.Where(k => k.Username == username).Include(k => k.Teams).FirstOrDefault();
                
                
        //         var task = Context.Taskovi.Where(t => t.ID == taskID).Include(t => t.Team).Include(t => t.Sprint).FirstOrDefault();
                
        //         bool flag = true;
        //         if(task.Team.Leader.ID == leader.ID)
        //             flag = false;

        //         if(task == null || task.Status != 3 || flag || task.Sprint.Status == 1)
        //             return BadRequest("Greska");
                    
        //         task.Status++;

        //         Context.SaveChanges();
        //         return Ok("Task je preuzet");
        //     }
        //     catch(Exception e)
        //     {
        //         return BadRequest(e.Message);
        //     } 

        // }

    }

}