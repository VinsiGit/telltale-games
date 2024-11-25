using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class test : MonoBehaviour
{
    public TextMeshProUGUI uiText; // Assign your UI Text object in the Inspector
    public TMP_InputField inputField; // Assign your InputField object in the Inspector
    public Button sendButton; // Assign your Button object in the Inspector
    private string apiUrl = "http://localhost:11434/api/generate"; // Local API URL

    void Start()
    {
        // Optionally, you can start with a default prompt
        StartCoroutine(FetchDataFromAPI("how are you going?"));

        // Add listener to the button
        sendButton.onClick.AddListener(OnButtonClick);
    }

    public void OnButtonClick()
    {
        // Get the prompt from the input field and start the API call coroutine
        string prompt = inputField.text;
        StartCoroutine(FetchDataFromAPI(prompt));
    }

    IEnumerator FetchDataFromAPI(string prompt)
    {
        // Create the JSON data
        string jsonData = $"{{\"model\": \"llama3.2\", \"prompt\": \"{prompt}\", \"stream\": false}}";

        // Create a new UnityWebRequest for the API call
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Send the request and wait for the response
            yield return request.SendWebRequest();

            // Check for errors
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error fetching data: " + request.error);
                uiText.text = "Error loading data!";
            }
            else
            {
                // Parse and update text
                string responseData = request.downloadHandler.text;
                Debug.Log("Data received: " + responseData);

                // Deserialize the JSON response
                ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(responseData);

                // Set the UI text dynamically
                uiText.text = apiResponse.response;
            }
        }
    }
}

[System.Serializable]
public class ApiResponse
{
    public string response;
}