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

                return Ok(activeBulletins); // Return active bulletins with 200 status
            }
            catch (Exception ex)
            {
                // Log the error as needed
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        // GET /api/bulletins/{id}
        [HttpGet("{id}")]
        public IActionResult GetBulletin([FromRoute] string id)
        {
            var bulletin = _bulletinCollection.Find(b => b.Id == new ObjectId(id)).FirstOrDefault();

            if (bulletin == null)
            {
                return NotFound(); // If the bulletin is not found, return a 404 status
            }

            return Ok(bulletin); // Return the found bulletin with a 200 status
        }


        //POST api/bulletins/bulletin/create
        [HttpPost("bulletin/create")]

        public IActionResult CreateBulletin(Bulletin bulletin)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Inserting the bulletin into the database
                    _bulletinCollection.InsertOne(bulletin);
                    return CreatedAtAction(nameof(CreateBulletin), new { id = bulletin.Id }, bulletin);
                    // return 201 created status code   location header /api/bulletins/id ret details of bulletin obj
                }
                catch (Exception ex)
                {
                    // Logging the error
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }

            return BadRequest(ModelState);
        }

        /* private readonly BulletinRepository _repo;

         // Add constructor
         public BulletinController()
         {
                 _repo = new BulletinRepository();
         }


         [HttpGet]
         [Authorize]
         public IActionResult Get()
         {
             var result = _repo.GetAll();
             return Ok(result);
         }

         // GET /api/bulletins/2
         [HttpGet("{id}")]
         [Authorize]
         public IActionResult GetBulletin([FromRoute] int id)
         {
             var bulletin = _repo.GetById(id);

             if (bulletin == null)
             {
                 return NotFound();
             }
             else
             {
                 return Ok(bulletin);
             }
         }*/


    }
}
