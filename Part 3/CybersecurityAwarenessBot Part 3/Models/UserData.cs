namespace CybersecurityAwarenessBotPart3.Models
{
    public class UserData
    {
        public string Name { get; set; } = "User";
        public string FavoriteTopic { get; set; } = string.Empty;

        public bool HasProvidedTopic => !string.IsNullOrEmpty(FavoriteTopic);
    }
}