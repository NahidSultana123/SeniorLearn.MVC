using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace SeniorLearnV3.Data
{
    public class Bulletin
    {
        [BsonId]
        public ObjectId Id { get; set; } //Unique identifier

        [BsonElement("title")]
        [Required(ErrorMessage = "Title is required")]
        public string? Title { get; set; } // Summarize the key point of the bulletin message

        [BsonElement("content")]
        [Required(ErrorMessage = "Content is required")]
        public string? Content { get; set; } //The body where the full messages is written.

        [BsonElement("createdByUserId")]
        public string? CreatedByUserId { get; set; } //The User ID of the creator

        [BsonElement("createdBy")]
        public string? CreatedBy { get; set; }  //The creator’s name

        [BsonElement("createdDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; //The date the bulletin was created.

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;  //A Boolean indicating if the bulletin is active. Default: true

        [BsonElement("recentComments")]
        public List<Comment> RecentComments { get; set; } = new List<Comment>(); //A list of comments associated with the bulletin.

    }
}
