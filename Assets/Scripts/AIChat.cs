using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.IO;
using System.Text.Json;

namespace AIChatNamespace
{
    public class AIChat : MonoBehaviour
    {
        public TextMeshProUGUI uiText; // Assign your UI Text object in the Inspector
        public TMP_InputField inputField; // Assign your InputField object in the Inspector
        public Button sendButton; // Assign your Button object in the Inspector
        public Button resetButton; // Assign your Reset Button object in the Inspector
        public Button resendButton; // Assign your Resend Button object in the Inspector
        public string masterStringPerson = "You are julius caesar, you only know what julius caesar would have know. "; // Master string for the person role
        private string masterString = " You only know what this person would have known. you are here to talk with them or help them if they ask a question. "; // Master string for the assistant role
        private string apiUrl;
        private string apiKey;
        private string modelName;
        private List<Message> messages = new List<Message>(); // List to store conversation messages

        void Start()
        {
            // Add the system role message at the top
            messages.Add(new Message { role = "system", content = masterStringPerson + masterString });
            LoadConfig();

            // Use the environment variables
            Debug.Log("API Key: " + apiKey);
            Debug.Log("API Url: " + apiUrl);
            Debug.Log("Model Name: " + modelName);

            // Add listener to the buttons
            sendButton.onClick.AddListener(OnButtonClick);
            resetButton.onClick.AddListener(OnResetButtonClick);
            resendButton.onClick.AddListener(OnResendButtonClick);
        }

        void LoadConfig()
        {
            Debug.Log($"Application.dataPath: {Application.dataPath}");
            string path = Path.Combine(Application.dataPath, "config.json").Replace("\\", "/");
            Debug.Log($"Path: {path}");
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                Config config = JsonUtility.FromJson<Config>(json);
                apiKey = config.API_KEY;
                apiUrl = config.API_URL;
                modelName = config.MODEL_NAME;
            }
            else
            {
                Debug.LogError("Config file not found!");
            }
        }

        public void OnButtonClick()
        {
            Debug.Log("Button clicked!");
            // Get the prompt from the input field and start the API call coroutine
            string prompt = inputField.text;
            Debug.Log("Prompt: " + prompt);
            messages.Add(new Message { role = "user", content = prompt }); // Add user message to messages list
            StartCoroutine(FetchDataFromAPI());
        }

        public void OnResetButtonClick()
        {
            // Clear the messages list and reset the UI text
            messages.Clear();
            uiText.text = "";

            // Add the system role message at the top
            messages.Add(new Message { role = "system", content = masterStringPerson + masterString.Replace("$number", "3") });
        }

        public void OnResendButtonClick()
        {
            Debug.Log("Resend button clicked!");

            // Resend the current riddle without resetting the points
            if (messages.Count > 0)
            {
                // Find the last assistant message (the current riddle)
                Message lastAssistantMessage = messages.FindLast(m => m.role == "assistant");
                if (lastAssistantMessage != null)
                {
                    // Add the last assistant message to the messages list again
                    messages.Add(new Message { role = "assistant", content = lastAssistantMessage.content });
                    uiText.text = lastAssistantMessage.content; // Update the UI text with the current riddle
                }
            }
        }

        IEnumerator FetchDataFromAPI()
        {
            // Create the JSON data
            var requestData = new RequestData
            {
                model = modelName,
                messages = messages,
                stream = false
            };
            string jsonData = JsonUtility.ToJson(requestData);
            UnityEngine.Debug.Log("JSON Data: " + jsonData); // Log the JSON data

            // Create a new UnityWebRequest for the API call
            using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

                // Send the request and wait for the response
                yield return request.SendWebRequest();

                // Check for errors
                if (request.result != UnityWebRequest.Result.Success)
                {
                    UnityEngine.Debug.LogError("Error fetching data: " + request.error);
                    uiText.text = "Error loading data!";
                }
                else
                {
                    // Parse and update text
                    string responseDataAI = request.downloadHandler.text;
                    UnityEngine.Debug.Log("Data received: " + responseDataAI);

                    // Deserialize the JSON response
                    ApiResponse jsonResponse = JsonUtility.FromJson<ApiResponse>(responseDataAI);
                    string content = jsonResponse.choices[0].message.content;
                    // ollama string content = jsonResponse.message.content;
                    Debug.Log("Content: " + content);

                    // Add response to messages list
                    messages.Add(new Message { role = "assistant", content = content });

                    // Update the UI text with the entire conversation
                    uiText.text = content; // string.Join("\n", messages.ConvertAll(m => $"{m.role}: {m.content}"));
                }
            }
        }
    }

    [System.Serializable]
    public class Message
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    public class RequestData
    {
        public string model;
        public List<Message> messages;
        public bool stream;
    }

    [System.Serializable]
    public class ApiResponse
    {
        public List<Choice> choices;
    }

    [System.Serializable]
    public class Choice
    {
        public Message message;
    }

    [System.Serializable]
    public class Config
    {
        public string API_KEY;
        public string API_URL;
        public string MODEL_NAME;
    }
}