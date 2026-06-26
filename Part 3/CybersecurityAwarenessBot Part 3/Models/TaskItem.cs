using System;

namespace CybersecurityAwarenessBotPart3.Models
{
    public class TaskItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}