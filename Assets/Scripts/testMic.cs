using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Whisper.Utils;
using TMPro;
using Button = UnityEngine.UI.Button;
using Toggle = UnityEngine.UI.Toggle;

namespace Whisper.Samples
{
    /// <summary>
    /// Record audio clip from microphone and make a transcription.
    /// </summary>
    public class testMic : MonoBehaviour
    {
        public WhisperManager whisper;
        public MicrophoneRecord microphoneRecord;
        public bool streamSegments = true;
        public bool printLanguage = true;

        [Header("UI")] 
        public Button button;
        public Text buttonText;
        public Text outputText;
        public Text timeText;
        public Dropdown languageDropdown;
        public Toggle translateToggle;
        public Toggle vadToggle;
        public ScrollRect scroll;

        // New fields for API interaction
        public TextMeshProUGUI uiText; // Assign your UI Text object in the Inspector
        public TMP_InputField inputField; // Assign your InputField object in the Inspector
        public Button sendButton; // Assign your Button object in the Inspector
        private string apiUrl = "http://localhost:11434/api/generate"; // Local API URL
        
        private string _buffer;

        private void Awake()
        {
            whisper.OnNewSegment += OnNewSegment;
            whisper.OnProgress += OnProgressHandler;
            
            microphoneRecord.OnRecordStop += OnRecordStop;
            
            button.onClick.AddListener(OnButtonPressed);
            languageDropdown.value = languageDropdown.options
                .FindIndex(op => op.text == whisper.language);
            languageDropdown.onValueChanged.AddListener(OnLanguageChanged);

            translateToggle.isOn = whisper.translateToEnglish;
            translateToggle.onValueChanged.AddListener(OnTranslateChanged);

            vadToggle.isOn = microphoneRecord.vadStop;
            vadToggle.onValueChanged.AddListener(OnVadChanged);

            // Add listener to the send button
            sendButton.onClick.AddListener(OnSendButtonClick);
        }

        private void OnVadChanged(bool vadStop)
        {
            microphoneRecord.vadStop = vadStop;
        }

        private void OnButtonPressed()
        {
            if (!microphoneRecord.IsRecording)
            {
                microphoneRecord.StartRecord();
                buttonText.text = "Stop";
            }
            else
            {
                microphoneRecord.StopRecord();
                buttonText.text = "Record";
            }
        }
        
        private async void OnRecordStop(AudioChunk recordedAudio)
        {
            buttonText.text = "Record";
            _buffer = "";

            var sw = new Stopwatch();
            sw.Start();
            
            var res = await whisper.GetTextAsync(recordedAudio.Data, recordedAudio.Frequency, recordedAudio.Channels);
            if (res == null || !outputText) 
                return;

            var time = sw.ElapsedMilliseconds;
            var rate = recordedAudio.Length / (time * 0.001f);
            timeText.text = $"Time: {time} ms\nRate: {rate:F1}x";

            var text = res.Result;
            if (printLanguage)
                text += $"\n\nLanguage: {res.Language}";
            
            outputText.text = text;
            UiUtils.ScrollDown(scroll);

            // Send the transcribed text to the API
            StartCoroutine(FetchDataFromAPI(text));
        }
        
        private void OnLanguageChanged(int ind)
        {
            var opt = languageDropdown.options[ind];
            whisper.language = opt.text;
        }
        
        private void OnTranslateChanged(bool translate)
        {
            whisper.translateToEnglish = translate;
        }

        private void OnProgressHandler(int progress)
        {
            if (!timeText)
                return;
            timeText.text = $"Progress: {progress}%";
        }
        
        private void OnNewSegment(WhisperSegment segment)
        {
            if (!streamSegments || !outputText)
                return;

            _buffer += segment.Text;
            outputText.text = _buffer + "...";
            UiUtils.ScrollDown(scroll);
        }

        // New method for handling send button click
        public void OnSendButtonClick()
        {
            // Get the prompt from the input field and start the API call coroutine
            string prompt = inputField.text;
            StartCoroutine(FetchDataFromAPI(prompt));
        }

        // New coroutine for fetching data from API
        IEnumerator FetchDataFromAPI(string prompt)
        {
            // Create the JSON data
            string jsonData = $"{{\"model\": \"llama3.2\", \"prompt\": \"{prompt}\", \"stream\": false}}";
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
}