using AutoMapper;
using Diary.Application.Mapping;

namespace Diary.Tests.UnitTests.Configurations;

public static class MapperConfiguration
{
    public static IMapper GetMapperConfiguration()
    {
        var mockMapper = new AutoMapper.MapperConfiguration(cfg => { cfg.AddProfile(new ReportMapping()); });
        return mockMapper.CreateMapper();
    }
}