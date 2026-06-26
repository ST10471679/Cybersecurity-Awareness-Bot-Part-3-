using System;
using System.Collections.Generic;
using CybersecurityAwarenessBotPart3.Models;

namespace CybersecurityAwarenessBotPart3.Services
{
    public class ActivityLogger
    {
        public List<ActivityLog> Logs { get; set; } = new List<ActivityLog>();

        public void Log(string action)
        {
            Logs.Add(new ActivityLog
            {
                Timestamp = DateTime.Now,
                Action = action
            });
        }
    }
}