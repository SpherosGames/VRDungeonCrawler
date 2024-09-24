using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestLock : MonoBehaviour
{

    [SerializeField] private Rigidbody ChestLid;
    [SerializeField] private Chest LockedChest;
    [SerializeField] private Rigidbody LockModel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckOpenLock(other);
    }

    private void CheckOpenLock(Collider Other)
    {
        if (Other.GetComponent<Item>() == null) return;
        if (Other.GetComponent<Item>().ItemType == "Key" && !LockedChest.HasBeenOpened)
        {
            print("Opened chest");
            ChestLid.isKinematic = false;
            LockModel.isKinematic = false;
            Other.GetComponent<Item>().ItemType = "UsedKey";
        }
    }
}
