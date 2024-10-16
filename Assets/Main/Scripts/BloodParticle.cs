using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BloodParticle : MonoBehaviour
{
    [SerializeField] private DecalProjector projector;
    [SerializeField] private LayerMask environmentLayer;

    private IEnumerator Start()
    {
        Vector3 scale = projector.size;
        Vector3 pivot = projector.pivot;

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 2, environmentLayer))
        {
            //Debug.DrawRay(hit.point, Vector3.up, Color.red, 10);
            scale.z = hit.distance + 0.5f;
            pivot.z = scale.z / 2;
        }
        else
        {
            scale.z = 0;
            pivot.z = scale.z / 2;
        }

        projector.size = scale;
        projector.pivot = pivot;

        projector.fadeFactor = 0;

        yield return Fade(0, 1, 10);


        yield return new WaitForSeconds(0.1f);

        yield return Fade(1, 0, 0.1f);

        Destroy(gameObject);
    }

    private IEnumerator Fade(float from, float to, float speed)
    {
        // Set initial fadeFactor
        projector.fadeFactor = from;

        while (!Mathf.Approximately(projector.fadeFactor, to))
        {
            // Move fadeFactor towards target value (either fade in or fade out)
            projector.fadeFactor = Mathf.MoveTowards(projector.fadeFactor, to, speed * Time.deltaTime);

            // Print the current fading state
            //if (to == 0)
            //{
            //    print("fading in");
            //}
            //else if (to == 1)
            //{
            //    print("fading out");
            //}

            // Wait for the next frame
            yield return null;
        }
    }
}
