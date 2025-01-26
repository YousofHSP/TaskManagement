using AutoMapper;

namespace DTO.CustomMapping;

public class CustomMappingProfile: Profile
{
    public CustomMappingProfile(IEnumerable<IHaveCustomMapping> haveCustomMappings)
    {
        foreach(var item in haveCustomMappings)
            item.CreateMappings(this);
    }
}