using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{

    public string attackType;
    public GameObject attached;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (attackType == "Charge")
        {
            transform.localScale = new Vector3(attached.GetComponent<ChargeEnemyScript>().chargerAtkDist, 1, 1);
            transform.position = new Vector3(attached.GetComponent<BoxCollider2D>().bounds.extents.x + (0.5f * transform.localScale.x), attached.transform.position.y, 0);
        }
        else if (attackType == "Headbutt")
        {

        }
        else if (attackType == "Skeleton")
        {

        }
        else if (attackType == "Player")
        {

        }
    }
}
