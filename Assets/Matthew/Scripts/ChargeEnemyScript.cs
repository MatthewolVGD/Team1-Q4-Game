using System.Collections;
using UnityEngine;

public class ChargeEnemyScript : MonoBehaviour
{
    #region MainVariables
    [Header("Movement")]
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
    bool stunned;//Whether enemy is stunned
    public float stunTime;//How long enemy is stunned
    float ogStunTime;//OG how long enemy is stunned for reset purposes
    public float followClose;//How close the enemy will get before it stops following the player
    public float beforeChargeStopTime;//How long the enemy waits before starting their charge
    public float chargeDur;//How long the enemy charges for
    public GameObject healthBar;//Health bar prefab
    public GameObject healthBarAccess;//Access to health bar
    public float jumpDist;//Distance away from wall that enemy will jump
    public float headbuttTimer;
    float ogHeadbuttTimer;
    public float headbuttDistance;
    public int headbuttDamage;
    public Transform headbuttArea;
    public LayerMask playerLayer;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        ogAttackTimer = attackTimer;
        ogStunTime = stunTime;
        rb = GetComponent<Rigidbody2D>();
        healthBarAccess = Instantiate(healthBar);
        healthBarAccess.GetComponent<EnemyHealthBar>().maxHealth = health;
        healthBarAccess.GetComponent<EnemyHealthBar>().currentHealth = health;
        healthBarAccess.GetComponent<EnemyHealthBar>().attached = gameObject;

        ogHeadbuttTimer = headbuttTimer;

    }

    // Update is called once per frame
    void Update()
    {

        if (Vector2.Distance(transform.position, player.transform.position) < chasePlayerDist && !charging && !stunned)//Only chase player if they're within this distance
        {
            Movement();
        }
        
        if(Physics2D.OverlapBoxAll(gameObject.transform.position, new Vector2(2f * chargerAtkDist, 2f * gameObject.transform.localScale.y), playerLayer ) != null)
        {
            if (attackTimer <= 0f && !stunned)
            {
                StartCoroutine(ChargeAttack());
                attackTimer = ogAttackTimer;
            }
        }
        if(Physics2D.OverlapCircleAll(headbuttArea.position, headbuttDistance, playerLayer) != null)
        {
            if (headbuttTimer <= 0f && !stunned && !charging)
            {
                Headbutt();
                headbuttTimer = ogHeadbuttTimer;
            }
        }

        if(gameObject.GetComponent<SpriteRenderer>().flipX == true)
        {
            headbuttArea.position = new Vector3(transform.position.x + (0.5f * gameObject.GetComponent<BoxCollider2D>().bounds.extents.x) + 2f, transform.position.y, headbuttArea.transform.position.z);
        }
        else if(gameObject.GetComponent<SpriteRenderer>().flipX == false)
        {
            headbuttArea.position = new Vector3(transform.position.x - (0.5f * gameObject.GetComponent<BoxCollider2D>().bounds.extents.x) - 2f, transform.position.y, headbuttArea.transform.position.z);
        }
        if (!stunned && !charging)
        {
            attackTimer -= Time.deltaTime;
        }

        if(stunned && stunTime > 0f)
        {
            stunTime -= Time.deltaTime;
            Debug.Log("Stunned");
        }
        else if (stunned && stunTime <= 0f)
        {
            stunTime = ogStunTime;
            stunned = false;
        }

        
    }

    void Movement()//Handles all enemy movement
    {

        if (transform.position.x - player.position.x < -followClose)//Player to right
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (transform.position.x - player.position.x > followClose)//Player to left
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (transform.position.x - player.position.x >= -followClose || transform.position.x - player.position.x <= followClose)
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
        RaycastHit2D wallToRight = Physics2D.Raycast(transform.position + new Vector3(DistanceToEdgeOfBox + 0.01f, 0, 0), Vector2.right, jumpDist, layerMask);
        RaycastHit2D wallToLeft = Physics2D.Raycast(transform.position - new Vector3(DistanceToEdgeOfBox + 0.01f, 0, 0), -Vector2.right, jumpDist, layerMask);
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
        yield return new WaitForSeconds(beforeChargeStopTime);
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
        yield return new WaitForSeconds(chargeDur);
        charging = false;
        Debug.Log("Stop charging");
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        yield break;
    }

    public void Damage(int damage, GameObject dealer)//Handles enemy taking damage
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        dealer.gameObject.GetComponent<Player>().currentHealth += detAdd;
        healthBarAccess.GetComponent<EnemyHealthBar>().currentHealth = health;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (charging && collision.gameObject.tag == "Player")
        {
            player.gameObject.GetComponent<Player>().Damage(damage);
        }
        if (charging && collision.gameObject.tag == "Terrain")
        {
            stunned = true;
        }
    }

    void Headbutt()
    {
        player.gameObject.GetComponent<Player>().Damage(headbuttDamage);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(2f * chargerAtkDist, 2f * gameObject.transform.localScale.y, 1));
        Gizmos.DrawWireSphere(headbuttArea.position, headbuttDistance);
    }
}
