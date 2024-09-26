using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestLock : MonoBehaviour
{
    [SerializeField] private Rigidbody ChestLid;
    [SerializeField] private Chest LockedChest;
    [SerializeField] private Rigidbody LockModel;

    private void OnTriggerEnter(Collider other)
    {
        CheckOpenLock(other);
    }

    /// <summary>
    /// Checks if you can open the lock on trigger, opens it if you can
    /// </summary>
    private void CheckOpenLock(Collider Other)
    {
        //If it isnt an item, it cant be a key
        if (Other.GetComponent<Item>() == null) return;

        //If it is a key and the chest hasnt been openend
        if (Other.GetComponent<Item>().ItemType == "Key" && !LockedChest.HasBeenOpened)
        {
            //Makes the lid and lock do gravity
            ChestLid.isKinematic = false;
            LockModel.isKinematic = false;

            //Makes the key unusable
            Other.GetComponent<Item>().ItemType = "UsedKey";

            //Sets it to the locks position, turns off its own gravity and changes its parent
            GameObject key = Other.gameObject;
            key.GetComponent<Rigidbody>().isKinematic = true;
            key.transform.parent = LockModel.transform;
            key.GetComponent<BoxCollider>().enabled = false;
            key.transform.localPosition = new Vector3(0, 0, 0);
            LockModel.transform.parent = null;
        }
    }
}