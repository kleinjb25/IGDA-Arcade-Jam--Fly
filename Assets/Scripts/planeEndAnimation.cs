using UnityEngine;
using System.Collections;

public class planeEndAnimation : MonoBehaviour
{
    public Camera animationCamera;
    public Vector3 animationCameraPosition = new Vector3(42f, 11.5f, -139f);
    public ParticleSystem smokePrefab;
    public Transform smokeSpawnPoint;
    public float smokeDelay = 0.2f;
    private Vector3 originalCameraPosition;

    public IEnumerator playAnimation(bool didCrash)
    {
        Camera c = animationCamera != null ? animationCamera : Camera.main;
        float smokeLifeTime = 0;

        if (c != null)
        {
            originalCameraPosition = c.transform.position;
            c.transform.position = animationCameraPosition;
        }
        // initial climb
        yield return movePhase(1.75f, new Vector3(-5, 0, 0), new Vector3(50, 6, 0));
        // continuing the climb
        yield return movePhase(1.25f, new Vector3(15, 0, 0), new Vector3(52, 5.5f, 0));

        if (didCrash)
        {
            // rotate to nose-dive
            yield return movePhase(0.25f, new Vector3(-15, 0, 700), new Vector3(20, -20, 0));
            
            float elapsed = 0;
            bool smokeSpawned = false;
            while (elapsed < 1)
            {
                elapsed += Time.deltaTime;
                // actual nose-dive movement
                transform.position += new Vector3(20, -100, 0) * Time.deltaTime;
                if (!smokeSpawned && elapsed >= smokeDelay) // if the smoke hasn't spawned in yet and it's been more than smokeDelay seconds, spawn it
                {
                    smokeLifeTime = spawnSmoke();
                    smokeSpawned = true;
                }
                yield return null;
            }
            if (!smokeSpawned) smokeLifeTime = spawnSmoke(); // in case smokeDelay exceeds animation length
        }
        else
        {
            // fly away safely
            yield return movePhase(1.75f, new Vector3(-7, 0, 0), new Vector3(55, 6.2f, 0));
        }
        yield return new WaitForSeconds(smokeLifeTime);
        if (c != null) c.transform.position = originalCameraPosition;

    }

    public IEnumerator movePhase(float duration, Vector3 rotSpeed, Vector3 moveSpeed)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.Rotate(rotSpeed * Time.deltaTime);
            transform.position += moveSpeed * Time.deltaTime;
            yield return null;
        }
    }

    private float spawnSmoke()
    {
        if (smokePrefab == null)
        {
            Debug.LogWarning("no smoke prefab? D:");
            return 0;
        }
        Vector3 spawnPos = smokeSpawnPoint != null ? smokeSpawnPoint.position : transform.position;
        ParticleSystem smoke = Instantiate(smokePrefab, spawnPos, smokePrefab.transform.rotation);
        smoke.Play();
        float lifetime = smoke.main.duration + smoke.main.startLifetime.constantMax;
        Destroy(smoke.gameObject, lifetime);
        return lifetime / 3;
    }
}
