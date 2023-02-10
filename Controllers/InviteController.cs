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
    [Authorize]
    [Route("[controller]")]
    public class InviteController : ControllerBase
    {
        public IMongoCollection<Team> teamCollection;
        public IMongoCollection<Korisnik> korisnikCollection;
        public InviteController(){
                teamCollection = new DataProvider().ConnectToMongo<Team>("team");
                korisnikCollection = new DataProvider().ConnectToMongo<Korisnik>("korisnik");
        }
       
        [Route("RequestToJoin/{teamID}/{poruka}")]
        [HttpPost]
        public ActionResult requestToJoin([FromRoute] string teamID ,[FromRoute] string poruka) 
        {
            try{
                var invite = new Invite{Poruka = poruka, Verzija = 2};
                
                var tim = teamCollection.Find(t => t.ID == teamID).FirstOrDefault();
                var username = User.FindFirstValue(ClaimTypes.Name);
                
                var korisnik = korisnikCollection.Find(k => k.Username == username).FirstOrDefault();

                if(tim == null || korisnik == null)
                {
                    return BadRequest("korisnik ili tim ne postoji");
                }

                invite.ID = ObjectId.GenerateNewId().ToString();
                invite.KorisnikRef = ObjectId.Parse(korisnik.ID);
                teamCollection.FindOneAndUpdate(Builders<Team>.Filter.Eq(t => t.ID, tim.ID), Builders<Team>.Update.Push<Invite>(t => t.Invites, invite));
                
                
                

                
                
                return Ok("Request sent");

            }catch(Exception e){
                return BadRequest(e.Message);
            }
        }

        [Route("AcceptRequest")]
        [HttpDelete]
        public async Task<ActionResult> AcceptRequest([FromBody] Invite invite) 
        {
            try{
                
                var request = teamCollection.Find(Builders<Team>.Filter.ElemMatch(t => t.Invites, i => i.ID == invite.ID)).FirstOrDefault().Invites.FirstOrDefault(i => i.ID == invite.ID);
                
                var korisnik = await korisnikCollection.Find(k => k.ID == invite.Korisnik.ID).FirstOrDefaultAsync();
                
                var team = await teamCollection.Find(t => t.ID == invite.Team.ID ).FirstOrDefaultAsync();
               
                var username = User.FindFirstValue(ClaimTypes.Name);
                
                var leader = korisnikCollection.Find(k => k.Username == username).FirstOrDefault();

                if(request == null || korisnik == null || team == null){
                    return BadRequest("greska");
                }
                if(ObjectId.Parse(leader.ID) != team.LeaderRef){
                    return BadRequest("greska");
                }

                
                

                

                teamCollection.FindOneAndUpdate(Builders<Team>.Filter.Eq(t => t.ID, team.ID), Builders<Team>.Update.Push<ObjectId>(t => t.KorisniciRef, ObjectId.Parse(korisnik.ID)));
                teamCollection.FindOneAndUpdate(Builders<Team>.Filter.Eq(t => t.ID, team.ID), Builders<Team>.Update.PullFilter<Invite>(t => t.Invites, i => i.KorisnikRef == ObjectId.Parse(korisnik.ID)));
                
                
                return Ok("korisnik je dodat");

            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }

        }


        [Route("CheckInvites")]
        [HttpGet]
        public async Task<ActionResult> CheckInvitesKorisnik([FromBody]Korisnik k) 
        {
            try{
                
                
                var invites = (await teamCollection.FindAsync(Builders<Team>.Filter.ElemMatch(t => t.Invites, i => i.KorisnikRef == ObjectId.Parse(k.ID)))).ToList().Select(t => t.Invites.Find(i => i.KorisnikRef == ObjectId.Parse(k.ID)));

                return Ok(invites);

            }
            catch(Exception e){
                return BadRequest(e.Message);
            }
        }

        [Route("GetRequestsForTeam/{teamID}")]
        [HttpGet]
        public async Task<ActionResult> GetRequestsForTeam([FromRoute] string teamID)
        {
            try{
                var team = authorizeLeader(teamID);

                if(team == null)
                    return BadRequest();
                    
                
                
                
                
                
                
                
                

                var invites = (await teamCollection.FindAsync(t => t.ID == teamID)).FirstOrDefault().Invites.Select(i => {
                    i.Korisnik = korisnikCollection.Find(k => k.ID == i.KorisnikRef.ToString()).FirstOrDefault();
                    return i;
                });

                return Ok(invites);

            }
            catch(Exception e){
                return BadRequest(e.Message);
            }
        }

        [Route("AcceptUser/{userR}/{inviteID}/{teamID}")] 
        [HttpPut]
        public ActionResult AcceptUser(string userR, string inviteID, string teamID) 
        {
            try{
                var username = User.FindFirstValue(ClaimTypes.Name);
                
                var korisnik = korisnikCollection.Find(k => k.Username == username).FirstOrDefault();
                if(korisnik == null)
                    return BadRequest();
                
                
                
                
                
                
                
                var team = teamCollection
                    .Aggregate()
                    .Lookup("korisnik", "leaderRef", "_id", "leader")
                    .Lookup("korisnik", "korisniciRef", "_id", "korisnici")
                    .As<TeamBson>()
                    .FirstOrDefault();

                if(team == null)
                    return BadRequest();
                

                if(team.Leader[0].ID != korisnik.ID)
                    return BadRequest();


                
                var invite = team.Invites.Find(i => i.ID == inviteID);
                if(invite == null)
                    return BadRequest();

                
                var user = korisnikCollection.Find(k => k.Username == userR).FirstOrDefault();

                if(user == null)
                    return BadRequest();
            

                
                teamCollection.FindOneAndUpdate(Builders<Team>.Filter.Eq(t => t.ID, team.ID), Builders<Team>.Update.Push<ObjectId>(t => t.KorisniciRef, ObjectId.Parse(user.ID)));

                teamCollection.FindOneAndUpdate(Builders<Team>.Filter.Eq(t => t.ID, team.ID), Builders<Team>.Update.PullFilter<Invite>(t => t.Invites, i => i.ID == inviteID));

                
                
                

                return Ok(new {
                    ID = user.ID,
                    Username = user.Username
                });

            }
            catch(Exception e){
                return BadRequest(e.Message);
            }
        }
       protected Korisnik returnKorisnik()
        {
            try
            {   
                var username = User.FindFirstValue(ClaimTypes.Name);
                var korisnik = korisnikCollection.Find( u => u.Username == username).FirstOrDefault();
                return korisnik;
                
            }catch(Exception e){
                return null;
            }
        }
        
        protected Team authorizeLeader(string teamID)
        {
            try
            {
                
                
                
                
                
                var tim = teamCollection
                    .Aggregate()
                    .Lookup("korisnik", "leaderRef", "_id", "leader")
                    .Lookup("korisnik", "korisnikRef", "_id", "korisnici")
                    .As<TeamBson>()
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



        
    }

}