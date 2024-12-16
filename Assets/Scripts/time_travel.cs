using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;

public class time_travel : MonoBehaviour
{
    public GameObject rotatingObject;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    void Start()
    {
        Debug.Log("started");
        rotatingObject.SetActive(false);
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
        Debug.Log("grab");
        TriggerAction();
    }

    void OnRelease(SelectExitEventArgs args)
    {

    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (other.CompareTag("Controller"))
        {
            TriggerAction();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Controller"))
        {

        }
    }
    void TriggerAction()
    {
        Debug.Log("Scan Complete!");
        rotatingObject.SetActive(true);
    }
    void Update()
    {
    }
}
