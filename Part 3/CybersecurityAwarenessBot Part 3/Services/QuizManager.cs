using System.Collections.Generic;
using CybersecurityAwarenessBotPart3.Models;

namespace CybersecurityAwarenessBotPart3.Services
{
    public class QuizManager
    {
        public List<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
        public int Score { get; set; }
    }
}