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
    [Authorize]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        
        public TeamMakerContext Context {get;set;}
        private readonly IConfiguration _configuration;
        private IMongoCollection<Korisnik> korisnikCollection;
        private IMongoCollection<Team> teamCollection;
        public AdminController(TeamMakerContext context, IConfiguration configuration){
            Context=context;
            _configuration = configuration;
            
            korisnikCollection = new DataProvider().ConnectToMongo<Korisnik>("korisnik");
            teamCollection = new DataProvider().ConnectToMongo<Team>("team");
        }

        [Route("TestMetoda")]
        [HttpGet]
        [Authorize(Roles = "Admin")]

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

        [Route("GetAllTeams")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult GetAllTeams() // radi
        {
            try{
                var teams = teamCollection
                    .Aggregate()
                    .Lookup("korisnik", "korisniciRef", "_id", "korisnici")
                    .Lookup("korisnik", "leaderRef", "_id", "leader")
                    .As<TeamBson>()
                    .ToList();

                if(teams == null || teams.Count == 0)
                {
                  return BadRequest("Nijedan tim u bazi ne postoji");
                }
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

        [Route("DeleteTeam/{id}")]
        [HttpDelete]
        [Authorize(Roles = "Admin")]

        public  ActionResult DeleteTeam(string id) // radi
        {
            try{
                var filter = Builders<Team>.Filter.Eq(t => t.ID, id);
                teamCollection.DeleteOne(filter);
                // var o =  Context.Timovi.Where(a=>a.ID==id).Include(x => x.Objave).
                                                                                
                //                                                                     Include(x => x.Invites).
                //                                                                     Include(x => x.Sprints).
                //                                                                     Include(x => x.Tasks).
                                                                            
                //                                                                     FirstOrDefault();
                
                // if(o == null ){
                //     return BadRequest("greska");
                // }
                
         
                //  foreach (var item in o.Invites)
                // {
                //   Context.Invites.Remove(item);  
                // }

                //  foreach (var item in o.Sprints)
                // {
                //   Context.Sprints.Remove(item);  
                // }

                //  foreach (var item in o.Tasks)
                // {
                //   Context.Taskovi.Remove(item);  
                // }

                //   foreach (var item in o.Objave)
                // {
                //   Context.Objave.Remove(item);  
                // }


                //  Context.Timovi.Remove(o);

                //  Context.SaveChanges();
                return Ok("Tim je izbrisan");

            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("GetAllKorisnici")]
        [HttpGet]
        [Authorize(Roles = "Admin")]

        public ActionResult GetAllKorisnici() // radi
        {
            try
            {
                // var o = Context.Korisnici.Where(x=> x.Tip == 1).Include(x => x.Teams).ToList();
                var o = korisnikCollection.Find(k => k.Tip == 1).ToList();

                if(o == null)
                {
                  return BadRequest("Nijedan standardan korisnik ne postoji");
                }

                 return Ok(o.Select(t => new {
                    ID = t.ID,
                    Username = t.Username,
                    Ime = t.Ime,
                    Prezime = t.Prezime,
                    Opis = t.Opis,
                    Indeks = t.Indeks,
                    Email = t.Email,
                    Fakultet = t.Fakultet
                }));
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [Route("DeleteKorisnik/{id}")]
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public  ActionResult DeleteKorisnik(string id) // radi brisanje korisnika, treba da se odradi brisanje ostalih entiteta koji su povezani sa njim
        {
            try{
                // var o =  Context.Korisnici.Where(a=>(a.ID==id) && (a.Tip == 1)).Include(o => o.InvitesRcv).
                //                                                                     Include(x => x.Objave).
                //                                                                     Include(x => x.OceneRcv).
                //                                                                     Include(x => x.OceneSnd).
                //                                                                     Include(x => x.Taskovi).
                //                                                                     Include(x => x.PorukeRcv).
                //                                                                     Include(x => x.PorukeSnd).
                //                                                                     Include(x => x.TeamLeader).                                                                                
                //                                                                     FirstOrDefault();

                var filter = Builders<Korisnik>.Filter.Eq(k => k.ID, id);
                korisnikCollection.DeleteOne(filter);
                
                // if(o == null ){
                //     return BadRequest("greska");
                // }

                //  foreach (var item in o.TeamLeader)
                // {
                //   o.TeamLeader = null;
                // }
                
           
                //  foreach (var item in o.PorukeRcv)
                // {
                //   Context.Poruke.Remove(item);  
                // }

                //  foreach (var item in o.PorukeRcv)
                // {
                //   Context.Poruke.Remove(item);  
                // }

                // foreach (var item in o.Objave)
                // {
                //   Context.Objave.Remove(item);  
                // }

                // foreach(var item in o.Taskovi)
                // {
                //     Context.Taskovi.Remove(item);
                // }

                // foreach(var item in o.InvitesRcv)
                // {
                //     Context.Invites.Remove(item);
                // }

                //  Context.Korisnici.Remove(o);

                //  Context.SaveChanges();
                return Ok("Korisnik je uspesno izbrisan");

            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }


    
    }

}