using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BloodParticle : MonoBehaviour
{
    [SerializeField] private DecalProjector Projector;
    [SerializeField] private LayerMask EnvironmentLayer;

    private IEnumerator Start()
    {
        Vector3 Scale = Projector.size;
        Vector3 Pivot = Projector.pivot;

        float maxDistance = 2;

        //Shoots a ray in the forward direction to check if it hits an object.
        //If it hit an object it adjusts the projection distance of the decal projector to reach the wall.
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit Hit, maxDistance, EnvironmentLayer))
        {
            //I add 0.5 because if the decal is shot from an angle it might not hit the wall entirerly.
            Scale.z = Hit.distance + 0.5f;
            Pivot.z = Scale.z / 2;
        }
        else
        {
            Scale.z = 0;
            Pivot.z = Scale.z / 2;
        }

        Projector.size = Scale;
        Projector.pivot = Pivot;

        Projector.fadeFactor = 0;

        //Quickly fade into view.
        yield return Fade(0, 1, 10);

        //Wait a bit.
        yield return new WaitForSeconds(0.1f);

        //Fade out of view.
        yield return Fade(1, 0, 0.1f);
        
        //Destroy object to improve performance as it will not be seen anymore.
        Destroy(gameObject);
    }

    private IEnumerator Fade(float from, float to, float speed)
    {
        Projector.fadeFactor = from;

        while (!Mathf.Approximately(Projector.fadeFactor, to))
        {
            // Move fadeFactor towards target value (either fade in or fade out).
            Projector.fadeFactor = Mathf.MoveTowards(Projector.fadeFactor, to, speed * Time.deltaTime);

            yield return null;
        }
    }
}
