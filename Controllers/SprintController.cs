using Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class SprintController : ControllerBase
    {
        // public TeamMakerContext Context { get; set; }

        // public SprintController(TeamMakerContext context)
        // {
        //     Context = context;
            
        // }


        // [HttpGet]
        // [Route("GetSprints/{teamID}")]

        // public ActionResult GetSprints([FromRoute] int teamID)
        // {
        //     try
        //     {


                
        //         var username = User.FindFirstValue(ClaimTypes.Name);
        //         var korisnik = Context.Korisnici.Where(k => k.Username == username).FirstOrDefault();

        //         CheckSprints(korisnik);

        //         var team = Context.Timovi.Include(t => t.Korisnici)
        //                                     .Include(t => t.Sprints)
        //                                     .ThenInclude(s => s.Taskovi)
        //                                     .Where(t => t.ID == teamID && t.Korisnici.Contains(korisnik))
        //                                     .FirstOrDefault();

        //         if(team == null)
        //             return BadRequest("tim ne postoji");


        //         if(!team.Korisnici.Contains(korisnik))
        //             return BadRequest("korisnik ne pripada timu");

        //         return Ok(team.Sprints);
        //     }
        //     catch(Exception e)
        //     {
        //         return BadRequest(e.Message);
        //     }
        // }

        // [HttpGet]
        // [Route("GetSprintTasks/{sprintID}")]

        // public ActionResult GetSprintTasks([FromRoute] int sprintID)
        // {
        //     try
        //     {
                
        //         var username = User.FindFirstValue(ClaimTypes.Name);
        //         var korisnik = Context.Korisnici.Where(k => k.Username == username).FirstOrDefault();
               
        //         var sprint = Context.Sprints.Include(t => t.Taskovi)
        //                                     .Include(t => t.Team)
        //                                     .ThenInclude(t => t.Korisnici)
        //                                     .Where(t => t.ID == sprintID && t.Team.Korisnici.Contains(korisnik))
        //                                     .FirstOrDefault();

        //         if(sprint == null)
        //             return BadRequest("sprint ne postoji");


        //         return Ok(sprint.Taskovi);

        //     }
        //     catch(Exception e)
        //     {
        //         return BadRequest(e.Message);
        //     }
        // }

        // private void CheckSprints(Korisnik korisnik)
        // {

        //     try
        //     {
        //         foreach( var team in korisnik.Teams)
        //         {
        //             foreach(var sprint in team.Sprints)
        //             {
        //                 if(sprint.EndSprint < DateTime.Now && sprint.Status == 0)
        //                 {
                           
        //                     sprint.Status = 1;
        //                 }
        //             }
        //         }
        //         Context.SaveChanges();
        //     }
        //     catch(Exception e)
        //     {
        //     }
        // }

        // [HttpPost]
        // [Route("PostSprint/{teamID}")]
        // public ActionResult PostSprint([FromBody] Sprint sprint, [FromRoute] int teamID)
        // {
        //     try
        //     {
                
        //         var username = User.FindFirstValue(ClaimTypes.Name);
             
        //         var team = Context.Timovi.Include(t => t.Korisnici).Include(t => t.Leader).Where(t => (t.ID == teamID) && (t.Leader.Username == username)).FirstOrDefault();

        //         if(team == null)
        //             return BadRequest("Team ne postoji");

        //         if((DateTime)sprint.EndSprint < DateTime.Now || (DateTime)sprint.StartSprint > (DateTime)sprint.EndSprint)
        //             return BadRequest("Datum los");

        //         Sprint s = new Sprint();
        //         s.Opis = sprint.Opis;
        //         s.StartSprint = (DateTime)sprint.StartSprint;
        //         s.EndSprint = (DateTime)sprint.EndSprint;
        //         s.Status = sprint.Status;
        //         s.Team = team;
        //         s.Taskovi = new List<Models.Task>();

        //         foreach(var t in  sprint.Taskovi)
        //         {
        //             var task = Context.Taskovi.Where(k => (k.ID == t.ID) && (k.Status == 0)).FirstOrDefault();

        //             if(task != null)
        //             {
        //                 task.Status++;
        //                 s.Taskovi.Add(task);

        //             }
        //         }
        //         Context.Sprints.Add(s);
        //         Context.SaveChanges();

        //         return Ok("sprint posted");

        //     }catch(Exception e)
        //     {
        //         return BadRequest(e.Message);

        //     }



        // }

    }
}
