using UnityEngine;
using UnityEngine.Events;

public class StandInArea : MonoBehaviour
{
    [Header("Customization Values")]

    [Tooltip("Put whatever you want to activate after the player stood in the area for a while")]
    [SerializeField] private UnityEvent OnEnoughTimeIn;
    [SerializeField] private Gradient ChangeColorOnTime;
    [SerializeField] private float TimeBeforeUse;

    [Tooltip("The cooldown between going back")]
    [SerializeField] private float Cooldown;

    [Header("Objects")]
    [SerializeField] private ParticleSystem Particles;

    //Changing values
    private float CurrentCooldown;
    private float CurrentTime;
    private bool WasActivated = false;
    private bool PlayerInArea = false;

    void Start()
    {
        //Sets the time to 0 and sets the particle color to color 0 in ChangeColorOnTime
        CurrentTime = 0;
        ChangeParticleColor();
    }

    void Update()
    {
        //If the player is near and the activation isnt on cooldown
        if(PlayerInArea && CurrentCooldown <= 0)
        {
            //Add time and change the particle to the time
            CurrentTime += Time.deltaTime;
            ChangeParticleColor();

            //If we at enough time: Activate EnoughTimeInArea();
            if(CurrentTime > TimeBeforeUse) { EnoughTimeInArea(); }
        }else
        {
            //If not on cooldown, reduce current time and change particle accordingly
            if (CurrentTime > -1 && CurrentCooldown <= 0)
            {
                CurrentTime -= (Time.deltaTime * 2);
                ChangeParticleColor();
            }

            //Reduce cooldown
            CurrentCooldown -= Time.deltaTime;
        }

        //If not on cooldown and its been active before
        if(WasActivated && CurrentCooldown <= 0 && CurrentTime <= -1)
        {
            //Check in area if the player is still there
            BoxCollider Box = GetComponent<BoxCollider>();
            Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, new Vector3(Box.size.x,Box.size.y,Box.size.z), Quaternion.identity);
            for(int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].gameObject.layer == 12)
                {
                    //It cant enter unless it wasnt activated, to avoid a spam bug
                    WasActivated = false;

                    //If yes, OnPlayerEnter.
                    OnPlayerEnter();
                }
            }

            //Check done, Dont check again unless active again
            WasActivated = false;
        }
    }

    private void ChangeParticleColor()
    {
        //Get current color
        Color CurrentColor = ChangeColorOnTime.Evaluate(CurrentTime / 5);

        //Get the main
        ParticleSystem.MainModule main = Particles.main;

        //Changes its color to current color
        main.startColor = CurrentColor;

        //Gets existing particles
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[Particles.particleCount];
        Particles.GetParticles(particles);

        //Sets all their colors
        for (int i = 0; i < particles.Length; i++)
        {
            //StartColor is the color it currently is, idk why its called "startcolor"
            particles[i].startColor = CurrentColor;
        }
        Particles.SetParticles(particles, particles.Length);
    }

    private void EnoughTimeInArea()
    {
        //Makes sure it checks for the player after cooldown
        WasActivated = true;

        //Does the unity event
        OnEnoughTimeIn.Invoke();

        //Resets player in there, otherwise it will spam use
        PlayerInArea = false;

        //Sets the cooldown
        CurrentCooldown = Cooldown;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Layer 12 is the body layer, So it checks for the player
        if (other.gameObject.layer == 12)
        {
            OnPlayerEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Layer 12 is the body layer, So it checks for the player
        if (other.gameObject.layer == 12)
        {
            OnPlayerExit();
        }
    }

    private void OnPlayerEnter()
    {
        //Add more if needed
        if(!WasActivated)
        {
            PlayerInArea = true;
        }
    }

    private void OnPlayerExit()
    {
        //Add more if needed
        PlayerInArea = false;
    }
}
