using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;

[RequireComponent(typeof(NavMeshSurface))]
public class NavmeshGenerator : MonoBehaviour
{
    private NavMeshSurface NavMeshSurface;

    private void Awake()
    {
        NavMeshSurface = GetComponent<NavMeshSurface>();
    }

    //Wait for the dungeon to generate and then build the navmesh.
    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        NavMeshSurface.BuildNavMesh();
    }
}
