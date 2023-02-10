using Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Bson;
namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class SprintController : ControllerBase
    {
        public TeamMakerContext Context { get; set; }

        private IMongoCollection<Korisnik> korisnikCollection;
        private IMongoCollection<Team> teamCollection;
        public SprintController(TeamMakerContext context)
        {
            Context = context;
            DataProvider dp = new DataProvider();
            korisnikCollection = dp.ConnectToMongo<Korisnik>("korisnik");
            teamCollection = dp.ConnectToMongo<Team>("team");

        }

        [HttpGet]
        [AllowAnonymous]
        [Route("GetSprintsList/{teamID}")]

        public ActionResult GetSprintsList([FromRoute] string teamID)
        {
            try
            {
                var team = teamCollection
                    .Find(t => t.ID == teamID)
                    .Project<Team>(Builders<Team>.Projection.Exclude(t => t.Sprints.Select(s => s.Taskovi)))
                    .FirstOrDefault();


                if(team == null)
                    return BadRequest("tim ne postoji");

                return Ok(team.Sprints);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpGet]
        [Route("GetSprints/{teamID}")]

        public ActionResult GetSprints([FromRoute] string teamID)
        {
            try
            {
                var username = User.FindFirstValue(ClaimTypes.Name);
                var korisnik = korisnikCollection.Find(k => k.Username == username).FirstOrDefault();

                CheckSprints(korisnik);//t

                var team = teamCollection
                    .Aggregate()
                    .Lookup("korisnik", "korisniciRef", "_id", "korisnici")
                    .As<TeamBson>()
                    .Match(t => t.ID == teamID && t.Korisnici.Select(k => k.Username).Contains(username))
                    .FirstOrDefault();


                if(team == null)
                    return BadRequest("tim ne postoji");


                if(!team.Korisnici.Select(k => k.Username).Contains(username))
                    return BadRequest("korisnik ne pripada timu");

                return Ok(team.Sprints);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("GetSprintTasks/{sprintID}")]

        public ActionResult GetSprintTasks([FromRoute] string sprintID)
        {
            try
            {
                
                var username = User.FindFirstValue(ClaimTypes.Name);
                var korisnik = korisnikCollection.Find(k => k.Username == username).FirstOrDefault();
                
                var sprint = (teamCollection
                            .Aggregate()
                            .Lookup("korisnik", "korisniciRef", "_id", "korisnici")
                            .As<TeamBson>()
                            .Match( Builders<TeamBson>.Filter.ElemMatch(t => t.Sprints, s => s.ID == sprintID) & 
                                    Builders<TeamBson>.Filter.ElemMatch(t => t.Korisnici, k => k.ID == korisnik.ID)))
                                                            .FirstOrDefault().Sprints.FirstOrDefault(s => s.ID == sprintID);

                if(sprint == null)
                    return BadRequest("sprint ne postoji");


                return Ok(sprint.Taskovi);

            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private void CheckSprints(Korisnik korisnik)
        {

            try
            {
                foreach(var teamID in korisnik.TeamsRef)
                {
                    Team team = (Team)teamCollection.Find(t => t.ID == teamID.ToString()).FirstOrDefault();   
                    foreach(var sprint in team.Sprints)
                    {
                        if(sprint.EndSprint < DateTime.Now && sprint.Status == 0)
                        {

                            sprint.Status = 1;
                        }

                    }
                    teamCollection.ReplaceOne(Builders<Team>.Filter.Eq(t => t.ID, team.ID), team);

                }
            }
            catch(Exception e){

            }
        }

        [HttpPost]
        [Route("PostSprint/{teamID}")]
        public ActionResult PostSprint([FromBody] Sprint sprint, [FromRoute] string teamID)
        {
            try
            {
                var username = User.FindFirstValue(ClaimTypes.Name);   
                var team = teamCollection
                            .Aggregate()
                            .Lookup("korisnik", "leaderRef", "_id", "leader")
                            .As<TeamBson>().Match(t => (t.ID == teamID) &&
                                                     t.Leader.Select(l => l.Username).ToList()[0] == username)
                            .FirstOrDefault();

                if(team == null)
                    return BadRequest("Team ne postoji");

                if((DateTime)sprint.EndSprint < DateTime.Now || (DateTime)sprint.StartSprint > (DateTime)sprint.EndSprint)
                    return BadRequest("Datum los");

                // Sprint s = new Sprint();
                // s.ID = ObjectId.GenerateNewId().ToString();
                // s.Opis = sprint.Opis;
                // s.StartSprint = (DateTime)sprint.StartSprint;
                // s.EndSprint = (DateTime)sprint.EndSprint;
                // s.Status = sprint.Status;
                // s.Team = team;
                // s.Taskovi = new List<Models.Task>();
                sprint.ID = ObjectId.GenerateNewId().ToString();
                sprint.Team = team;
                sprint.Taskovi = new List<Models.Task>();


                foreach(var t in  sprint.Taskovi)
                {
                    Models.Task task = team.Tasks.Where(x => (x.ID == t.ID) && (x.Status == 0)).FirstOrDefault();
                    if(task != null)
                    {
                        task.Status++;
                        sprint.Taskovi.Add(task);

                    }
                }
                teamCollection.UpdateOne(Builders<Team>.Filter.Eq(t => t.ID, teamID), Builders<Team>.Update.Push(t => t.Sprints, sprint));

                return Ok("sprint posted");

            }catch(Exception e)
            {
                return BadRequest(e.Message);

            }



        }

    }
}
