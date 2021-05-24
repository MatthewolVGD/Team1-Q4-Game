using UnityEngine;
using System.Collections;
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
    public GameObject healthBar;//Health bar prefab
    public GameObject healthBarAccess;//Access to health bar
    public float jumpDist;//Distance away from wall that enemy will jump
    public float followClose;//How close the enemy will get before it stops following the player
    Vector3 attackPos;
    public LayerMask playerLayer;
    public float attackOffset;
    ParticleSystem hitParticles;
    public float particleActiveTime;
    public float stepHeight;
    public float stepSmooth;
    Rigidbody2D rb2;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        ogAttackTimer = attackTimer;
        rb = GetComponent<Rigidbody2D>();
        healthBarAccess = Instantiate(healthBar);
        healthBarAccess.GetComponent<EnemyHealthBar>().maxHealth = health;
        healthBarAccess.GetComponent<EnemyHealthBar>().currentHealth = health;
        healthBarAccess.GetComponent<EnemyHealthBar>().attached = gameObject;

        attackPos = transform.position + new Vector3(attackOffset, 0, 0);
        hitParticles = GetComponent<ParticleSystem>();
        hitParticles.Stop();


        rb2 = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float yColl = gameObject.GetComponent<BoxCollider2D>().bounds.extents.y;
        float xColl = gameObject.GetComponent<BoxCollider2D>().bounds.extents.x;
        RaycastHit2D stepLower = Physics2D.Raycast(transform.position + new Vector3(xColl, -yColl + 0.01f, 0), Vector2.right, 0.1f);
        RaycastHit2D stepUpper = Physics2D.Raycast(transform.position + new Vector3(xColl, -yColl + stepHeight, 0), Vector2.right, 0.1f);
        if (gameObject.GetComponent<SpriteRenderer>().flipX == false)
        {
            stepLower = Physics2D.Raycast(transform.position + new Vector3(xColl - 0.04f, -yColl + 0.01f, 0), Vector2.right, 0.1f);
            stepUpper = Physics2D.Raycast(transform.position + new Vector3(xColl - 0.04f, -yColl + stepHeight, 0), Vector2.right, 0.1f);
            Debug.DrawRay(transform.position + new Vector3(xColl - 0.04f, -yColl + 0.01f, 0), Vector2.right, Color.red);
            Debug.DrawRay(transform.position + new Vector3(xColl - 0.04f, -yColl + stepHeight, 0), Vector2.right, Color.red);
        }
        else if (gameObject.GetComponent<SpriteRenderer>().flipX == true)
        {
            stepLower = Physics2D.Raycast(transform.position + new Vector3(-xColl - 0.06f, -yColl + 0.01f, 0), -Vector2.right, 0.1f);
            stepUpper = Physics2D.Raycast(transform.position + new Vector3(-xColl - 0.06f, -yColl + stepHeight, 0), -Vector2.right, 0.1f);
            Debug.DrawRay(transform.position + new Vector3(-xColl - 0.06f, -yColl + 0.01f, 0), -Vector2.right, Color.red);
            Debug.DrawRay(transform.position + new Vector3(-xColl - 0.06f, -yColl + stepHeight, 0), -Vector2.right, Color.red);
        }
        if (stepLower.collider != null)
        {
            

            if (stepUpper.collider == null)
            {
                rb2.position -= new Vector2(0f, -stepSmooth);
            }
        }



        if (Vector2.Distance(transform.position, player.transform.position) < chasePlayerDist)//Only chase player if they're within this distance
        {
            Movement();
        }

        attackTimer -= Time.deltaTime;

        if (Physics2D.OverlapCircleAll(attackPos, attackDist, playerLayer) != null)
        {
            if (Vector2.Distance(transform.position, player.transform.position) <= attackDist && attackTimer <= 0f)//Only attack player if they're within this distance, skeleton
            {
                Attack();
                attackTimer = ogAttackTimer;
            }
        }
        

        if (gameObject.GetComponent<SpriteRenderer>().flipX == false)
        {
            attackPos = transform.position + new Vector3(attackOffset, 0, 0);
        }
        else if (gameObject.GetComponent<SpriteRenderer>().flipX == true)
        {
            attackPos = transform.position - new Vector3(attackOffset, 0, 0);
        }

        
    }

    void Movement()//Handles all enemy movement
    {


        if (transform.position.x - player.position.x < 0f)//Player to right
        {

            GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (transform.position.x - player.position.x > 0f)//Player to left
        {

            GetComponent<SpriteRenderer>().flipX = true;
        }


        if (transform.position.x - player.position.x < -followClose)//Player to right
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);

        }
        else if (transform.position.x - player.position.x > followClose)//Player to left
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);

        }
        else if (transform.position.x - player.position.x >= -followClose || transform.position.x - player.position.x <= followClose)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }



        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        float DistanceToTheGround = GetComponent<BoxCollider2D>().bounds.extents.y;
        float DistanceToEdgeOfBox = GetComponent<BoxCollider2D>().bounds.extents.x;
        RaycastHit2D grounded = Physics2D.Raycast(transform.position - new Vector3(0, DistanceToTheGround + 0.02f, 0), Vector2.down, 0.01f);
        Debug.DrawRay(transform.position + new Vector3(0, -DistanceToTheGround - 0.02f, 0), Vector2.down, Color.blue);
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

    void Attack()//Handles enemy attack
    {
        
        player.gameObject.GetComponent<Player>().Damage(damage);
        
    }

    public void Damage(int damage, GameObject dealer)//Handles enemy taking damage
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
            dealer.gameObject.GetComponent<Player>().currentHealth += detAdd;
        }
        
        healthBarAccess.GetComponent<EnemyHealthBar>().currentHealth = health;
        
        StartCoroutine(Particles());
    }
    private void OnDrawGizmosSelected()
    {
      
        Gizmos.DrawWireSphere(attackPos, attackDist);
    }

    IEnumerator Particles()
    {
        
        hitParticles.Play();
        yield return new WaitForSeconds(particleActiveTime);
        hitParticles.Stop();
    }
}
