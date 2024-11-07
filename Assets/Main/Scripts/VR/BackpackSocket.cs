using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BackpackSocket : MonoBehaviour
{
    [SerializeField] private int backpackNonInteractLayer;
    [SerializeField] private float scaleFactor;
    
    private int interactableLayer = 0;
    private Vector3 interactableScale;

    public void Socket(SelectEnterEventArgs args)
    {
        GameObject interactableObject = args.interactableObject.transform.gameObject;

        interactableScale = interactableObject.transform.lossyScale;
        interactableLayer = interactableObject.layer;

        interactableObject.layer = backpackNonInteractLayer;
        //interactableObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }

    public void UnSocket(SelectExitEventArgs args)
    {
        GameObject interactableObject = args.interactableObject.transform.gameObject;

        interactableObject.layer = interactableLayer;
        interactableObject.transform.localScale = interactableScale;
    }
}
