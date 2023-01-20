using HtmlTags;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using SFA.DAS.Activities;
using SFA.DAS.Activities.Client;
using SFA.DAS.Activities.Extensions;

namespace SFA.DAS.EmployerAccounts.Web.Helpers;

public class ActivitiesHelper
{
    private readonly IActionContextAccessor _actionContextAccessor;

    public ActivitiesHelper(IActionContextAccessor actionContextAccessor)
    {
        _actionContextAccessor = actionContextAccessor;
    }

    public HtmlTag Activities(ActivitiesResult result)
    {
        if (!result.Activities.Any())
        {
            return new HtmlTag("p").Text("You have no recent activity");
        }

        var date = DateTime.MinValue;
        var now = DateTime.UtcNow.ToGmtStandardTime();
        var lastActivity = result.Activities.Last();
        var ol = new HtmlTag("ol").AddClass("timeline").AddClass("timeline--complete");
        var urlHelper = GetUrlHelper();
        var activitiesUrl = urlHelper.Activities(result.Total);

        foreach (var activity in result.Activities)
        {
            var at = activity.At.ToGmtStandardTime();
            var activityUrl = urlHelper.Activity(activity);
            var activityText = GetActivityText(activity);

            ol.Add("li", li => li.AddClasses(activity.At.Date != date ? "first" : "", activity == lastActivity ? "last" : "")
                .Append("h4", h4 => h4.Title(at.ToString("U")).Text(at.ToRelativeFormat(now)))
                .Append("p", p => p.AddClass("activity").Text(activityText))
                .Append("p", p =>
                {
                    p.AddClass("meta").AppendText("At").AppendHtml(" ")
                        .Append("time", time => time.Text(at.ToString("h:mm tt"))).AppendHtml(" ");

                    if (!string.IsNullOrEmpty(activityUrl))
                    {
                        p.Add("a", a => a.Attr("href", activityUrl).Text("See details"));
                    }
                })
            );

            date = activity.At.Date;
        }

        if (result.Activities.Count() != result.Total)
        {
            ol.Add("li", li => li.AddClass("all-activity")
                .Add("p", p => p.AddClass("activity"))
                .Add("a", a => a.Attr("href", activitiesUrl).Text("See all activity")));
        }

        return ol;

    }

    public HtmlTag LatestActivities(AggregatedActivitiesResult result)
    {
        if (!result.Aggregates.Any())
        {
            return new HtmlTag("p").Text("You have no recent activity");
        }
        
        var now = DateTime.UtcNow.ToGmtStandardTime();
        var ol = new HtmlTag("ol").Id("item-list");
        var urlHelper = GetUrlHelper();
        var activitiesUrl = urlHelper.Activities();

        foreach (var aggregate in result.Aggregates)
        {
            var at = aggregate.TopHit.At.ToGmtStandardTime();
            var activityText = GetActivityText(aggregate.TopHit, aggregate.Count);

            ol.Add("li", li => li.AddClass("item")
                .Append("div", div => div.AddClass("item-label").Text(at.ToRelativeFormat(now)))
                .Append("div", div => div.AddClass("item-description").Text(activityText))
            );
        }

        ol.Add("li", li => li.AddClass("item all-activity")
            .Append("div", div => div.AddClass("item-label")
                .Append("a", a => a.Attr("href", activitiesUrl).Text("See all activity"))
            )
        );

        return ol;

    }

    private static string GetActivityText(Activity activity, long count = 1)
    {
        var localizer = activity.Type.GetLocalizer();
        var text = count > 1 ? localizer.GetPluralText(activity, count) : localizer.GetSingularText(activity);

        return text;
    }

    private UrlHelper GetUrlHelper()
    {
        return new UrlHelper(_actionContextAccessor.ActionContext);
    }
}

public static class ActivitiesUrlHelperExtensions
{
    public static string Activities(this UrlHelper urlHelper, long? take = null)
    {
        var url = urlHelper.Action("Index", "Activities", new { take });

        return url;
    }

    public static string Activity(this UrlHelper urlHelper, Activity activity)
    {
        var action = activity.Type.GetAction();
        var url = action != null ? urlHelper.Action(action.Item1, action.Item2) : null;

        return url;
    }
}