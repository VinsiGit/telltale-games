using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class test : MonoBehaviour
{
    public TextMeshProUGUI uiText; // Assign your UI Text object in the Inspector
    public TMP_InputField inputField; // Assign your InputField object in the Inspector
    public Button sendButton; // Assign your Button object in the Inspector
    public string masterString = "you are julius caesar, you only know what julius caesar would have know. the player has to solve a puzzle by guessing the answer to the question. you will give them 3 riddles beginning with easy and medium and then hard. you only give them the next riddle when they have solve the previous one. you can help the player but you can't solve it. (you are in a 3d environment where you player is in vr and has a terminal where they can give a answer. what you say will be spoken to them, only speak to them, you are a human. keep your answer short. when the player has answered correctly give them this icon \"[ok]\", this is so the program can know that the answer is correct)"; // Master string for the system role
    public GameObject boxPrefab;	
	private string apiUrl = "http://localhost:11434/api/chat"; // Local API URL

    private List<Message> messages = new List<Message>(); // List to store conversation messages

    void Start()
    {
        // Add the system role message at the top
        messages.Add(new Message { role = "system", content = masterString });

        // Optionally, you can start with a default prompt
        // StartCoroutine(FetchDataFromAPI("how are you going?"));

        // Add listener to the button
        sendButton.onClick.AddListener(OnButtonClick);
    }

    public void OnButtonClick()
    {
        // Get the prompt from the input field and start the API call coroutine
        string prompt = inputField.text;
        messages.Add(new Message { role = "user", content = prompt }); // Add user message to messages list
		StartCoroutine(FetchDataFromAPI());

    }
	public void LightUpBox()
	{
		if (boxPrefab != null)
		{
			// Get the Renderer component and change its material color
			Renderer boxRenderer = boxPrefab.GetComponent<Renderer>();
			if (boxRenderer != null)
			{
				boxRenderer.material.color = Color.green; // Change the material color to green
			}
			else
			{
				UnityEngine.Debug.LogError("Box prefab does not have a Renderer component.");
			}
		}
		else
		{
			UnityEngine.Debug.LogError("Box prefab is not assigned.");
		}
	}
	IEnumerator FetchDataFromAPI()
	{
		// Create the JSON data
		var requestData = new RequestData
		{
			model = "llama3.2",
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
				string responseData = request.downloadHandler.text;
				UnityEngine.Debug.Log("Data received: " + responseData);

				// Deserialize the JSON response
				ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(responseData);

				// Check for [ok] in the response content
				string content = apiResponse.message.content;
				if (content.Contains("[ok]"))
				{
					LightUpBox(); // Light up a box
					content = content.Replace("[ok]", ""); // Remove [ok] from the content
				}

				// Add response to messages list
				messages.Add(new Message { role = "assistant", content = apiResponse.message.content });

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
    public Message message;
}