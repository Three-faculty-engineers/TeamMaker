using Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Bson;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]


    public class PorukaController : ControllerBase
    {
        public TeamMakerContext Context {get;set;}

        private IMongoCollection<Poruka> porukaCollection;
        private IMongoCollection<Korisnik> korisnikCollection;

        public PorukaController(TeamMakerContext context)
        {
            Context = context;
            DataProvider dp = new DataProvider();
            porukaCollection = dp.ConnectToMongo<Poruka>("poruka");
            korisnikCollection = dp.ConnectToMongo<Korisnik>("korisnik");
        }
       
        [HttpGet]
        [Route("GetChats")]
        public ActionResult GetChats()
        {
            try
            {
                var username = User.FindFirstValue(ClaimTypes.Name);
                Korisnik k1 = korisnikCollection.Find(k => k.Username == username).FirstOrDefault();

                var korisnici = k1.PorukeRcv.Select(k => k.korisnikRef).Distinct().ToList();
                korisnici.AddRange(k1.PorukeSnd.Select(k => k.korisnikRef).Distinct().ToList());
            

                return Ok(korisnici.Distinct().Select(k => new 
                {
                    ID = k,
                    Username = korisnikCollection.Find(t => t.ID == k).FirstOrDefault().Username
                }));
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpGet]
        [Route("GetPorukeIzmedjuDvaKor/{kor2ID}")]
        public ActionResult GetPorukeIzmedjuDvaKor(string kor2ID) 
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            Korisnik k1 = korisnikCollection.Find(k => k.Username == username).FirstOrDefault();
            Korisnik k2 = korisnikCollection.Find(k => k.ID == kor2ID).FirstOrDefault();
          
            
            try
            {
                var por1 = k1.PorukeSnd.Where(p => p.korisnikRef == k2.ID).ToList().Select(p => new {
                    ID = p.ID,                   
                    KorisnikRcv = new { ID = k2.ID ,
                        Username = k2.Username},
                    KorisnikSnd = new { ID = k1.ID,
                        Username = k1.Username},
                    Tekst = p.Tekst,
                    Vreme = p.Vreme
                }).ToList();

               var por2 = k1.PorukeRcv.Where(p => p.korisnikRef == k2.ID).ToList().Select(p => new {
                    ID = p.ID,                    
                    KorisnikRcv = new { ID = k1.ID ,
                        Username = k1.Username},
                    KorisnikSnd = new { ID = k2.ID,
                        Username = k2.Username},
                    Tekst = p.Tekst,
                    Vreme = p.Vreme
                }); 

                por1.AddRange(por2);

                return Ok(por1);
                

            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("CreatePoruka/{korUsername2}/{poruka}")]
        [HttpPost]

         public ActionResult CreatePoruka(string korUsername2,string poruka)
        {

            try
            {
                 var username = User.FindFirstValue(ClaimTypes.Name);

                  Korisnik k1 = korisnikCollection.Find(k => k.Username == username).First();
                  Korisnik k2 = korisnikCollection.Find(k => k.Username == korUsername2).First();  
              
                
                if(k2 == null)
                    return BadRequest("Korisnik ne postoji");

                DateTime vremeSad = DateTime.Now;

                Poruka porr = new Poruka
                {
                    ID = ObjectId.GenerateNewId().ToString(),
                    Tekst = poruka,
                    Vreme = vremeSad,
                };  
                //Iz razloga sto svaki korisnika ima snd i rcv poruke pa je suvisno da se npr. u send nizu atributu pamti i sendref i isto za rcv.
                porr.korisnikRef = k2.ID;
                korisnikCollection.UpdateOne(Builders<Korisnik>.Filter.Eq(t => t.ID, k1.ID), Builders<Korisnik>.Update.Push(t => t.PorukeSnd, porr));

                
                porr.korisnikRef = k1.ID;
                korisnikCollection.UpdateOne(Builders<Korisnik>.Filter.Eq(t => t.ID, k2.ID), Builders<Korisnik>.Update.Push(t => t.PorukeRcv, porr));


                return Ok(new 
                {   userSent = k1.Username,
                    userReceived = k2.Username,
                    txt = porr.Tekst
                });
                    

            }
            catch(Exception e)
            {

                return BadRequest(e.Message);

            }


        }


    }

}