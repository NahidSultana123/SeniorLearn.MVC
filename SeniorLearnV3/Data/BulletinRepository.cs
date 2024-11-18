using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SeniorLearnV3.Data
{
   /* [Route("api/[controller]")]
    [ApiController]
    public class BulletinRepository : ControllerBase
    {

        private static readonly List<Bulletin> bulletins = new List<Bulletin>()
        {
              new Bulletin
            {
                Id = 1,
                Title = "Upcoming Gardening Workshop",
                Content = "Join our weekend workshop on advanced gardening techniques! Learn tips and tricks from professionals.",
                CreatedDate = DateTime.Now,
                CreatedById = "b6335bfb-391c-4d50-adf5-2a859f5f5ea3",
                IsActive = true
            },

            new Bulletin
            {
                Id = 2,
                Title = "System Maintenance Notice",
                Content = "Please note that the SeniorLearn platform will undergo maintenance on Saturday at 8 PM.",
                CreatedDate = DateTime.Now.AddDays(-2),
                CreatedById = "173ef34b-19c4-48e8-aada-4c3d17bfe57f",
                IsActive = true
            }

        };

        // Method that returns list of all bulletins
        public IEnumerable<Bulletin> GetAll()
        {
            return bulletins;
        }

        // method that returns a single bulletin that matches the bulletin id
        public Bulletin? GetById(int id)
        {
            var bulletin = bulletins
                .Where(b => b.Id == id)
                .FirstOrDefault();

            return bulletin;
        }
    }*/
}
