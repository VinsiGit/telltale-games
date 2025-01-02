using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;

public class time_travel : MonoBehaviour
{
    public GameObject rotatingObject;
    public GameObject videoObject;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    void Start()
    {
        Debug.Log("started");
        rotatingObject.SetActive(false);
        videoObject.SetActive(false);
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
        TriggerAction();
    }

    void OnRelease(SelectExitEventArgs args)
    {

    }
    void TriggerAction()
    {
        rotatingObject.SetActive(true);
    }
    void Update()
    {
    }
}
