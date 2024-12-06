using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeniorLearnV3.Data;
using System;
using static SeniorLearnV3.Areas.Member.Models.DeliveryPattern.Create;

//TODO: implement CRUD operations using MVC with appropriate HTTP METHODS (done for Topics, first cut with scaffolding)
namespace SeniorLearnV3.Areas.Administration.Controllers
{
    public class TopicController : AdministrationAreaController
    {

        public TopicController(ApplicationDbContext context) :base (context)
        {         
        }

        // GET: Administration/Topics
        // This method retrieves all topics from the database and returns them to the view for display.

        public async Task<IActionResult> Index()
        {
            return View(await _context.Topics.ToListAsync());
        }

        // GET: Administration/Topics/Details/5
        // This method retrieves the details of a specific topic by its ID and returns it to the view.
        // If the topic is not found, it returns a "Not Found" result.

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topic = await _context.Topics
                .FirstOrDefaultAsync(m => m.Id == id);
            if (topic == null)
            {
                return NotFound();
            }

            return View(topic);
        }

        // GET: Administration/Topics/Create
        // This method returns the view for creating a new topic.

        public IActionResult Create()
        {
            return View();
        }

        // POST: Administration/Topics/Create
        // This method handles the creation of a new topic. It adds the topic to the database if the model is valid.
 
        [HttpPost]
        [ValidateAntiForgeryToken]   
        public async Task<IActionResult> Create(Models.Topic.Create model) //Me
        {           
            if (ModelState.IsValid)
            {
                var topic = new Topic { Name = model.Name, Description = model.Description, OrganisationId = 1 };
                _context.Topics.Add(topic);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            return View(model);
        }

        // GET: Administration/Topics/Edit/1
        // This method retrieves the topic details by ID and prepares the data for editing.
        // If the topic is not found, it returns a "Not Found" result.

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topic = await _context.Topics.FindAsync(id);

            if (topic == null)
            {
                return NotFound();
            }
            var m = new Models.Topic.Edit
            {
                Id = topic.Id,
                Name = topic.Name,
                Description = topic.Description,
            };
            return View(m);
          
        }


        // POST: Administration/Topics/Edit/5
        // This method updates the topic details in the database, based on the provided model,
        // and redirects to the topic list if the update is successful.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Models.Topic.Edit m)
        {
            var topic = await _context.Topics.FirstOrDefaultAsync(t => t.Id == m.Id);

            if (topic == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
               
                    topic.Name = m.Name!;
                    topic.Description = m.Description!;
                   
                    _context.Update(topic);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

            }
               
            
            return View(topic);
        }


        // GET: Administration/Topics/Delete/1
        // This method retrieves and displays the topic to be deleted,
        // based on the provided ID. If the topic is not found, it returns a "Not Found" response.

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topic = await _context.Topics
                .FirstOrDefaultAsync(m => m.Id == id);
            if (topic == null)
            {
                return NotFound();
            }

            return View(topic);
        }

        // POST: Administration/Topics/Delete/5
        // This method handles the deletion of a topic.
        // It removes the topic from the database and saves the changes. Afterward, it redirects to the index page.

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic != null)
            {
                _context.Topics.Remove(topic);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // This method checks if a topic exists in the database based on its ID.
        private bool TopicExists(int id)
        {
            return _context.Topics.Any(e => e.Id == id);
        }
    }
}
