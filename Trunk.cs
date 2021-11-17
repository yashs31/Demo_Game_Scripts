using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trunk : MonoBehaviour
{
    // Start is called before the first frame update

    //MOVEMENT
    [Header("MOVEMENT")]
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] int isFacingRight = 1;
    [SerializeField] Transform groundCheck;
    [SerializeField] float distance;
    [SerializeField] LayerMask groundLayer;

    //LOS
    [Header("LOS")]
    [SerializeField] float lineOfSight = 2f;
    [SerializeField] Transform eyes;
    [SerializeField] Transform projectileSpawnPoint;
    RaycastHit2D hit;

    //DAMAGE TO PLAYER
    [Header("DAMAGE TO PLAYER")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed = 20f;
    [SerializeField] LayerMask playerLayer;

    [Header("SELF DAMAGE")]
    //[SerializeField] int damageTaken = 7;
    [SerializeField] RectTransform statusIndicatorCanvas;

    Animator animator;
    //TrunkHealth health;
    Player player;

    Rigidbody2D myRigidBody;

    void Start()
    {
        animator = GetComponent<Animator>();
        //health = GetComponent<TrunkHealth>();
        myRigidBody = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();
        if (statusIndicatorCanvas == null)
        {
            Debug.LogError("No health indicator image is assigned");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        FlipPlayerAtEndOfPlatform();
        CheckIfPlayerInLOS();
    }

    private void Movement()
    {
        if (IsFacingRight())
        {
            if (animator.GetBool("attack") == true)
            {
                myRigidBody.velocity = new Vector2(0f, 0f);

            }
            else
            {
                myRigidBody.velocity = new Vector2(moveSpeed, 0f);
            }
        }
        else
        {
            if (animator.GetBool("attack") == true)
            {
                myRigidBody.velocity = new Vector2(0f, 0f);
            }
            else
            {
                myRigidBody.velocity = new Vector2(-moveSpeed, 0f);
            }
        }
    }

    void FlipPlayerAtEndOfPlatform()
    {
        hit = Physics2D.Raycast(groundCheck.position, Vector2.down, distance,groundLayer);
        //Debug.DrawRay(groundCheck.position, Vector3.down * distance, Color.red);
        if (hit.collider==false)   //hit.collider==false
        {
            //end of platform reached since no collider in the front anymore
            if (transform.eulerAngles.y == 180)     //if reached right end
            {
                isFacingRight = -1;
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else if (transform.eulerAngles.y == 0)
            {
                isFacingRight = 1;
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }
        statusIndicatorCanvas.transform.rotation = Quaternion.Euler(0.0f, gameObject.transform.rotation.y * -1.0f, 0.0f);   // change rotation of healthbar canvas
    }

    void CheckIfPlayerInLOS()
    {

        RaycastHit2D hit = Physics2D.Raycast(eyes.position, Vector2.right * lineOfSight * isFacingRight, lineOfSight, playerLayer);
        //Debug.DrawRay(eyes.position, Vector3.right * lineOfSight * isFacingRight, Color.red);
        if (hit.collider == false)
        {
            return;
        }
        if (hit.collider.gameObject.CompareTag("Player"))
        {
            animator.SetTrigger("attack");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.TakeDamage(2);
        }
    }

    void Attack()
    {
        GameObject bullet = (GameObject)Instantiate(bulletPrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(bulletSpeed * isFacingRight, 0);
    }


    bool IsFacingRight()
    {
        return transform.eulerAngles.y == 180;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(eyes.transform.position, Vector2.right * lineOfSight * isFacingRight);
    }
}
