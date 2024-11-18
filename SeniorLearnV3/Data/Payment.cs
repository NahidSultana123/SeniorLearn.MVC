namespace SeniorLearnV3.Data
{
    public class Payment
    {
        public int Id { get; set; }
        public int MemberId { get; set; } // Foreign key property
        public Member Member { get; set; } = default!; // Navigation property
        public enum PaymentMethods { Cash, EFT, CreditCard, Cheque }
        public PaymentMethods PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }

        public bool Approved { get; set; }
    }
}

// TODO: Calculate Outstanding fee later
//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;

//public class MemberService
//{
//    private readonly ApplicationDbContext _context;

//    public MemberService(ApplicationDbContext context)
//    {
//        _context = context;
//    }

//    public async Task<decimal> CalculateOutstandingFeesAsync(int memberId, decimal totalFee)
//    {
//        // Retrieve the member with their payments
//        var member = await _context.Members
//            .Include(m => m.Payments) // Include related payments
//            .SingleOrDefaultAsync(m => m.Id == memberId);

//        if (member == null)
//        {
//            throw new ArgumentException("Member not found.");
//        }

//        // Calculate the total amount of approved payments
//        decimal totalPayments = member.Payments
//            .Where(p => p.Approved) // Assuming you only want approved payments
//            .Sum(p => p.Amount);

//        // Calculate outstanding fees
//        decimal outstandingFees = totalFee - totalPayments;

//        return outstandingFees;
//    }
//}

//// Example usage:
//public async Task ExampleUsageAsync()
//{
//    var context = new ApplicationDbContext(); // Use dependency injection in real applications
//    var memberService = new MemberService(context);

//    decimal totalFee = 50m; // The total fee for the member
//    int memberId = 1; // The ID of the member

//    decimal outstandingFees = await memberService.CalculateOutstandingFeesAsync(memberId, totalFee);
//    Console.WriteLine($"Outstanding Fees: {outstandingFees}");
//}
