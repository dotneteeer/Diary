namespace Diary.Domain.Enum;

public enum ErrorCodes
{
    //Report:0-10
    //User:10-20
    InternalServerError=10,
    
    ReportsNotFound=0,
    ReportNotFound=1,
    ReportAlreadyExists=2,
    
    UserNotFound=11
    
    
}