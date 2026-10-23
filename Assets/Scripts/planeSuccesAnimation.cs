using UnityEngine;

public class planeSuccesAnimation : MonoBehaviour
{
    float clock1 = 1.75f;  //each "clock" represents a different phase of the animation. In this "success" animation, there are 3 phases total (2 built, one WIP)
    float clock2 = 1.25f;  //The second phase is slightly shorter than the first; this is partly because the plane spawns in offscreen, leaving time for a quick fade-in.
    float clock3 = 1.75f;  //This is where the "success" and "game over" animations differ. If you succeed, the plane continues flying uneventfully

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3(-77, 5, 0);  //Where the plane starts from at the beginning of the round. This code is redundant, because the plane is already
    }                                                 //placed in the world at this spot. But when we add the other planes, this could be useful.

    // Update is called once per frame
    void Update()
    {
        if (clock1 > 0)
        {
            Debug.Log("clock1 = " + clock1);
            transform.Rotate(-5f * Time.deltaTime, 0, 0);  //In the first phase, the plane slightly rotates to the left.
            transform.position = new Vector3(transform.position.x + 50 * Time.deltaTime, transform.position.y + 6 * Time.deltaTime, 0); //it also rises up somewhat, and quickly moves horizontally.
            clock1 = clock1 - Time.deltaTime;  //Tying everything to Time.deltaTime makes sure that the speeds will be consistent, regardless of how fast the computer running the game is.
        } 
        else if (clock2 > 0) //Only after clock1 has run its course does clock2 start.
        {
            Debug.Log("clock2 = " + clock2); //The second phase is very similar to the first. Just the movements themselves are different.
            transform.Rotate(15f * Time.deltaTime, 0, 0);  //In the second phase, the plane now overcorrects to the right. The minor rotations helping it look "floaty" as it flies
            transform.position = new Vector3(transform.position.x + 52 * Time.deltaTime, transform.position.y + 5.5f * Time.deltaTime, 0); //The plane is horizontally slightly faster than phase 1, but climbs slightly slower.
            clock2 = clock2 - Time.deltaTime;
        } 
        else if (clock3 > 0) //And clock3 starts last, after clock 2.
        {
            Debug.Log("clock3 = " + clock3); //The second phase is very similar to the first. Just the movements themselves are different.
            transform.Rotate(-7f * Time.deltaTime, 0, 0);  //Finally, the plane stabilizes out again, returning to a flatter rotation as it climbs offscreen
            transform.position = new Vector3(transform.position.x + 55 * Time.deltaTime, transform.position.y + 6.2f * Time.deltaTime, 0); //The plane keeps horizontally speeding up, while the rate of climb gets faster again.
            clock3 = clock3 - Time.deltaTime; //The plane should dissappear offscreen between "1.10" and "0.8." That's when the fade-out to return to gameplay should start. Once it's offscreen, it doesn't really matter what the plane itself does.
        }
    }
    }
