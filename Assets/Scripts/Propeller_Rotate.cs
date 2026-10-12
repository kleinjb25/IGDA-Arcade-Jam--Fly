using UnityEngine;

public class Propeller_Rotate : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, 80 * Time.deltaTime); //Slowly rotates propeller sprite counter-clockwise (from front), to simulate fast movement.
        
    }
}
