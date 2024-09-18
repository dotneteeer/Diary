using Serilog;

namespace Diary.Domain.Helpers;

public class HangfireLogHelper
{
    private readonly ILogger _logger;

    public HangfireLogHelper(ILogger logger)
    {
        _logger = logger;
    }

    public void LogLoggedUser(string userName) => _logger.Information("{user} has just logged in", userName);

    public void LogClosingSession(string userName) =>
        _logger.Information("Session of {user} will be closed in 15min", userName);

    public void LogClosedSession(string userName) => _logger.Information("Session of {user} has been closed", userName);

    public void LogReminder(string userName, int rolesCount) =>
        _logger.Information("{user} has {roles} roles", userName, rolesCount);
}