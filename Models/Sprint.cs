using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    public class Sprint
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; } 

        [BsonElement("startSprint")]
        public DateTime StartSprint { get; set; }

        [BsonElement("endSprint")]
        public DateTime EndSprint { get; set; }   

        [BsonElement("status")]      
        public int Status { get; set; }
    
        [BsonElement("opis")]
        public string Opis { get; set; }
        
        [BsonIgnore]
        public Team Team { get; set; }

        [BsonElement("taskovi")]
        public List<Task> Taskovi { get; set; }

    }
}