using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ChestLock : MonoBehaviour
{
    [SerializeField] private Rigidbody ChestLid;
    [SerializeField] private Chest LockedChest;
    [SerializeField] private XRSocketInteractor SocketInteractor;

    public bool BeenUnlocked = false;

    /// <summary>
    /// Checks if you can open the lock on trigger, opens it if you can
    /// </summary>
    public void CheckOpenLock(SelectEnterEventArgs Args)
    {
        //If it isnt an item, it cant be a key.
        if (Args.interactableObject.transform.GetComponent<Item>() == null) return;

        //If it is a key and the chest hasn't been openend.
        if (Args.interactableObject.transform.GetComponent<Item>().ItemType == "Key" && !LockedChest.HasBeenOpened && !BeenUnlocked)
        {
            //Sets the chests hinge limits so that it can be opened by the player.
            JointLimits Limits = ChestLid.GetComponent<HingeJoint>().limits;
            Limits.min = -135;
            ChestLid.GetComponent<HingeJoint>().limits = Limits;

            //Makes the key unusable.
            Args.interactableObject.transform.GetComponent<Item>().ItemType = "UsedKey";

            //Set the keys position.
            Args.interactableObject.transform.SetPositionAndRotation(SocketInteractor.attachTransform.position, SocketInteractor.attachTransform.rotation);
            Args.interactableObject.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            SocketInteractor.socketActive = false;

            BeenUnlocked = true;
        }
    }
}
