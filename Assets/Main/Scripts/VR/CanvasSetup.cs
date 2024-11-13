using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(AimConstraint))]
public class CanvasSetup : MonoBehaviour
{
    private Canvas canvas;
    private AimConstraint aimConstraint;

    private Camera playerCamera;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        aimConstraint = GetComponent<AimConstraint>();
        playerCamera = FindObjectOfType<Camera>();
    }

    private void OnEnable()
    {
        ConstraintSource constraintSource = new()
        {
            sourceTransform = playerCamera.transform,
            weight = 1
        };

        aimConstraint.AddSource(constraintSource);

        canvas.worldCamera = playerCamera;
    }
}
