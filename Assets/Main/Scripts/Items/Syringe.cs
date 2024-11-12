using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Syringe : MonoBehaviour
{
    //Makes it easier to have syringe varients, especially when working with chests
    public SyringeScriptableobject Scriptable;

    private Rigidbody RigidB;

    private bool BeenUsed = false;

    private void Start()
    {
        RigidB = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Single use
        if (!BeenUsed)
        {
            //If its the "Body" layer, I failed to use a layer variable for this.
            if(collision.gameObject.layer == 12)
            {
                //If it is moving
                if(RigidB.velocity.magnitude > 0.3f)
                {
                    //Freeze the syringe
                    transform.parent = collision.transform;
                    RigidB.isKinematic = true;


                    StartCoroutine(UseSyringe());
                }
            }
        }
    }

    private IEnumerator UseSyringe()
    {
        //Uses the item
        BeenUsed = true;

        print("Used Syringe");

        //Wait for a bit
        yield return new WaitForSeconds(Scriptable.TimeToUse);

        //Drop syringe
        transform.parent = null;
        RigidB.isKinematic = false;

        //Heal
        FindObjectOfType<Player>().MaxHealth += Scriptable.InstantHealing;
    }
}
