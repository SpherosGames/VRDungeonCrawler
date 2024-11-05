using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ChestLock : MonoBehaviour
{
    [SerializeField] private Rigidbody ChestLid;
    [SerializeField] private Chest LockedChest;
    [SerializeField] private Rigidbody LockModel;
    [SerializeField] private XRSocketInteractor socketInteractor;
    [SerializeField] private InteractionLayerMask interactionLayerMask;
    public bool BeenUnlocked = false;

    /// <summary>
    /// Checks if you can open the lock on trigger, opens it if you can
    /// </summary>
    public void CheckOpenLock(SelectEnterEventArgs args)
    {
        //If it isnt an item, it cant be a key
        if (args.interactableObject.transform.gameObject.GetComponent<Item>() == null) return;

        //If it is a key and the chest hasnt been openend
        if (args.interactableObject.transform.gameObject.GetComponent<Item>().ItemType == "Key" && !LockedChest.HasBeenOpened && !BeenUnlocked)
        {
            //Sets the chests hinge limits higher
            JointLimits limits = ChestLid.GetComponent<HingeJoint>().limits;
            limits.min = -135;
            ChestLid.GetComponent<HingeJoint>().limits = limits;

            //Makes the key unusable
            args.interactableObject.transform.gameObject.GetComponent<Item>().ItemType = "UsedKey";

            args.interactableObject.transform.position = socketInteractor.attachTransform.position;
            args.interactableObject.transform.rotation = socketInteractor.attachTransform.rotation;
            args.interactableObject.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            socketInteractor.socketActive = false;

            BeenUnlocked = true;
        }
    }
}
