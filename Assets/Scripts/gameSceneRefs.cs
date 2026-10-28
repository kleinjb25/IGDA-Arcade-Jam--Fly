using UnityEngine;
using TMPro;

public class gameSceneRefs : MonoBehaviour
{
    public TextMeshProUGUI ttogText;
    public TextMeshProUGUI timerP1Text, timerP2Text;
    public TextMeshProUGUI timerP1SubheaderText, timerP2SubheaderText; //October 15 - Added by Patrick as part of a UI rework.
    public TextMeshProUGUI problemsLeftP1, problemsLeftP2;
    public TextMeshProUGUI results;
    public GameObject player2UI;
    public GameObject referencePlane;
    public playerCursor player2Cursor;
    public planeProperties[] planes;
    public AudioSource audioSourceCamera;
    public AudioClip VictoryFanfare;
    public AudioClip FailureFanfare;
    public AudioClip stageClear;
}
