using CybersecurityAwarenessBotPart3.Models;
using CybersecurityAwarenessBotPart3.Services;
using System;
using System.Collections.Generic;

namespace CybersecurityAwarenessBotPart3.Services
{
    /// <summary>
    /// Core chat logic: keyword routing via delegates, sentiment detection,
    /// conversation memory, follow-up flow, and random tip selection.
    /// </summary>
    public class ChatEngine
    {
        private readonly Dictionary<string, BotResponseHandler> _topicHandlers;
        private readonly List<string> _phishingTips;
        private readonly Dictionary<string, string> _sentimentTriggers;
        private readonly Dictionary<string, string> _expansionTips;
        private readonly Random _random = new();

        private string _lastDiscussedTopic = string.Empty;

        public UserData SessionUser { get; }

        public ChatEngine()
        {
            SessionUser = new UserData();
            _phishingTips = BuildPhishingTips();
            _sentimentTriggers = BuildSentimentTriggers();
            _expansionTips = BuildExpansionTips();
            _topicHandlers = BuildTopicHandlers();
        }

        public string ProcessInput(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
                return "You entered nothing. Please type a question so I can help you.";

            string input = userInput.ToLower().Trim();

            if (input.Contains("my name is"))
            {
                ExtractName(userInput);
                return $"Great to meet you, {SessionUser.Name}! What security topics are you looking into today?";
            }

            if (input.Contains("how are you"))
            {
                return $"I don't have feelings, {SessionUser.Name}. However, I am ready to help you with cybersecurity awareness.";
            }

            if (input.Contains("what's your purpose") || input.Contains("what is your purpose"))
            {
                return "My purpose is to teach users about common cyber threats,\n" +
                       "warn them about risks like phishing and scams also provide useful tips to protect personal\n" +
                       "information and help reduce the chances of cyber attacks.";
            }

            if (input.Contains("what can i ask you about"))
            {
                return "Ask about password safety,\n" +
                       "phishing, suspicious links, scams,\n" +
                       "safe browsing and basic online protection.";
            }

            if (input == "exit")
                return $"Goodbye {SessionUser.Name}. Stay safe online!";

            if (input.Contains("explain more") || input.Contains("tell me more") || input.Contains("give me a tip"))
                return HandleFollowUp(input);

            foreach (var trigger in _sentimentTriggers)
            {
                if (input.Contains(trigger.Key))
                {
                    return trigger.Value +
                           " Standard security advice: Always verify communication channels offline before sharing sensitive data.";
                }
            }

            if (input.Contains("password"))
                return InvokeTopicHandler("password", "Password Security Management", _topicHandlers["password"], userInput);

            if (input.Contains("phishing"))
                return InvokeTopicHandler("phishing", "Phishing Vectors & Mitigation", _topicHandlers["phishing"], userInput);

            if (input.Contains("safe browsing") || input.Contains("browse safely") || input.Contains("browsing"))
                return InvokeTopicHandler("browsing", "Safe Browsing Frameworks", _topicHandlers["browsing"], userInput);

            if (input.Contains("suspicious link") || input.Contains("link"))
                return InvokeTopicHandler("link", "Suspicious URL Identification", _topicHandlers["link"], userInput);

            if (input.Contains("scam"))
                return InvokeTopicHandler("scam", "Social Engineering Scams", _topicHandlers["scam"], userInput);

            return "I didn't quite understand that. Could you rephrase? You can ask about passwords, phishing, or safe browsing.";
        }

        private string InvokeTopicHandler(string topicKey, string displayTopic, BotResponseHandler handler, string userInput)
        {
            _lastDiscussedTopic = topicKey;

            if (!SessionUser.HasProvidedTopic)
                SessionUser.FavoriteTopic = displayTopic;

            string response = handler(userInput);

            if (SessionUser.Name != "User" && SessionUser.FavoriteTopic == displayTopic)
            {
                return $"Hey {SessionUser.Name}, since you're interested in {displayTopic.ToLower()}:\n{response}";
            }

            return response;
        }

        private string HandleFollowUp(string input)
        {
            if (_lastDiscussedTopic == "phishing" || input.Contains("tip"))
                return _phishingTips[_random.Next(_phishingTips.Count)];

            if (!string.IsNullOrEmpty(_lastDiscussedTopic) && _expansionTips.TryGetValue(_lastDiscussedTopic, out string? expansion))
                return expansion;

            return "We can explore deep dives into passwords, phishing scams, or safe browsing. Which one is worrying you?";
        }

        private void ExtractName(string userInput)
        {
            int index = userInput.IndexOf("my name is", StringComparison.OrdinalIgnoreCase);
            if (index < 0)
                return;

            string remaining = userInput.Substring(index + 10).Trim();
            if (!string.IsNullOrEmpty(remaining))
                SessionUser.Name = remaining;
        }

        private Dictionary<string, BotResponseHandler> BuildTopicHandlers()
        {
            return new Dictionary<string, BotResponseHandler>(StringComparer.OrdinalIgnoreCase)
            {
                { "password", RespondToPassword },
                { "phishing", RespondToPhishing },
                { "browsing", RespondToSafeBrowsing },
                { "link", RespondToSuspiciousLinks },
                { "scam", RespondToScam }
            };
        }

        private static List<string> BuildPhishingTips()
        {
            return new List<string>
            {
                "CRITICAL TIP: Check the sender's actual email domain address carefully for typos before interacting.",
                "CRITICAL TIP: Hover over hyperlinks to preview the actual destination URL path safely before clicking.",
                "CRITICAL TIP: If an alert demands extreme artificial urgency, treat it as highly suspicious operational sabotage.",
                "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organisations.",
                "Check the sender's actual email address carefully. A misplaced letter usually indicates a fraudulent phishing domain."
            };
        }

        private static Dictionary<string, string> BuildSentimentTriggers()
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "worried", "System detects elevated concern state. It's completely understandable—scammers use advanced psychology. Let's look at the facts." },
                { "frustrated", "System warning acknowledged. Cybersecurity constraints are complex, but keeping defense standards high prevents breaches." },
                { "curious", "I love that initiative! Staying curious is the single best defense mechanism against social engineering attacks." }
            };
        }

        private static Dictionary<string, string> BuildExpansionTips()
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "password", "ANALYSIS EXPANSION: Secure operators also make extensive use of localized hardware multi-factor tokens (MFA) to lock down remote entry protocols." },
                { "phishing", "EXPANSION: Report suspected phishing to your IT team or email provider so others are protected from the same campaign." },
                { "browsing", "EXPANSION: Use a reputable password manager and enable browser security extensions that block known malicious domains." },
                { "link", "EXPANSION: When in doubt, navigate to the site manually by typing the official URL instead of clicking embedded links." },
                { "scam", "EXPANSION: If you've shared information with a suspected scammer, contact your bank immediately and change affected passwords." }
            };
        }

        private static string RespondToPassword(string _)
        {
            return "What I can say is that, password safety is important for protecting your accounts from being hacked. \n" +
                   "A strong password should be long and include a mix of letters, numbers and symbols you should " +
                   "avoid using the same \npassword for different accounts. \n" +
                   "Here are some key tips: \n" +
                   "-Don't use personal information \n" +
                   "-Don't share your password \n" +
                   "-Avoid suspicious links or emails \n" +
                   "-Change your password if needed";
        }

        private static string RespondToPhishing(string _)
        {
            return "Phishing is a type of online scam where attackers trick people into giving away sensitive information like \n" +
                   "passwords or bank details by pretending to be a trusted source. This is usually done through fake emails, \n" +
                   "messages or websites that look real.\n" +
                   "Some signs of phishing:\n" +
                   "-Messages asking for your password or personal details.\n" +
                   "-Suspicious links or attachments.\n" +
                   "-Urgent or threatening (\"your account will be locked\").\n" +
                   "-Email addresses that look slightly wrong or wrong completely.\n" +
                   "To stay safe:\n" +
                   "-Don't click unknown links.\n" +
                   "-Never share your passwords.\n" +
                   "-Verify websites before entering information.";
        }

        private static string RespondToSafeBrowsing(string _)
        {
            return "Safe browsing means using the internet in a way that protects your personal information and avoids harmful\n" +
                   "websites. It helps prevent scams, viruses and data theft.\n" +
                   "Some key tips for safe browsing:\n" +
                   "-Only visit trusted and secure websites (look for https).\n" +
                   "-Don't click on suspicious links.\n" +
                   "-Avoid downloading files from unknown sources.\n" +
                   "-Keep your browser and antivirus updated.\n" +
                   "-Log out of accounts on shared devices.";
        }

        private static string RespondToSuspiciousLinks(string _)
        {
            return "A suspicious link is a link that may lead to a fake or harmful website designed to steal your information or\n" +
                   "infect your device. These links are often used in phishing scams and can look similar to real websites but\n" +
                   "have small differences.\n" +
                   "Some signs of a suspicious links:\n" +
                   "-Strange or misspelled website names.\n" +
                   "-Shortened links (like bit.ly) that hide the real address.\n" +
                   "-Links sent from unknown or unusual senders.\n" +
                   "-Messages creating urgency to click the link.\n" +
                   "To stay safe:\n" +
                   "-Don't click links you don't trust.\n" +
                   "-Hover over the link to see the real URL.\n" +
                   "-Type the website address manually instead of clicking.\n" +
                   "-Delete messages that seem suspicious.";
        }

        private static string RespondToScam(string _)
        {
            return "A scam is a dishonest attempt to trick people into giving away money or personal information by pretending\n" +
                   "to be someone trustworthy. Scammers often use messages, calls or fake websites to deceive victims.\n" +
                   "Some common signs of a scam:\n" +
                   "-Promises of easy money or prizes.\n" +
                   "-Requests for personal or banking details.\n" +
                   "-Urgent messages that pressure you to act quickly.\n" +
                   "-Unknown or suspicious contacts.\n" +
                   "To stay safe:\n" +
                   "-Don't share personal information.\n" +
                   "-Ignore offers that seem too good to be true.\n" +
                   "-Verify the source before responding.\n" +
                   "-Block and report suspicious contacts.\n";
        }
    }
}