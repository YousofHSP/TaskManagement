using AutoMapper;

namespace DTO.CustomMapping;

public interface IHaveCustomMapping
{
    void CreateMappings(Profile profile);
    
}