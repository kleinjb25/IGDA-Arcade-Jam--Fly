using System.Collections;
using UnityEngine;
public class planeMovementPath : MonoBehaviour
{

    public Vector3 startPosition = new Vector3(-20, 5, 0);
    public Vector3 inspectionPosition = new Vector3(-.5f, 1.25f, 0);
    public Vector3 endPosition = new Vector3(20, 5, 0);
    public Vector3 arrivalControlPoint = new Vector3(-8, 8, 0);
    public Vector3 departureControlPoint = new Vector3(8, 0, 0);
    public Vector3 sieRot = new Vector3(0, 0, -90); // starting and ending rotation are the same at all points in animation, but not in the middle
    public Vector3 arrRot = new Vector3(0, 0, -115);
    public Vector3 departRot = new Vector3(0, 0, -45);
    public float animDur = 3f;

    public AudioSource audioSource;
    public AudioClip ArrivalFile;
    public float ArrivalTimestamp;
    public AudioClip DepartureFile;
    public float DepartureTimestamp;

    private Quaternion FToR(Vector3 rot)
    {
        return Quaternion.Euler(rot);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Awake is called even if the script component is disabled, right when the object is initialized
    void Awake()
    {
        resetPlane();
    }

    // also called in gamemanager before each round, so that's why this method exists
    public void resetPlane()
    {
        transform.position = startPosition;
        transform.rotation = FToR(sieRot);
    }

    public IEnumerator arrive()
    {
        audioSource.clip = ArrivalFile;
        audioSource.time = ArrivalTimestamp;
        audioSource.Play();
        yield return animatePath(startPosition, arrivalControlPoint, inspectionPosition, FToR(sieRot), FToR(arrRot), FToR(sieRot), animDur);
    }

    public IEnumerator depart()
    {
        audioSource.clip = DepartureFile;
        audioSource.time = DepartureTimestamp;
        audioSource.Play();
        yield return animatePath(inspectionPosition, departureControlPoint, endPosition, FToR(sieRot), FToR(departRot), FToR(sieRot), animDur);
    }

    private IEnumerator animatePath(Vector3 start, Vector3 ctrlPt, Vector3 end, Quaternion startRot, Quaternion midRot, Quaternion endRot, float duration)
    {
        float elapsed = 0f;
        transform.position = start;
        transform.rotation = startRot;
        audioSource.pitch = 1.1f;
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
        audioSource.pitch = 1;
    }
}
