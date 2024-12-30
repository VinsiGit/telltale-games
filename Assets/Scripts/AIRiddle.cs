using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.IO;

namespace AIRiddleNamespace
{
    public class AIRiddle : MonoBehaviour
    {
        public TextMeshProUGUI uiText; // Assign your UI Text object in the Inspector
        public TMP_InputField inputField; // Assign your InputField object in the Inspector
        public Button sendButton; // Assign your Button object in the Inspector
        public Button resetButton; // Assign your Reset Button object in the Inspector
        public Button resendButton; // Assign your Resend Button object in the Inspector
        public string masterStringPerson = "You are Julius Caesar, you only know what Julius Caesar would have known. "; // Master string for the person role
        private string masterString = "The player has to solve a puzzle by guessing the answer to a riddle. The riddle awnser is $riddle make it about this. You can't tell the player the awnser. you will give them $number riddles beginning with easy until hard. you only give them the next riddle when they have solved the previous one. you can help the player but you can't solve it or tell them the answer. (you are in a 3d environment where the player is in VR and has a terminal where they can give an answer. what you say will be spoken to them, only speak to them, you are a human. keep your answer short. when the player has answered correctly give them a new riddle and this icon \"[ok]\", this is so the program can know that the answer is correct. you have to give them the next question after \"[ok]\" this is very important as else the user doesn't know what to do)"; // Master string for the system role

        public int numberOfQuestions = 3; // Number of questions to be answered correctly

        private string apiUrl;
        private string apiKey;
        private string modelName;
        private List<Message> messages = new List<Message>(); // List to store conversation messages
        private List<GameObject> boxes = new List<GameObject>(); // List to store box prefabs
        public List<string> riddleAnswers = new List<string>()
        {
            "Coin",
            "Tommorow",
            "Time",
            "Fire",
            "Wind",
            "Silence",
            "Mirror",
            "Stars",
            "Moon",
            "Sword"
        }; // List to store riddle answers
        private int correctAnswers = 0; // Number of correctly answered questions

        private bool done = false;

        public GameObject boxPrefab; // Assign your Box prefab in the Inspector
        void OnValidate()
        {
            if (numberOfQuestions < 1)
            {
                numberOfQuestions = 1;
            }
        }

        void Start()
        {
            // Add the system role message at the top
            messages.Add(new Message { role = "system", content = masterStringPerson + masterString.Replace("$number", numberOfQuestions.ToString()).Replace("$riddle", riddleAnswers[Random.Range(0, riddleAnswers.Count)]) });
            LoadConfig();

            // Instantiate the boxes
            for (int i = 0; i < numberOfQuestions; i++)
            {
                GameObject box = Instantiate(boxPrefab, transform.position + transform.right * (i * 0.25F + 0.1F), Quaternion.identity, transform);
                boxes.Add(box);
            }

            // Use the environment variables
            Debug.Log("API Key: " + apiKey);
            Debug.Log("API Url: " + apiUrl);
            Debug.Log("Model Name: " + modelName);

            messages.Add(new Message { role = "user", content = "give me a riddle" }); // Add user message to messages list
            StartCoroutine(FetchDataFromAPI());

            // Add listener to the buttons
            sendButton.onClick.AddListener(OnButtonClick);
            resetButton.onClick.AddListener(OnResetButtonClick);
            resendButton.onClick.AddListener(OnResendButtonClick); // Add listener for the resend button
        }

        void LoadConfig()
        {
            Debug.Log($"Application.persistentDataPath: {Application.persistentDataPath}");

            TextAsset configTextAsset = Resources.Load<TextAsset>("config");
            if (configTextAsset != null)
            {
                string json = configTextAsset.text;
                Config config = JsonUtility.FromJson<Config>(json);
                apiKey = config.API_KEY;
                apiUrl = config.API_URL;
                modelName = config.MODEL_NAME;

                // Log the loaded configuration
                Debug.Log($"Loaded Config - API Key: {apiKey}, API Url: {apiUrl}, Model Name: {modelName}");
            }
            else
            {
                Debug.LogError("Config file not found in Resources.");
                uiText.text = "Config file not found.";
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
            // Reset the correctAnswers count
            correctAnswers = 0;

            // Reset the color of all boxes to their original color
            foreach (GameObject box in boxes)
            {
                Renderer boxRenderer = box.GetComponent<Renderer>();
                if (boxRenderer != null)
                {
                    boxRenderer.material.color = Color.gray; // Change the material color to the original color (assuming white)
                }
                else
                {
                    UnityEngine.Debug.LogError("Box prefab does not have a Renderer component.");
                }
            }
            // Ask a new question (you can modify this to ask a different question)
            messages = new List<Message>();
            messages.Add(new Message { role = "system", content = masterStringPerson + masterString.Replace("$number", numberOfQuestions.ToString()).Replace("$riddle", riddleAnswers[Random.Range(0, riddleAnswers.Count)]) });
            StartCoroutine(FetchDataFromAPI());
        }
        public void OnResendButtonClick()
        {
            Debug.Log("Resend button clicked!");

            string updatedMasterString = masterStringPerson + $"The player has solved {correctAnswers} of the {numberOfQuestions} riddles. " + masterString.Replace("$number", numberOfQuestions.ToString()).Replace("$riddle", riddleAnswers[Random.Range(0, riddleAnswers.Count)]);
            messages = new List<Message>();
            messages.Add(new Message { role = "system", content = updatedMasterString });
            StartCoroutine(FetchDataFromAPI());

        }

        public void LightUpBox()
        {
            if (correctAnswers < numberOfQuestions)
            {
                Renderer boxRenderer = boxes[correctAnswers].GetComponent<Renderer>();
                if (boxRenderer != null)
                {
                    boxRenderer.material.color = Color.green; // Change the material color to green
                    boxRenderer.material.SetFloat("_Metallic", 1.0f);
                    correctAnswers++;
                    string updatedMasterString = masterStringPerson + $"The player has solved {correctAnswers} of the {numberOfQuestions} riddles. " + masterString.Replace("$number", numberOfQuestions.ToString()).Replace("$riddle", riddleAnswers[Random.Range(0, riddleAnswers.Count)]);
                    messages = new List<Message>();
                    messages.Add(new Message { role = "system", content = updatedMasterString });
                    if (correctAnswers == numberOfQuestions)
                    {
                        uiText.text = "Congratulations! You have solved all the riddles.";
                        done = true;
                    }
                    else
                    {
                        StartCoroutine(FetchDataFromAPI());
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError("Box prefab does not have a Renderer component.");
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
                    uiText.text = $"Error loading data! Error: {request.error}\nResponse Code: {request.responseCode}\nResponse: {request.downloadHandler.text}\nURL: {apiUrl}";
                }
                else
                {
                    // Parse and update text
                    string responseDataAI = request.downloadHandler.text;
                    UnityEngine.Debug.Log("Data received: " + responseDataAI);

                    // Deserialize the JSON response
                    ApiResponse jsonResponse = JsonUtility.FromJson<ApiResponse>(responseDataAI);
                    string content = jsonResponse.choices[0].message.content;
                    Debug.Log("Content: " + content);
                    if (content.Contains("[ok]"))
                    {
                        LightUpBox(); // Light up a box
                        content = content.Replace("[ok]", ""); // Remove [ok] from the content
                        // messages.Add(new Message { role = "user", content = "give me a riddle" });
                    }
                    else
                    {
                        // Add response to messages list
                        messages.Add(new Message { role = "assistant", content = content });

                        // Update the UI text with the entire conversation
                        uiText.text = content; // string.Join("\n", messages.ConvertAll(m => $"{m.role}: {m.content}"));
                    }
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