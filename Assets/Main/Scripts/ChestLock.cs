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

            GameObject key = Other.gameObject;
            key.GetComponent<Rigidbody>().isKinematic = true;
            key.transform.parent = LockModel.transform;
            key.GetComponent<BoxCollider>().enabled = false;
            key.transform.localPosition = new Vector3(0, 0, 0);
            LockModel.transform.parent = null;
        }
    }
}
