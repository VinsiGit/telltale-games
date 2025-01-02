using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Video;
public class turning : MonoBehaviour
{
    public float scanTime = 1.0f;
    public float rotationSpeed = 100f;
    public GameObject videoObject;
    private float fadeDuration = 3f;
    private Material videoMaterial;
    private float alpha = 0.0f;
    private bool isFading = false;
    private Vector3 centerPoint;
    private float timer = 0f;
    private bool isScanning = false;
    private bool isBeingGrabbed = false;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private void Start()
    {
        videoObject.SetActive(false);
        Renderer objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            centerPoint = objectRenderer.bounds.center;
        }
        else
        {
            Debug.LogWarning("No Renderer found on this object! Rotation may not work as intended.");
            centerPoint = transform.position;
        }
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);

        Renderer renderer = videoObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            videoMaterial = renderer.material;
        }

        if (videoMaterial != null)
        {
            Color color = videoMaterial.color;
            color.a = 0.0f;
            videoMaterial.color = color;
        }
    }
    void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        isBeingGrabbed = true;
        timer = 0f;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isBeingGrabbed = false;
        timer = 0f;
    }
    void TriggerAction()
    {
        SceneManager.LoadScene("RomanScene");
    }
    public void ActivateVideo()
    {
        videoObject.SetActive(true);

        VideoPlayer videoPlayer = videoObject.GetComponent<VideoPlayer>();
        if (videoPlayer != null)
        {
            Debug.Log("video start");
            videoPlayer.Play();
        }
        isFading = true;
        alpha = 0.0f;
    }
    void Update()
    {
        transform.RotateAround(centerPoint, Vector3.up, rotationSpeed * Time.deltaTime);
        if (isBeingGrabbed)
        {
            timer += Time.deltaTime;
            if (timer >= scanTime)
            {
                ActivateVideo();
                isBeingGrabbed = false;
                timer = 0f;
            }
        }
        if (isFading && videoMaterial != null)
        {
            alpha += Time.deltaTime / fadeDuration;
            if (alpha >= 1.0f)
            {
                alpha = 1.0f;
                isFading = false;
            }
            if (alpha == 1.0f)
            {
                TriggerAction();
            }
            Color color = videoMaterial.color;
            color.a = alpha;
            videoMaterial.color = color;
        }
    }
}
