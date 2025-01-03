using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using TMPro;

public class SubtitleManager : MonoBehaviour
{
    public TextMeshProUGUI subtitleText;
    public GameObject subtitleBox;
    public Transform playerCamera;
    public GameObject lookTarget;
    public InputAction nextSubtitleAction;
    public string[] subtitles;
    private int currentSubtitleIndex = 0;
    private float lookDistance = 50f;
    private bool looking = false;
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
        if (!looking)
        {
            ShowNextSubtitle();
        }
        else
        {
            subtitleText.text = "Tip: look at the right side of the room.";
        }
    }
    public void ShowNextSubtitle()
    {
        currentSubtitleIndex++;
        if (currentSubtitleIndex == 2)
        {
            looking = true;
        }

        if (currentSubtitleIndex < subtitles.Length)
        {
            ShowSubtitle(subtitles[currentSubtitleIndex]);
        }
        else
        {
            HideSubtitle();
        }
    }
    private bool IsPlayerLookingAtObject()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, lookDistance))
        {
            return hit.collider.gameObject == lookTarget;
        }

        return false;
    }
    void Update()
    {
        if (IsPlayerLookingAtObject() && looking)
        {
            looking = false;
            ShowNextSubtitle();
        }
    }
}
