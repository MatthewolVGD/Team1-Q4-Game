using UnityEngine;

public class Jump : MonoBehaviour
{
    public float jumpStrength = 400;
    private Rigidbody2D rb2;
    public int numberOfJumps;
    private int OGJumps;

    // Start is called before the first frame update
    void Start()
    {
        rb2 = GetComponent<Rigidbody2D>();
        OGJumps = numberOfJumps;
    }

    // Update is called once per frame
    void Update()
    {
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
    }
}
