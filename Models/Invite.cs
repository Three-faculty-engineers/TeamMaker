using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Models
{

    public class Invite
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }
        
        [BsonIgnore]
        public Team Team { get; set; }

        [BsonElement("korisnikRef")]
        public ObjectId KorisnikRef { get; set; }

        [BsonIgnore]
        public Korisnik Korisnik { get; set; }
        
        [BsonElement("verzija")]
        public int Verzija{get; set;}

        [BsonElement("poruka")]
        public string Poruka {get; set;}
    }


}