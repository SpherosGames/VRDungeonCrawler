using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class Bomb : MonoBehaviour {
    [SerializeField] private int TimeUntilExplosion;
    private bool HandState = false; // False is right and true is left

    void ActivateBomb() {

    }

    void Select(SelectEnterEventArgs Args) {
        GameObject controller = Args.interactor.transform.gameObject;
        //controller.name;
    }

    void Start() {

    }
}
