using AutoMapper;

namespace Ordering.Application.Mappings;

public interface IMapFrom<T>
{
    void Mapping(Profile profile) => 
        profile.CreateMap(typeof(T), GetType());
}