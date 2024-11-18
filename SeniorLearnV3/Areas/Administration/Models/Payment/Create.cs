//TODO: Add on later stage
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using static SeniorLearnV3.Data.Payment;
using static SeniorLearnV3.Data.Payment.PaymentMethods;


namespace SeniorLearnV3.Areas.Administration.Models.Payment
{
    public class Create
    {
        public int MemberId { get; set; }
        public int PaymentMethodId { get; set; } // To match enum PaymentMethods

        public SelectList PaymentMethods => new SelectList(Enum.GetValues<PaymentMethods>().Cast<PaymentMethods>().Select(p => new { Value = (int)p, Text = p.ToString() }), "Value", "Text");

        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public bool Approved { get; set; }
    }
}
