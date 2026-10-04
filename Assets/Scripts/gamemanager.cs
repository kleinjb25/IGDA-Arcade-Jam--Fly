using System.Collections;
using System.Threading;
using UnityEngine;

public class gamemanager : MonoBehaviour
{

    public static gamemanager instance { get; private set; }
    public float roundTimer = 10f, animLength = 5f, buffer = 5f;
    public byte roundNumber = 1;
    public const byte totalRounds = 5;
    public planeMovementPath pmp;
    public bool roundActive;
    public float timeRemaining;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (pmp == null)
        {
            Debug.LogError("no plane reference? :(");
            enabled = false;
            return;
        }
        StartCoroutine(GameLoop());
    }

    // wait for 1/3 buffer while playing plane animation and 3 2 1 go, then start round. after roundtimer is done, start buffer timer and play animation of plane flying off (and either crashing and exploding, or flying 'into the sunset'). repeat totalRounds times, incrementing roundNumber each time.
    private IEnumerator GameLoop()
    {
        for (roundNumber = 1; roundNumber < totalRounds; roundNumber++)
        {
            Debug.Log("starting round " + roundNumber);
            pmp.resetPlane();
            yield return pmp.arrive();
            yield return ttog();
            yield return inspection();
            roundActive = false;
            Debug.Log("round" + roundNumber + "done");
            yield return new WaitForSeconds(buffer);
            yield return pmp.depart();
            yield return new WaitForSeconds(buffer);
        }
    }

    private IEnumerator ttog()
    {
        for (int i = 3; i > 0; i--)
        {
            Debug.Log(i);
            yield return new WaitForSeconds(1f);
        }
        Debug.Log("GO!");
    }

    private IEnumerator inspection()
    {
        timeRemaining = roundTimer;
        roundActive = true;
        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            timeRemaining = Mathf.Max(0, timeRemaining);
            yield return null;
        }
        roundActive = false;
    }
}
