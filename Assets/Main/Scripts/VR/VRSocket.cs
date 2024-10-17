using UnityEngine;
using UnityEngine.Events;

public class VRSocket : MonoBehaviour
{
    [SerializeField] private SocketableType allowedSocketType;
    [SerializeField] private Transform socketPoint;
    [SerializeField] private float reSocketDelay = 1f;
    [SerializeField] private bool makeKinematic;
    [SerializeField] private float releaseDistance;
    [SerializeField] private RigidbodyConstraints contraintsr1;
    [SerializeField] private RigidbodyConstraints contraintsr2;
    [SerializeField] private RigidbodyConstraints contraintsr3;
    [SerializeField] private RigidbodyConstraints contraintsp1;
    [SerializeField] private RigidbodyConstraints contraintsp2;
    [SerializeField] private RigidbodyConstraints contraintsp3;

    [SerializeField] private UnityEvent<GameObject> OnSocket = new();
    [SerializeField] private UnityEvent<GameObject> OnUnsocket = new();

    public float ReleaseDistance => releaseDistance;

    private bool maySocket;
    private float reSocketTimer;

    public GameObject socketedObject;
    private Grabbable socketedGrabbable;

    private void OnTriggerEnter(Collider other)
    {
        if (!maySocket || socketedObject) return;

        if (other.TryGetComponent(out Socketable socket))
        {
            if (socket.GetSocketableType() == allowedSocketType)
            {
                SocketObject(other.gameObject);
            }
        }
    }

    private void Update()
    {
        if (!maySocket)
        {
            reSocketTimer -= Time.deltaTime;

            if (reSocketTimer <= 0)
            {
                maySocket = true;
                reSocketTimer = reSocketDelay;
            }
        }

        if(socketedObject)
        {
            //print(socketedObject.transform.eulerAngles);
        }
    }

    public void UnSocketObject(GameObject UnSocketObject)
    {
        if (!maySocket) return;

        OnUnsocket.Invoke(UnSocketObject);

        reSocketTimer = reSocketDelay;
        maySocket = false;

        if (socketedGrabbable.rb)
        {
            socketedGrabbable.rb.isKinematic = false;

            if (!socketedGrabbable.rb.isKinematic)
            {
                socketedGrabbable.rb.constraints = RigidbodyConstraints.None;
            }
        }

        socketedGrabbable.socket = null;
        socketedGrabbable = null;

        socketedObject = null;
    }

    private void SocketObject(GameObject _socketedObject)
    {
        //print(_socketedObject);
        OnSocket.Invoke(_socketedObject);

        //sockettimer to make sure the object doenst get immmediatly resocketed;
        reSocketTimer = reSocketDelay;
        maySocket = false;

        socketedObject = _socketedObject;
        socketedGrabbable = socketedObject.GetComponent<Grabbable>();
        //Release the socketable from the hand
        if (socketedGrabbable.hand)
        {
            socketedGrabbable.hand.ForceRelease();
        }

        //Set gravity of socketable off
        if (socketedGrabbable.rb)
        {
            socketedGrabbable.rb.velocity = Vector3.zero;
            socketedGrabbable.rb.angularVelocity = Vector3.zero;

            if (makeKinematic)
            {
                socketedGrabbable.rb.isKinematic = true;
            }
            else
            {
                //Rudo code here
                socketedGrabbable.rb.constraints = contraintsp1 | contraintsp2 | contraintsp3 | contraintsr1 | contraintsr2 | contraintsr3;
            }
        }
        else
        {
            Debug.Log("WARNING No rigidbody found, can't socket object", socketedObject);
        }

        socketedGrabbable.socket = this;

        if (socketPoint) socketedObject.transform.position = socketPoint.position;
        else socketedObject.transform.position = transform.position;

        socketedObject.transform.rotation = socketPoint.transform.rotation;
    }
}
