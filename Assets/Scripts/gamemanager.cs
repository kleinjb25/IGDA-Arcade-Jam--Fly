using UnityEngine;

public class gamemanager : MonoBehaviour
{

    public static gamemanager instance { get; private set; }
    public float roundTimer = 30f, animLength = 5f, buffer = 15f;
    public byte roundNumber = 1;
    public const byte totalRounds = 5;
    public planeMovementPath pmp;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persists across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // wait for 1/3 buffer while playing plane animation and 3 2 1 go, then start round. after roundtimer is done, start buffer timer and play animation of plane flying off (and either crashing and exploding, or flying 'into the sunset'. repeat totalRounds times, incrementing roundNumber each time.

    }
}
