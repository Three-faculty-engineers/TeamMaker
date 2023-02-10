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
        public string ID { get; set; }
        
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string korisnikRef { get; set; }

        [BsonElement("vreme")]
        public DateTime vreme {get; set;}
        
        [BsonElement("poruka")]
        public string poruka { get; set; }


    }


}