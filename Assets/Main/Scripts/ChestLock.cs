using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestLock : MonoBehaviour
{
    [SerializeField] private Rigidbody ChestLid;
    [SerializeField] private Chest LockedChest;
    [SerializeField] private Rigidbody LockModel;
    private bool BeenUnlocked = false;

    //private void OnTriggerEnter(Collider other)
    //{
    //    CheckOpenLock(other);
    //}

    /// <summary>
    /// Checks if you can open the lock on trigger, opens it if you can
    /// </summary>
    public void CheckOpenLock(GameObject Other)
    {
        //If it isnt an item, it cant be a key
        if (Other.GetComponent<Item>() == null) return;

        //If it is a key and the chest hasnt been openend
        if (Other.GetComponent<Item>().ItemType == "Key" && !LockedChest.HasBeenOpened && !BeenUnlocked)
        {
            //Sets the chests hinge limits higher
            JointLimits limits = ChestLid.GetComponent<HingeJoint>().limits;
            limits.min = -135;
            ChestLid.GetComponent<HingeJoint>().limits = limits;

            //Makes the key unusable
            Other.GetComponent<Item>().ItemType = "UsedKey";

            //BeenUnlocked = true;
        }
    }
}
