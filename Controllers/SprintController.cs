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
                    .Match(t => t.ID == teamID && t.Korisnici.Contains(korisnik))              
                    .FirstOrDefault();


                if(team == null)
                    return BadRequest("tim ne postoji");


                if(!team.Korisnici.Contains(korisnik))
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
//                var invites = (await teamCollection.FindAsync(Builders<Team>.Filter.ElemMatch(t => t.Invites, i => i.KorisnikRef == ObjectId.Parse(k.ID)))).ToList().Select(t => t.Invites.Find(i => i.KorisnikRef == ObjectId.Parse(k.ID)));
                
                
                var sprint = (teamCollection
                            .Aggregate()
                            .Lookup("korisnik", "korisniciRef", "_id", "korisnici")
                            .As<TeamBson>()
                            .Match( Builders<TeamBson>.Filter.ElemMatch(t => t.Sprints, s => s.ID == sprintID) & 
                                    Builders<TeamBson>.Filter.ElemMatch(t => t.Korisnici, k => k.ID == korisnik.ID)))//t.Korisnici.Select(k => k.Username).Contains(username)
                                                            .FirstOrDefault().Sprints.FirstOrDefault(s => s.ID == sprintID);
                
                // var sprint = Context.Sprints.Include(t => t.Taskovi)
                //                             .Include(t => t.Team)
                //                             .ThenInclude(t => t.Korisnici)
                //                             .Where(t => t.ID == sprintID && t.Team.Korisnici.Contains(korisnik))
                //                             .FirstOrDefault();

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
                    teamCollection.ReplaceOne(Builders<Team>.Filter.Eq(t => t.ID, team.ID), team);//.InsertOne(s);

                }

                //Context.SaveChanges();
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
                                                     t.Leader.Select(l => l.Username).Equals(username))
                            .FirstOrDefault();
                            //.Match( Builders<TeamBson>.Filter.(t => t., k => k.ID == korisnik.ID)


                //var team = Context.Timovi.Include(t => t.Korisnici).Include(t => t.Leader).Where(t => (t.ID == teamID) && (t.Leader.Username == username)).FirstOrDefault();

                if(team == null)
                    return BadRequest("Team ne postoji");

                if((DateTime)sprint.EndSprint < DateTime.Now || (DateTime)sprint.StartSprint > (DateTime)sprint.EndSprint)
                    return BadRequest("Datum los");

                Sprint s = new Sprint();
                s.Opis = sprint.Opis;
                s.StartSprint = (DateTime)sprint.StartSprint;
                s.EndSprint = (DateTime)sprint.EndSprint;
                s.Status = sprint.Status;
                s.Team = team;
                s.Taskovi = new List<Models.Task>();

                foreach(var t in  sprint.Taskovi)
                {
                    //var task = Context.Taskovi.Where(k => (k.ID == t.ID) && (k.Status == 0)).FirstOrDefault();
                    Models.Task task = team.Tasks.Where(x => (x.ID == t.ID) && (x.Status == 0)).FirstOrDefault(); //Builders<TeamBson>.Filter.ElemMatch(t => t.Korisnici, k => k.ID == korisnik.ID))
                    if(task != null)
                    {
//teamCollection.FindOneAndUpdate(Builders<Team>.Filter.Eq(t => t.ID, tim.ID), Builders<Team>.Update.Push<Invite>(t => t.Invites, invite));
                        task.Status++;
                        s.Taskovi.Add(task);

                    }
                }
                teamCollection.UpdateOne(Builders<Team>.Filter.Eq(t => t.ID, teamID), Builders<Team>.Update.Push(t => t.Sprints, sprint));//.InsertOne(s);
                //Context.SaveChanges();

                return Ok("sprint posted");

            }catch(Exception e)
            {
                return BadRequest(e.Message);

            }



        }

    }
}
