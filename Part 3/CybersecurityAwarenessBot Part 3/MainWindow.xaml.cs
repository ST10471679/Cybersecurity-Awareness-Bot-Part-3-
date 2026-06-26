using System.Windows;
using System.Linq;
using CybersecurityAwarenessBotPart3.Services;

namespace CybersecurityAwarenessBotPart3
{
    public partial class MainWindow : Window
    {
        private ChatEngine chatEngine = new ChatEngine();
        private TaskManager taskManager = new TaskManager();
        private ActivityLogger activityLogger = new ActivityLogger();
        private QuizManager quizManager = new QuizManager();

        private string logFile = "logs.json";
        private string taskFile = "tasks.json";

        public MainWindow()
        {
            InitializeComponent();
            LoadQuiz();
            LoadTasks();
            LoadLogs();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string input = UserInput.Text;

            if (string.IsNullOrWhiteSpace(input))
                return;

            string response = chatEngine.ProcessInput(input);

            ChatOutput.AppendText("You: " + input + "\n");
            ChatOutput.AppendText("Bot: " + response + "\n\n");

            activityLogger.Log("User asked chatbot: " + input);
            JsonStorage.Save(logFile, activityLogger.Logs);
            RefreshLogs();

            UserInput.Clear();
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            string task = TaskTitleBox.Text;

            if (string.IsNullOrWhiteSpace(task))
                return;

            TaskListBox.Items.Add(task);

            taskManager.Tasks.Add(new Models.TaskItem
            {
                Title = task
            });

            JsonStorage.Save(taskFile, taskManager.Tasks);

            activityLogger.Log("Task added: " + task);
            JsonStorage.Save(logFile, activityLogger.Logs);
            RefreshLogs();

            TaskTitleBox.Clear();
            TaskDescriptionBox.Clear();
        }

        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListBox.SelectedItem != null)
            {
                string selectedTask = TaskListBox.SelectedItem.ToString();

                activityLogger.Log("Task deleted: " + selectedTask);

                TaskListBox.Items.Remove(TaskListBox.SelectedItem);

                var taskToRemove = taskManager.Tasks.FirstOrDefault(t => t.Title == selectedTask);

                if (taskToRemove != null)
                {
                    taskManager.Tasks.Remove(taskToRemove);
                }

                JsonStorage.Save(taskFile, taskManager.Tasks);
                JsonStorage.Save(logFile, activityLogger.Logs);
                RefreshLogs();
            }
        }

        private void RefreshLogs()
        {
            ActivityLogListBox.Items.Clear();

            foreach (var log in activityLogger.Logs)
            {
                ActivityLogListBox.Items.Add($"{log.Timestamp}: {log.Action}");
            }
        }

        private void LoadQuiz()
        {
            quizManager.Questions.Add(new Models.QuizQuestion
            {
                Question = "What is phishing?",
                Options = new string[] { "Firewall", "Scam emails", "Antivirus", "Encryption" },
                CorrectAnswerIndex = 1
            });

            quizManager.Questions.Add(new Models.QuizQuestion
            {
                Question = "What makes a password strong?",
                Options = new string[] { "123456", "Your name", "Symbols + numbers + uppercase", "Birthdate" },
                CorrectAnswerIndex = 2
            });

            quizManager.Questions.Add(new Models.QuizQuestion
            {
                Question = "What does malware do?",
                Options = new string[] { "Protects files", "Speeds internet", "Damages systems", "Creates backups" },
                CorrectAnswerIndex = 2
            });

            quizManager.Questions.Add(new Models.QuizQuestion
            {
                Question = "What should you do before clicking a suspicious link?",
                Options = new string[] { "Click immediately", "Verify source", "Share with friends", "Ignore warnings" },
                CorrectAnswerIndex = 1
            });

            quizManager.Questions.Add(new Models.QuizQuestion
            {
                Question = "What does 2FA stand for?",
                Options = new string[] { "Two-factor authentication", "Two firewall access", "Two file approval", "Double antivirus" },
                CorrectAnswerIndex = 0
            });

            quizManager.Questions.Add(new Models.QuizQuestion
            {
                Question = "Which one is a safe browsing habit?",
                Options = new string[] { "Ignore updates", "Use secure websites", "Share passwords", "Open all attachments" },
                CorrectAnswerIndex = 1
            });

            quizManager.Questions.Add(new Models.QuizQuestion
            {
                Question = "What does HTTPS mean?",
                Options = new string[] { "Secure website connection", "Fast internet", "Cloud storage", "Email security" },
                CorrectAnswerIndex = 0
            });

            quizManager.Questions.Add(new Models.QuizQuestion
            {
                Question = "Why should software be updated?",
                Options = new string[] { "For fun", "To waste data", "For security patches", "To slow computer" },
                CorrectAnswerIndex = 2
            });

            quizManager.Questions.Add(new Models.QuizQuestion
            {
                Question = "Which is an example of personal data?",
                Options = new string[] { "Password", "Favorite color", "Wallpaper", "Battery level" },
                CorrectAnswerIndex = 0
            });

            quizManager.Questions.Add(new Models.QuizQuestion
            {
                Question = "What is social engineering?",
                Options = new string[] { "Building apps", "Tricking people for information", "Network repair", "Cloud storage" },
                CorrectAnswerIndex = 1
            });

            ShowQuestion();
        }

        private void LoadTasks()
        {
            taskManager.Tasks = JsonStorage.Load<Models.TaskItem>(taskFile);

            TaskListBox.Items.Clear();

            foreach (var task in taskManager.Tasks)
            {
                TaskListBox.Items.Add(task.Title);
            }
        }

        private void ShowQuestion()
        {
            if (quizManager.Questions.Count == 0)
                return;

            var question = quizManager.Questions[0];

            QuizQuestionText.Text = question.Question;
            QuizOptionsList.Items.Clear();

            foreach (var option in question.Options)
            {
                QuizOptionsList.Items.Add(option);
            }
        }

        private void SubmitQuizButton_Click(object sender, RoutedEventArgs e)
        {
            if (QuizOptionsList.SelectedIndex == -1)
                return;

            var question = quizManager.Questions[0];

            if (QuizOptionsList.SelectedIndex == question.CorrectAnswerIndex)
            {
                MessageBox.Show("Correct!");
                quizManager.Score++;
                activityLogger.Log("Quiz answer correct");
            }
            else
            {
                MessageBox.Show("Incorrect!");
                activityLogger.Log("Quiz answer incorrect");
            }

            // Remove answered question
            quizManager.Questions.RemoveAt(0);
            JsonStorage.Save(logFile, activityLogger.Logs);
            RefreshLogs();

            // Show next question or finish quiz
            if (quizManager.Questions.Count > 0)
            {
                ShowQuestion();
            }
            else
            {
                QuizQuestionText.Text = $"Quiz Finished! Final Score: {quizManager.Score}/10";
                QuizOptionsList.Items.Clear();
            }
        }

        private void LoadLogs()
        {
            activityLogger.Logs = JsonStorage.Load<Models.ActivityLog>(logFile);
            RefreshLogs();
        }
    }
}