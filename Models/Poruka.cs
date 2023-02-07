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
        public string ID { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string KorisnikSndRef { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string KorisnikRcvRef { get; set; }

        [BsonElement("korisniksnd")]
        public List<Korisnik> KorisnikSnd { get; set; }

        [BsonElement("korisnikrcv")]
        public List<Korisnik> KorisniRcv { get; set; }

        [BsonElement("vreme")]
        public DateTime Vreme {get; set;}

        [BsonElement("tekst")]
        public string Tekst {get; set;}


    }
}