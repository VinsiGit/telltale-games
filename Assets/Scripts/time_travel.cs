using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;

public class time_travel : MonoBehaviour
{
    public float scanTime = 3.0f;
    private float timer = 0f;
    private bool isScanning = false;
    private bool isBeingGrabbed = false;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("started");
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }
    void OnDestroy()
    {
        // Unregister grab events to avoid memory leaks
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        isBeingGrabbed = true;
        Debug.Log("grab");
        timer = 0f; // Reset the timer when grabbed
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isBeingGrabbed = false;
        timer = 0f; // Reset the timer when released
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (other.CompareTag("Controller"))
        {
            isScanning = true;
            timer = 0f;
            Debug.Log(isScanning);
        }
    }
    void OnTriggerStay(Collider other)
    {
        Debug.Log(other);
        if (isScanning && other.CompareTag("Controller"))
        {
            timer += Time.deltaTime;
            if (timer >= scanTime)
            {
                TriggerAction();
                isScanning = false;
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Controller"))
        {
            isScanning = false;
            timer = 0f;
        }
    }
    void TriggerAction()
    {
        Debug.Log("Scan Complete!");
        SceneManager.LoadScene("RomanScene");
    }
    // Update is called once per frame
    void Update()
    {
        if (isBeingGrabbed)
        {
            timer += Time.deltaTime;
            if (timer >= scanTime)
            {
                TriggerAction();
                isBeingGrabbed = false; // Prevent re-triggering
                timer = 0f; // Reset the timer
            }
        }
    }
}
