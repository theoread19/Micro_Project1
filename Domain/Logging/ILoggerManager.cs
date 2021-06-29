using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Logging
{
    public interface ILoggerManager
    {
        public void LogInfo(string message);
        public void LogWarn(string message);
        public void LogDebug(string message);
        public void LogError(string message);
    }
}
