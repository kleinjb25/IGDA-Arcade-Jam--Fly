using UnityEngine;

public class planeCrashAnimation : MonoBehaviour
{
    float clock1 = 1.75f;  //each "clock" represents a different phase of the animation. In this "game over" animation, there are 4 phases total (3 built, one WIP)
    float clock2 = 1.25f;  //The second phase is slightly shorter than the first; this is partly because the plane spawns in offscreen, leaving time for a quick fade-in.
    float clock3 = 0.125f;  //This is where the "success" and "game over" animations differ. If you fail, the plane rapidly rotates to face downward...
    float clock4 = 1f;  //...and speeds into the ground.

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
        else if (clock3 > 0) //And clock3 starts after clock 2, and such...
        {
            Debug.Log("clock3 = " + clock3); //The second phase is very similar to the first. Just the movements themselves are different.
            transform.Rotate(-15f * Time.deltaTime, 0, 700f * Time.deltaTime);  //In a fraction of a second, the plane suddenly snaps into facing the floor head-on
            transform.position = new Vector3(transform.position.x + 20 * Time.deltaTime, transform.position.y + -20 * Time.deltaTime, 0); //The plane doesn't completely stop horizontally, and does start to fall. Really, this section is mostly about the rotation, though
            clock3 = clock3 - Time.deltaTime; //This ultra-short phase should last a single-digit number of frames.
        }
        else if (clock4 > 0) //Truth is, a clock4 probably wasn't needed. The plane continues plummiting through the floor and past it, after which it's just not visible and what it does doesn't matter.
        {                    //However, it didn't feel right to have it continue falling indefinately forever. Even if we're gonna fade out before it gets very far.
            Debug.Log("clock4 = " + clock4);
            transform.Rotate(0, 0, 0);  //It doesn't need to rotate anymore; it's already facing the ground, and going too fast for any slight tilting to be appreciable
            transform.position = new Vector3(transform.position.x + 20 * Time.deltaTime, transform.position.y + -100 * Time.deltaTime, 0); //The plane travels into (and through) the ground at high velocity.
            clock4 = clock4 - Time.deltaTime; //The plane should dissappear offscreen between "0.89" and "0.74." That's when the explosion effect should start; the fade-out to return to gameplay can wait half a second to a second afterward. Once it's offscreen, it doesn't really matter what the plane itself does.
        }
    }
    }
