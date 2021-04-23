using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{

    #region MainVariables
    public float speed;//Enemy Speed
    public int health;//Enemy Health
    public float jumpStrength;//Enemy Jump Velocity
    public int damage;//Enemy Damage
    public float attackTimer;//Time between attacks
    public float attackDist;//How close the enemy has to be to attack the player
    float ogAttackTimer;//Tracking original attack time for reset purposes
    public Transform player;//The player
    Rigidbody2D rb;//Enemy's rigidbody
    public float chasePlayerDist;//How close the player has to be to chase them
    public int detAdd;//Amount of determination added to player after this enemy dies
    public bool isCharger;//Whether the enemy is a skeleton or a charging enemy
    public float chargerAtkDist;//How close the player has to be for a charging enemy to attack them
    public float chargeSpeed;//Speed enemy charges at
    public bool charging;//Whether enemy is charging
    #endregion
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
        if(Vector2.Distance(transform.position, player.transform.position) < attackDist && attackTimer <= 0f && !isCharger)//Only attack player if they're within this distance, skeleton
        {
            Attack();
            attackTimer = ogAttackTimer;
        }

        if (Vector2.Distance(transform.position, player.transform.position) < chargerAtkDist && attackTimer <= 0f && isCharger)//Only attack player if they're within this distance, charger
        {
            StartCoroutine(ChargeAttack());
            attackTimer = ogAttackTimer;
        }
    }

    void Movement()//Handles all enemy movement
    {
        if(!isCharger)
        {
            if (transform.position.x - player.position.x < -3f)//Player to right
            {
                rb.velocity = new Vector2(speed, rb.velocity.y);
            }
            else if (transform.position.x - player.position.x > 3f)//Player to left
            {
                rb.velocity = new Vector2(-speed, rb.velocity.y);
            }
        }
        else if(isCharger)
        {
            if (transform.position.x - player.position.x < -6f)//Player to right
            {
                rb.velocity = new Vector2(speed, rb.velocity.y);
            }
            else if (transform.position.x - player.position.x > 6f)//Player to left
            {
                rb.velocity = new Vector2(-speed, rb.velocity.y);
            }
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

    void Attack()//Handles enemy attack
    {
        player.gameObject.GetComponent<Player>().Damage(damage);
    }

    IEnumerator ChargeAttack()
    {
        if (transform.position.x - player.position.x < -0.1f)//Player to right
        {
            rb.velocity = new Vector2(chargeSpeed, rb.velocity.y);
        }
        else if (transform.position.x - player.position.x > 0.1f)//Player to left
        {
            rb.velocity = new Vector2(-chargeSpeed, rb.velocity.y);
        }
        charging = true;
        yield return new WaitForSeconds(3f);
        charging = false;
        
    }
    public void Damage(int damage, GameObject dealer)//Handles enemy taking damage
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        dealer.gameObject.GetComponent<Player>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(charging && collision.gameObject.tag == "Player")
        {
            player.gameObject.GetComponent<Player>().Damage(damage);
        }
    }
}
