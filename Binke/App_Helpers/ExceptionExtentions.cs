using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Doctyme.Model;

namespace Binke
{
    public static class ExceptionExtentions
    {
        public static ErrorLog ParseException(this Exception exception,string source)
        {
            ErrorLog log                = new ErrorLog { AppType = "SEARCH_CONT_LOG" };
            log.InnerExceptionMessage   = exception.InnerException?.Message;
            log.InnerStackTrace         = exception.InnerException?.StackTrace;
            log.LogDate                 = DateTime.UtcNow;
            log.Message                 = exception.Message;
            log.Source                  = exception.Source;
            log.Stack                   = exception.StackTrace;
            log.TargetSite              = exception.TargetSite.Name;
            log.Type                    = exception.GetType().Name;
            return log;
        }
    }
}
