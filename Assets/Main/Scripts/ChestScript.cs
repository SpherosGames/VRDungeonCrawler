using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : MonoBehaviour
{
    [SerializeField] private Transform SpawnSpot;

    [Header("Item chances")]
    [SerializeField] private ItemScriptableobject[] PossibleItems;
    [SerializeField] private int MaxPossibleItems;
    [SerializeField] private int MinPossibleItems;

    [SerializeField] private float TimeBetweenItems = 0.5f;

    //The force which the items comes out of the chest with
    [SerializeField] private int ItemLaunchUpAmount = 500;
    [SerializeField] private int ItemSpread = 100;

    [Header("Material")]
    [SerializeField] private Material DefaultTrailMat;

    [Header("Particles")]
    [SerializeField] private GameObject SpawnParticle;

    private bool hasBeenOpened = false;

    private void Update()
    {
        //Temporary chest opening key
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(OpenChest());
        }
        #endif
    }

    public IEnumerator OpenChest()
    {
        //Cant open multiple times
        if (!hasBeenOpened)
        {
            //Chooses the amount of items
            int TotalItems = Random.Range(MinPossibleItems, MaxPossibleItems + 1);

            //Gets the total weight of all items
            int TotValue = 0;
            for (int i = 0; i < PossibleItems.Length; i++)
            {
                TotValue += PossibleItems[i].Weight;
            }

            //Spawns the items, and then waits before spawning another
            for (int i = 0; i < TotalItems; i++)
            {
                SpawnItem(TotValue);
                yield return new WaitForSeconds(TimeBetweenItems);
            }

            //Cant open this chest again
            hasBeenOpened = true;
        }
    }
    public void SpawnItem(int TotalValue)
    {
        //Spawns a particle and destroys it after 5 seconds
        Destroy(Instantiate(SpawnParticle, SpawnSpot.position, SpawnSpot.rotation), 5);

        //Chooses the random "weight"/item
        int Value = Random.Range(1, TotalValue);

        int CurrentValue = 0;

        //Finds the item
        for (int j = 0; j < PossibleItems.Length; j++)
        {
            CurrentValue += PossibleItems[j].Weight;
            if (CurrentValue > Value)
            {
                //Spawns the item
                GameObject item = Instantiate(PossibleItems[j].Prefab, SpawnSpot.position, gameObject.transform.rotation);

                //Adds a rigidbody if it doesnt have one
                if (item.GetComponent<Rigidbody>() == null)
                {
                    item.AddComponent(typeof(Rigidbody));
                }
                Rigidbody RigidB = item.GetComponent<Rigidbody>();

                //Chooses its velocity and adds that force
                int x = Random.Range(-ItemSpread, ItemSpread);
                int y = Random.Range(-ItemSpread, ItemSpread);

                RigidB.AddForce(new Vector3(x, ItemLaunchUpAmount, y));

                //Adds a trail renderer
                if(item.GetComponent<TrailRenderer>() == null)
                {
                    item.AddComponent<TrailRenderer>();
                }
                TrailRenderer Trail = item.GetComponent<TrailRenderer>();
                Trail.enabled = true;

                //Sets the color to the one in the scriptable object
                Trail.colorGradient = PossibleItems[j].TrailGradient;

                //Extra settings to make the trail look better
                Trail.startWidth = 0.3f;
                Trail.endWidth = 0.3f;
                Trail.time = 0.5f;
                Trail.material = DefaultTrailMat;

                //Gives the item an item script if it doesnt have one so the trail gets destroyed once touching the floor
                if(item.GetComponent<Item>() == null)
                {
                    item.AddComponent<Item>();
                }

                //Finishes the for loop
                j = 100;
            }
        }
    }
}
