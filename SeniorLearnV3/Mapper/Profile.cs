namespace SeniorLearnV3.Mapper
{
    public class Profile : AutoMapper.Profile
    {
        public Profile()
        {
            CreateMap<Data.Member, Areas.Administration.Models.Member.Manage>().ReverseMap();
        }
       
    }
}
