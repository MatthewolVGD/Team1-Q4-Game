using UnityEngine;

public class RollerScript : MonoBehaviour
{
    private static int nScreens = 3;
    private GameObject[] creditScreens = new GameObject[nScreens];
    private static int swapCount = 0;


    // Use this for initialization
    void Start()
    {
        //For each credit screen, add a new reference here:
        creditScreens[0] = GameObject.Find("programmers + tristan");
        creditScreens[1] = GameObject.Find("rune");
        creditScreens[2] = GameObject.Find("anika");
    


        //Turn them all off...
        for (int i = 0; i < nScreens; i++)
        {
            creditScreens[i].SetActive(false);
        }
        //Except, turn back on element 0
        creditScreens[0].SetActive(true);
    } //Start


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Toggle
            int currentScene = swapCount % nScreens;
            creditScreens[currentScene].SetActive(false);
            swapCount++;
            currentScene = swapCount % nScreens;
            creditScreens[currentScene].SetActive(true);


        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }



    } ////Update



}

