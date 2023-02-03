using Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]


    public class PorukaController : ControllerBase
    {
        // public TeamMakerContext Context {get;set;}

        // public PorukaController(TeamMakerContext context){
        //     Context = context;
        // }

        // // [HttpGet]
        // // [Route("GetPorukeKorisnik/{kor}")]

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

        
        // [HttpGet]
        // [Route("GetChats")]
        // public ActionResult GetChats()
        // {
        //     try
        //     {
        //         var username = User.FindFirstValue(ClaimTypes.Name);
        //         Korisnik k1 = Context.Korisnici.Where(k => k.Username == username).FirstOrDefault();
        //         int korID = k1.ID;


        //         var korisnici = Context.Poruke.Include(k => k.KorisnikRcv)
        //                                         .Include(k => k.KorisnikSnd)
        //                                         .Where(o => o.KorisnikRcv.ID == korID || o.KorisnikSnd.ID == korID)
        //                                         .Select(p =>  new{
        //                                             ID = (p.KorisnikRcv.ID == korID) ? p.KorisnikSnd.ID : p.KorisnikRcv.ID,
        //                                             Username = (p.KorisnikRcv.ID == korID) ? p.KorisnikSnd.Username : p.KorisnikRcv.Username,
        //                                         })
        //                                         .ToList();
                


        //         return Ok(korisnici.Distinct());
        //     }
        //     catch(Exception e)
        //     {
        //         return BadRequest(e.Message);
        //     }
        // }


        // [HttpGet]
        // [Route("GetPorukeIzmedjuDvaKor/{kor2ID}")]

        // public ActionResult GetPorukeIzmedjuDvaKor(int kor2ID)
        // {

        //     var username = User.FindFirstValue(ClaimTypes.Name);
        //     Korisnik k1 = Context.Korisnici.Where(k => k.Username == username).FirstOrDefault();
        //     int kor1ID = k1.ID;

        //     try{

        //         var poruke = Context.Poruke.Where( k => ( (k.KorisnikRcv.ID == kor1ID && k.KorisnikSnd.ID == kor2ID ) ||
        //                                     (k.KorisnikRcv.ID==kor2ID && k.KorisnikSnd.ID == kor1ID) ) )
        //                                     .Select( p => new {                                               
        //                                         ID = p.ID,
        //                                         KorisnikRcv = new { ID = p.KorisnikRcv.ID, Username = p.KorisnikRcv.Username},
        //                                         KorisnikSnd = new { ID = p.KorisnikSnd.ID, Username = p.KorisnikSnd.Username},
        //                                         Tekst = p.Tekst,
        //                                         Vreme = p.Vreme
        //                                     })
        //                                     .ToList();
        //         return Ok(poruke);

        //     }
        //     catch(Exception e)
        //     {
        //         return BadRequest(e.Message);
        //     }
        // }

        // [HttpGet]
        // [Route("GetAllChats/{korID}")]

        // public ActionResult GetAllChats(int korID)
        // {
        //     try{
        //         var username = User.FindFirstValue(ClaimTypes.Name);
        //         var korisnik = Context.Korisnici.Where( k => k.Username == username && k.ID == korID).FirstOrDefault();
        //         if(korisnik == null)
        //             return BadRequest("bad request");
                    
        //         var s=Context.Poruke.Where(k=>k.KorisnikRcv.ID==korID).Select(x=>x.KorisnikSnd).Distinct();
        //         var d=Context.Poruke.Where(k=>k.KorisnikSnd.ID==korID).Select(x=>x.KorisnikRcv).Distinct();

        //         var both = s.Concat(d).Distinct();
        //         return Ok(both);


              
        //     }
        //     catch(Exception e)
        //     {
        //         return BadRequest(e.Message);
        //     }
        // }

        // [Route("CreatePoruka/{korUsername2}/{poruka}")]
        // [HttpPost]

        //  public ActionResult CreatePoruka(string korUsername2,string poruka)
        // {

        //     try{
        //         var username = User.FindFirstValue(ClaimTypes.Name);

        //         Korisnik k1=Context.Korisnici.Where(k=>k.Username==username).First();
        //         Korisnik k2=Context.Korisnici.Where(k=>k.Username==korUsername2).First();
                
        //         if(k2 == null)
        //             return BadRequest("Korisnik ne postoji");

        //         DateTime vremee = DateTime.Now;

        //         if(k1!=null&&k2!=null){

        //         Poruka porr = new Poruka{KorisnikSnd=k1,Tekst=poruka,KorisnikRcv=k2,Vreme=vremee};
        //         Context.Poruke.Add(porr);
        //         Context.SaveChanges(); 

        //         return Ok(new {ID = k2.ID,
        //                     Username = k2.Username});
        //         }
        //         return BadRequest("ne");

        //     }catch(Exception e){

        //         return BadRequest(e.Message);

        //     }


        // }


    }

}