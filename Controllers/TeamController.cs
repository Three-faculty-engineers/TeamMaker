using Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MongoDB.Driver;
using MongoDB.Bson;

namespace WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class TeamController : ControllerBase
    {
        private IMongoCollection<Team> teamCollection;
        private IMongoCollection<Korisnik> korisnikCollection;
        public TeamController(){
            DataProvider dp = new DataProvider();
            teamCollection = dp.ConnectToMongo<Team>("team");
            korisnikCollection = dp.ConnectToMongo<Korisnik>("korisnik");
        }

        [Route("GetAllTeams")]
        [HttpGet]
        [Authorize]
        public ActionResult GetAllTeams() 
        {
            try{
                var teams = teamCollection
                    .Aggregate()
                    .Lookup("korisnik", "korisniciRef", "_id", "korisnici")
                    .Lookup("korisnik", "leaderRef", "_id", "leader")
                    .As<TeamBson>()
                    .ToList();
                return Ok(teams.Select(t => new {
                    id = t.ID,
                    Ime = t.Ime,
                    Opis = t.Opis,
                    Leader = new { Username = t.Leader[0].Username, id = t.Leader[0].ID },
                    Korisnici = t.Korisnici.Select(k => new {id = k.ID, Username = k.Username})
                }));

            }catch(Exception e){
                return BadRequest(e.Message);
            }
        }

        [Route("CreateTeam")]
        [HttpPost]
        public ActionResult CreateTeam([FromBody] Team t) 
        {
            try{
                if(string.IsNullOrEmpty(t.Ime) || string.IsNullOrEmpty(t.Opis))
                    return BadRequest("Greska prilikom kreiranja tima");

                var username = User.FindFirstValue(ClaimTypes.Name);
                var korisnik = korisnikCollection.Find(k => k.Username == username).FirstOrDefault();
                if(korisnik == null)
                    return BadRequest("korisnik ne postoji");
                
                Team team = new Team{Ime = t.Ime , LeaderRef=ObjectId.Parse(korisnik.ID), Opis = t.Opis, Objave = new List<Objava>(), Invites = new List<Invite>(), Sprints = new List<Sprint>(), Tasks = new List<Models.Task>()};
                
                team.KorisniciRef = new List<ObjectId>();
                team.KorisniciRef.Add(ObjectId.Parse(korisnik.ID));
            
                teamCollection.InsertOne(team);

                return Ok("Tim je kreiran");

            }catch(Exception e){

                return BadRequest(e.InnerException.Message);

            }
        }

        protected Korisnik returnKorisnik()
        {
            try
            {   
                var username = User.FindFirstValue(ClaimTypes.Name);
                var korisnik = korisnikCollection.Find(u => u.Username == username).FirstOrDefault();
                return korisnik;
                
            }catch(Exception e){
                return null;
            }
        }


        protected TeamBson authorizeLeader(string teamID)
        {
            try
            {
                
                var tim = teamCollection
                    .Aggregate()
                    .Lookup("korisnik", "leaderRef", "_id", "leader")
                    .Lookup("korisnik", "korisniciRef", "_id", "korisnici")
                    .As<TeamBson>()
                    .Match(Builders<TeamBson>.Filter.Eq(t => t.ID, teamID))
                    .FirstOrDefault();
                var lead = returnKorisnik();
                if(lead.ID == tim.Leader[0].ID)
                    return tim;
                else
                    return null;
            }catch(Exception e){
                return null;
            }
        }

        [Route("GetOwnTeams")]
        [HttpGet]
        public ActionResult GetOwnTeams() 
        {
            try{ 

                var username = User.FindFirstValue(ClaimTypes.Name);
                var kor = korisnikCollection.Find(x=>x.Username == username).FirstOrDefault();
                
                var timovi = teamCollection
                .Aggregate()
                .Lookup("korisnik", "korisniciRef", "_id", "korisnici")
                .Lookup("korisnik", "leaderRef", "_id", "leader")
                .As<TeamBson>()
                .Match(x => x.Korisnici.Select(k => k.Username).Contains(username))
                .ToList();

                if(timovi == null)
                {
                    return BadRequest("Korisnik nije ni u jednom timu");
                }

    
                return Ok(timovi.Select(t => new {
                    id = t.ID,
                    Ime = t.Ime,
                    Leader = new { Username = t.Leader[0].Username, id = t.Leader[0].ID },
                    Korisnici = t.Korisnici.Select(k => new {id = k.ID, Username = k.Username}),
                    Opis = t.Opis
                }));

            }catch(Exception e){
                return BadRequest(e.Message);
            }

        }

        [Route("GetTeam/{teamID}")]
        [HttpGet]
        public ActionResult getTeam([FromRoute] string teamID) 
        {
            try
            {
                var username = User.FindFirstValue(ClaimTypes.Name);
                var korisnik = korisnikCollection.Find(k => k.Username == username).FirstOrDefault();

                var team = teamCollection
                .Aggregate()
                .Lookup("korisnik", "korisniciRef", "_id", "korisnici")
                .Lookup("korisnik", "leaderRef", "_id", "leader")
                .As<TeamBson>()
                .Match(t => (t.ID == teamID) && (t.Korisnici.Select(k => k.Username).Contains(username)))
                .FirstOrDefault();

                if(team != null)
                    return Ok(team);
                return BadRequest("Ne postoji tim ili tim ne pripada korisniku");
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("GetOtherActiveTeams")]
        [HttpGet]

        public ActionResult GetOtherGetOtherActiveTeams()
        {
            try
            {
                var username = User.FindFirstValue(ClaimTypes.Name);
                var korisnik = korisnikCollection.Find(k => k.Username == username).FirstOrDefault();

                
                var timovi = teamCollection
                .Aggregate()
                .Lookup("korisnik", "korisniciRef", "_id", "korisnici")
                .Lookup("korisnik", "leaderRef", "_id", "leader")
                .As<TeamBson>()
                .Match(x => x.NeedsMembers == true && !x.Korisnici.Select(k => k.Username).Contains(username))
                .ToList();

                if(timovi == null)
                {
                    return BadRequest("Nema timova za join slobodnih");
                }
                /*
                var a = Context.Korisnici.Where(x => x.ID == idkor).FirstOrDefault();
                if(a == null) 
                {return BadRequest("Korisnik ne postoji");}

                List<Team> OtherTeams = new List<Team>();

                foreach (var tim in timovi)
                {
                    bool f = true;
                    foreach(var user in tim.Korisnici)
                    {
                        if(user.ID == idkor)
                        {
                            f=false;
                        }
                    }
                    if(f)
                    {
                        OtherTeams.Add(tim);
                    }
                }                                                                                                    
                */
                 return Ok(timovi.Select(t => new {
                    id = t.ID,
                    Ime = t.Ime,
                    Opis = t.Opis,
                    Leader = new { Username = t.Leader[0].Username, id = t.Leader[0].ID },
                    Korisnici = t.Korisnici.Select(k => new {id = k.ID, Username = k.Username})
                }));                                                                               
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

  
        [Route("KickUserFromTeam/{teamID}/{userID}")]
        [HttpPut] 
        public ActionResult KickUserFromTeam(string teamID, string userID)
        {
            try{ 

                var team = authorizeLeader(teamID);

                if(team == null)
                    return BadRequest("greska");

               var user = korisnikCollection.Find(k => k.ID == userID).FirstOrDefault();
               if(user == null)
                    return BadRequest("greska");

                
                if(team.Korisnici.Select(k => k.ID).Contains(userID))
                {
                    team.Korisnici.Remove(user);
                    team.KorisniciRef.Remove(ObjectId.Parse(user.ID));
                }else
                {
                    return BadRequest("greska");
                }

                
                teamCollection.UpdateOne(Builders<Team>.Filter.Eq(t => t.ID, team.ID), Builders<Team>.Update.Set(t => t.KorisniciRef, team.KorisniciRef));

                return Ok("user removed");
            }catch(Exception e){
                return BadRequest(e.Message);
            }
        }

        [Route("lookingForMembers/{teamID}/{flag}")]
        [HttpPut] 
        public ActionResult lookingForMembers ( string teamID, bool flag)
        {
            try
            {               
                var team = authorizeLeader(teamID);

                
                
                

                teamCollection.UpdateOne(Builders<Team>.Filter.Eq(t => t.ID, teamID), Builders<Team>.Update.Set(t => t.NeedsMembers, flag));

                return Ok("team updated");
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("TestMetoda")]
        [HttpGet]
        public ActionResult testiraj()
        {
            try
            {
                return Ok("Radi!");
            }
            catch(Exception e)
            {
                return BadRequest(e);
            }
        }
    }

}