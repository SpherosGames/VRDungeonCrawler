using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string ItemType;
    public virtual void UseItem()
    {

    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 4 && gameObject.GetComponent<TrailRenderer>() != null)
        {
            gameObject.GetComponent<TrailRenderer>().emitting = false;
            StartCoroutine(DeleteTrail(gameObject.GetComponent<TrailRenderer>()));
        }
    }

    private IEnumerator DeleteTrail(TrailRenderer Trail)
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(Trail);
    }
}
