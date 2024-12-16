using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;
using UnityEngine;

public class turning : MonoBehaviour
{
    public float scanTime = 1.0f;
    public float rotationSpeed = 100f;
    private Vector3 centerPoint;
    private float timer = 0f;
    private bool isScanning = false;
    private bool isBeingGrabbed = false;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private void Start()
    {
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
    }
    void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        isBeingGrabbed = true;
        Debug.Log("grab");
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
    void Update()
    {
        transform.RotateAround(centerPoint, Vector3.up, rotationSpeed * Time.deltaTime);
        if (isBeingGrabbed)
        {
            timer += Time.deltaTime;
            if (timer >= scanTime)
            {
                TriggerAction();
                isBeingGrabbed = false;
                timer = 0f;
            }
        }
    }
}
