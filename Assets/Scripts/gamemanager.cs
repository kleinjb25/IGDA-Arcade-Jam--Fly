using UnityEngine;

public class gamemanager : MonoBehaviour
{

    public static gamemanager instance { get; private set; }
    public float roundTimer = 30f, animLength = 5f, buffer = 15f;
    public byte roundNumber = 1, totalRounds = 5;

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
        
    }
}
