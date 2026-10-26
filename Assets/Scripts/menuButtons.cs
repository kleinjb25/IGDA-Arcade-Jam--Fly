using UnityEngine;

public class menuButtons : MonoBehaviour
{
    public void startGame(GameObject buttonClicked)
    {
        if (gamemanager.instance == null)
        {
            Debug.LogError("no gamemanager? D:");
            return;
        }
        gamemanager.instance.loadMainGame(buttonClicked);
    }
}
