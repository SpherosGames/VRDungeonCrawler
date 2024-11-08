using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class Bomb : MonoBehaviour {
    [SerializeField] private int TimeUntilExplosion;
    [SerializeField] private ParticleSystem ExplosionParticle;
    [SerializeField] private ParticleSystem SparksParticle;
    [SerializeField] private InputActionProperty leftActionProperty;
    [SerializeField] private InputActionProperty rightActionProperty;

    private bool HandState = false; // False is right and true is left
    private bool BombThrown = false;
    private bool Held = false;

    private IEnumerator ActivateBomb() {
        BombThrown = true;
        SparksParticle.Play();
        yield return new WaitForSeconds(TimeUntilExplosion);
        ExplosionParticle.Play();
        SparksParticle.Stop();
        yield return new WaitForSeconds(0.4f);
        Destroy(gameObject);
    }

    public void Select(SelectEnterEventArgs Args) {
        GameObject controller = Args.interactor.transform.gameObject;
        if (controller.CompareTag("LeftController")) {
            HandState = true;
        }

        if (controller.CompareTag("RightController")) {
            HandState = false;
        }

        Held = true;
    }

    public void DeSelect() {
        Held = false;
    }

    void Update() {
        if (Held != true) return;
        if (leftActionProperty.action.ReadValue<float>() > 0.5f && HandState == true) {
            if (BombThrown) return;
            StartCoroutine(ActivateBomb());
        }

        if (rightActionProperty.action.ReadValue<float>() > 0.5f && HandState == false) {
            if (BombThrown) return;
            StartCoroutine(ActivateBomb());
        }
    }
}
