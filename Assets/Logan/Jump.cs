using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    public float jumpStrength = 400;
    public bool grounded;
    private Rigidbody2D rb2;

    // Start is called before the first frame update
    void Start()
    {
        rb2 = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        int layerMask = 1 << 9;
        layerMask = ~layerMask;

        float DistanceToTheGround = GetComponent<BoxCollider2D>().bounds.extents.y;
        grounded = Physics2D.Raycast(transform.position, Vector2.down, DistanceToTheGround + 0.01f);
        Debug.DrawRay(transform.position, Vector2.down, Color.blue);
        if (Input.GetButtonDown("Jump") && grounded)
        {
            rb2.AddForce(new Vector2(0, jumpStrength));
        }
    }
}
