namespace MailTagger.Helpers;

public static class OpenAiHelper
{
    public static string GetSystemPrompt()
    {
        return @"You are an AI assistant that helps customer support teams by classifying customer messages into relevant categories. 

Given a customer message, your task is to assign one or more of the following tags based on the content:

- Bug Report
- Billing Issue
- Praise
- Complaint
- Feature Request
- Technical Support
- Sales Inquiry
- Security Concern
- Spam/Irrelevant
- Refund Request
- Shipping/Delivery
- Other

Guidelines:
- Return the most relevant tags.
- Use multiple tags if the message covers more than one.
- If the message is off-topic, unclear, or not relevant, use ""Spam/Irrelevant"".
- If the message doesn’t fit any category, use ""Other"".

Output format (strict JSON):
[{""email"": ""<email>"", ""tags"": [""<tag1>"", ""<tag2>""]}]
";
    }

    public static object GetToolPrompt()
    {
        return new[]
        {
            new
            {
                type = "function",
                function = new
                {
                    name = "mailTagger",
                    description = "Tag a list of emails with appropriate tags",
                    parameters = new
                    {
                        type = "object",
                        properties = new
                        {
                            data = new
                            {
                                type = "array",
                                description = "A list of emails to be tagged",
                                items = new
                                {
                                    type = "object",
                                    properties = new
                                    {
                                        email = new
                                        {
                                            type = "string",
                                            description = "The email that is being tagged"
                                        },
                                        tags = new
                                        {
                                            type = "array",
                                            description = "Analize message of the mail and assign one or more relevant tags for the email",
                                            items = new
                                            {
                                                type = "string",
                                                @enum = new[] {
                                                    "Bug Report",
                                                    "Billing Issue",
                                                    "Praise",
                                                    "Complaint",
                                                    "Feature Request",
                                                    "Technical Support",
                                                    "Sales Inquiry",
                                                    "Security Concern",
                                                    "Spam/Irrelevant",
                                                    "Refund Request",
                                                    "Shipping/Delivery",
                                                    "Other"
                                                }
                                            }
                                        }
                                    },
                                    required = new[] { "email", "tags" }
                                }
                            }
                        },
                        required = new[] { "data" }
                    }
                }
            }
        };
    }
}
