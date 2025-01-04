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

    public Vector2 targetScaleXZ = new Vector2(1f, 1f);

    private Vector3 initialPosition;
    private Vector3 initialScale;
    private Vector3 targetPosition;
    private float fadeDuration = 3f;
    private float rotationSpeedMax = 100f;
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
        initialPosition = videoObject.transform.position;
        initialScale = videoObject.transform.localScale;

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            targetPosition = new Vector3(mainCamera.transform.position.x, initialPosition.y, mainCamera.transform.position.z);
        }
        else
        {
            Debug.LogError("Main Camera not found. Please ensure the Main Camera tag is set.");
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

        VideoPlayer videoPlayer = videoObject.GetComponentInChildren<VideoPlayer>();
        if (videoPlayer != null)
        {
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

            float rotationSpeed = Mathf.Lerp(0, rotationSpeedMax, alpha);
            videoObject.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

            Camera mainCamera = Camera.main;
            targetPosition = new Vector3(mainCamera.transform.position.x, initialPosition.y, mainCamera.transform.position.z);

            Vector3 currentPosition = videoObject.transform.position;
            videoObject.transform.position = new Vector3(
                Mathf.Lerp(initialPosition.x, targetPosition.x, alpha),
                currentPosition.y,
                Mathf.Lerp(initialPosition.z, targetPosition.z, alpha)
            );
            Vector3 currentScale = videoObject.transform.localScale;
            videoObject.transform.localScale = new Vector3(
                Mathf.Lerp(initialScale.x, targetScaleXZ.x, alpha),
                currentScale.y,
                Mathf.Lerp(initialScale.z, targetScaleXZ.y, alpha)
            );
        }
    }
}
