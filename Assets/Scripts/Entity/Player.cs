using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour {
    
    public Vector2 maxVelocity;
    Vector2 originalMaxVelocity;
    public Vector3 velocity;

    public float moveSpeed = 10;

    float gravity = -40;
    public float jumpHeight = 10;
    float jumpTimer = 0;
    float maxJumpTime = 0.2f;

    bool rolled;
    float rollTimer = 0;

    float maxSlideTime = 0.25f;
    float slideTime;
    bool wallSlideOn;

    Vector3 oldPos;

    public float wallSlideSpeedMax = 3;
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;
    float disableWallSlideTimer;

    Animator myAnimator;
    SpriteRenderer myRenderer;

    Transform animationGameObject;
    public Vector3 myRotation;
    ParticleSystem myParticleSystem;
    //bool running;
    public bool canMove = true;

    Controller2D controller;

    public Vector2 newSize;
    public Vector2 newPosition;
    public Vector2 originalSize;
    public Vector2 originalPosition;

    Inventory inventory;
    public Womb theWomb = null;

    public bool gunEquipped = true;

    bool buttonHeld = false;
	// Use this for initialization
	void Start () {
        controller = GetComponent<Controller2D>();
        animationGameObject = transform.FindChild("Animations");
        myAnimator = animationGameObject.GetComponent<Animator>();
        myRenderer = animationGameObject.GetComponent<SpriteRenderer>();
        originalMaxVelocity = maxVelocity;
        myRotation = animationGameObject.eulerAngles;
        myParticleSystem = transform.FindChild("PlayerParticles").GetComponent<ParticleSystem>();
        inventory = GameObject.Find("Main").GetComponent<Inventory>();
        if (GameObject.Find("Womb"))
        {
            theWomb = GameObject.Find("Womb").GetComponent<Womb>();
        }
	}

    Vector2 input;
    bool wallSliding = false;

    public void resetPlayer()
    {
        myRenderer.flipX = false;
        velocity = Vector3.zero;
        input.x = 0;
        controller.ResetPos();
    }

    public void MoveLeft()
    {
        input.x = -1;
        buttonHeld = true;
    }
    
    public void MoveRight()
    {
        input.x = 1;
        buttonHeld = true;
    }

    public void ReleaseLeft()
    {
        if(input.x != 1)
        {
            buttonHeld = false;
        }
    }

    public void ReleaseRight()
    {
        if(input.x != -1)
        {
            buttonHeld = false;
        }
    }

    public void ClickJump()
    {
        input.y = 1;
    }

    public void HoldJump()
    {
        input.y = 1;
    }

    public void ReleaseJump()
    {
        input.y = 0;
    }

    // Update is called once per frame
    void Update () {
        //reset
        if (GlobalVars.resetTimer > 0)
        {
            resetPlayer();
        }
        if (!GlobalVars.paused)
        {
            //input
            if (Input.GetKey(KeyCode.A)&&canMove)
            {
                input.x = -1;
            }
            else if (Input.GetKey(KeyCode.D)&&canMove)
            {
                input.x = 1;
            }
            else if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !buttonHeld)
            {
                input.x = 0;
            }
            if(!canMove)
            {
                input = Vector2.zero;
                velocity.x = 0;
            }

            //wallSlide
            int wallDirX = (controller.collisions.left) ? -1 : 1;
            if (controller.collisions.left && input.x == -1 || controller.collisions.right && input.x == 1)
            {
                wallSlideOn = true;
            }
            if (Input.GetKeyDown(KeyCode.S) && disableWallSlideTimer <= 0)
            {
                disableWallSlideTimer = 0.5f;
            }
            if (disableWallSlideTimer > 0)
            {
                wallSlideOn = false;
            }
            if (disableWallSlideTimer > 0)
            {
                disableWallSlideTimer -= Time.deltaTime;
            }
            if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y <= jumpHeight && wallSlideOn)
            {
                if (wallSliding == false)
                {
                    maxVelocity.y = wallSlideSpeedMax;
                    wallSliding = true;
                }
                if (slideTime > 0 && !Input.GetKeyDown(KeyCode.Space) && velocity.y < jumpHeight && !(input.x == 0 && Input.GetKeyDown(KeyCode.Space)))
                {
                    velocity.x = 0;
                    if (input.x != wallDirX && input.x != 0)
                    {
                        slideTime -= Time.deltaTime;
                    }
                    else {
                        slideTime = maxSlideTime;
                    }
                    if (!myParticleSystem.isPlaying)
                    {
                        myParticleSystem.Play();
                    }
                }
                else
                {
                    slideTime = maxSlideTime;
                }
            }
            else
            {
                wallSliding = false;
                maxVelocity.y = originalMaxVelocity.y;
            }
            if (wallSliding)
            {
                if (maxVelocity.y < originalMaxVelocity.y)
                {
                    maxVelocity.y = maxVelocity.y + 0.5f;
                }
            }

            //general movement
            if (!rolled)
            {
                velocity.x = Mathf.Clamp(velocity.x, -maxVelocity.x, maxVelocity.x);
            }
            velocity.y = Mathf.Clamp(velocity.y, -maxVelocity.y, Mathf.Infinity);
            velocity.y += gravity * Time.deltaTime;
            //if (canMove)
            //{
                controller.Move(velocity * Time.deltaTime);
            //}`

            Vector3 distanceBetweenOldAndNewPos = transform.position - controller.collisions.positionOld;

            //collide with ground.
            if (controller.collisions.above || controller.collisions.below)
            {
                if (controller.collisions.below)
                {
                    wallSlideOn = false;
                }
                velocity.y = 0;
                if (velocity.y != 0)
                {
                    jumpTimer = 0;
                }
            }

            //collide with walls.
            if (controller.collisions.left && velocity.x < 0 && !controller.collisions.climbingSlope)
            {
                velocity.x = 0;
            }

            if (controller.collisions.right && velocity.x > 0 && !controller.collisions.climbingSlope)
            {
                velocity.x = 0;
            }

            //jump and WallJump

            if (Input.GetKeyDown(KeyCode.Space) && (controller.collisions.canJump || controller.collisions.below) && !controller.collisions.ceilingAbove&&canMove)
            {
                velocity.y += jumpHeight;
                jumpTimer = maxJumpTime;
            }

            if (Input.GetKeyDown(KeyCode.Space) && wallSliding&&canMove)
            {
                if (velocity.y <= 6)
                {
                    if (wallDirX == input.x)
                    {
                        velocity.x = -wallDirX * wallJumpClimb.x;
                        velocity.y = wallJumpClimb.y;
                        if (input.x == 1)
                        {
                            myRenderer.flipX = false;
                        }
                        else
                        {
                            myRenderer.flipX = true;
                        }
                    }
                    else if (input.x == 0)
                    {
                        wallSlideOn = false;
                        velocity.x = -wallDirX * wallJumpOff.x;
                        velocity.y = wallJumpOff.y;
                    }
                    else
                    {
                        velocity.x = -wallDirX * wallLeap.x;
                        velocity.y = wallLeap.y;
                        if (input.x == -1)
                        {
                            myRenderer.flipX = true;
                        }
                        else
                        {
                            myRenderer.flipX = false;
                        }
                    }
                    //jumpTimer = maxJumpTime;
                }
            }

            if (Input.GetKey(KeyCode.Space) && !(controller.collisions.canJump || controller.collisions.below) && jumpTimer > 0 && !controller.collisions.above && velocity.y > 0 && !controller.collisions.ceilingAbove&&canMove)
            {
                velocity.y += jumpHeight * 4 * Time.deltaTime;
                jumpTimer -= Time.deltaTime;
            }

            if (jumpTimer == 0)
            {
                if (controller.collisions.above)
                {
                    velocity.y = 0;
                    //velocity.y -= jumpHeight;
                }
            }

            //left and right
            if (input.x == -1 && !rolled)
            {
                velocity.x -= moveSpeed * Time.deltaTime;
            }
            if (input.x == 1 && !rolled)
            {
                velocity.x += moveSpeed * Time.deltaTime;
            }

            if (input.x != -1 && input.x != 1 && velocity.x != 0)
            {
                velocity.x = Mathf.Lerp(velocity.x, 0f, Time.deltaTime * 5f);
            }

            //roll
            if (rollTimer > 0)
            {
                rollTimer -= Time.deltaTime;
            }
            //crouch
            if (Input.GetKey(KeyCode.S) && controller.collisions.below &&canMove || controller.collisions.ceilingAbove && controller.collisions.below&&canMove)
            {
                if (!rolled)
                {
                    controller.myCollider.transform.localScale = newSize;
                    controller.myCollider.transform.localPosition = newPosition;
                    controller.UpdateRayCastOrigins();
                    controller.CalculateRaySpacing();
                    rolled = true;
                }
            }
            else if ((!Input.GetKey(KeyCode.S) || !controller.collisions.below) && !controller.collisions.ceilingAbove)
            {
                controller.myCollider.transform.localScale = originalSize;
                controller.myCollider.transform.localPosition = originalPosition;
                controller.UpdateRayCastOrigins();
                controller.CalculateRaySpacing();
                rolled = false;
            }

            if (rolled)
            {
                velocity.x = Mathf.Clamp(velocity.x, -originalMaxVelocity.x / 2f, originalMaxVelocity.x / 2f);
                if (input.x == 1)
                {
                    velocity.x += (moveSpeed * 0.5f) * Time.deltaTime;
                }
                if (input.x == -1)
                {
                    velocity.x -= (moveSpeed * 0.5f) * Time.deltaTime;
                }
            }

            if (Input.GetKeyUp(KeyCode.S) || !controller.collisions.below)
            {
                rolled = false;
            }

            //sprint
            if (Input.GetKey(KeyCode.LeftShift))
            {
                maxVelocity.x = originalMaxVelocity.x * 1.25f;
            }
            else if (!Input.GetKey(KeyCode.LeftShift))
            {
                maxVelocity.x = originalMaxVelocity.x;
            }

            //Rotation
            if (!controller.collisions.below)
            {
                if (input.x == -1 && !controller.collisions.left)
                {
                    if (velocity.y >= 0)
                    {
                        myRotation.z += 0.45f;
                    }
                    if (velocity.y < 0)
                    {
                        myRotation.z -= 0.35f;
                        myRotation.z = Mathf.Clamp(myRotation.z, -5f, 5f);
                    }
                }
                if (input.x == 1 && !controller.collisions.right)
                {
                    if (velocity.y >= 0)
                    {
                        myRotation.z -= 0.45f;
                    }
                    if (velocity.y < 0)
                    {
                        myRotation.z += 0.35f;
                        myRotation.z = Mathf.Clamp(myRotation.z, -5f, 5f);
                    }
                }
                if (input.x == -1 && controller.collisions.right)
                {
                    myRotation.z += 0.75f;
                }
                if (input.x == 1 && controller.collisions.left)
                {
                    myRotation.z -= 0.75f;
                }
                if (input.x == 0 || (controller.collisions.left && input.x == -1 || controller.collisions.right && input.x == 1))
                {
                    myRotation.z = Mathf.Lerp(myRotation.z, 0f, Time.deltaTime * 4f);
                }
                //animationGameObject.eulerAngles = Vector3.Lerp(animationGameObject.eulerAngles, new Vector3(0, 0, controller.collisions.slopeAngle), Time.deltaTime*2f);
            }
            else
            {
                myRotation = Vector3.zero;
            }

            myRotation.z = Mathf.Clamp(myRotation.z, -8f, 8f);
            animationGameObject.eulerAngles = myRotation;
            //particleSystemShape
            ParticleSystem.ShapeModule shape = myParticleSystem.shape;
            //animations
            if (rolled || controller.collisions.ceilingAbove && controller.collisions.below)
            {
                myAnimator.SetBool("ducked", true);
            }
            else if (!rolled && !controller.collisions.ceilingAbove)
            {
                myAnimator.SetBool("ducked", false);
            }
            if (theWomb == null)
            {
                myAnimator.SetBool("born", true);
            }
            if (wallSliding && wallSlideOn)
            {
                if (wallDirX == 1 && !myRenderer.flipX || wallDirX == -1 && myRenderer.flipX)
                {
                    myAnimator.SetBool("rightWallGrab", true);
                    myAnimator.SetBool("leftWallGrab", false);
                    if (Mathf.Abs(velocity.y) > 6)
                    {
                        shape.box = new Vector3(0.5f, 1f, shape.box.z);
                        myParticleSystem.Play();
                    }
                }
                else if (wallDirX == 1 && myRenderer.flipX || wallDirX == -1 && !myRenderer.flipX)
                {
                    myAnimator.SetBool("leftWallGrab", true);
                    myAnimator.SetBool("rightWallGrab", false);
                    if (Mathf.Abs(velocity.y) > 6)
                    {
                        shape.box = new Vector3(0.5f, 1f, shape.box.z);
                        myParticleSystem.Play();
                    }
                }
            }
            else
            {
                myAnimator.SetBool("rightWallGrab", false);
                myAnimator.SetBool("leftWallGrab", false);
            }
            if (Mathf.Abs(velocity.x) > 2&&canMove)
            {
                if (myAnimator.GetBool("running") == false && controller.collisions.canJump && !controller.collisions.left && !controller.collisions.right)
                {
                    myAnimator.SetBool("running", true);
                }
                if (velocity.x < 0)
                {
                    if (controller.collisions.below)
                    {
                        myRenderer.flipX = true;
                    }
                }
                else
                {
                    if (controller.collisions.below)
                    {
                        myRenderer.flipX = false;
                    }
                }
            }
            if (Mathf.Abs(velocity.x) < 2 && controller.collisions.canJump)
            {
                if (myAnimator.GetBool("running") == true)
                {
                    //particleSystem.maxParticles = 25;
                    myAnimator.SetBool("running", false);
                }
            }

            if (!controller.collisions.above && (controller.collisions.canJump || controller.collisions.below) && velocity.y <= 0)
            {
                if (myAnimator.GetBool("grounded") == false)
                {
                    if (myParticleSystem != null)
                    {
                        shape.box = new Vector3(1f, 0.5f, shape.box.z);
                        myParticleSystem.maxParticles = 25;
                        myParticleSystem.Play();
                    }
                    myAnimator.SetBool("grounded", true);
                }
                if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("land") && myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    myAnimator.SetBool("landed", false);
                }
                if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("runland") && myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    myAnimator.SetBool("runlanded", false);
                }
                if (myAnimator.GetBool("falling") == true && myAnimator.GetBool("grounded") == true)
                {
                    myAnimator.SetBool("landed", true);
                    myAnimator.SetBool("runlanded", true);
                    myAnimator.SetBool("falling", false);
                }
            }
            else if (!controller.collisions.canJump && !controller.collisions.below)
            {
                if (myAnimator.GetBool("grounded") == true)
                {
                    myAnimator.SetBool("grounded", false);
                    myAnimator.SetBool("landed", false);
                    myAnimator.SetBool("runlanded", false);
                }
                if (velocity.y < -2)
                {
                    if (myAnimator.GetBool("falling") == false)
                    {
                        myAnimator.SetBool("falling", true);
                    }
                }
                if (velocity.y > 0)
                {
                    if (myAnimator.GetBool("falling") == true)
                    {
                        myAnimator.SetBool("falling", false);
                    }
                }
            }
        }
	}

    void OnTriggerStay2D(Collider2D info)
    {
        if (info.gameObject.layer == LayerMask.NameToLayer("doors"))
        {
            Door door = info.gameObject.GetComponent<Door>();
            if(Input.GetKeyDown(KeyCode.E))
            {
                if (inventory.keyList.Count > 0 && door.locked)
                {
                    door.useItemOnDoor(inventory.keyList[0]);
                }
            }
            if(Input.GetKeyDown(KeyCode.W)&&!door.locked)
            {
                door.OpenDoor();
            }
        }
    }
}
