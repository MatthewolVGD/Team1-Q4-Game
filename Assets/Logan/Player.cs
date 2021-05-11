using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float speed;
    private Rigidbody2D rb2;
    private SpriteRenderer sr;
    public float jumpStrength;
    public int numberOfJumps;
    private int OGJumps;
    public int damage;
    public float maxHealth;
    public float currentHealth;
    public float dashSpeed;
    private float dashTime;
    public float startDashTime;
    private int direction;
    public bool dashing;
    public int dashes;
    public float dashTim;
    private float OGDashTim;
    public Image healthbar;
    public float fallMultiplier = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb2 = GetComponent<Rigidbody2D>();
        OGJumps = numberOfJumps;
        currentHealth = maxHealth;
        dashTime = startDashTime;
        dashes = 0;
        OGDashTim = dashTim;
    }

    private void FixedUpdate()
    {

        //Move right
        if (Input.GetAxis("Horizontal") > 0 && !dashing)
        {
            sr.flipX = false;
            rb2.velocity = (new Vector2(speed, rb2.velocity.y));
        }
       

        //Move left
        if (Input.GetAxis("Horizontal") < 0 && !dashing)
        {
            sr.flipX = true;
            rb2.velocity = (new Vector2(-speed, rb2.velocity.y));
        }

        //Stop
        if (Input.GetAxis("Horizontal") == 0 && !dashing)
        {
            rb2.velocity = (new Vector2(0, rb2.velocity.y));
        }
    }

    void Update()
    {
        //Better Jumping
        if (rb2.velocity.y < 0)
        {
            rb2.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        //Healthbar
        healthbar.fillAmount = currentHealth / maxHealth;

        Debug.DrawRay(transform.position, gameObject.transform.right, Color.red);
        //Player flipping
        if (rb2.velocity.x < 0f)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }

        //Jump
        if (Input.GetButtonDown("Jump"))
        {
            numberOfJumps++;
        }
        if (Input.GetButtonDown("Jump") && numberOfJumps <= 1)
        {
            rb2.velocity = new Vector2(rb2.velocity.x, jumpStrength);
        }

        float DistanceToTheGround = GetComponent<BoxCollider2D>().bounds.extents.y;
        RaycastHit2D grounded = Physics2D.Raycast(transform.position - new Vector3(0, DistanceToTheGround + 0.03f, 0), Vector2.down, 0.01f);
        Debug.DrawRay(transform.position - new Vector3(0, DistanceToTheGround + 0.03f, 0), Vector2.down, Color.blue);
        if (grounded.collider != null)
        {
            Debug.Log(grounded.collider.gameObject.name);
            if (grounded.collider.gameObject.tag == "Terrain")
            {
                numberOfJumps = OGJumps;
                if(dashes > 0)
                {
                    dashes--;
                }
            }
        }

        //Attack
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }

        //Dash
        dashTim -= Time.deltaTime;
        if(dashTim <= 0)
        {
            if (direction == 0)
            {
                if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.LeftShift) && dashes < 1)
                {
                    dashing = true;
                    direction = 1;
                    dashes++;
                }
                else if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.LeftShift) && dashes < 1)
                {
                    dashing = true;
                    direction = 2;
                    dashes++;
                }
            }
            else
            {
                if (dashTime <= 0)
                {
                    direction = 0;
                    dashTime = startDashTime;
                    dashing = false;
                    dashTim = OGDashTim;
                }
                else
                {
                    dashTime -= Time.deltaTime;
                    if (direction == 1)
                    {
                        rb2.velocity = Vector2.left * dashSpeed;
                    }
                    else if (direction == 2)
                    {
                        rb2.velocity = Vector2.right * dashSpeed;
                    }
                }
            }
        }
    }

    //Damage
    public void Damage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
    //Attack
    void Attack()
    {
        int layermask = 1 << 9;
        layermask = ~layermask;
        RaycastHit2D potentialEnemy;

        if (gameObject.GetComponent<SpriteRenderer>().flipX)
        {
            potentialEnemy = Physics2D.Raycast(transform.position, -gameObject.transform.right, 4f, layermask);
        }
        else
        {
            potentialEnemy = Physics2D.Raycast(transform.position, gameObject.transform.right, 4f, layermask);
        }



        if (potentialEnemy.collider != null)
        {
            if (potentialEnemy.collider.gameObject.tag == "Enemy")
            {
                potentialEnemy.collider.gameObject.GetComponent<EnemyScript>().Damage(damage, gameObject);
            }
        }

    }

    void Awake()
    {
        rb2 = GetComponent<Rigidbody2D>();
    }
}
