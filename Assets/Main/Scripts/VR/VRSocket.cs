using UnityEngine;
using UnityEngine.Events;

public class VRSocket : MonoBehaviour
{
    [SerializeField] private SocketableType AllowedSocketType;
    [SerializeField] private Transform SocketPoint;
    [SerializeField] private float ReSocketDelay = 1f;
    [SerializeField] private bool MakeKinematic;
    [SerializeField] private float ReleaseDistance;
    [SerializeField] private RigidbodyConstraints Contraintsr1;
    [SerializeField] private RigidbodyConstraints Contraintsr2;
    [SerializeField] private RigidbodyConstraints Contraintsr3;
    [SerializeField] private RigidbodyConstraints Contraintsp1;
    [SerializeField] private RigidbodyConstraints Contraintsp2;
    [SerializeField] private RigidbodyConstraints Contraintsp3;

    [SerializeField] private UnityEvent<GameObject> OnSocket = new();
    [SerializeField] private UnityEvent<GameObject> OnUnsocket = new();

    public float GetReleaseDistance => ReleaseDistance;

    private bool MaySocket;
    private float ReSocketTimer;

    public GameObject SocketedObject;
    private Grabbable SocketedGrabbable;

    private void OnTriggerEnter(Collider Other)
    {
        if (!MaySocket || SocketedObject) return;

        //Socket when the object hits the socket's trigger collider.
        if (Other.TryGetComponent(out Socketable Socket))
        {
            if (Socket.GetSocketableType() == AllowedSocketType)
            {
                SocketObject(Other.gameObject);
            }
        }
    }

    private void Update()
    {
        //Reallow socketing when its been "ReSocketDelay" seconds.
        if (!MaySocket)
        {
            ReSocketTimer -= Time.deltaTime;

            if (ReSocketTimer <= 0)
            {
                MaySocket = true;
                ReSocketTimer = ReSocketDelay;
            }
        }
    }

    /// <summary>
    /// If there is a socketable in this socket, it will unsocket it.
    /// </summary>
    /// <param name="UnSocketObject">The object it will unsocket.</param>
    public void UnSocketObject(GameObject UnSocketObject)
    {
        if (!MaySocket) return;

        OnUnsocket.Invoke(UnSocketObject);

        //Add a delay for resocketing, so that it doesn't immediatly resocket it.
        ReSocketTimer = ReSocketDelay;
        MaySocket = false;

        //Change the rigidbody so that it can be moved again.
        if (SocketedGrabbable.Rb)
        {
            SocketedGrabbable.Rb.isKinematic = false;

            if (!SocketedGrabbable.Rb.isKinematic)
            {
                SocketedGrabbable.Rb.constraints = RigidbodyConstraints.None;
            }
        }

        //Reset variables for later use.
        SocketedGrabbable.Socket = null;
        SocketedGrabbable = null;

        SocketedObject = null;
    }

    private void SocketObject(GameObject ObjectToSocket)
    {
        OnSocket.Invoke(ObjectToSocket);

        //Add a delay for unsocketing, so that it doesn't immediatly unsocket it.
        ReSocketTimer = ReSocketDelay;
        MaySocket = false;

        SocketedObject = ObjectToSocket;
        SocketedGrabbable = SocketedObject.GetComponent<Grabbable>();

        //Turn gravity of socketable off.
        if (SocketedGrabbable.Rb)
        {
            SocketedGrabbable.Rb.velocity = Vector3.zero;
            SocketedGrabbable.Rb.angularVelocity = Vector3.zero;

            //Use MakeKinematic if it doesn't need to move. Use constraints if you want to still be able to move it, but only on certain axis.
            if (MakeKinematic)
            {
                SocketedGrabbable.Rb.isKinematic = true;
            }
            else
            {
                SocketedGrabbable.Rb.constraints = Contraintsp1 | Contraintsp2 | Contraintsp3 | Contraintsr1 | Contraintsr2 | Contraintsr3;
            }
        }
        else
        {
            Debug.Log("WARNING: No rigidbody found, can't socket object", SocketedObject);
        }

        SocketedGrabbable.Socket = this;

        //Set position and rotation of the SocketedObject.
        if (SocketPoint) SocketedObject.transform.position = SocketPoint.position;
        else SocketedObject.transform.position = transform.position;

        SocketedObject.transform.rotation = SocketPoint.transform.rotation;
    }
}
