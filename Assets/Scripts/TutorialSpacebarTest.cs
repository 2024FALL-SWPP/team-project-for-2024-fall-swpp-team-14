using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSpacebarTest : MonoBehaviour
{
    public RiveScreenTutorial riveScreenTutorial;
    private int narrationInt1;
    private int narrationInt2;
    // Start is called before the first frame update
    void Start()
    {
        narrationInt1 = riveScreenTutorial.narrationInt;
        Debug.Log("Initial Narration Int: " + narrationInt1);
        Invoke(nameof(playOnce), 1f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void playOnce()
    {
        //Spacebar triggger once
        riveScreenTutorial.OnSpacePressed();
        narrationInt2 = riveScreenTutorial.narrationInt;
        Debug.Log("Next Narration Int: " + narrationInt2);
        if (narrationInt1 + 1 == narrationInt2)
        {
            Debug.Log("Space bar trigger test 1 successful");
        }

    }
}
