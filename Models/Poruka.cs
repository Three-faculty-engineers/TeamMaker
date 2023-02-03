using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Models
{

    public class Poruka
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public int ID { get; set; }

        [BsonElement("korisnikSnd")]
        public MongoDBRef KorisnikSndRef { get; set; }

        [BsonIgnore]
        public Korisnik KorisnikSnd { get; set; }

        [BsonElement("korisnikRcv")]
        public MongoDBRef KorisnikRcvRef { get; set; }

        [BsonElement("vreme")]
        public DateTime Vreme {get; set;}

        [BsonElement("tekst")]
        public string Tekst {get; set;}


    }
}