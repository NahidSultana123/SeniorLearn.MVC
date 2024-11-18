using Microsoft.AspNetCore.Razor.TagHelpers;
using SeniorLearnV3.Data.Identity;

//TODO: Change icons later using https://icons.getbootstrap.com/

namespace SeniorLearnV3.TagHelpers
{
    [HtmlTargetElement("user-role-icon", Attributes = "role-type,role-active")]
    public class UserRoleIconTagHelper : TagHelper
    {
        [HtmlAttributeName("role-type")]
        public UserRole.RoleTypes RoleType { get; set; }
        [HtmlAttributeName("role-active")]
        public bool RoleActive { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "i";
            var iconClass = "user-role-icon d-inline-block justify-content-center bi";
            switch (RoleType)
            {
                case UserRole.RoleTypes.STANDARD:
                    iconClass += " bi-person-vcard-fill ";
                    break;
                case UserRole.RoleTypes.PROFESSIONAL:
                    iconClass += " bi-mortarboard-fill ";
                    break;
                case UserRole.RoleTypes.HONORARY:
                    iconClass += " bi-trophy-fill ";
                    break;
                default:
                    iconClass = "";
                    break;
            }
            string color = RoleActive ? "success" : "dark";

            iconClass += $" border border-5 border-{color} rounded p-1 px-2 h1 text-{color}";
            output.Attributes.SetAttribute("class", iconClass);
            output.Attributes.SetAttribute("title", $"ROLE: {RoleType} ACTIVE: {(RoleActive ? "YES" : "NO")}");
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}
