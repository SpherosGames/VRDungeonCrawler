using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BackpackSocket : MonoBehaviour
{
    [SerializeField] private int BackpackNonInteractLayer;
    [SerializeField] private float ScaleFactor;
    
    private int InteractableLayer = 0;
    private Vector3 InteractableScale;

    public void Socket(SelectEnterEventArgs args)
    {
        GameObject interactableObject = args.interactableObject.transform.gameObject;

        InteractableScale = interactableObject.transform.lossyScale;
        InteractableLayer = interactableObject.layer;

        //Scale the socket down when socketing.
        interactableObject.layer = BackpackNonInteractLayer;
        interactableObject.transform.localScale = new Vector3(ScaleFactor, ScaleFactor, ScaleFactor);
    }

    public void UnSocket(SelectExitEventArgs args)
    {
        GameObject interactableObject = args.interactableObject.transform.gameObject;

        //Reset scale back to saved scale.
        interactableObject.layer = InteractableLayer;
        interactableObject.transform.localScale = InteractableScale;
    }
}
