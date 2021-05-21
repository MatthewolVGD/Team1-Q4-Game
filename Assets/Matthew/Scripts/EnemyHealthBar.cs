using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{

    public float maxHealth;
    public float currentHealth;
    public GameObject attached;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(attached.GetComponent<Transform>().position.x, (attached.GetComponent<BoxCollider2D>().bounds.extents.y) + attached.GetComponent<Transform>().position.y, 0);
        transform.localScale = new Vector3(3f * (currentHealth / maxHealth), 0.5f, 1f);
        if (transform.localScale.x <= 0f)
        {
            Destroy(gameObject);
        }
        
    }
}
