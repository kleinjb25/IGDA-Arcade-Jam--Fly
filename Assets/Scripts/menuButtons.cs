using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class menuButtons : MonoBehaviour
{
    public TextMeshProUGUI coopLabel;
    private gamemanager g;
    int mode;

    public void Start()
    {
        g = gamemanager.instance;
        if (g == null)
        {
            Debug.LogError("no gamemanager? D:");
            return;
        }
        mode = g.coop ? 2 : 1;
        coopLabel.text = g.coop ? $"{mode} Players" : $"{mode} Player";
    }
    public void Update()
    {
        if (g == null || Keyboard.current == null) return;
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            g.coop = false;
            updateModeLabel();
        }

        if (Keyboard.current.f11Key.wasPressedThisFrame)
        {
            g.coop = true;
            updateModeLabel();
        }

        if (Keyboard.current.f1Key.wasPressedThisFrame || Keyboard.current.f6Key.wasPressedThisFrame || Keyboard.current.f7Key.wasPressedThisFrame || Keyboard.current.f12Key.wasPressedThisFrame)
        {
            startGame();
        }
    }

    private void updateModeLabel()
    {
        mode = g.coop ? 2 : 1;
        coopLabel.text = g.coop ? $"{mode} Players" : $"{mode} Player";
        Debug.Log($"coop is {g.coop}");
    }

    public void startGame()
    {
        if (g != null) g.loadMainGame();
    }
}
