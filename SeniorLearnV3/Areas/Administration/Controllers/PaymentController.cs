using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SeniorLearnV3.Data;
using SeniorLearnV3.Data.Identity;


namespace SeniorLearnV3.Areas.Administration.Controllers
{
    public class PaymentController : AdministrationAreaController
    {

        public PaymentController(ApplicationDbContext context) : base(context) { }

        // GET: Administration/Payment
        // This method retrieves and displays the payments associated with a specific member, identified by their id.
        public async Task<IActionResult> Index(int id) // TODO: check sync with Payment- done
        {
            ViewBag.MemberId = id;
            return View(await _context.Payments.Include(p => p.Member).Where(p => p.MemberId == id).ToListAsync());
        }


        // GET: Administration/Payment/Create
        // This method initializes and returns a view to create a payment for a specific member, identified by their id.
        public IActionResult Create(int id)  
            => View(new Models.Payment.Create { MemberId = id });

        // This method handles the creation of a payment for a specific member,
        // updates their outstanding fees based on their role, and saves the changes to the database.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.Payment.Create m)
        {

            var member = await _context.Members.Include(mem => mem.Payments).FirstAsync(mem => mem.Id == m.MemberId);

            var organisation = await _context.Organisations.FirstOrDefaultAsync(o => o.Id == member.OrganisationId);


            if (ModelState.IsValid)
            {
                var payment = new Payment
                {
                    MemberId = m.MemberId,
                    PaymentMethod = (Payment.PaymentMethods)m.PaymentMethodId,
                    Amount = m.Amount,
                    PaymentDate = m.PaymentDate,
                    Approved = m.Approved
                };
               

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                // retrieve all UserRoles for this user/member
                var userRoles = await _context.UserRoles
                    .Where(ur => ur.User.Id == member.UserId) // Assuming the UserId is a string (as in IdentityUser)
                    .ToListAsync();

                //find out highest role that is active
                var highestActiveRole = userRoles
                    .Where(ur => ur.Active)                  // Filter only active roles
                    .OrderByDescending(ur => ur.Order)       // Order by Order in descending (highest first)
                    .FirstOrDefault();

                // Recalculate the outstanding fees after the payment
          
                if (highestActiveRole != null)
                {
                    var updatedOutstandingFees = member.CalculateOutstandingFees(highestActiveRole.RoleId, organisation);
                    member.OutstandingFees = updatedOutstandingFees;
                }
              
                _context.Members.Update(member);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index), new { id = m.MemberId });
            }
            return View(m);
        }


        // This method retrieves a payment by its ID for deletion,
        // ensuring the payment exists before displaying the delete confirmation view.

        /* public async Task<IActionResult> Delete(int? id)
         {
             if (id == null)
             {
                 return NotFound();
             }

             var payment = await _context.Payments
                 .Include(p => p.Member)
                 .FirstOrDefaultAsync(m => m.Id == id);
             if (payment == null)
             {
                 return NotFound();
             }

             return View(payment);
         }*/

        // POST: Administration/Payment/Delete/5
        /* [HttpPost, ActionName("Delete")]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> DeleteConfirmed(int id)
         {
             var payment = await _context.Payments.FindAsync(id);
             if (payment != null)
             {
                 _context.Payments.Remove(payment);
             }
             await _context.SaveChangesAsync();
             return RedirectToAction(nameof(Index));
         }*/
    }
}
