using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace SeniorLearnV3.Data
{
    public class Bulletin
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("title")]
        [Required(ErrorMessage = "Title is required")]
        public string? Title { get; set; }
   
        [BsonElement("content")]
        [Required(ErrorMessage = "Content is required")]
        public string? Content { get; set; }

        [BsonElement("createdByUserId")]
        public string? CreatedByUserId { get; set; }

        [BsonElement("createdBy")]
        public string? CreatedBy { get; set; }  //User name

        [BsonElement("createdDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;  // Default to active 

        [BsonElement("recentComments")]
        public List<Comment> RecentComments { get; set; } = new List<Comment>();

    
    }
}
