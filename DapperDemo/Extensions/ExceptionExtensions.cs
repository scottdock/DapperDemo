using System;
using System.Collections.Generic;
using System.Text;

namespace DapperDemo.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetFullMessage(this Exception ex)
        {
            var msg = (ex.InnerException != null)
                ? $"{ex.InnerException.Message} {ex.Message} {ex.StackTrace}"
                : $"{ex.Message} {ex.StackTrace}";

            return msg;
        }
    }
}
