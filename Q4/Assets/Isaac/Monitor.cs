using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Monitor : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetKeyDown(KeyCode.C))
        {

            SceneManager.LoadScene("CreditRoller"); //Requires "Using" (see above)

        }

        if (Input.GetKeyDown(KeyCode.P))
        {

            SceneManager.LoadScene("MainScene"); //Requires "Using" (see above)

        }

        if (Input.GetKeyDown(KeyCode.O))
        {

            SceneManager.LoadScene("MainMenu"); //Requires "Using" (see above)

        }




    }
}
