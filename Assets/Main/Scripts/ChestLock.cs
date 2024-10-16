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

            //Makes the lock do gravity
            //Destroy(LockModel.GetComponent<HingeJoint>());
            LockModel.gameObject.layer = LayerMask.NameToLayer("Chest");

            //Makes the key unusable
            Other.GetComponent<Item>().ItemType = "UsedKey";

            //Sets it to the locks position, turns off its own gravity and changes its parent
            //GameObject key = Other.gameObject;
            //key.GetComponent<Rigidbody>().isKinematic = true;
            //key.transform.parent = LockModel.transform;
            //key.GetComponent<BoxCollider>().enabled = false;
            //key.transform.localPosition = new Vector3(0, 0, -1);
            //key.transform.localEulerAngles = new Vector3(0, 90, 0);
            //LockModel.transform.parent = null;
            BeenUnlocked = true;
        }
    }
}
