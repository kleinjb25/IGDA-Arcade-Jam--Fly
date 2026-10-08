using System.Collections;
using System.Threading;
using UnityEngine;
using TMPro;

public class gamemanager : MonoBehaviour
{

    public static gamemanager instance { get; private set; }
    public float roundTimer = 10f, animLength = 5f, buffer = 5f;
    public byte roundNumber = 1;
    public const byte totalRounds = 5;
    public planeMovementPath pmp;
    public bool roundActive;
    public float timeRemaining;

    public TextMeshProUGUI ttogText;
    public TextMeshProUGUI timerP1Text;
    public TextMeshProUGUI timerP2Text;

    public problemSpot[] spots;

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
    void Start()
    {
        ttogText.text = "";
        timerP1Text.text = "";
        timerP2Text.text = "";
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
        for (roundNumber = 1; roundNumber <= totalRounds; roundNumber++)
        {
            Debug.Log("starting round " + roundNumber);
            resetProblemSpots();
            pmp.resetPlane();
            yield return pmp.arrive();
            yield return ttog();
            yield return inspection();
            roundActive = false;
            revealResults();
            Debug.Log("round" + roundNumber + "done");
            // TODO: show current scores during buffer
            yield return new WaitForSeconds(buffer);
            yield return pmp.depart();
            yield return new WaitForSeconds(buffer);
        }
        // TODO: show final scores and go to main menu
    }

    private IEnumerator ttog()
    {
        for (int i = 3; i > 0; i--)
        {
            ttogText.text = "Find what's wrong with the plane!\n" + i;
            yield return new WaitForSeconds(1);
        }
        ttogText.text = "Find what's wrong with the plane!\nGO!";
        yield return new WaitForSeconds(.75f);
        ttogText.text = "";
    }

    private IEnumerator inspection()
    {
        timeRemaining = roundTimer;
        roundActive = true;
        enableProblemSpots();
        while (timeRemaining > 0)
        {
            //TODO: set up clicking on parts of the plane, checking if it's right, and scoring
            timeRemaining -= Time.deltaTime;
            timeRemaining = Mathf.Max(0, timeRemaining);
            timerP1Text.text = $"{timeRemaining:F0} seconds left!";
            timerP2Text.text = $"{timeRemaining:F0} seconds left!";
            yield return null;
        }
        timerP1Text.text = "";
        timerP2Text.text = "";
        roundActive = false;
    }

    private void enableProblemSpots()
    {
        foreach(problemSpot p in spots)
        {
            if (p != null) p.SetInteractable(true);
        }
    }

    private void revealResults()
    {
        foreach (problemSpot p in spots)
        {
            if (p != null) p.revealResult();
        }
    }

    private void resetProblemSpots()
    {
        foreach(problemSpot p in spots)
        {
            if (p != null)
            {
                p.gameObject.SetActive(true);
                p.resetSpot();
            }
        }
    }
}
