using AutoMapper;
using Diary.Domain.Dto.Report;
using Diary.Domain.Entity;
using Diary.Domain.Extensions;

namespace Diary.Application.Mapping;

public class ReportMapping : Profile
{
    public ReportMapping()
    {
        CreateMap<Report, ReportDto>()
            .ForCtorParam("Id", x => x.MapFrom(y => y.Id))
            .ForCtorParam("Name", x => x.MapFrom(y => y.Name))
            .ForCtorParam("Description", x => x.MapFrom(y => y.Description))
            .ForCtorParam("DateLastEdited",
                x => x.MapFrom(y => y.LastEditedAt.ToLongUtcString()))
            .ReverseMap();
    }
}