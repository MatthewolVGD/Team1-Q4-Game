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
    public bool isGrounded;
    public float chasePlayerDist;
    public int detAdd;
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

    void Movement()
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
        Ray enemyRay = new Ray(transform.position - new Vector3(0, DistanceToTheGround, 0), Vector2.down);
        RaycastHit2D grounded = Physics2D.Raycast(transform.position - new Vector3(0, DistanceToTheGround + 0.01f,0), Vector2.down, 0.01f);
        Debug.DrawRay(transform.position + new Vector3(0, -DistanceToTheGround, 0), Vector2.down, Color.blue);
        bool wallToRight = Physics2D.Raycast(transform.position, Vector2.right, 4f, layerMask);
        bool wallToLeft = Physics2D.Raycast(transform.position, -Vector2.right, 4f, layerMask);
        if(grounded.collider != null)
        {
            if (player.transform.position.y > transform.position.y + 1f && grounded.collider.gameObject.tag == "Terrain")
            {

                if (wallToRight || wallToLeft)
                {

                    rb.velocity = new Vector2(rb.velocity.x, jumpStrength);
                }

            }
            Debug.Log(grounded.collider.gameObject.name);
        }
        
        
    }

    void Attack()
    {

    }

    public void Damage(int damage, GameObject dealer)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        
    }
}
