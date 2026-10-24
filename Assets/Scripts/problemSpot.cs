using UnityEngine;

public class problemSpot : MonoBehaviour
{
    public bool isFaulty;
    public GameObject p1Icon, p2Icon, greenCircle, redCircle;
    private bool pSelectedByP1;
    private bool pSelectedByP2;
    private bool revealed;
    private Collider spotCollider;
    public bool selectedByP1 => pSelectedByP1;
    public bool selectedByP2 => pSelectedByP2; // these 2 lines only allow other scripts to read the state of the var, but don't show it in inspector
    public bool wasSelected => pSelectedByP1 || pSelectedByP2;

    public GameObject fixedPart;
    public GameObject brokenPart;
    public bool fixedWhenVisibleOnly;

    private void Awake()
    {
        spotCollider = GetComponent<Collider>(); //establishes which collider to use
        resetSpot();
    }

    public void resetSpot() //disables everything visual, and the collider
    {
        pSelectedByP1 = false;
        pSelectedByP2 = false;
        revealed = false;
        if (p1Icon != null) p1Icon.SetActive(false);
        if (p2Icon != null) p2Icon.SetActive(false);
        if (greenCircle != null) greenCircle.SetActive(false);
        if (redCircle != null) redCircle.SetActive(false);
        if (spotCollider != null) spotCollider.enabled = false;
    }

    public void SetInteractable(bool interactable) //enables the collider and shows the green circle once its time to start selecting things
    {
        if (spotCollider != null) spotCollider.enabled = interactable && !revealed;
        if (greenCircle != null) greenCircle.SetActive(interactable && !revealed);
    }

    public void setFaulty(bool faulty)  //Establishes this problem spot as the faulty one. Shows the broken version of the part (or just hides it if that's the case)
    {
        isFaulty = faulty;
        if (fixedPart != null) fixedPart.SetActive(fixedWhenVisibleOnly ? !faulty : !faulty);
        if (brokenPart != null) brokenPart.SetActive(faulty);
    }

    public bool select(int playerNum)
    {
        if (!gamemanager.instance.hasAttemptsLeft(playerNum)) return false;   //Lines 52-54 are safety checks to make sure you're not selecting when you shouldn't be
        if (revealed) return false;
        if (gamemanager.instance == null || !gamemanager.instance.roundActive) return false;
        if (playerNum == 1)
        {
            if (pSelectedByP1) return false;            //safety check
            pSelectedByP1 = true;                        //selects item
            if (p1Icon != null) p1Icon.SetActive(true);   //activates the "waypoints" that confirm selections
        }
        else if (playerNum == 2)
        {
            if (pSelectedByP2) return false;
            pSelectedByP2 = true;
            if (p2Icon != null) p2Icon.SetActive(true);
        }
        else return false;   //saftety checks
        return true;
    }

    public void revealResult() //updates UI as needed
    {
        revealed = true;
        if (p1Icon != null) p1Icon.SetActive(false);  //deactivates "waypoint" items
        if (p2Icon != null) p2Icon.SetActive(false);
        if (spotCollider != null) spotCollider.enabled = false;
        if (isFaulty && wasSelected)                                 //decides whether to show green circle (was selected correctly) or red circle (faulty spot was missed) or nothing (wasn't faulty at all)
        {
            if (greenCircle != null) greenCircle.SetActive(true);
            if (redCircle != null) redCircle.SetActive(false);
        }
        else if (isFaulty)
        {
            if (greenCircle != null) greenCircle.SetActive(false);
            if (redCircle != null) redCircle.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
