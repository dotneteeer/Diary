using Diary.Application.GraphQl.Subscriptions.ReportSubscriptions;
using Diary.Domain.Entity;
using Diary.Domain.Result;
using HotChocolate.Subscriptions;
using HotChocolate.Types;

namespace Diary.Application.GraphQl.Types.SubscriptionsTypes;

public class ReportSubscriptionType : ObjectType<ReportSubscription>
{
    protected override void Configure(IObjectTypeDescriptor<ReportSubscription> descriptor)
    {
        descriptor.Description("Subscription for report");

        descriptor.Field(x => x.OnReportAdded(default!))
            .Description("Subscription for adding a report")
            .Subscribe(async context =>
            {
                var receiver = context.Service<ITopicEventReceiver>();

                var sourceStream =
                    await receiver.SubscribeAsync<BaseResult<Report>>(nameof(ReportSubscription.OnReportAdded));

                return sourceStream;
            });
    }
}