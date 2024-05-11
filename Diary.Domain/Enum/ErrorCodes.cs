namespace Diary.Domain.Enum;

public enum ErrorCodes
{
    //Report:0-9
    //User:11-20
    //Authorization:21-30
    InternalServerError=10,
    
    ReportsNotFound=0,
    ReportNotFound=1,
    ReportAlreadyExists=2,
    
    UserNotFound=11,
    UserAlreadyExists=12,
    
    PasswordMismatch=21,
    PasswordIsWrong=22
    
    
}