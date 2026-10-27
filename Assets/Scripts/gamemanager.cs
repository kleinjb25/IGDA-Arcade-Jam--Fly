using NUnit.Framework.Constraints;
using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditorInternal.ReorderableList;

public class gamemanager : MonoBehaviour
{
    public bool coop;   //Is 1p (F) or 2p (T)?
    public static gamemanager instance { get; private set; } 
    public float roundTimer = 10f, animLength = 5f, buffer = 5f;  //timing of game elements
    public byte roundNumber = 1, faults;
    public byte totalRounds;   //length of planes array
    public bool roundActive;
    public float timeRemaining;
    public int attemptsP1, attemptsP2;
    public int scoreP1, scoreP2;
    public TextMeshProUGUI ttogText;
    public TextMeshProUGUI timerP1Text, timerP2Text;
    public TextMeshProUGUI timerP1SubheaderText, timerP2SubheaderText; //October 15 - Added by Patrick as part of a UI rework.
    public TextMeshProUGUI problemsLeftP1, problemsLeftP2;
    public TextMeshProUGUI results;
    public GameObject player2UI;
    public playerCursor player2Cursor;
    public planeProperties[] planes;
    private planeProperties currentPlane;
    public problemSpot[] spots;
    public GameObject referencePlane;
    public AudioSource audioSourceCamera;
    public AudioClip VictoryFanfare;
    public AudioClip FailureFanfare;

    private void Awake()
    {
        if (instance != null && instance != this)   //is there a duplicate game manager? (don't want this logic running twice)
        {
            Debug.LogWarning("destroying duplicate gamemanager.");
            Destroy(gameObject);
            return;
        }

        instance = this;                        //make sure *this* is the instance of gamemanager we're running
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;    //Overrides default Unity logic for loading scenes with OnSceneLoaded() below

        if (instance == this)
            instance = null;
    }

    public void loadMainGame() //Function that starts the game
    {       
        SceneManager.LoadScene("mainGame");
    }

    private void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        if (s.name == "mainGame") setupGameScene();
    }

    private void setupGameScene()
    {
        gameSceneRefs refs =                         //Lines 68-91 are about finding objects in the scene and establishing a reference/variable for them.
        FindAnyObjectByType<gameSceneRefs>();
        ttogText = refs.ttogText;
        timerP1Text = refs.timerP1Text;
        timerP2Text = refs.timerP2Text;
        timerP1SubheaderText = refs.timerP1SubheaderText;
        timerP2SubheaderText = refs.timerP2SubheaderText;
        problemsLeftP1 = refs.problemsLeftP1;
        problemsLeftP2 = refs.problemsLeftP2;
        results = refs.results;
        player2UI = refs.player2UI;
        player2Cursor = refs.player2Cursor;
        planes = refs.planes;
        referencePlane = refs.referencePlane;
        audioSourceCamera = refs.audioSourceCamera;
        VictoryFanfare = refs.VictoryFanfare;
        FailureFanfare = refs.FailureFanfare;
        foreach (planeProperties plane in planes)
        {
            if (plane != null) plane.gameObject.SetActive(false);
        }
        if (player2UI != null)
            player2UI.SetActive(coop);
        if (player2Cursor != null)
            player2Cursor.enabled = coop;

        totalRounds = (byte)planes.Length;
        StartCoroutine(GameLoop());           //starts main gameplay loop
    }
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "mainGame")       //resets and prepares the UI text
        {
            ttogText.text = "";
            timerP1Text.text = "";
            timerP2Text.text = "";
            problemsLeftP1.text = "";
            problemsLeftP2.text = "";
            if (planes.Length == 0)
            {
                Debug.LogError("no planes? :(");
                enabled = false;
                return;
            }
        }
        
    }

    // wait for 1/3 buffer while playing plane animation and 3 2 1 go, then start round. after roundtimer is done, start buffer timer and play animation of plane flying off (and either crashing and exploding, or flying 'into the sunset'). repeat totalRounds times, incrementing roundNumber each time.
    private IEnumerator GameLoop()
    {
        bool crashedThisRound = false;
        for (roundNumber = 1; roundNumber <= totalRounds; roundNumber++)
        {
            Debug.Log("starting round " + roundNumber);
            activatePlaneForRound();
            if (currentPlane == null) yield break;  //failsafe if something's null with the current plane
            resetProblemSpots();
            setupRound();
            currentPlane.pmp.resetPlane();
            yield return currentPlane.pmp.arrive();
            yield return ttog();
            yield return inspection();
            roundActive = false;
            crashedThisRound = revealResults();
            Debug.Log("round" + roundNumber + "done");
            yield return new WaitForSeconds(buffer / 2);
            results.gameObject.SetActive(false);
            yield return currentPlane.pmp.depart();
            planeEndAnimation endAnim = currentPlane.GetComponent<planeEndAnimation>();
            if (endAnim != null) yield return endAnim.playAnimation(crashedThisRound);
            else Debug.LogWarning($"no planeEndAnimation on {currentPlane.name}? D:");
            currentPlane.gameObject.SetActive(false);
            yield return new WaitForSeconds(1);
        }
        if (coop) results.text = $"Player 1's final score: {scoreP1} pts\n" + $"Player 2's final score: {scoreP2} pts";
        else results.text = $"Player 1's final score: {scoreP1} pts";
        audioSourceCamera.PlayOneShot(VictoryFanfare, 2f);
        results.gameObject.SetActive(true);
        yield return new WaitForSeconds(buffer);
        results.gameObject.SetActive(false);
        scoreP1 = 0;
        scoreP2 = 0;
        SceneManager.LoadScene("mainMenu");
    }

    private void activatePlaneForRound()     //picks the plane that's gonna spawn next and spawns it
    {
        foreach (planeProperties p in planes)
        {
            if (p != null) p.gameObject.SetActive(false);
        }
        System.Collections.Generic.List<planeProperties> availablePlanes = new System.Collections.Generic.List<planeProperties>();
        foreach (planeProperties p in planes)
        {
            if (p != null && p.pmp != null) availablePlanes.Add(p);
        }
        if (availablePlanes.Count == 0)
        {
            Debug.LogError("no planes? :(");
            return;
        }
        currentPlane = availablePlanes[roundNumber - 1];
        currentPlane.gameObject.SetActive(true);
        referencePlane.GetComponent<Image>().sprite = currentPlane.correctImage; //Updates the sprite image
        spots = currentPlane.spots;
        Debug.Log("selected plane: " + currentPlane.gameObject.name);
    }

    private void resetProblemSpots()
    {
        foreach (problemSpot ps in spots)
        {
            if (ps != null)
            {
                ps.gameObject.SetActive(true);
                ps.resetSpot();
            }
        }
    }

    private void setupRound()  //Randomly picks what's wrong with the plane
    {
        System.Collections.Generic.List<problemSpot> shuffledSpots = new System.Collections.Generic.List<problemSpot>();  //list of a plane's problem spots
        
        foreach(problemSpot ps in spots)
        {
            if (ps != null) shuffledSpots.Add(ps);
        }
        foreach (problemSpot ps2 in shuffledSpots) ps2.setFaulty(false);  //sets all problem spots to "fixed"
        if (shuffledSpots.Count == 0)
        {
            faults = 0;
            attemptsP1 = 0;
            attemptsP2 = 0;
            updateAttemptsLeft();
            Debug.LogWarning("no problemSpots on this plane? :(");
            return;
        }
        for (int i = 0; i < shuffledSpots.Count; i++)   //shuffles the order of problem spots so a random one can be picked below
        {
            int randInd = Random.Range(i, shuffledSpots.Count);
            (shuffledSpots[i], shuffledSpots[randInd]) = (shuffledSpots[randInd], shuffledSpots[i]);
        }
        int currMaxFaults = currentPlane != null ? currentPlane.maxFaults : 0; // null check
        int max = Mathf.Clamp(currMaxFaults, 1, shuffledSpots.Count);
        faults = (byte)Random.Range(1, max + 1);                                 //How many faults should there be for this round?
        for (int j = 0; j < faults; j++) shuffledSpots[j].setFaulty(true);       //Sets which faults should be the broken one
        attemptsP1 = faults;
        attemptsP2 = faults;
        updateAttemptsLeft();
    }
    
    private IEnumerator ttog()  // ttog = "Three Two One Go." Updates countdown UI text
    {  
        ttogText.gameObject.SetActive(true);
        for (int i = 3; i > 0; i--)
        {
            ttogText.text = "Find what's wrong\nwith the plane!\n" + i;
            yield return new WaitForSeconds(1);
        }
        ttogText.text = "Find what's wrong\nwith the plane!\nGO!";
        yield return new WaitForSeconds(.69f);
        ttogText.text = "";
        ttogText.gameObject.SetActive(false);
    }

    private IEnumerator inspection()  //updates the round timer
    {
        timeRemaining = roundTimer;
        roundActive = true;
        enableProblemSpots();
        while (timeRemaining > 0)
        {
            if (!hasAttemptsLeft(1))
            {
                if (!coop || !hasAttemptsLeft(2)) break;  //checks if each player has selections ("checks") left. Ends the round if they've all made them.
            }
            timeRemaining -= Time.deltaTime;
            timeRemaining = Mathf.Max(0, timeRemaining);
            timerP1Text.text = $"{timeRemaining:F2}";         // October 15 - Patrick removed the "Seconds left" text here, replacing it with a separate subheader box that just says "left."
            timerP2Text.text = $"{timeRemaining:F2}";
            timerP1SubheaderText.text = "seconds left!";
            timerP2SubheaderText.text = "seconds left!";
            yield return null;
        }
        timerP1Text.text = "";
        timerP2Text.text = "";
        timerP1SubheaderText.text = "";
        timerP2SubheaderText.text = "";
        roundActive = false;
    }

    private void enableProblemSpots()   //Makes it so every problem spot shows onscreen and is selectable
    {
        foreach(problemSpot p in spots)
        {
            if (p != null) p.SetInteractable(true);
        }
    }

    private bool revealResults()   //Logic that detects who missed what spots
    {
        bool p1Missed = false;
        bool p2Missed = false;
        bool shouldCrash = false;
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
        if (!coop)    //alters UI text depending on 1p or 2p
        {
            if (p1Missed) {
                results.text = $"You missed a spot!!! D:\n" + $"Player 1's score: {scoreP1} pts";
                shouldCrash = true;
        } 
            else results.text = $"Player 1's score: {scoreP1} pts";
        }
        else
        {
            if (p1Missed && p2Missed)
            {
                results.text = $"Both players missed a spot!!! D:\n" + $"Player 1's score: {scoreP1} pts\n" + $"Player 2's score: {scoreP2} pts";
                shouldCrash = true;
        }
            else if (p1Missed) results.text = $"Player 1 missed a spot!!! D:\n" + $"Player 1's score: {scoreP1} pts\n" + $"Player 2's score: {scoreP2} pts";
            else if (p2Missed) results.text = $"Player 2 missed a spot!!! D:\n" + $"Player 1's score: {scoreP1} pts\n" + $"Player 2's score: {scoreP2} pts";
            else results.text = $"Player 1's score: {scoreP1} pts\n" + $"Player 2's score: {scoreP2} pts";
        }
        results.gameObject.SetActive(true);
        if (shouldCrash) {                                                //plays failure fanfare if the plane's gonna crash.
            audioSourceCamera.PlayOneShot(FailureFanfare, 2f);
        }
    return shouldCrash;
    }

    public bool hasAttemptsLeft(int playerNum) //tracks if each player has any selections left
    {
        if (playerNum == 1) return attemptsP1 > 0;
        else if (playerNum == 2) return attemptsP2 > 0;
        else return false;
    }

    public void useAttempt(int playerNum, problemSpot spot) //Registers a selection, then gives score if it was a correct one
    {
        if (playerNum == 1)
        {
            attemptsP1--;
            if (spot.isFaulty) scoreP1 += (int)(69*timeRemaining); //No particular reason for 69 specifically, just FUNNI
        } 
        else if (playerNum == 2)
        {
            attemptsP2--;
            if (spot.isFaulty) scoreP2 += (int)(69 * timeRemaining);
        }
        Debug.Log($"p{playerNum} selected {spot.gameObject.name}, {(playerNum == 1 ? attemptsP1 : attemptsP2)} attempts left");
        updateAttemptsLeft();
    }

    private void updateAttemptsLeft() //updates UI
    {
        problemsLeftP1.text = $"{attemptsP1:F0} / {faults:F0}\n";       // October 15 - Patrick removes "attempts left" from these strings in an attempt to
        problemsLeftP2.text = $"{attemptsP2:F0} / {faults:F0}\n";       // streamline the UI. Shouldn't affect display of the number.
    }
}
