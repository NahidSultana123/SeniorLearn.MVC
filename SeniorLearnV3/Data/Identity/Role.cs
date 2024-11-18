using Microsoft.AspNetCore.Identity;

namespace SeniorLearnV3.Data.Identity
{
    public class Role : IdentityRole
    {

        public Role() { }
        public Role(string name)
            : base(name)
        {

        }
    }
}
