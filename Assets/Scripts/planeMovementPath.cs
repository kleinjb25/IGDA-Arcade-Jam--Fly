using UnityEngine;

public class planeMovementPath : MonoBehaviour
{

    public float startX = -20f, startY = 5f, stopXfirst = -0.5f, stopYfirst = 1.25f, stopXsecond = 20f, stopYsecond = 5f; // positions
    public float seRotZ = -90f, midRotZ1 = -115f, midRotZ2 = -45f; // starting and ending rotation are the same at all points in animation, but not in the middle

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position.Set(startX, startY, 0);
        transform.rotation.Set(0, 0, seRotZ, 0);
    }

    public void movePlane()
    {
        Debug.Log("plane is going to move");

    }
}
