using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using SeniorLearnV3.Data;

namespace SeniorLearnV3.Controllers.Api
{
    [Route("api/bulletins")]
    [ApiController]
    //[Authorize]
    public class BulletinController : ControllerBase
    {


        private readonly IMongoCollection<Bulletin> _bulletinCollection;

        public BulletinController(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("BulletinDb"));
            var database = client.GetDatabase("bulletindb");
            _bulletinCollection = database.GetCollection<Bulletin>("bulletins");
            
        }

        // create SeedBulletins end point
        //method to insert the bulletins into the database and returns a success message.

        [HttpPost("seed")]
        public IActionResult SeedBulletins()
        {
            var bulletins = new List<Bulletin>
        
            {
                new Bulletin
                {
                    Title = "Welcome to SeniorLearn!",
                    Content = "Thanks for joining in. Learn, Grow and connect to our community",
                    CreatedBy = "admin1@seniorlearn.org.au",
                    CreatedByUserId = "173ef34b-19c4-48e8-aada-4c3d17bfe57f", // accurate id for the user name from sql db
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true,
                    RecentComments = new List<Comment>
                    {
                        new Comment { Author = "senior-10@citizen.com", Content = "Thank you! Looking forward to it.", CreatedDate = DateTime.UtcNow },
                        new Comment { Author = "senior@citizen.email", Content = "This is amazing!", CreatedDate = DateTime.UtcNow }
                    }
                },
                new Bulletin
                {
                    Title = "Upcoming Workshop on Gardening",
                    Content = "Join us for a hands-on gardening workshop this Friday.",
                    CreatedBy = "professional@citizen.email",
                    CreatedByUserId = "08f146c7-2e53-4bc5-bf22-c16efcfbf6e5", // accurate id for professional@citizen.email
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true,
                    RecentComments = new List<Comment>
                    {
                        new Comment { Author = "senior-60@citizen.com", Content = "I’m excited! How do I join?", CreatedDate = DateTime.UtcNow },
                        new Comment { Author = "senior-10@citizen.com", Content = "Sounds interesting!", CreatedDate = DateTime.UtcNow }
                    }
                }
             };

            _bulletinCollection.InsertMany(bulletins);

            return Ok("Bulletins data seeded successfully!");
        }

        // Retrieves and returns all bulletins from the collection.
        [HttpGet]
        public IActionResult GetAllBulletins()
        {
            var bulletins = _bulletinCollection.Find(FilterDefinition<Bulletin>.Empty).ToList();

            if (bulletins == null || bulletins.Count == 0)
            {
                return NotFound(); // If no bulletins are found, return a 404 status
            }

            return Ok(bulletins); // Return the list of bulletins with a 200 status
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetAllActiveBulletins()
        {
            try
            {
                // Filter to fetch only active bulletins
                var filter = Builders<Bulletin>.Filter.Eq(b => b.IsActive, true);

                // Fetch active bulletins
                var activeBulletins = await _bulletinCollection.Find(filter).ToListAsync();

                if (activeBulletins == null || activeBulletins.Count == 0)
                {
                    return NotFound("No active bulletins found."); // Return 404 if no active bulletins
                }

                // Convert the Id field to string for each bulletin
                var bulletinsWithStringId = activeBulletins.Select(b => new
                {
                    Id = b.Id.ToString(),        // Convert ObjectId to string
                    Title = b.Title,
                    Content = b.Content,
                    CreatedByUserId = b.CreatedByUserId,
                    CreatedBy = b.CreatedBy,
                    CreatedDate = b.CreatedDate,
                    IsActive = b.IsActive,
                    RecentComments = b.RecentComments
                });

                return Ok(bulletinsWithStringId); // returning all active bulletins with 200 status


                //return Ok(activeBulletins); // Return active bulletins with 200 status
            }
            catch (Exception ex)
            {
                // Log the error as needed
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //drafts created by a user
        [HttpGet("drafts/{userId}")]
        public async Task<IActionResult> GetMyDrafts([FromRoute] string userId)
        {
             try
             {
                // Filter to fetch only draft bulletins for a specific user
                var filter = Builders<Bulletin>.Filter.And(
                    Builders<Bulletin>.Filter.Eq(b => b.IsActive, false), // Filter by draft status
                    Builders<Bulletin>.Filter.Eq(b => b.CreatedByUserId, userId) // Filter by user ID
                );

                var draftBulletins = await _bulletinCollection.Find(filter).ToListAsync();

                if (draftBulletins == null || draftBulletins.Count == 0)
                {
                    return NotFound("No draft bulletins found.");
                }

                return Ok(draftBulletins); // Return the list of draft bulletins
             }

            catch (Exception ex)
             {
                return StatusCode(500, $"Internal server error: {ex.Message}");
             }
        }

        // Retrieves a specific bulletin by its ID.

        // GET /api/bulletins/{bulletinId}

        //[HttpGet("{bulletinId}")]
        [HttpGet("bulletin/{bulletinId}")]
        public async Task<IActionResult> GetBulletinById(string bulletinId)
        {
            // Validate and convert the bulletinId
            if (!ObjectId.TryParse(bulletinId, out ObjectId objectId))
            {
                return BadRequest(new { Message = "Invalid Bulletin ID format." });
            }
            //var objectId = new ObjectId(bulletinId);

            var bulletin = await _bulletinCollection.Find(b => b.Id == objectId).FirstOrDefaultAsync();

            if (bulletin == null)
            {
                return NotFound(); // If the bulletin is not found, return a 404 status
            }

            return Ok(bulletin); // Return the found bulletin with a 200 status
        }




        // To create a new bulletin
        // Creates a new bulletin and inserts it into the database.
        // Returns a 201 status code with the created bulletin and its location,
        // or a 400 status code if the model is invalid, or a 500 status code in case of an error.

        //POST api/bulletins/bulletin/create
        [HttpPost]

        public IActionResult CreateBulletin([FromBody] Bulletin bulletin) // Ensures the bulletin is bound from the request body
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Inserting the bulletin into the database
                    bulletin.CreatedDate = DateTime.UtcNow;
                    _bulletinCollection.InsertOne(bulletin);
                    return CreatedAtAction(nameof(CreateBulletin), new { id = bulletin.Id }, bulletin);
                    // return 201 created status code  and returns details of bulletin object
                }
                catch (Exception ex)
                {
                    // Logging the error
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }

            return BadRequest(ModelState);
        }


    }
}
