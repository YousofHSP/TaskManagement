using AutoMapper;
using DTO.CustomMapping;
using Entity.Common;
using System.ComponentModel.DataAnnotations;

namespace DTO;

public interface IBaseDto
{
    
}
public abstract class BaseDto<TDto, TEntity, TKey>  : IBaseDto,IHaveCustomMapping
    where TDto : class
    where TEntity :class, IEntity<TKey>
{
    public TKey? Id { get; init; }
    
    public TEntity ToEntity(IMapper mapper)
    {
        return mapper.Map<TEntity>(CastToDerivedClass(this, mapper));
    }
    public TEntity ToEntity(TEntity entity, IMapper mapper)
    {
        return mapper.Map(CastToDerivedClass(this, mapper), entity);
    }

    public static TDto FromEntity(TEntity model, IMapper mapper)
    {
        return mapper.Map<TDto>(model);
    }

    private static TDto CastToDerivedClass(BaseDto<TDto, TEntity, TKey> baseInstance, IMapperBase mapper)
    {
        return mapper.Map<TDto>(baseInstance);
    }
    public void CreateMappings(Profile profile)
    {
        var mappingExpression = profile.CreateMap<TDto, TEntity>();

        var dtoType = typeof(TDto);
        var entityType = typeof(TEntity);
        //Ignore any property of source (like Post.Author) that dose not contains in destination 
        foreach (var property in entityType.GetProperties())
        {
            if (dtoType.GetProperty(property.Name) == null)
                mappingExpression.ForMember(property.Name, opt => opt.Ignore());
        }

        CustomMappings(mappingExpression.ReverseMap());
    }

    protected virtual void CustomMappings(IMappingExpression<TEntity, TDto> mapping)
    {
    }

}
public abstract class BaseDto<TDto, TEntity> : BaseDto<TDto, TEntity, int>
    where TDto : class
    where TEntity :class, IEntity<int>
{
}