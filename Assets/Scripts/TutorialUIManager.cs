using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUIManager : MonoBehaviour
{
    public GameObject door1; //original (7.94, -0.71, 6.55), new (7.94, -0.71, 5.06)
    public GameObject door2;
    public GameObject door3;
    public GameObject robot1;
    public GameObject server;
    public GameObject laptop1;
    public GameObject laptop2;
    public RiveScreenTutorial riveScreenTutorial;
    private int narrationInt;
    private bool[] isDone = new bool[4] { false, false, false, false };
    private Vector3 door1Target = new Vector3(7.94f, -0.71f, 5);
    private Vector3 door2Target = new Vector3(6.8f, -0.71f, -2);
    private Vector3 door3Target = new Vector3(-2.8f, -0.71f, -5);

    // Start is called before the first frame update
    void Start()
    {
        robot1.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (door1 == null || door2 == null || door3 == null || riveScreenTutorial == null)
        {
            return;
        }
        if (robot1 == null)
        {
            riveScreenTutorial.triggerNarrationInt();
        }
        narrationInt = riveScreenTutorial.getNarrationInt();
        Debug.Log("Narration Int: " + narrationInt);

        switch (narrationInt)
        {
            case 5: //open door 1
                if (!isDone[0])
                {
                    door1.transform.position = Vector3.MoveTowards(door1.transform.position, door1Target, 2 * Time.deltaTime);
                    if (door1.transform.position == door1Target)
                    {
                        isDone[0] = true;
                    }
                }
                break;
            case 10: //make robot1
                robot1.SetActive(true);
                break;
            case 11: //open door 2
                if (!isDone[1])
                {
                    door2.transform.position = Vector3.MoveTowards(door2.transform.position, door2Target, 2 * Time.deltaTime);
                    if (door2.transform.position == door2Target)
                    {
                        isDone[1] = true;
                    }
                }
                break;
            case 15: //open door3
                if (!isDone[2])
                {
                    door3.transform.position = Vector3.MoveTowards(door3.transform.position, door3Target, 2 * Time.deltaTime);
                    if (door3.transform.position == door3Target)
                    {
                        isDone[2] = true;
                    }
                }
                break;
            default:
                break;
        }
    }
}
