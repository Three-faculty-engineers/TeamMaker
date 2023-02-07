using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Models
{

    public class Korisnik
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("sifra")]
        public string Sifra { get; set; }

        [BsonElement("salt")]
        public string Salt { get; set; }

        [BsonElement("indeks")]
        public int Indeks {get; set;}

        [BsonElement("fakultet")]
        public string Fakultet {get; set;}

        [BsonElement("ime")]
        public string Ime { get; set; }

        [BsonElement("tip")]
        public int Tip { get; set; } 

        [BsonElement("prezime")]
        public string Prezime { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }
        
        [BsonElement("opis")]
        public string Opis { get; set; }


        // public byte[] Slika { get; set; }

        [BsonElement("teams")]
        public List<MongoDBRef> TeamsRef { get; set; }

        [BsonIgnore]
        public List<Team> Teams{ get; set; }

        [BsonElement("teamLeader")]
        public List<MongoDBRef> TeamLeaderRef { get; set; }

        [BsonIgnore]
        public List<Team> TeamLeader { get; set; }

        [BsonElement("oceneSnd")]
        public List<Ocena> OceneSnd { get; set; }

        [BsonElement("oceneRcv")]
        public List<Ocena> OceneRcv { get; set; }

        [BsonElement("porukeSnd")]
        public List<Poruka> PorukeSnd { get; set; }

        [BsonElement("porukeRcv")]
        public List<Poruka> PorukeRcv { get; set; }

        [BsonIgnore]
        public List<Task> Taskovi { get; set; }

        [BsonIgnore]
        public List<Invite> InvitesRcv { get; set; }

        [BsonIgnore]
        public List<Objava> Objave  {get; set; }
        // [BsonElement("taskovi")]
        // public List<Task> Taskovi { get; set; }

        [BsonElement("invites")]
        public List<Invite> Invites { get; set; }

        public static implicit operator MongoDBRef(Korisnik v)
        {
            throw new NotImplementedException();
        }

        // [BsonElement("objave")]
        // public List<Objava> Objave  {get; set; }
    }
}