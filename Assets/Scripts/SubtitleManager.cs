using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using TMPro;

public class SubtitleManager : MonoBehaviour
{
    public TextMeshProUGUI subtitleText;
    public GameObject subtitleBox;
    public InputAction nextSubtitleAction;
    public string[] subtitles;
    private int currentSubtitleIndex = 0;
    private void OnEnable()
    {
        nextSubtitleAction.Enable();
        nextSubtitleAction.performed += OnNextSubtitlePressed;
    }

    private void OnDisable()
    {
        nextSubtitleAction.Disable();
        nextSubtitleAction.performed -= OnNextSubtitlePressed;
    }
    // Start is called before the first frame update
    void Start()
    {
        subtitleText.gameObject.SetActive(false);
        subtitleBox.SetActive(false);

        if (subtitles.Length > 0)
        {
            ShowSubtitle(subtitles[currentSubtitleIndex]);
        }
    }
    private void HideSubtitle()
    {
        subtitleText.gameObject.SetActive(false);
        subtitleBox.SetActive(false);
    }
    public void ShowSubtitle(string message, float duration = -1f)
    {
        subtitleText.gameObject.SetActive(true);
        subtitleBox.SetActive(true);

        subtitleText.text = message;
    }
    private void OnNextSubtitlePressed(InputAction.CallbackContext context)
    {
        ShowNextSubtitle();
    }
    public void ShowNextSubtitle()
    {
        currentSubtitleIndex++;

        if (currentSubtitleIndex < subtitles.Length)
        {
            ShowSubtitle(subtitles[currentSubtitleIndex]);
        }
        else
        {
            HideSubtitle();
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
