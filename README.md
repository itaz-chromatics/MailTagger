This is a .NET 8 API that uses either OpenAI (GPT-4.1-Nano) or Gemini (Gemini-2-Flash) to automatically generate context-based tags from user messages. The LLM provider can be dynamically selected via the API request.

Features

Built with .NET 8
Supports OpenAI and Gemini as LLM providers
Dynamic model selection via API (llmProvider param)
Configurable base URLs and model names in appsettings.json
Returns structured tags based on user messages

LLM Providers
Provider llmProvider Value Model Used
OpenAI 1 gpt-4.1-nano
Gemini 2 gemini-2-flash

Sample Request
POST /MailTagger
{
"data": [
{
"email": "11@11.com",
"message": "I’ve been charged twice and I need my money back"
},
{
"email": "22@22.com",
"message": "I’ve been charged twice. Your service is bad"
}
],
"llmProvider": 1
}

Sample Response
[
{
"email": "11@11.com",
"tags": ["Billing Issue", "Refund Request"]
},
{
"email": "22@22.com",
"tags": ["Billing Issue", "Complaint"]
}
]
