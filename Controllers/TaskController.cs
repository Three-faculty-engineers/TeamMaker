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
        public IMongoCollection<Team> teamCollection;
        public IMongoCollection<Korisnik> korisnikCollection;
        public TaskController(){
                DataProvider dp = new DataProvider();
                teamCollection = dp.ConnectToMongo<Team>("team");
                korisnikCollection = dp.ConnectToMongo<Korisnik>("korisnik");
        }
        
        [HttpGet]
        [Route("GetTasks/{teamID}")]
        public ActionResult GetTasks([FromRoute] string teamID)
        {
            try
            {
              
                var username = User.FindFirstValue(ClaimTypes.Name);

                var team = teamCollection
                    .Aggregate()
                    .Lookup("korisnik", "leaderRef", "_id", "leader")
                    .As<TeamBson>()
                    .Match(t => t.ID == teamID && t.Leader[0].Username == username)
                    .FirstOrDefault();
                                       
                if(team == null)
                    return BadRequest("Team ne postoji");

                var tasks = team.Sprints.SelectMany(s => s.Taskovi).Select(t => new {
                    ID = t.ID,
                    Ime = t.Ime,
                    Opis = t.Opis,
                    Status = t.Status,
                    Korisnik = t.KorisnikRef == null ? null : korisnikCollection.Find(k => k.ID == t.KorisnikRef)
                });

                return Ok(tasks);
                
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("GetTasksWithStatus/{teamID}/{status}")]
        public ActionResult GetTasksWithStatus([FromRoute] string teamID, [FromRoute] int status)
        {
            try
            {
               
                var username = User.FindFirstValue(ClaimTypes.Name);
                var team = teamCollection
                    .Aggregate()
                    .Lookup("korisnik", "leaderRef", "_id", "leader")
                    .As<TeamBson>()
                    .Match(t => t.ID == teamID && t.Leader[0].Username == username)
                    .Project<TeamBson>(Builders<TeamBson>.Projection.Include(t => t.Sprints))
                    .FirstOrDefault();
                                    
                if(team == null)
                    return BadRequest("Team ne postoji");

                var tasks = team.Sprints.SelectMany(s => s.Taskovi)
                    .ToList()
                    .FindAll(t => t.Status == status)
                    .Select(t => new {
                        ID = t.ID,
                        Ime = t.Ime,
                        Opis = t.Opis,
                        Status = t.Status 
                    }
                );

                return Ok(tasks);
                
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPost]
        [Route("CreateTask/{teamID}")]
        public async Task<ActionResult> CreateTask([FromBody] Models.Task task, [FromRoute] string teamID)
        {
            try
            {    
              

                var username = User.FindFirstValue(ClaimTypes.Name);
                var team = teamCollection
                    .Aggregate()
                    .Lookup("korisnik", "leaderRef", "_id", "leader")
                    .As<TeamBson>()
                    .Match(t => t.ID == teamID && t.Leader[0].Username == username)
                    .FirstOrDefault();

                if(team == null)
                    return BadRequest("Team ne postoji");

                task.ID = ObjectId.GenerateNewId().ToString();

                await teamCollection.UpdateOneAsync(
                    Builders<Team>.Filter.Eq(t => t.ID, teamID) & 
                    Builders<Team>.Filter.ElemMatch(t => t.Sprints, s => s.ID == task.Sprint.ID), 
                    Builders<Team>.Update.Push("sprints.$.taskovi", task));

                return Ok(task);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

       
        [HttpPut]
        [Route("TakeTask/{taskID}")]
        public ActionResult TakeTask([FromRoute] string taskID)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);

            try
            {
                var korisnik = korisnikCollection.Find(k => k.Username == username).FirstOrDefault();

                var team = teamCollection
                    .Find(Builders<Team>.Filter.ElemMatch(t => t.Sprints, t => t.Taskovi.Select(t => t.ID).Contains(taskID)))
                    .Project<Team>(Builders<Team>.Projection.Include(t => t.Sprints).Include(t => t.KorisniciRef))
                    .FirstOrDefault();

                var sprintIndex = team.Sprints.FindIndex(s => s.Taskovi.Select(t => t.ID).Contains(taskID));
                
                var taskIndex = team.Sprints[sprintIndex].Taskovi.FindIndex(t => t.ID == taskID);

                if(taskIndex == -1 || team.Sprints[sprintIndex].Taskovi[taskIndex].Status != 1)
                {
                    return BadRequest("Greska");
                }

                if(!team.KorisniciRef.Contains(ObjectId.Parse(korisnik.ID)))
                {
                    return BadRequest("Greska");
                }

                team.Sprints[sprintIndex].Taskovi[taskIndex].Status++;
                team.Sprints[sprintIndex].Taskovi[taskIndex].KorisnikRef = korisnik.ID;

                teamCollection.UpdateOne(
                    Builders<Team>.Filter.Eq(t => t.ID, team.ID) & 
                    Builders<Team>.Filter.ElemMatch(t => t.Sprints, s => s.ID == team.Sprints[sprintIndex].ID), 
                    Builders<Team>.Update.Set("sprints.$.taskovi", team.Sprints[sprintIndex].Taskovi));

                return Ok("Task je preuzet");
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Route("SendForReview/{taskID}")]
        public ActionResult SendForReview([FromRoute] string taskID)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);

            try
            {
                var korisnik = korisnikCollection.Find(k => k.Username == username).FirstOrDefault();

                var team = teamCollection
                    .Find(Builders<Team>.Filter.ElemMatch(t => t.Sprints, t => t.Taskovi.Select(t => t.ID).Contains(taskID)))
                    .Project<Team>(Builders<Team>.Projection.Include(t => t.Sprints).Include(t => t.KorisniciRef))
                    .FirstOrDefault();

                var sprintIndex = team.Sprints.FindIndex(s => s.Taskovi.Select(t => t.ID).Contains(taskID));

                var taskIndex = team.Sprints[sprintIndex].Taskovi.FindIndex(t => t.ID == taskID);

                if(taskIndex == -1 || team.Sprints[sprintIndex].Taskovi[taskIndex].Status != 2)
                {
                    return BadRequest("Greska");
                }

                if(!team.KorisniciRef.Contains(ObjectId.Parse(korisnik.ID)))
                {
                    return BadRequest("Greska");
                }

                team.Sprints[sprintIndex].Taskovi[taskIndex].Status++;

                teamCollection.UpdateOne(
                    Builders<Team>.Filter.Eq(t => t.ID, team.ID) & 
                    Builders<Team>.Filter.ElemMatch(t => t.Sprints, s => s.ID == team.Sprints[sprintIndex].ID), 
                    Builders<Team>.Update.Set("sprints.$.taskovi", team.Sprints[sprintIndex].Taskovi));

                return Ok("Task je preuzet");
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    
       
        [HttpPut]
        [Route("ApproveTask/{taskID}")]
        public ActionResult ApproveTask([FromRoute] string taskID)
        {

            var username = User.FindFirstValue(ClaimTypes.Name);

            try
            {
            
                var leader = korisnikCollection.Find(k => k.Username == username).FirstOrDefault();

                var team = teamCollection
                    .Find(Builders<Team>.Filter.ElemMatch(t => t.Sprints, t => t.Taskovi.Select(t => t.ID).Contains(taskID)))
                    .Project<Team>(Builders<Team>.Projection.Include(t => t.Sprints).Include(t => t.KorisniciRef).Include(t => t.LeaderRef))
                    .FirstOrDefault();

                var sprintIndex = team.Sprints.FindIndex(s => s.Taskovi.Select(t => t.ID).Contains(taskID));
                
                var taskIndex = team.Sprints[sprintIndex].Taskovi.FindIndex(t => t.ID == taskID);

                if(team.LeaderRef != ObjectId.Parse(leader.ID))
                {
                    return BadRequest("Greska");
                }


                if(taskIndex == -1 || team.Sprints[sprintIndex].Taskovi[taskIndex].Status != 3)
                {
                    return BadRequest("Greska");
                }

                
                if(!team.KorisniciRef.Contains(ObjectId.Parse(leader.ID)))
                {
                    return BadRequest("Greska");
                }

                team.Sprints[sprintIndex].Taskovi[taskIndex].Status++;

                teamCollection.UpdateOne(
                    Builders<Team>.Filter.Eq(t => t.ID, team.ID) & 
                    Builders<Team>.Filter.ElemMatch(t => t.Sprints, s => s.ID == team.Sprints[sprintIndex].ID), 
                    Builders<Team>.Update.Set("sprints.$.taskovi", team.Sprints[sprintIndex].Taskovi));
                    
                return Ok("Task je preuzet");
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            } 

        }

    }

}