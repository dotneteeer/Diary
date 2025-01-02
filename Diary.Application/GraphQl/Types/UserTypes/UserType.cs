using Diary.Domain.Entity;
using HotChocolate.Types;

namespace Diary.Application.GraphQl.Types.UserTypes;

public class UserType : ObjectType<User>
{
    protected override void Configure(IObjectTypeDescriptor<User> descriptor)
    {
        descriptor.Description("A user");

        descriptor.Field(x => x.Id).Description("The Id of a user");
        descriptor.Field(x => x.Login).Description("The login of a user");
        descriptor.Field(x => x.Password).Ignore();
        descriptor.Field(x => x.Reports).Description("The reports of a user");
        descriptor.Field(x => x.Roles).Ignore();
        descriptor.Field(x => x.UserToken).Ignore();

        descriptor.Field(x => x.CreatedAt).Description("Date of the report creation");
        descriptor.Field(x => x.CreatedBy).Description("The owner of the report creation");
        descriptor.Field(x => x.UpdatedAt).Description("Date of the report update");
        descriptor.Field(x => x.UpdatedBy).Description("The owner of the report update");
        descriptor.Field(x => x.LastEditedAt).Description("Date of last report edit");
        descriptor.Field(x => x.LastEditedBy).Description("The owner of last report edit");
    }
}