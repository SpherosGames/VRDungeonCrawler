using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Syringe : MonoBehaviour
{
    //To check if it touches the player
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
            print(collision.gameObject.layer);
            if(collision.gameObject.layer == 12)
            {
                if(RigidB.velocity.magnitude > 0.3f)
                {
                    transform.parent = collision.transform;
                    RigidB.isKinematic = true;
                    StartCoroutine(UseSyringe());
                }
            }
        }
    }

    private IEnumerator UseSyringe()
    {
        yield return new WaitForSeconds(Scriptable.TimeToUse);
        transform.parent = null;
        RigidB.isKinematic = false;
        FindObjectOfType<Player>().TakeDamage(-Scriptable.InstantHealing);
        BeenUsed = true;
    }
}
