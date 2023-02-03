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
        
        public TeamMakerContext Context {get;set;}
        public IMongoCollection<Team> teamCollection;
        public IMongoCollection<Korisnik> korisnikCollection;
        public InviteController(TeamMakerContext context){
                Context=context;
                teamCollection = new DataProvider().ConnectToMongo<Team>("team");
                korisnikCollection = new DataProvider().ConnectToMongo<Korisnik>("korisnik");
        }
       
        [Route("RequestToJoin/{teamID}/{poruka}")]
        [HttpPost]
        public ActionResult requestToJoin([FromRoute] string teamID ,[FromRoute] string poruka) // radi
        {
            try{
                var invite = new Invite{Poruka = poruka, Verzija = 2};
                // var tim = Context.Timovi.Where(t => t.ID  == teamID).FirstOrDefault();
                var tim = teamCollection.Find(t => t.ID == teamID).FirstOrDefault();
                var username = User.FindFirstValue(ClaimTypes.Name);
                // var korisnik = Context.Korisnici.Where(k => k.Username == username).FirstOrDefault();
                var korisnik = korisnikCollection.Find(k => k.Username == username).FirstOrDefault();

                if(tim == null || korisnik == null)
                {
                    return BadRequest("korisnik ili tim ne postoji");
                }

                invite.ID = ObjectId.GenerateNewId().ToString();
                invite.KorisnikRef = ObjectId.Parse(korisnik.ID);
                teamCollection.FindOneAndUpdate(Builders<Team>.Filter.Eq(t => t.ID, tim.ID), Builders<Team>.Update.Push<Invite>(t => t.Invites, invite));
                // korisnikCollection.FindOneAndUpdate(Builders<Korisnik>.Filter.Eq(k => k.ID, korisnik.ID), Builders<Korisnik>.Update.Push<Invite>(k => k.Invites, invite));
                // invite.Korisnik = (Korisnik)korisnik;
                // invite.Team = (Team) tim;

                // Context.Invites.Add(invite);
                // await Context.SaveChangesAsync();
                return Ok("Request sent");

            }catch(Exception e){
                return BadRequest(e.Message);
            }
        }

        [Route("AcceptRequest")]
        [HttpDelete]
        public async Task<ActionResult> AcceptRequest([FromBody] Invite invite) // radi
        {
            try{
                // var request = await Context.Invites.Where(i => i.ID == invite.ID && i.Verzija==2).FirstOrDefaultAsync();
                var request = teamCollection.Find(Builders<Team>.Filter.ElemMatch(t => t.Invites, i => i.ID == invite.ID)).FirstOrDefault().Invites.FirstOrDefault(i => i.ID == invite.ID);
                // var korisnik = await Context.Korisnici.Where(k => k.ID == invite.Korisnik.ID).FirstOrDefaultAsync();
                var korisnik = await korisnikCollection.Find(k => k.ID == invite.Korisnik.ID).FirstOrDefaultAsync();
                // var team = await Context.Timovi.Where(t => t.ID == invite.Team.ID ).FirstOrDefaultAsync();
                var team = await teamCollection.Find(t => t.ID == invite.Team.ID ).FirstOrDefaultAsync();
               
                var username = User.FindFirstValue(ClaimTypes.Name);
                // var leader = Context.Korisnici.Where(k => k.Username == username).FirstOrDefault();
                var leader = korisnikCollection.Find(k => k.Username == username).FirstOrDefault();

                if(request == null || korisnik == null || team == null){
                    return BadRequest("greska");
                }
                if(ObjectId.Parse(leader.ID) != team.LeaderRef){
                    return BadRequest("greska");
                }

                // team.Korisnici.Add(korisnik);
                // Context.Invites.Remove(request);

                // await Context.SaveChangesAsync();

                teamCollection.FindOneAndUpdate(Builders<Team>.Filter.Eq(t => t.ID, team.ID), Builders<Team>.Update.Push<ObjectId>(t => t.KorisniciRef, ObjectId.Parse(korisnik.ID)));
                teamCollection.FindOneAndUpdate(Builders<Team>.Filter.Eq(t => t.ID, team.ID), Builders<Team>.Update.PullFilter<Invite>(t => t.Invites, i => i.KorisnikRef == ObjectId.Parse(korisnik.ID)));
                // korisnikCollection.FindOneAndUpdate(Builders<Korisnik>.Filter.Eq(k => k.ID, korisnik.ID), Builders<Korisnik>.Update.PullFilter<Invite>(k => k.Invites, i => i.ID == request.ID));
                
                return Ok("korisnik je dodat");

            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }

        }


        [Route("CheckInvites")]
        [HttpGet]
        public async Task<ActionResult> CheckInvitesKorisnik([FromBody]Korisnik k) // radi
        {
            try{
                // var invites = await Context.Invites.Where(t=>t.Korisnik.ID==k.ID && t.Verzija==1).ToListAsync();
                // var invites = (await teamCollection.FindAsync(Builders<Team>.Filter.ElemMatch(t => t.Invites, i => i.KorisnikRef.ToString() == k.ID))).FirstOrDefault();
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
                    
                // var invites = await Context.Invites.Where(t => t.Team.ID==teamID && t.Verzija==2)
                //                                     .Include(t => t.Korisnik)
                //                                     .Select(t => new {
                //                                         ID = t.ID,
                //                                         Username = t.Korisnik.Username,
                //                                         Poruka = t.Poruka
                //                                     })
                //                                     .ToListAsync();

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
        public ActionResult AcceptUser(string userR, string inviteID, string teamID) // radi
        {
            try{
                var username = User.FindFirstValue(ClaimTypes.Name);
                // var korisnik = Context.Korisnici.Where(k => k.Username == username).FirstOrDefault();//mi
                var korisnik = korisnikCollection.Find(k => k.Username == username).FirstOrDefault();
                if(korisnik == null)
                    return BadRequest();
                
                // var team = Context.Timovi.Where(t => t.ID == teamID).Include(t=>t.Leader).Include(t=>t.Korisnici).FirstOrDefault();
                // var team = teamCollection.Find(t => t.ID == teamID).ToList().Select(t => {
                //     t.Leader = korisnikCollection.Find(k => k.ID == t.LeaderRef.Id.AsString).FirstOrDefault();
                //     return t;
                // }).FirstOrDefault();
                
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


                // var invite = Context.Invites.Where( i => i.ID == inviteID).FirstOrDefault();
                var invite = team.Invites.Find(i => i.ID == inviteID);
                if(invite == null)
                    return BadRequest();

                // var user = Context.Korisnici.Where(k => k.Username == userR).FirstOrDefault(); 
                var user = korisnikCollection.Find(k => k.Username == userR).FirstOrDefault();

                if(user == null)
                    return BadRequest();
            

                // team.Korisnici.Add(user);
                teamCollection.FindOneAndUpdate(Builders<Team>.Filter.Eq(t => t.ID, team.ID), Builders<Team>.Update.Push<ObjectId>(t => t.KorisniciRef, ObjectId.Parse(user.ID)));

                teamCollection.FindOneAndUpdate(Builders<Team>.Filter.Eq(t => t.ID, team.ID), Builders<Team>.Update.PullFilter<Invite>(t => t.Invites, i => i.ID == inviteID));

                // Context.Invites.Remove(invite);
                
                // await Context.SaveChangesAsync();

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
                // var tim = Context.Timovi.Include(t => t.Leader).Include(t => t.Korisnici).Where(t => t.ID == teamID).FirstOrDefault();
                // var tim = teamCollection.Find(t => t.ID == teamID).ToList().Select(t => {
                //     t.Leader = korisnikCollection.Find(k => k.ID == t.LeaderRef.Id.AsString).FirstOrDefault();
                //     return t;
                // }).FirstOrDefault();
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