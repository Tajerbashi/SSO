using AutoMapper.Internal.Mappers;

namespace SSO.SharedKernel.Utilities.Library.MapperProvider;

public interface IMapperAdapter
{
    TDestination Map<TSource, TDestination>(TSource source);

    List<TDestination> Map<TSource, TDestination>(List<TSource> source);
    IEnumerable<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> source);
}
