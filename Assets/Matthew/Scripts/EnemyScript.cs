using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float speed;
    public int health;
    public float jumpStrength;
    public int damage;
    public float attackTimer;
    public float attackDist;
    float ogAttackTimer;
    public Transform player;
    Rigidbody2D rb;
    bool isGrounded;
    public float chasePlayerDist;
    // Start is called before the first frame update
    void Start()
    {
        ogAttackTimer = attackTimer;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector2.Distance(transform.position, player.transform.position) < chasePlayerDist)//Only chase player if they're within this distance
        {
            Movement();
        }

        attackTimer -= Time.deltaTime;
        if(Vector2.Distance(transform.position, player.transform.position) < attackDist && attackTimer <= 0f)
        {
            Attack();
            attackTimer = ogAttackTimer;
        }
    }

    public void Movement()
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
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, DistanceToTheGround + 0.01f);

        bool wallToRight = Physics2D.Raycast(transform.position, Vector2.right, 4f, layerMask);
        bool wallToLeft = Physics2D.Raycast(transform.position, -Vector2.right, 4f, layerMask);

        
        if (player.transform.position.y > transform.position.y + 1f && isGrounded)
        {
            
            if (wallToRight || wallToLeft)
            {
                
                rb.velocity = new Vector2(rb.velocity.x, jumpStrength);
            }

        }
    }

    public void Attack()
    {

    }
}
