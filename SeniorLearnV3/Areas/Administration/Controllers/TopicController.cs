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
        public async Task<IActionResult> Index()
        {
            return View(await _context.Topics.ToListAsync());
        }

        // GET: Administration/Topics/Details/5
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Administration/Topics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Name,Description")] Topic topic) //Earlier
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        private bool TopicExists(int id)
        {
            return _context.Topics.Any(e => e.Id == id);
        }
    }
}
