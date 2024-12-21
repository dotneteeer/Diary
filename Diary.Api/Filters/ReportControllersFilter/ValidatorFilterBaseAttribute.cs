namespace Diary.Api.Filters.ReportControllersFilter;

/// <summary>
/// Base class for validators in report controller
/// </summary>
public abstract class ValidatorFilterBaseAttribute : Attribute
{
    /// <summary>
    /// Default authentication scheme
    /// </summary>
    protected const string AuthenticationScheme = "Bearer";

    /// <summary>
    /// Name of identifier to get from route
    /// </summary>
    protected string IdentifierName { get; init; }
}