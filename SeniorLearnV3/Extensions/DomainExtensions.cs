namespace SeniorLearnV3.Extensions
{
    public static class DomainExtensions
    {
        //TODO: Calculate Renew Dates (inital rough cut needs clarity + testing etc...)
        public static DateTime GetNextMonthlyRenewalDate(this DateTime d)
        {
            var nextRenewalDate = new DateTime(d.Year, d.Month, 1);
            //TODO: Test and determine behaviour if today is first day of the month?
            //If so do is the renewal set to the first day of the next month?
            //Current behaviour only moves forward to next month if current day is 2nd day of month or later
            if (d.Day > 1)
            {
                nextRenewalDate = nextRenewalDate.AddMonths(1);
            }
            return nextRenewalDate;
        }

        public static DateTime GetProfessionalTrialRenewalDate(this DateTime d)
        {
            var nextRenewalDate = new DateTime(d.Year, d.Month, 1);
            if (d.Day > 1)
            {
                nextRenewalDate = nextRenewalDate.AddMonths(1);
            }
            nextRenewalDate = nextRenewalDate.AddMonths(3);
            return nextRenewalDate;
        }

        public static DateTime GetNextAnnualRenewalDate(this DateTime d)
        {
            var nextRenewalDate = new DateTime(d.Year, d.Month, 1);
            if (d.Day > 1)
            {
                nextRenewalDate = nextRenewalDate.AddMonths(1);
            }
            nextRenewalDate = nextRenewalDate.AddMonths(12);
            return nextRenewalDate;
        }

        public static bool ToBool(this int i) => i > 0;


    }
}
