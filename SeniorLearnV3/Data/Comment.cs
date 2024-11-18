using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace SeniorLearnV3.Data
{
    public class Comment
    {
        [Required]
        [Range(0, 10000)]
        [BsonElement("content")]
        public string? Content { get; set; }

        [BsonElement("author")]
        public string? Author { get; set; } // user name who added the comment

        [BsonElement("createdDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    }
}
