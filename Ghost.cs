using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("MOVEMENT")]
    [SerializeField] float moveSpeed = 2f;
    int isFacingRight = 1;
    float timeRemaining = 3f;
    bool hasDisappeard=false;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float distance;

    //DAMAGE TO PLAYER
    [Header("DAMAGE TO PLAYER")]
    [SerializeField] int damageToPlayer = 12;
    [SerializeField] LayerMask playerLayer;

    Rigidbody2D myrigidBody;
    Animator animator;
    CapsuleCollider2D collider;
    Player player;
    RaycastHit2D hit;
    void Start()
    {
        player = FindObjectOfType<Player>();
        //animator = GetComponent<Animator>();
        myrigidBody = GetComponent<Rigidbody2D>();
        //health = GetComponent<ChickenHealth>();
        collider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();


    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        FlipPlayerAtEndOfPlatform();
        TriggerAppearDisappearSequence();
    }

    void Movement()
    {
        if (IsFacingRight())
        {
            myrigidBody.velocity = new Vector2(moveSpeed, 0f);
        }
        else
        {
            myrigidBody.velocity = new Vector2(-moveSpeed, 0f);
        }

    }
    void FlipPlayerAtEndOfPlatform()
    {
        hit = Physics2D.Raycast(groundCheck.position, Vector2.down, distance, groundLayer);
        //Debug.DrawRay(groundCheck.position, Vector3.down * distance, Color.red);
        if (hit.collider == false)    //hit.collider==false
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
        //Disappear();
    }

    void TriggerAppearDisappearSequence()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        else if (timeRemaining <= 0)
        {
            hasDisappeard = !hasDisappeard;
            if (hasDisappeard)
            {
                Disappear();
            }
            else
            {
                Appear();
            }

        }
    }

    void Disappear()
    {

        collider.enabled = false;
        animator.SetTrigger("disappear");
        timeRemaining = Random.Range(2, 5);
        
    }

    void Appear()
    {
        collider.enabled = true;
        timeRemaining = Random.Range(2, 5); 
        animator.SetTrigger("appear");
    }

    bool IsFacingRight()
    {
        return transform.eulerAngles.y == 180;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            player.TakeDamage(damageToPlayer);
        }
    }
}
