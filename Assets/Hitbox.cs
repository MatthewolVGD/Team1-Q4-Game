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
        if (attackType != null)
        {
            if (attackType == "Charge")
            {
                transform.localScale = new Vector3(attached.GetComponent<ChargeEnemyScript>().chargerAtkDist, attached.transform.localScale.y, 1);
                transform.position = new Vector3((attached.transform.position.x + attached.GetComponent<BoxCollider2D>().bounds.extents.x) + (0.5f * transform.localScale.x * attached.transform.localScale.x), attached.transform.position.y, 0);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (attackType != null)
        {
            if (attackType == "Charge" && collision.gameObject.tag == "Player")
            {
                attached.GetComponent<ChargeEnemyScript>().canCharge = true;
                Debug.Log("Yee yee brother");
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

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (attackType != null)
        {
            if (attackType == "Charge" && collision.gameObject.tag == "Player")
            {
                attached.GetComponent<ChargeEnemyScript>().canCharge = false;
                Debug.Log("Less yee yee");
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
}
