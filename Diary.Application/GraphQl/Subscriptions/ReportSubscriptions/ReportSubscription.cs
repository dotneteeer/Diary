using Diary.Domain.Entity;
using Diary.Domain.Result;
using HotChocolate;
using HotChocolate.Types;

namespace Diary.Application.GraphQl.Subscriptions.ReportSubscriptions;

public class ReportSubscription
{
    [Subscription]
    [Topic]
    public BaseResult<Report> OnReportAdded([EventMessage] BaseResult<Report> report) => report;
}