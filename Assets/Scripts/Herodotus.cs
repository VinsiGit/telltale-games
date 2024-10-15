using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

public class Herodotus : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnGrab(SelectEnterEventArgs args)
    {
        // Print message when object is grabbed
        Debug.Log($"{gameObject.name} has been grabbed!");

        // Freeze the object's position and rotation
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        }
    }

    void OnRelease(SelectExitEventArgs args)
    {
        // Print message when object is released
        Debug.Log($"{gameObject.name} has been released!");

        // Unfreeze the object's position and rotation (optional if you want it to move after releasing)
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;  // Allow full movement and rotation
        }
    }
}
