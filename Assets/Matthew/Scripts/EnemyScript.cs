﻿using System.Collections;
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
        if(Vector2.Distance(transform.position, player.transform.position) < attackDist && attackTimer <= 0f)//Only attack player if they're within this distance, skeleton
        {
            Attack();
            attackTimer = ogAttackTimer;
        }

        
    }

    void Movement()//Handles all enemy movement
    {
        
        
            if (transform.position.x - player.position.x < -3f)//Player to right
            {
                rb.velocity = new Vector2(speed, rb.velocity.y);
            }
            else if (transform.position.x - player.position.x > 3f)//Player to left
            {
                rb.velocity = new Vector2(-speed, rb.velocity.y);
            }
            else if(transform.position.x - player.position.x >= -3f || transform.position.x - player.position.x <= 3f)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        


        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        float DistanceToTheGround = GetComponent<BoxCollider2D>().bounds.extents.y;
        Ray enemyRay = new Ray(transform.position - new Vector3(0, DistanceToTheGround, 0), Vector2.down);
        RaycastHit2D grounded = Physics2D.Raycast(transform.position - new Vector3(0, DistanceToTheGround + 0.01f,0), Vector2.down, 0.01f);
        Debug.DrawRay(transform.position + new Vector3(0, -DistanceToTheGround, 0), Vector2.down, Color.blue);
        RaycastHit2D wallToRight = Physics2D.Raycast(transform.position, Vector2.right, 4f, layerMask);
        RaycastHit2D wallToLeft = Physics2D.Raycast(transform.position, -Vector2.right, 4f, layerMask);
        if(grounded.collider != null)
        {
            if (player.transform.position.y > transform.position.y + 1f && grounded.collider.gameObject.tag == "Terrain")
            {
                if (wallToRight.collider != null && wallToLeft.collider != null)
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
        }
        dealer.gameObject.GetComponent<Player>();
    }

}
