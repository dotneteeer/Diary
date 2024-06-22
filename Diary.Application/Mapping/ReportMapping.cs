using System.Globalization;
using AutoMapper;
using Diary.Domain.Dto.Report;
using Diary.Domain.Entity;

namespace Diary.Application.Mapping;

public class ReportMapping : Profile
{
    public ReportMapping()
    {
        var enUsCulture = CultureInfo.CreateSpecificCulture("en-US");
        CreateMap<Report, ReportDto>()
            .ForCtorParam("Id", x=>x.MapFrom(y=>y.Id))
            .ForCtorParam("Name", x=>x.MapFrom(y=>y.Name))
            .ForCtorParam("Description", x=>x.MapFrom(y=>y.Description))
            .ForCtorParam("DateCreated", x=>x.MapFrom(y=>y.CreatedAt.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss",enUsCulture)))
            .ReverseMap();
    }
}