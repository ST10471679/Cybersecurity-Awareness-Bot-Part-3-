using System.Collections.Generic;
using CybersecurityAwarenessBotPart3.Models;

namespace CybersecurityAwarenessBotPart3.Services
{
    public class TaskManager
    {
        public List<TaskItem> Tasks { get; set; } = new List<TaskItem>();

        public void AddTask(TaskItem task)
        {
            Tasks.Add(task);
        }

        public void RemoveTask(TaskItem task)
        {
            Tasks.Remove(task);
        }
    }
}