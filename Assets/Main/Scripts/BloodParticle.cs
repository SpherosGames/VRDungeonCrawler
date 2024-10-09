using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BloodParticle : MonoBehaviour
{
    [SerializeField] private DecalProjector projector;

    private IEnumerator Start()
    {
        projector.fadeFactor = Mathf.MoveTowards(1, 0, 0.001f * Time.deltaTime);

        if (projector.fadeFactor < 1)
        {
            print("fading in");
            yield return null;
        }

        yield return new WaitForSeconds(1);
        print("fading out");

        projector.fadeFactor = Mathf.MoveTowards(0, 1, 0.001f * Time.deltaTime);
    }

    private void Update()
    {
        //timer -= Time.deltaTime;
    }
}
