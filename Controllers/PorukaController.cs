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

        // [HttpGet]
        // [Route("GetPorukeKorisnik/{kor}")]

        // // public ActionResult GetPoruke(int kor){

        // //     try{
        // //         var poruke= Context.Poruke.Where(o=>o.KorisnikRcv.ID==kor||o.KorisnikSnd.ID==kor);

        // //         return Ok(poruke);
        // //     }
        // //     catch(Exception e)
        // //     {
        // //         return BadRequest(e.Message);
        // //     }

        // // }

        
        [HttpGet]
        [Route("GetChats")]
        public ActionResult GetChats()
        {
            try
            {
                var username = User.FindFirstValue(ClaimTypes.Name);
                Korisnik k1 = korisnikCollection.Find(k => k.Username == username).FirstOrDefault();
                string korID = k1.ID;

                
                var korisnici = porukaCollection.Aggregate()
                    .Lookup("korisnik", "KorisnikSndRef", "_id", "korisniksnd")
                    .Lookup("korisnik", "KorisnikRcvRef", "_id", "korisnikrcv")
                    .As<Poruka>()
                    .Match(k => k.KorisnikRcvRef == korID || k.KorisnikSndRef == korID)
                    .ToList()
                    .Select(p => new
                    {
                        ID = (p.KorisnikRcvRef == korID) ? p.KorisnikSndRef : p.KorisnikRcvRef,
                        Username = (p.KorisnikRcvRef == korID) ? p.KorisnikSnd.Where(k => k.ID == p.KorisnikSndRef).Select(u => u.Username).FirstOrDefault()
                                                                : p.KorisniRcv.Where(k => k.ID == p.KorisnikRcvRef).Select(u => u.Username).FirstOrDefault()
                    }).ToList();


                return Ok(korisnici.Distinct());
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpGet]
        [Route("GetPorukeIzmedjuDvaKor/{kor2ID}")]
        public ActionResult GetPorukeIzmedjuDvaKor(string kor2ID) //front-end int?
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            Korisnik k1 = korisnikCollection.Find(k => k.Username == username).FirstOrDefault();
            Korisnik k2 = korisnikCollection.Find(k => k.ID == kor2ID).FirstOrDefault();
          
            string kor1ID = k1.ID;
            try{
                                        

                var poruke = porukaCollection.Find(k => ((k.KorisnikRcvRef == kor1ID && k.KorisnikSndRef == kor2ID) ||
                                                        (k.KorisnikRcvRef == kor2ID && k.KorisnikSndRef == kor1ID))).ToList().
                                                        Select(p =>                                                       
                                                        new
                                                        {
                                                            ID = p.ID,
                                                            KorisnikRcv = new { ID = p.KorisnikRcvRef,
                                                                Username = p.KorisnikRcvRef == kor1ID ? username : k2.Username},
                                                            KorisnikSnd = new { ID = p.KorisnikSndRef,
                                                                Username = p.KorisnikSndRef == kor1ID ? username : k2.Username},
                                                            Tekst = p.Tekst,
                                                            Vreme = p.Vreme
                                                        });
                                                                                            
                return Ok(poruke);

            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // [HttpGet]
        // [Route("GetAllChats/{korID}")]

        // public ActionResult GetAllChats(int korID)
        // {
        //     try{
        //         var username = User.FindFirstValue(ClaimTypes.Name);
        //         var korisnik = Context.Korisnici.Where( k => k.Username == username /*&& k.ID == korID*/).FirstOrDefault();
        //         if(korisnik == null)
        //             return BadRequest("bad request");

        //         var s = porukaCollection.Find(k => k.KorisnikRcvRef == korisnik.ID).;
        //         var d = porukaCollection.Find(k => k.KorisnikSndRef == korisnik.ID);
        //         //var s=Context.Poruke.Where(k=>k.KorisnikRcvRef.ID==korID).Select(x=>x.KorisnikSnd).Distinct();
        //         //var d=Context.Poruke.Where(k=>k.KorisnikSndRef.ID==korID).Select(x=>x.KorisnikRcv).Distinct();
            
        //         //var both = s.Concat(d).Distinct();
        //         return Ok(s);


              
        //     }
        //     catch(Exception e)
        //     {
        //         return BadRequest(e.Message);
        //     }
        // }

        [Route("CreatePoruka/{korUsername2}/{poruka}")]
        [HttpPost]

         public ActionResult CreatePoruka(string korUsername2,string poruka)
        {

            try
            {
                var username = User.FindFirstValue(ClaimTypes.Name);

                  Korisnik k1 = korisnikCollection.Find(k => k.Username == username).First();
               // Korisnik k1=Context.Korisnici.Where(k=>k.Username==username).First();
                  Korisnik k2 = korisnikCollection.Find(k => k.Username == korUsername2).First();  
               // Korisnik k2=Context.Korisnici.Where(k=>k.Username==korUsername2).First();
                
                if(k2 == null)
                    return BadRequest("Korisnik ne postoji");

                DateTime vremee = DateTime.Now;

                if(k1!=null&&k2!=null){

                Poruka porr = new Poruka{KorisnikSndRef=k1.ID,Tekst=poruka,KorisnikRcvRef=k2.ID,Vreme=vremee};
                porukaCollection.InsertOne(porr);
                //Context.Poruke.Add(porr)
                //Context.SaveChanges(); 

                return Ok(new 
                {   userSent = k1.Username,
                    userReceived = k2.Username,
                    txt = porr.Tekst
                });
                }
                return BadRequest("ne");

            }
            catch(Exception e)
            {

                return BadRequest(e.Message);

            }


        }


    }

}