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

namespace WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class KorisnikController : ControllerBase
    {

        public TeamMakerContext Context { get; set; }
        private readonly IConfiguration _configuration;

        IMongoCollection<Korisnik> korisnikCollection;

        public KorisnikController(TeamMakerContext context, IConfiguration configuration)
        {
            Context = context;
            _configuration = configuration;
            korisnikCollection = new DataProvider().ConnectToMongo<Korisnik>("korisnik");
        }


        [Route("LogIn")]
        [AllowAnonymous]
        [HttpPost]
        public ActionResult LogIn(Korisnik korisnik){
            try{
                var k = korisnikCollection.Find(k => k.Username == korisnik.Username).FirstOrDefault();

                if(k == null)
                {
                    return BadRequest("ne postoji takav profil");
                }

                byte[] saltConvert = Convert.FromBase64String(k.Salt);

                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: korisnik.Sifra,
                    salt: saltConvert,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));

                if(k.Sifra == hashed)
                {         
                    string token = CreateToken(k);   
                    CheckSprints(k);
                    return Ok(new {token = token, username = k.Username, role = k.Tip});
                }
                else 
                {
                    return BadRequest("ne postoji takav profil");
                }
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private void CheckSprints(Korisnik korisnik)
        {

            try
            {
                foreach(var team in korisnik.Teams)
                {
                    foreach(var sprint in team.Sprints)
                    {
                        if(sprint.EndSprint < DateTime.Now && sprint.Status == 0)
                        {

                            sprint.Status = 1;
                        }
                    }
                }
                Context.SaveChanges();
            }
            catch(Exception e){

            }
        }

        private string CreateToken(Korisnik kor)
        {
            List<Claim> claims = null;
            if(kor.Tip == 1)
            {
                claims = new List<Claim>{
                    new Claim(ClaimTypes.Name, kor.Username),
                    new Claim(ClaimTypes.Role, "Standard")

                };
            }else if(kor.Tip == 2)
            {
                 claims = new List<Claim>{
                    new Claim(ClaimTypes.Name, kor.Username),
                    new Claim(ClaimTypes.Role, "Admin")

                };
            }

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1), 
                signingCredentials: creds
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }


        [Route("Register")]
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register([FromBody] Korisnik kor)
        {
            try
            {
                var korisnici = korisnikCollection.Find(k => k.Username == kor.Username || k.Username == kor.Email || k.Indeks == kor.Indeks).ToList();

                foreach (var user in korisnici)
                {
                    if (kor.Username == user.Username)
                    {
                        return BadRequest("Korisnik sa username-om vec postoji");
                    }
                    if (kor.Email == user.Email)
                    {
                        return BadRequest("Korisnik sa Email-om vec postoji");
                    }
                    if (kor.Indeks == user.Indeks)
                    {
                        return BadRequest("Korisnik sa Indeksom vec postoji");
                    }
                }
                if (string.IsNullOrEmpty(kor.Ime) || kor.Ime.Length > 50)
                {
                    return BadRequest("length");
                }
                if (string.IsNullOrEmpty(kor.Prezime) || kor.Prezime.Length > 50)
                {
                    return BadRequest("length");
                }
                if (string.IsNullOrEmpty(kor.Email) || kor.Email.Length > 150)
                {
                    return BadRequest("length");
                }
                if (string.IsNullOrEmpty(kor.Username) || kor.Username.Length > 50)
                {
                    return BadRequest("length");
                }
                if (string.IsNullOrEmpty(kor.Sifra) || kor.Sifra.Length > 50)
                {
                    return BadRequest("length");
                }
                if (kor.Indeks < 0 || kor.Indeks > 9999999)
                {
                    return BadRequest("indeks");
                }
                kor.Tip = 1;
                byte[] salt = new byte[128 / 8];

                salt = RandomNumberGenerator.GetBytes(16);
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: kor.Sifra,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));
                kor.Sifra = hashed;
                kor.Salt = Convert.ToBase64String(salt);

                kor.OceneRcv = new List<Ocena>();
                kor.OceneSnd = new List<Ocena>();
                kor.PorukeRcv = new List<Poruka>();
                kor.PorukeSnd = new List<Poruka>();
                
                korisnikCollection.InsertOne(kor);
                return Ok("Korisnik je uspesno registrovan");

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [Route("getUserFromToken")]
        [HttpPost]
        [Authorize]
        public ActionResult getUserFromToken()
        {
            try
            {
                string username = User?.Identity?.Name;
                var korisnik = korisnikCollection.Find(k => k.Username == username).FirstOrDefault();

                if (korisnik == null)
                    return BadRequest("korisnik ne postoji");
                return Ok(korisnik);

            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
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
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [Route("updateKorisnika")]
        [HttpPut]
        [Authorize]
        public ActionResult updateKorisnika([FromBody] Korisnik kor)
        {
            try
            {

                string username = User.FindFirstValue(ClaimTypes.Name);

                var korisnik = korisnikCollection.Find(k => k.Username == username).FirstOrDefault();

                if (korisnik == null)
                    return BadRequest("korisnik ne postoji");

                
                byte[] salt = new byte[128 / 8];
                salt = RandomNumberGenerator.GetBytes(16);
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: kor.Sifra,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));

                korisnik.Sifra = hashed;
                korisnik.Salt = Convert.ToBase64String(salt);

                var filter = Builders<Korisnik>.Filter.Eq(s => s.Username, username);
                korisnikCollection.ReplaceOne(filter, korisnik);

                return Ok(korisnik);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }

        // [Route("uploadImage")]
        // [HttpPut]
        // [Authorize]
        // public async Task<IActionResult> UploadImageProfile([FromForm]FileViewModel fileviewmodel)
        // {

        //     string username = User.FindFirstValue(ClaimTypes.Name);

        //     if (ModelState.IsValid)
        //     {
        //         using (var memoryStream = new MemoryStream())
        //         {
        //             await fileviewmodel.File.CopyToAsync(memoryStream);


        //             if (memoryStream.Length < 2097152)
        //             {

        //                 var korisnik = Context.Korisnici.Where(k => k.Username == username).FirstOrDefault();
        //                 korisnik.Slika = memoryStream.ToArray();

        //             }else
        //             {
        //                     ModelState.AddModelError("File", "The file is too large.");
        //             }

        //             Context.SaveChanges();
        //         }

        //     }

        //     var returndata = Context.Korisnici
        //         .Where(k => k.Username == username)
        //         .Select(c => new ReturnData()
        //         {
        //             Name = c.Username,
        //             ImageBase64 = String.Format("data:image/png;base64,{0}", Convert.ToBase64String(c.Slika))
        //         }).FirstOrDefault();

        //     return Ok(returndata);
        // }

        // [Route("getCurrentUserImage")]
        // [HttpGet]
        // [Authorize]
        //  public  ActionResult getCurrentUserImage()
        //  {
        //     try
        //     {
        //         string username = User.FindFirstValue(ClaimTypes.Name);
        //         var returndata = Context.Korisnici
        //             .Where(k => k.Username == username)
        //             .Select(c => new ReturnData()
        //             {
        //                 Name = c.Username,
        //                 ImageBase64 = String.Format("data:image/png;base64,{0}", Convert.ToBase64String(c.Slika))
        //             }).FirstOrDefault();

        //         return Ok(returndata);
        //     }catch(Exception e)
        //     {
        //         return BadRequest(e.Message);
        //     }
        //  }

        // [Route("getUserImage/{userID}")]
        // [HttpGet]
        // [Authorize]
        //  public  ActionResult getUserImage([FromRoute] int userID)
        //  {
        //     try
        //     {
        //         var returndata = Context.Korisnici
        //             .Where(k => k.ID == userID)
        //             .Select(c => new ReturnData()
        //             {
        //                 Name = c.Username,
        //                 ImageBase64 = String.Format("data:image/png;base64,{0}", Convert.ToBase64String(c.Slika))
        //             }).FirstOrDefault();

        //         return Ok(returndata);
        //     }catch(Exception e)
        //     {
        //         return BadRequest(e.Message);
        //     }
        //  }

        //  [Route("getTeamPictures/{teamID}")]
        //  [HttpGet]
        //  [Authorize]
        // public  ActionResult getTeamPictures([FromRoute] int teamID)
        // {
        //     var tim = Context.Timovi.Include(t => t.Korisnici)
        //                                 .Where(t => t.ID == teamID)
        //                                 .FirstOrDefault();

        //     List<ReturnData> returnList = new List<ReturnData>();

        //     foreach( var korisnik in tim.Korisnici)
        //     {
        //         if(korisnik.Slika != null)
        //         returnList.Add(new ReturnData()
        //                     {
        //                         Id = korisnik.ID,
        //                         Name = korisnik.Username,
        //                         ImageBase64 = String.Format("data:image/png;base64,{0}", Convert.ToBase64String(korisnik.Slika))
        //                     });
        //         else
        //         returnList.Add(new ReturnData()
        //         {   
        //             Id = korisnik.ID,
        //             Name = korisnik.Username,
        //             ImageBase64 = null
        //         });
        //     }

        //     return Ok(returnList);
        // }


        public class FileViewModel
        {
            public string Name { get; set; }
            public IFormFile File { get; set; }
            public List<IFormFile> Files { get; set; }
        }

        public class ReturnData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string ImageBase64 { get; set; }
        }

    }

}