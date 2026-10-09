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

    private void Awake()
    {
        spotCollider = GetComponent<Collider>();
        resetSpot();
    }

    public void resetSpot()
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

    public void SetInteractable(bool interactable)
    {
        if (spotCollider != null) spotCollider.enabled = interactable && !revealed;
        if (greenCircle != null) greenCircle.SetActive(interactable && !revealed);
    }

    public bool select(int playerNum)
    {
        if (!gamemanager.instance.hasAttemptsLeft(playerNum)) return false;
        if (revealed) return false;
        if (gamemanager.instance == null || !gamemanager.instance.roundActive) return false;
        if (playerNum == 1)
        {
            if (pSelectedByP1) return false;
            pSelectedByP1 = true;
            if (p1Icon != null) p1Icon.SetActive(true);
        }
        else if (playerNum == 2)
        {
            if (pSelectedByP2) return false;
            pSelectedByP2 = true;
            if (p2Icon != null) p2Icon.SetActive(true);
        }
        else return false;
        return true;
    }

    public void revealResult()
    {
        revealed = true;
        if (p1Icon != null) p1Icon.SetActive(false);
        if (p2Icon != null) p2Icon.SetActive(false);
        if (spotCollider != null) spotCollider.enabled = false;
        if (isFaulty && wasSelected)
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
