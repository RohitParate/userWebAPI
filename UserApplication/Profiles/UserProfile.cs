using AutoMapper;

namespace UserApplication.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile() { 
            // CreateMap<Source object, destination object>();
            CreateMap<Entities.User, Models.UserDto>();
            // it'll map properties of user entity with user dto object.
            //if property doesn't exist it'll be ignored

            CreateMap<Models.CreateUserDto, Entities.User>();

            CreateMap<Models.UpdateUserDto, Entities.User>();

            CreateMap<Entities.User, Models.UpdateUserDto>();
        }
    }
}
