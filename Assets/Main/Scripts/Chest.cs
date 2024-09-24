using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private Transform SpawnSpot;

    [Header("Item chances")]
    [Tooltip("Make sure to have the array sorted from most common to rarest")]
    [SerializeField] private ItemScriptableobject[] PossibleItems;

    [Tooltip("Max = x, Min = y")]
    [SerializeField] private Vector2 MaxMinPossibleItems;

    [SerializeField] private float TimeBetweenItems = 0.5f;

    [SerializeField] private int ItemLaunchUpAmount = 500;
    [SerializeField] private int ItemSpread = 100;

    [Header("Material")]
    [SerializeField] private Material DefaultTrailMat;

    [Header("Particles")]
    [SerializeField] private GameObject SpawnParticle;

    private bool HasBeenOpened = false;

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

    /// <summary>
    /// Opens the chest, dropping all items in a few seconds.
    /// </summary>
    /// <returns></returns>
    public IEnumerator OpenChest()
    {
        //Cant open multiple times
        if (!HasBeenOpened)
        {
            //Chooses the amount of items
            int TotalItems = (int)Random.Range(MaxMinPossibleItems.y,MaxMinPossibleItems.x + 1);

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
            HasBeenOpened = true;
        }
    }

    /// <summary>
    /// Spawns an actual item
    /// </summary>
    public void SpawnItem(int TotalValue)
    {
        //Chooses the random "weight"/item
        int Value = Random.Range(1, TotalValue);

        int CurrentValue = 0;

        //Finds the item
        for (int j = 0; j < PossibleItems.Length; j++)
        {
            CurrentValue += PossibleItems[j].Weight;
            if (CurrentValue > Value)
            {
                //Spawns a particle
                GameObject parti = Instantiate(SpawnParticle, SpawnSpot.position, SpawnSpot.rotation);

                //Sets the burst particle count based on the rarity of the item.
                ParticleSystem.Burst burs = new ParticleSystem.Burst();
                burs.count = (j + 1) * (3 * (j + 1));
                parti.transform.GetChild(0).GetComponent<ParticleSystem>().emission.SetBurst(0,burs);

                //Destroys the particle after 5 seconds
                Destroy(parti, 5);

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
