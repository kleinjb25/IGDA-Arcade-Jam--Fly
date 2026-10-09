using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditorInternal.ReorderableList;

public class gamemanager : MonoBehaviour
{
    public bool coop = true;
    public static gamemanager instance { get; private set; }
    public float roundTimer = 10f, animLength = 5f, buffer = 5f;
    public byte roundNumber = 1, faults;
    public const byte totalRounds = 3;
    public planeMovementPath pmp;
    public bool roundActive;
    public float timeRemaining;
    public int attemptsP1, attemptsP2;
    public int scoreP1, scoreP2;
    public TextMeshProUGUI ttogText;
    public TextMeshProUGUI timerP1Text, timerP2Text;
    public TextMeshProUGUI problemsLeftP1, problemsLeftP2;
    public TextMeshProUGUI results;
    public GameObject player2UI;
    public playerCursor player2Cursor;

    public problemSpot[] spots;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning("Duplicate GameManager found; destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (instance == this)
            instance = null;
    }

    public void loadMainGame(GameObject buttonClicked)
    {
        coop = buttonClicked.name == "p2";
        Debug.Log($"coop is {coop}");
        SceneManager.LoadScene("mainGame");
    }

    private void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        if (s.name == "mainGame") setupGameScene();
    }

    private void setupGameScene()
    {
        gameSceneRefs refs =
        FindAnyObjectByType<gameSceneRefs>();

        pmp = refs.pmp;
        ttogText = refs.ttogText;
        timerP1Text = refs.timerP1Text;
        timerP2Text = refs.timerP2Text;
        problemsLeftP1 = refs.problemsLeftP1;
        problemsLeftP2 = refs.problemsLeftP2;
        results = refs.results;
        player2UI = refs.player2UI;
        player2Cursor = refs.player2Cursor;
        spots = refs.spots;
        setupPlayers();

        StartCoroutine(GameLoop());
    }
    private void setupPlayers()
    {
        if (player2UI != null)
            player2UI.SetActive(coop);
        if (player2Cursor != null)
            player2Cursor.enabled = coop;
    }
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "mainGame")
        {
            ttogText.text = "";
            timerP1Text.text = "";
            timerP2Text.text = "";
            problemsLeftP1.text = "";
            problemsLeftP2.text = "";
            if (pmp == null)
            {
                Debug.LogError("no plane reference? :(");
                enabled = false;
                return;
            }
        }
        
    }

    // wait for 1/3 buffer while playing plane animation and 3 2 1 go, then start round. after roundtimer is done, start buffer timer and play animation of plane flying off (and either crashing and exploding, or flying 'into the sunset'). repeat totalRounds times, incrementing roundNumber each time.
    private IEnumerator GameLoop()
    {
        for (roundNumber = 1; roundNumber <= totalRounds; roundNumber++)
        {
            Debug.Log("starting round " + roundNumber);
            resetProblemSpots();
            setupRound();
            pmp.resetPlane();
            yield return pmp.arrive();
            yield return ttog();
            yield return inspection();
            roundActive = false;
            revealResults();
            Debug.Log("round" + roundNumber + "done");
            yield return new WaitForSeconds(buffer);
            results.gameObject.SetActive(false);
            yield return pmp.depart();
            yield return new WaitForSeconds(buffer / 2);
        }
        results.text = $"Player 1's final score: {scoreP1} pts\n" + $"Player 2's final score: {scoreP2} pts";
        results.gameObject.SetActive(true);
        yield return new WaitForSeconds(buffer);
        results.gameObject.SetActive(false);
        SceneManager.LoadScene("mainMenu");
    }

    private void resetProblemSpots()
    {
        foreach (problemSpot p in spots)
        {
            if (p != null)
            {
                p.gameObject.SetActive(true);
                p.resetSpot();
            }
        }
    }

    private void setupRound()
    {
        faults = 0;
        foreach(problemSpot p in spots)
        {
            if (p != null && p.isFaulty) faults++;
        }
        attemptsP1 = faults;
        attemptsP2 = faults;
        updateAttemptsLeft();
    }

    private IEnumerator ttog()
    {
        ttogText.gameObject.SetActive(true);
        for (int i = 3; i > 0; i--)
        {
            ttogText.text = "Find what's wrong with the plane!\n" + i;
            yield return new WaitForSeconds(1);
        }
        ttogText.text = "Find what's wrong with the plane!\nGO!";
        yield return new WaitForSeconds(.75f);
        ttogText.text = "";
        ttogText.gameObject.SetActive(false);
    }

    private IEnumerator inspection()
    {
        timeRemaining = roundTimer;
        roundActive = true;
        enableProblemSpots();
        while (timeRemaining > 0)
        {
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
        bool p1Missed = false;
        bool p2Missed = false;
        foreach (problemSpot p in spots)
        {
            if (p != null) p.revealResult();
            if (p == null || !p.isFaulty) // skip to the next problem spot if it's empty or non-faulty, so the next two ifs would only run if it's faulty
                continue;
            if (!p.selectedByP1)
                p1Missed = true;
            if (coop && !p.selectedByP2)
                p2Missed = true;
        }
        if (!coop)
        {
            if (p1Missed) results.text = $"You missed a spot!!! D:\n" + $"Player 1's score: {scoreP1} pts";
            else results.text = $"Player 1's score: {scoreP1} pts";
        }
        else
        {
            if (p1Missed && p2Missed) results.text = $"Both players missed a spot!!! D:\n" + $"Player 1's score: {scoreP1} pts\n" + $"Player 2's score: {scoreP2} pts";
            else if (p1Missed) results.text = $"Player 1 missed a spot!!! D:\n" + $"Player 1's score: {scoreP1} pts\n" + $"Player 2's score: {scoreP2} pts";
            else if (p2Missed) results.text = $"Player 2 missed a spot!!! D:\n" + $"Player 1's score: {scoreP1} pts\n" + $"Player 2's score: {scoreP2} pts";
            else results.text = $"Player 1's score: {scoreP1} pts\n" + $"Player 2's score: {scoreP2} pts";
        }
        results.gameObject.SetActive(true);
    }

    public bool hasAttemptsLeft(int playerNum)
    {
        if (playerNum == 1) return attemptsP1 > 0;
        else if (playerNum == 2) return attemptsP2 > 0;
        else return false;
    }

    public void useAttempt(int playerNum, problemSpot spot)
    {
        if (playerNum == 1)
        {
            attemptsP1--;
            if (spot.isFaulty) scoreP1 += (int)(69*timeRemaining);
        } 
        else if (playerNum == 2)
        {
            attemptsP2--;
            if (spot.isFaulty) scoreP2 += (int)(69 * timeRemaining);
        }
        Debug.Log($"p{playerNum} selected {spot.gameObject.name}, {(playerNum == 1 ? attemptsP1 : attemptsP2)} attempts left");
        updateAttemptsLeft();
    }

    private void updateAttemptsLeft()
    {
        problemsLeftP1.text = $"{attemptsP1:F0} / {faults:F0}\nattempts left!";
        problemsLeftP2.text = $"{attemptsP2:F0} / {faults:F0}\nattempts left!";
    }
}
