using Diary.Domain.Entity;
using Diary.Domain.Interfaces.Repositories;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace Diary.Application.GraphQl.Types.ReportTypes;

public class ReportType : ObjectType<Report>
{
    protected override void Configure(IObjectTypeDescriptor<Report> descriptor)
    {
        descriptor.Description("The report of a user");

        descriptor.Field(x => x.Id).Description("The ID of the report");
        descriptor.Field(x => x.Name).Description("The name of the report");
        descriptor.Field(x => x.Description).Description("The description of the report");
        descriptor.Field(x => x.User).Description("The owner of the report");
        descriptor.Field(x => x.UserId).Description("The id of report's owner");

        descriptor.Field(x => x.CreatedAt).Description("Date of the report creation");
        descriptor.Field(x => x.CreatedBy).Description("The owner of the report creation");
        descriptor.Field(x => x.UpdatedAt).Description("Date of the report update");
        descriptor.Field(x => x.UpdatedBy).Description("The owner of the report update");
        descriptor.Field(x => x.LastEditedAt).Description("Date of last report edit");
        descriptor.Field(x => x.LastEditedBy).Description("The owner of last report edit");

        descriptor.Field(x => x.User)
            .ResolveWith<Resolvers>(x => Resolvers.GetUser(default!, default!));
    }

    private sealed class Resolvers
    {
        private Resolvers()
        {
        }

        public static async Task<User> GetUser([Parent] Report report, [Service] IBaseRepository<User> userRepository)
        {
            var result = await userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == report.UserId);

            return result!;
        }
    }
}