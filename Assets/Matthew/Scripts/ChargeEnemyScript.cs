using System.Collections;
using UnityEngine;

public class ChargeEnemyScript : MonoBehaviour
{
    #region MainVariables
    public float speed;//Enemy Speed
    public int health;//Enemy Health
    public float jumpStrength;//Enemy Jump Velocity
    public int damage;//Enemy Damage
    public float attackTimer;//Time between attacks
    float ogAttackTimer;//Tracking original attack time for reset purposes
    public Transform player;//The player
    Rigidbody2D rb;//Enemy's rigidbody
    public float chasePlayerDist;//How close the player has to be to chase them
    public int detAdd;//Amount of determination added to player after this enemy dies
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
        if (Vector2.Distance(transform.position, player.transform.position) < chasePlayerDist && !charging)//Only chase player if they're within this distance
        {
            Movement();
        }
        attackTimer -= Time.deltaTime;
        if (Mathf.Abs(transform.position.x - player.transform.position.x) <= chargerAtkDist && attackTimer <= 0f)//Only attack player if they're within this distance, charger
        {
            StartCoroutine(ChargeAttack());
            attackTimer = ogAttackTimer;
        }
    }

    void Movement()//Handles all enemy movement
    {

        if (transform.position.x - player.position.x < -6f)//Player to right
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (transform.position.x - player.position.x > 6f)//Player to left
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (transform.position.x - player.position.x >= -6f || transform.position.x - player.position.x <= 6f)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }


        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        float DistanceToTheGround = GetComponent<BoxCollider2D>().bounds.extents.y;
        float DistanceToEdgeOfBox = GetComponent<BoxCollider2D>().bounds.extents.x;
        Ray enemyRay = new Ray(transform.position - new Vector3(0, DistanceToTheGround, 0), Vector2.down);
        RaycastHit2D grounded = Physics2D.Raycast(transform.position - new Vector3(0, DistanceToTheGround + 0.01f, 0), Vector2.down, 0.01f);
        Debug.DrawRay(transform.position + new Vector3(0, -DistanceToTheGround, 0), Vector2.down, Color.blue);
        RaycastHit2D wallToRight = Physics2D.Raycast(transform.position + new Vector3(DistanceToEdgeOfBox + 0.01f, 0, 0), Vector2.right, 2f, layerMask);
        RaycastHit2D wallToLeft = Physics2D.Raycast(transform.position - new Vector3(DistanceToEdgeOfBox + 0.01f, 0, 0), -Vector2.right, 2f, layerMask);
        Debug.DrawRay(transform.position + new Vector3(DistanceToEdgeOfBox + 0.01f, 0, 0), Vector2.right, Color.blue);
        Debug.DrawRay(transform.position + new Vector3(-DistanceToEdgeOfBox - 0.01f, 0, 0), -Vector2.right, Color.blue);
        if (grounded.collider != null)
        {
            if (player.transform.position.y > transform.position.y + 1f && grounded.collider.gameObject.tag == "Terrain")
            {
                if (wallToRight.collider != null || wallToLeft.collider != null)
                {
                    if (wallToRight.collider.gameObject.tag == "Terrain" || wallToLeft.collider.gameObject.tag == "Terrain")
                    {

                        rb.velocity = new Vector2(rb.velocity.x, jumpStrength);
                    }
                }


            }
        }


    }

    IEnumerator ChargeAttack()
    {
        charging = true;
        string chargeDir = null;
        rb.velocity = new Vector2(0, rb.velocity.y);
        if (transform.position.x - player.position.x < -0.1f)//Player to right
        {
            chargeDir = "Right";
        }
        else if (transform.position.x - player.position.x > 0.1f)//Player to left
        {
            chargeDir = "Left";
        }
        Debug.Log("Charging");
        gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
        yield return new WaitForSeconds(1.5f);
        Debug.Log("Moving");
        if (chargeDir != null)
        {
            if (chargeDir == "Right")
            {
                rb.velocity = new Vector2(chargeSpeed, rb.velocity.y);
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else if (chargeDir == "Left")
            {
                rb.velocity = new Vector2(-chargeSpeed, rb.velocity.y);
                GetComponent<SpriteRenderer>().flipX = false;
            }
        }
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(3f);
        charging = false;
        Debug.Log("Stop charging");
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
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
        if (charging && collision.gameObject.tag == "Player")
        {
            player.gameObject.GetComponent<Player>().Damage(damage);
        }
    }
}
