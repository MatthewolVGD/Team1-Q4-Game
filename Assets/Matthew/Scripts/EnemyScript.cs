using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float speed;
    public float jumpStrength;
    public int damage;
    public float attackTimer;
    float ogAttackTimer;
    public Transform player;
    Rigidbody2D rb;
    bool isGrounded;
    
    // Start is called before the first frame update
    void Start()
    {
        ogAttackTimer = attackTimer;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x - player.position.x < -1.5)//Player to right
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
        }
        else if (transform.position.x - player.position.x > 1.5)//Player to left
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
        }
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        float DistanceToTheGround = GetComponent<BoxCollider2D>().bounds.extents.y;
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, DistanceToTheGround + 0.01f, layerMask);
        if (player.transform.position.y > transform.position.y + 1 && isGrounded)
        {
            Debug.Log("Boom");
            rb.velocity = new Vector2(rb.velocity.x, jumpStrength);
        }
    }
}
