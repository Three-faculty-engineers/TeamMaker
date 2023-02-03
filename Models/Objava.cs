using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Models
{

    public class Objava
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public int ID { get; set; }
        
        [BsonIgnore]
        public Team Team { get; set; }

        [BsonElement("korisnik")]
        public MongoDBRef KorisnikRef { get; set; }

        [BsonIgnore]
        public Korisnik Korisnik { get; set; }

        [BsonElement("vreme")]
        public DateTime Vreme {get; set;}
        
        [BsonElement("poruka")]
        public string Poruka { get; set; }


    }


}