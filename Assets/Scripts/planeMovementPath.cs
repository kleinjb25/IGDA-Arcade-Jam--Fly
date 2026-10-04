using System.Collections;
using UnityEngine;

public class planeMovementPath : MonoBehaviour
{

    public Vector3 startPosition = new Vector3(-20f, 5f, 0f);
    public Vector3 inspectionPosition = new Vector3(-.5f, 1.25f, 0f);
    public Vector3 endPosition = new Vector3(20f, 5f, 0f);
    public Vector3 arrivalControlPoint = new Vector3(-8f, 8f, 0f);
    public Vector3 departureControlPoint = new Vector3(8f, 0f, 0f);
    public float sieRotZ = -90f; // starting and ending rotation are the same at all points in animation, but not in the middle
    public float arrRotZ = -115f;
    public float departRotZ = -45f;
    public float animDur = 3f;

    private Quaternion FToR(float z)
    {
        return Quaternion.Euler(0f, 0f, z);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Awake is called even if the script component is disabled, right when the object is initialized
    void Awake()
    {
        resetPlane();
    }

    public void resetPlane()
    {
        transform.position = startPosition;
        transform.rotation = FToR(sieRotZ);
    }

    public IEnumerator arrive()
    {
        yield return animatePath(startPosition, arrivalControlPoint, inspectionPosition, FToR(sieRotZ), FToR(arrRotZ), FToR(sieRotZ), animDur);
    }

    public IEnumerator depart()
    {
        yield return animatePath(inspectionPosition, departureControlPoint, endPosition, FToR(sieRotZ), FToR(departRotZ), FToR(sieRotZ), animDur);
    }

    private IEnumerator animatePath(Vector3 start, Vector3 ctrlPt, Vector3 end, Quaternion startRot, Quaternion midRot, Quaternion endRot, float duration)
    {
        float elapsed = 0f;
        transform.position = start;
        transform.rotation = startRot;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            // set up bezier curve
            float inverseT = 1f - smoothT;
            transform.position = (Mathf.Pow(inverseT, 2) * start) + (2 * smoothT * inverseT * ctrlPt) + (Mathf.Pow(smoothT, 2) * end);

            if (smoothT < .5f)
            {
                transform.rotation = Quaternion.Slerp(startRot, midRot, smoothT * 2);
            } 
            else
            {
                transform.rotation = Quaternion.Slerp(midRot, endRot, (smoothT - .5f) * 2);
            }
            yield return null;
        }
        transform.position = end;
        transform.rotation = endRot;
    }

    public void movePlane()
    {
        Debug.Log("plane is going to move");

    }
}
