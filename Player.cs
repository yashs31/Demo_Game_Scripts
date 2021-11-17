using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    //Config
    [Header("CONFIG")]
    [SerializeField] int health = 50;           //TODO if getting errors make public
    [SerializeField] float runSpeed = 6f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] protected Vector2 deathKick = new Vector2(100f, 25f);
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask climbingLayer;
    [SerializeField] LayerMask hazardsLayer;
    //[SerializeField] GameObject swimming;
    Vector2 playerVelocity;
    Vector2 climbVelocity;
    float controlThrow;

    int hazardLayerThornDamage = 50;
    float gravityScaleAtStart;

    //SFX
    [Header("SFX")]
    [SerializeField] AudioClip hitSFX;
    [SerializeField] AudioClip jumpSFX;
    [SerializeField] AudioClip gameOverSFX;

    [Header("VFX")]
    [SerializeField] ParticleSystem dust;

    //scriptable Objects
    [Header("SCRIPTABLE OBJS")]
    public IntValue maxhealth;
    public IntValue currhealth;

    //State
    [Header("STATES")]
    bool isAlive = true;
    bool isFacingRight = true;
    bool playerHasVerticalSpeed;
    bool playerHasHorizontalSpeed;
    [SerializeField]bool isGrounded = true;
    bool doubleJumpAllowed = false;
    public bool playerHasKey = false;
    public int numOfKeysCollected = 0;


    [Header("ABILITY CONFIG")]
    [SerializeField] bool canClimb;
    [SerializeField] bool canDoubleJump;

    //Cached Reference
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider2D;
    BoxCollider2D myFeet;
    AudioSource audio;
    //SceneTransitions scene;
    GameManager gameManager;
    ControlsCanvas controlsCanvas;
    LevelLoader levelLoader;


    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myFeet = GetComponent<BoxCollider2D>();
        myBodyCollider2D = GetComponent<CapsuleCollider2D>();
        //scene = FindObjectOfType<SceneTransitions>();
        currhealth.health = maxhealth.health;                      //set scriptable currhealth = value from the scriptable maxhealth
        gameManager = FindObjectOfType<GameManager>();
        gravityScaleAtStart = myRigidBody.gravityScale;
        controlsCanvas = FindObjectOfType<ControlsCanvas>();
        levelLoader = FindObjectOfType<LevelLoader>();
        
    }

    // Update is called once per frame

    void Update()
    {
        isGrounded = myFeet.IsTouchingLayers(groundLayer);
        if (!isAlive)    //dead
        {
            return;
        }
        Run();
        Jump();
        if(canClimb)
        {
            ClimbLadder();
        }
        //ClimbLadder();
        FlipSprite();
        CheckDeath();
        IsTouchingHazardsLayer();
        currhealth.health = health;
    }

    void Run()
    {
        controlThrow = CrossPlatformInputManager.GetAxis("Horizontal"); //-1 or +1
        playerVelocity = new Vector2(controlThrow * runSpeed, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;
        /*
        Vector3 movement = new Vector3(CrossPlatformInputManager.GetAxis("Horizontal"), 0f,0f);
        transform.position+= movement * Time.deltaTime * runSpeed;
        */
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;

        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    void ClimbLadder()
    {
        if (!myBodyCollider2D.IsTouchingLayers(climbingLayer))
        {
            isGrounded = true;
            //myAnimator.SetBool("jump", false);
            myAnimator.SetBool("climbing", false);
            myRigidBody.gravityScale = gravityScaleAtStart;   //not on ladder then keep original gravity scale
            controlsCanvas.DisableClimbButton();

            return;
        }
        myAnimator.SetBool("jump", false);
        controlsCanvas.EnableClimbButton();
        controlThrow = CrossPlatformInputManager.GetAxisRaw("Vertical");
        climbVelocity = new Vector2(myRigidBody.velocity.x, controlThrow * climbSpeed);
        myRigidBody.velocity = climbVelocity;
        myRigidBody.gravityScale = 0f;
        playerHasVerticalSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("climbing", playerHasVerticalSpeed);
    }

    void Jump()
    {
        
        if (isGrounded)
        {
            //myAnimator.SetBool("jump", false);
            doubleJumpAllowed = true;
        }
        if (CrossPlatformInputManager.GetButtonDown("Jump") && isGrounded)
        {
            CreateDust();
            //myAnimator.SetBool("jump", true);
            myAnimator.SetTrigger("jump");
            myRigidBody.velocity = Vector2.up * jumpForce;
            AudioSource.PlayClipAtPoint(jumpSFX, transform.position, 1);
        }
        else if (CrossPlatformInputManager.GetButtonDown("Jump") && doubleJumpAllowed &&canDoubleJump)
        {
            //myAnimator.SetBool("jump", true);
            myAnimator.SetTrigger("jump");
            myRigidBody.velocity = Vector2.up * jumpForce;
            AudioSource.PlayClipAtPoint(jumpSFX, transform.position, 1);
            doubleJumpAllowed = false;
        }

    }

    void FlipSprite()
    {
        playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        if (controlThrow > 0 && !isFacingRight)
        {
            isFacingRight = true;
            transform.Rotate(0, 180, 0);
            CreateDust();
        }
        else if (controlThrow < 0 && isFacingRight)
        {
            isFacingRight = false;
            transform.Rotate(0, 180, 0);
            CreateDust();
        }
    }

    //Make player not to slip of the platform
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<HorizontalMovingPlatform>()||collision.GetComponent<FallingPlatform>())
        {
            this.transform.parent = collision.gameObject.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
       this.transform.parent = null;
    }
    //

    public void TakeDamage(int damageAmount)
    {
        AudioSource.PlayClipAtPoint(hitSFX, transform.position);
        myAnimator.SetTrigger("isHit");
        //health = health - damageAmount;
        health = health - damageAmount;   //remove health from the scriptable health value
        //myRigidBody.AddForce(deathKick, ForceMode2D.Impulse);
        myRigidBody.velocity = deathKick;
        //StartCoroutine(waitForHitAnim());
    }


    //required for collectibles
    public void IncreaseHealth(int increaseAmount)
    {
        //health = health + increaseAmount;
        health = health + increaseAmount;  //increase health in the scriptable obj

        if (health >= 50)        //original -> health>=50
        {
            health = 50;         //original -> health=50
        }
    }


    //TODO only for debugging
    public void DecreaseHealth(int decreaseAmount)
    {
        if (health <= 0)
        {
            health = 0;
        }
        else
        {
            health = health - decreaseAmount;
        }

    }
    //Hazard layer
    void IsTouchingHazardsLayer()
    {
        if (myFeet.IsTouchingLayers(hazardsLayer) || myBodyCollider2D.IsTouchingLayers(hazardsLayer))
        {
            //TakeDamage(hazardLayerThornDamage);
            DecreaseHealth(hazardLayerThornDamage);
        }

    }

    void CheckDeath()
    {
        if (health <= 0)    //origina -> health<=0
        {
            isAlive = false;
            //Destroy(gameObject);
            AudioSource.PlayClipAtPoint(gameOverSFX, transform.position);
            gameManager.RestartLevel();
            //health = 0;
        }

    }

    void CreateDust()
    {
        dust.Play();
    }

    public int GetHealth()
    {
        return health;
    }

    public void SetHealth(int healthToSet)
    {
        health = healthToSet;
    }

    public int GetNumOfKeysCollected()
    {
        return numOfKeysCollected;
    }

    public void SetNumOfKeysCollected()
    {
        numOfKeysCollected+=1;
    }

    /*IEnumerator waitForHitAnim()
    {
        myAnimator.SetBool("isHit", true);
        yield return new WaitForSeconds(1f);
        myAnimator.SetBool("isHit", false);
    }
    IEnumerator JumpAnim()
    {
        myAnimator.SetBool("jump", true);
        yield return new WaitForSeconds(1f);
        myAnimator.SetBool("jump", false);
    }
    */

}

