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

    public class ObjavaController : ControllerBase
    {
        public TeamMakerContext Context {get;set;}

        private IMongoCollection<Objava> objavaCollection;
        private IMongoCollection<Korisnik> korisnikCollection;
        private IMongoCollection<Team> teamCollection;

        public ObjavaController(TeamMakerContext context){
            Context = context;
            DataProvider dp = new DataProvider();
            objavaCollection = dp.ConnectToMongo<Objava>("objava");
            korisnikCollection = dp.ConnectToMongo<Korisnik>("korisnik");
            teamCollection = dp.ConnectToMongo<Team>("team");
        }

        [HttpGet]
        [Route("GetObjave/{teamID}")]
        public ActionResult GetObjave([FromRoute]string teamID){

            try{

                var username = User.FindFirstValue(ClaimTypes.Name);
                var korisnik = korisnikCollection.Find(k => k.Username == username).FirstOrDefault();

                var team = teamCollection.Find(t => t.ID == teamID).FirstOrDefault();


                if(!team.KorisniciRef.Contains(ObjectId.Parse(korisnik.ID)))
                    return BadRequest("korisnik ne pripada timu");

                var objave = team.Objave.ToList().Select(o => new {
                                                        ID = o.ID,
                                                        Korisnik = new {ID = korisnik.ID, Username = korisnik.Username},
                                                        Vreme = o.vreme,
                                                        Poruka = o.poruka
                });
         
                                            

                return Ok(objave);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }

        }


        [Route("CreateObjava/{teamID}/{poruka}")]
        [HttpPost]
       
        public ActionResult CreateObjava([FromRoute] string teamID, [FromRoute] string poruka)
        {
            try{

                var username = User.FindFirstValue(ClaimTypes.Name);
                var korisnik = korisnikCollection.Find(k => k.Username == username).FirstOrDefault();
       
                 var team = teamCollection.Aggregate()
                     .Lookup("korisnik", "korisniciRef", "_id", "korisnici")
                     .As<TeamBson>()
                     .Match(t => t.ID == teamID)
                     .FirstOrDefault();

                if(korisnik == null || team == null)
                    return BadRequest("korisnik ne postoji");
                if(!team.KorisniciRef.Contains(ObjectId.Parse(korisnik.ID)))
                    return BadRequest("korisnik ne pripada timu"); 

                DateTime vremeSada = DateTime.Now;
                Objava objava = new Objava
                { 
                    ID = ObjectId.GenerateNewId().ToString(),
                    korisnikRef = korisnik.ID,
                    poruka = poruka,
                    vreme= vremeSada 
                };
                

                teamCollection.UpdateOne(Builders<Team>.Filter.Eq(t => t.ID, teamID), Builders<Team>.Update.Push(t => t.Objave, objava));
                return Ok("objava je kreirana");

            }catch(Exception e){

                return BadRequest(e.Message);

            }


        }

        [Route("GetObjaveSve")]
        [HttpGet]
        public ActionResult GetObjaveSve()
        {
            try
            {
                
                var username = User.FindFirstValue(ClaimTypes.Name);
                var korisnik = korisnikCollection.Find(k => k.Username == username).FirstOrDefault();
               
                var timovi = teamCollection.Find(t => t.KorisniciRef.Contains(ObjectId.Parse(korisnik.ID))).ToList();

                List<Objava> lista = new List<Objava>(); 

                var objave = timovi.Select(t => t.Objave.Select(o => new {
                                                    id = o.ID,
                                                    poruka = o.poruka,
                                                    team = t.Ime,
                                                    vreme = o.vreme,
                                                    korisnik = korisnikCollection.Find(k => k.ID == o.korisnikRef).FirstOrDefault().Username
                })).SelectMany(t => t);
                
                return Ok(objave);
                

            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }


    }

}