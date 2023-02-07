using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Models
{

    public class Task
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }

        [BsonElement("ime")]
        public string Ime { get; set; }

        [BsonElement("poeni")]
        public int Poeni { get; set; }

        [BsonElement("opis")]
        public string Opis { get; set; }

        [BsonElement("status")]
        public int Status {get; set;}

        [BsonElement("vreme")]
        public int Vreme {get;set;} 

        [BsonIgnore]
        public Team Team { get; set; }

        [BsonIgnore]
        public Sprint Sprint { get; set; }

        [BsonElement("korisnik")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string KorisnikRef { get; set; }
        
        [BsonIgnore]
        public Korisnik Korisnik { get; set; }
    }


}
