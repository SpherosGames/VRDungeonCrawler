using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Syringe : MonoBehaviour
{
    //I dont think its worth it to make a scriptableobject for a small amount of items.
    [SerializeField] LayerMask PlayerLayer;

    public SyringeScriptableobject Scriptable;

    private Rigidbody RigidB;

    private bool BeenUsed = false;

    private void Start()
    {
        RigidB = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!BeenUsed)
        {
            if(collision.gameObject.layer == PlayerLayer || collision.gameObject.GetComponent<Player>())
            {
                if(RigidB.velocity.magnitude > 0.3f)
                {
                    transform.parent = collision.transform;
                    RigidB.isKinematic = true;
                    StartCoroutine(UseSyringe(collision.gameObject));
                }
            }
        }
    }

    private IEnumerator UseSyringe(GameObject Player)
    {
        yield return new WaitForSeconds(Scriptable.TimeToUse);
        transform.parent = null;
        RigidB.isKinematic = false;
        Player.GetComponent<Player>().TakeDamage(-Scriptable.InstantHealing);
        BeenUsed = true;
    }
}
