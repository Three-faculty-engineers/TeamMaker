using Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]

    public class ObjavaController : ControllerBase
    {
        // public TeamMakerContext Context {get;set;}

        // public ObjavaController(TeamMakerContext context){
        //     Context = context;
        // }

        // [HttpGet]
        // [Route("GetObjave/{teamID}")]
        // public ActionResult GetObjave([FromRoute]int teamID){

        //     try{

        //         var username = User.FindFirstValue(ClaimTypes.Name);
        //         var korisnik=Context.Korisnici.Where( k => k.Username == username ).FirstOrDefault();

        //         var team = Context.Timovi.Include(t => t.Korisnici).Where( t=> t.ID == teamID).FirstOrDefault();
                
        //         if(!team.Korisnici.Contains(korisnik))
        //             return BadRequest("korisnik ne pripada timu");

        //         var objave = Context.Objave.Include(o=>o.Korisnik)
        //                                     .Where(o=>o.Team.ID==teamID)
        //                                     .ToList()
        //                                     .Select(o => new {
        //                                         ID = o.ID,
        //                                         Korisnik = new {ID = o.Korisnik.ID, Username = o.Korisnik.Username},
        //                                         Vreme = o.Vreme,
        //                                         Poruka = o.Poruka
        //                                     });
                                            

        //         return Ok(objave);
        //     }
        //     catch(Exception e)
        //     {
        //         return BadRequest(e.Message);
        //     }

        // }


        // [Route("CreateObjava/{teamID}/{poruka}")]
        // [HttpPost]
       
        // public ActionResult CreateObjava([FromRoute] int teamID, [FromRoute] string poruka)
        // {
        //     try{

        //         var username = User.FindFirstValue(ClaimTypes.Name);
        //         var korisnik=Context.Korisnici.Where( k => k.Username == username ).FirstOrDefault();

        //         var team = Context.Timovi.Where( t => t.ID == teamID ).Include(t => t.Korisnici).FirstOrDefault();

        //         if(korisnik == null || team == null)
        //             return BadRequest("korisnik ne postoji");
        //         if(!team.Korisnici.Contains(korisnik))
        //             return BadRequest("korisnik ne pripada timu"); 

        //         DateTime vremee = DateTime.Now;
        //         Objava objava = new Objava{ Korisnik=korisnik, Team=team, Poruka=poruka, Vreme=vremee };
                
        //         Context.Objave.Add(objava);
        //         Context.SaveChanges(); 

        //         return Ok("objava je kreirana");

        //     }catch(Exception e){

        //         return BadRequest(e.Message);

        //     }


        // }

        // [Route("GetObjaveSve")]
        // [HttpGet]
        // public ActionResult GetObjaveSve()
        // {
        //     try
        //     {
                
        //         var username = User.FindFirstValue(ClaimTypes.Name);
        //         var korisnik = Context.Korisnici.Include( t => t.Teams).ThenInclude(t => t.Korisnici).ThenInclude(o => o.Objave).Where( k => k.Username == username )
        //                                         .FirstOrDefault();
        //         List<Objava> lista = new List<Objava>(); 

        //         foreach( var team in korisnik.Teams)
        //         {
        //             lista = lista.Concat(team.Objave).ToList();
        //         }

        //         return Ok(lista.Select(t => new {id = t.ID, poruka = t.Poruka, team = t.Team.Ime, vreme=t.Vreme, korisnik = t.Korisnik.Username}).ToList());

        //     }catch(Exception e)
        //     {
        //         return BadRequest(e.Message);
        //     }
        // }


    }

}