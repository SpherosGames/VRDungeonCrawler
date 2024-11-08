using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

[RequireComponent(typeof(NavMeshSurface))]
public class NavmeshGenerator : MonoBehaviour
{
    private NavMeshSurface navMeshSurface;

    private void Awake()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        navMeshSurface.BuildNavMesh();
    }
}
