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
       
    }

    // Update is called once per frame
    void Update()
    {
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
        

        if (gameObject.GetComponent<SpriteRenderer>().flipX == true)
        {
            attackPos = transform.position + new Vector3(attackOffset, 0, 0);
        }
        else if (gameObject.GetComponent<SpriteRenderer>().flipX == false)
        {
            attackPos = transform.position - new Vector3(attackOffset, 0, 0);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Damage(1, player.gameObject);
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
        GetComponent<SpriteRenderer>().color = Color.red;
        player.gameObject.GetComponent<Player>().Damage(damage);
        GetComponent<SpriteRenderer>().color = Color.white;
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
        Debug.Log("Here");
        StartCoroutine(Particles());
    }
    private void OnDrawGizmosSelected()
    {
      
        Gizmos.DrawWireSphere(attackPos, attackDist);
    }

    IEnumerator Particles()
    {
        Debug.Log("Trying");
        hitParticles.Play();
        yield return new WaitForSeconds(particleActiveTime);
        hitParticles.Stop();
    }
}
