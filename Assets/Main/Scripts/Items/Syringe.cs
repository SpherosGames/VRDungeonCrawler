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
        Material[] OldMats = gameObject.GetComponent<MeshRenderer>().materials;
        Material[] NewMats = new Material[2];
        NewMats[0] = OldMats[0];
        NewMats[1] = Scriptable.SyringeMaterial;
        gameObject.GetComponent<MeshRenderer>().materials = NewMats;
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
        Player Plr = FindObjectOfType<Player>();
        Plr.MaxHealth += Scriptable.MaxHpIncrease;
        Plr.TakeDamage(-Scriptable.InstantHealing);
        Plr.AddStrength(Scriptable.PermaDamageBoost, true);
    }
}
