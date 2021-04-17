using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speedCap;
    public float accel;
    private Rigidbody2D rb2;
    private SpriteRenderer sr;
    public float jumpStrength;
    public int numberOfJumps;
    private int OGJumps;
    public int damage;
    public int maxHealth;
    private int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb2 = GetComponent<Rigidbody2D>();
        OGJumps = numberOfJumps;
        currentHealth = maxHealth;
    }

    private void FixedUpdate()
    {

        //Move right
        if (Input.GetAxis("Horizontal") > 0)
        {
            sr.flipX = false;
            rb2.AddForce(new Vector2(accel, 0));
        }

        //Move left
        if (Input.GetAxis("Horizontal") < 0)
        {
            sr.flipX = true;
            rb2.AddForce(new Vector2(-accel, 0));
        }
    }

    void Update()
    {
        //Jump
        if (Input.GetButtonDown("Jump"))
        {
            numberOfJumps++;
        }
        if (Input.GetButtonDown("Jump") && numberOfJumps <= 1)
        {
            rb2.AddForce(new Vector2(0, jumpStrength));
        }

        float DistanceToTheGround = GetComponent<BoxCollider2D>().bounds.extents.y;
        RaycastHit2D grounded = Physics2D.Raycast(transform.position - new Vector3(0, DistanceToTheGround + 0.01f, 0), Vector2.down, 0.01f);
        Debug.DrawRay(transform.position, Vector2.down, Color.blue);

        if (grounded.collider.gameObject.tag == "Terrain")
        {
            numberOfJumps = OGJumps;
        }

        //Speed Limit
        if (rb2.velocity.x > speedCap)
        {
            rb2.velocity = new Vector2(speedCap, rb2.velocity.y);
        }

        if (rb2.velocity.x < -speedCap)
        {
            rb2.velocity = new Vector2(-speedCap, rb2.velocity.y);
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
}
