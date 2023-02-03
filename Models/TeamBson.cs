using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Models
{
    public class TeamBson: Team
    {
        // [BsonId]
        // [BsonRepresentation(BsonType.ObjectId)]
        // public string ID { get; set; }
        
        // [BsonElement("ime")]
        // public string Ime { get; set; }

        // [BsonElement("leader")]
        // public MongoDBRef LeaderRef {get; set;}

        // [BsonElement("leaderRef")]
        // public ObjectId LeaderRef {get; set;}

        [BsonElement("leader")]
        public List<Korisnik> Leader{get; set;}

        // [BsonElement("korisnici")]
        // public List<MongoDBRef> KorisniciRef { get; set; }

        // [BsonElement("korisniciRef")]
        // public List<ObjectId> KorisniciRef { get; set; }

        [BsonElement("korisnici")]
        public List<Korisnik> Korisnici { get; set; }

        // [BsonElement("needsMembers")]
        // public bool NeedsMembers {get; set;}

        // [BsonElement("opis")]
        // public string Opis { get; set; }

        // [BsonIgnore]
        // public List<Task> Tasks { get; set; }

        // [BsonElement("objave")]
        // public List<Objava> Objave { get; set; }

        // [BsonElement("sprints")]
        // public List<Sprint> Sprints { get; set; }

        // [BsonElement("invites")]
        // public List<Invite> Invites{get; set;}
    }


}