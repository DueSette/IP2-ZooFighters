using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseCharacterBehaviour : MonoBehaviour
{
    #region Buttons and axes
    //======= (SOME KIND OF) Command pattern, these booleans serve as a medium between the actual button press and the function that is called
    //Buttons
    private bool aButton;
    private bool bButton;
    private bool xButton;
    private bool yButton;
    private bool startButton;

    //Analog sticks
    private float lStickHor;
    private float rStickVer;

    //bumpers and triggers
    private bool rBumper;
    private bool lBumper;
    private bool rTrig;

    #endregion

    #region General Data
    private enum JoyStick { J1, J2, J3, J4 };
    JoyStick jStick;

    private GameManagerScript gmScript;

    //Gameplay variables
    public int maxHealth = 100;
    [Tooltip("This represents health but is updated with the UI, meaning more slowly")]
    public int displayedHealth;
    [Tooltip("This is the actual real health of the character, it's updated BEFORE the UI")]
    public int currHealth;
    public int livesLeft;

    //Base stats
    public bool alive = true;
    public int totalLives;
    public float movSpeed;
    public float jumpPower;
    public float bodyMass;
    [Tooltip("Set 1 for default damage, go below one for below average damage and viceversa")]
    public float damageMod = 1;

    //Weapon related variables
    public bool isArmed = false;
    private GameObject equippedWeapon;
    public Sprite equippedWeaponSprite;
    public string equippedWeaponName;
    public BaseWeaponScript weaponScript;
    [Tooltip("The game object that the weapon will be childed too")]
    public GameObject weaponSlot;

    //Physics related variables
    public bool slowFall;
    public float internalVel;
    [SerializeField]
    private float maxInternalHspeed = 20;
    [SerializeField]
    private float minInternalHspeed = -20;
    public bool canMove = true;
    public bool grounded = true;
    public bool canExtraJump = true;
    private bool drag = false;
    [HideInInspector]
    public float moveDisableDuration = 0;
    [HideInInspector]
    public float moveDisableTimeElapsed = 0;

    Rigidbody rb;
    #endregion

    //Makes the character able to be controlled only by the passed joystick
    //this function is called by SelectorBehaviour
    public void ReceiveJoystick(int joyNum)
    {
        print("joystick num \"" + joyNum +"\" selected its character");

        switch (joyNum)
        {
            case 0:
                {
                    jStick = JoyStick.J1;
                }
                break;
            case 1:
                {
                    jStick = JoyStick.J2;
                }
                break;
            case 2:
                {
                    jStick = JoyStick.J3;
                }
                break;
            case 3:
                {
                    jStick = JoyStick.J4;
                }
                break;
            default:
                break;
        }
    }

    public virtual void Start()
    {
        gmScript = GameManagerScript.gmInstance;
        GetComponent<Rigidbody>().mass = bodyMass;
        rb = GetComponent<Rigidbody>();
    }

    //Define here all the actual commands (shoot, jump, etc)
    //Also update HP here
    public virtual void Update()
    {
        internalVel = rb.velocity.x;

        CheckInput();

        //Checking if character is still alive
        if(GetHealth() <= 0 && alive)
        {
            StartCoroutine(CharacterDeath());
        }

        //Movement disabling/enabling algorithm
        if(!canMove)
        {
            moveDisableTimeElapsed += Time.deltaTime;
            if(moveDisableTimeElapsed >= moveDisableDuration)
            {
                canMove = true;
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
                moveDisableTimeElapsed = 0;
            }
        }
        
        //=====COMBAT GAME FUNCTIONS
        if (gmScript.GetGameState() == GameManagerScript.GameState.inGame && alive)
        {
            if (aButton)
            {
                if (grounded)
                {
                    Jump();
                }
                else if (canExtraJump)
                {
                    Jump();
                    canExtraJump = false;
                }
            }
            if (bButton && isArmed)
            {
                TossWeapon();
            }
            if (xButton)
            {
                print("x pressed");
            }
            if (yButton)
            {
                print("y pressed");
            }
            if (startButton)
            {
                StartCoroutine(gmScript.TogglePause());
            }
            if (rTrig)
            {
                if (isArmed)
                {
                    weaponScript.Fire(damageMod, Mathf.Sign(transform.rotation.y));
                }
            }
            if (lStickHor != 0 && canMove)
            {
                Move(lStickHor);
            }
        }
    }
    
    public virtual void FixedUpdate()
    {
        //Generally makes gravity stronger when already falling
        if (rb.velocity.y < 0)
        {
            rb.AddForce(Physics.gravity * 3);
        }

        //Mimics air resistance outside of the rigidbody class
        if (drag)
        {
            rb.velocity -= -rb.velocity * 1.5f;
        }

        //Determines if the character is NOT on a platform, then adds extra gravity
        if (!grounded)
        {
            if(slowFall && rb.velocity.y > 0)
            {
                rb.AddForce(Physics.gravity);
            }
            else
            {
                rb.AddForce(Physics.gravity * 2.5f);
            }
        }       
    }
    
    public virtual void Move(float stickDirection)
    {
        transform.Translate(new Vector3(1 * Mathf.Sign(stickDirection), 0, 0) * Time.deltaTime * movSpeed, Space.World);

        if (Mathf.Sign(stickDirection) == rb.velocity.x)
        {
            drag = true;
        }

        /*if (internalVel < maxInternalHspeed && internalVel > minInternalHspeed)
        {
            if (stickDirection != 0 && canMove)
            {
                transform.Translate(new Vector3(1 * Mathf.Sign(stickDirection), 0, 0) * Time.deltaTime * movSpeed, Space.World);
                //GetComponent<Rigidbody>().AddForce(new Vector3(10 * Mathf.Sign(stickDirection), 0, 0) * Time.deltaTime * movSpeed, ForceMode.Impulse);
            }
        }
        */

        if (internalVel > maxInternalHspeed)
        {
            internalVel = maxInternalHspeed - 0.5f;
        }
        if (internalVel < minInternalHspeed)
        {
            internalVel = minInternalHspeed + 0.5f;
        }
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, -90 * Mathf.Sign(stickDirection), transform.rotation.z));
    }

    public virtual void Jump()
    {
        if(rb.velocity.y < 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
        rb.AddForce(new Vector3(0, jumpPower, 0), ForceMode.Impulse);
    }

    /*
    private bool CheckIfGrounded()
    {
        RaycastHit hit;
        Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z), Vector3.down, out hit);

        if (hit.collider == null)
        {
            //this can happen when the character is jumping and there's absolutely nothing below it.
            //Without this if statement the program will think we are grounded.
            return false;
        }
        if (hit.distance < 1)
        {
            return true;
        }  
        else
        {
            return false;
        }
    }
    */

    //Performs some form of command pattern in relation to the current JoyStick Enum and GameState, checking which buttons have been pressed
    public void CheckInput()
    {
        rTrig = false;

        switch (jStick)
        {
            case JoyStick.J1:
                {
                    startButton = Input.GetKeyDown(KeyCode.Joystick1Button7);

                    if (!gmScript.paused)
                    {
                        aButton = Input.GetKeyDown(KeyCode.Joystick1Button0);
                        bButton = Input.GetKeyDown(KeyCode.Joystick1Button1);
                        xButton = Input.GetKeyDown(KeyCode.Joystick1Button2);
                        yButton = Input.GetKeyDown(KeyCode.Joystick1Button3);
                        lBumper = Input.GetKeyDown(KeyCode.Joystick1Button4);
                        rBumper = Input.GetKeyDown(KeyCode.Joystick1Button5);

                        lStickHor = Input.GetAxis("LeftJoyHorizontal");

                        if (Input.GetAxis("J1RT") > 0.4f)
                        {
                            rTrig = true;
                        }

                        if(Input.GetKeyDown(KeyCode.Joystick1Button0))
                        {
                            slowFall = true;
                        }
                        if(Input.GetKeyUp(KeyCode.Joystick1Button0))
                        {
                            slowFall = false;
                        }
                    }
                }
                break;
            case JoyStick.J2:
                {
                    startButton = Input.GetKeyDown(KeyCode.Joystick2Button7);

                    if (!gmScript.paused)
                    {                      
                        aButton = Input.GetKeyDown(KeyCode.Joystick2Button0);
                        bButton = Input.GetKeyDown(KeyCode.Joystick2Button1);
                        xButton = Input.GetKeyDown(KeyCode.Joystick2Button2);
                        yButton = Input.GetKeyDown(KeyCode.Joystick2Button3);
                        lBumper = Input.GetKeyDown(KeyCode.Joystick2Button4);
                        rBumper = Input.GetKeyDown(KeyCode.Joystick2Button5);

                        lStickHor = Input.GetAxis("LeftJoy2Horizontal");

                        if (Input.GetAxis("J2RT") > 0.4f)
                        {
                            rTrig = true;
                        }

                        if (Input.GetKeyDown(KeyCode.Joystick2Button0))
                        {
                            slowFall = true;
                        }
                        if (Input.GetKeyUp(KeyCode.Joystick2Button0))
                        {
                            slowFall = false;
                        }
                    }
                }
                break;
            case JoyStick.J3:
                {
                    startButton = Input.GetKeyDown(KeyCode.Joystick3Button7);

                    if (!gmScript.paused)
                    {
                        aButton = Input.GetKeyDown(KeyCode.Joystick3Button0);
                        bButton = Input.GetKeyDown(KeyCode.Joystick3Button1);
                        xButton = Input.GetKeyDown(KeyCode.Joystick3Button2);
                        yButton = Input.GetKeyDown(KeyCode.Joystick3Button3);
                        lBumper = Input.GetKeyDown(KeyCode.Joystick3Button4);
                        rBumper = Input.GetKeyDown(KeyCode.Joystick3Button5);

                        lStickHor = Input.GetAxis("LeftJoy3Horizontal");

                        if (Input.GetAxis("J3RT") > 0.4f)
                        {
                            rTrig = true;
                        }

                        if (Input.GetKeyDown(KeyCode.Joystick3Button0))
                        {
                            slowFall = true;
                        }
                        if (Input.GetKeyUp(KeyCode.Joystick3Button0))
                        {
                            slowFall = false;
                        }
                    }
                }
                break;
            case JoyStick.J4:
                {
                    startButton = Input.GetKeyDown(KeyCode.Joystick4Button7);

                    if (!gmScript.paused)
                    {
                        aButton = Input.GetKeyDown(KeyCode.Joystick4Button0);
                        bButton = Input.GetKeyDown(KeyCode.Joystick4Button1);
                        xButton = Input.GetKeyDown(KeyCode.Joystick4Button2);
                        yButton = Input.GetKeyDown(KeyCode.Joystick4Button3);
                        lBumper = Input.GetKeyDown(KeyCode.Joystick4Button4);
                        rBumper = Input.GetKeyDown(KeyCode.Joystick4Button5);

                        lStickHor = Input.GetAxis("LeftJoy4Horizontal");

                        if (Input.GetAxis("J4RT") > 0.4f)
                        {
                            rTrig = true;
                        }

                        if (Input.GetKeyDown(KeyCode.Joystick4Button0))
                        {
                            slowFall = true;
                        }
                        if (Input.GetKeyUp(KeyCode.Joystick4Button0))
                        {
                            slowFall = false;
                        }
                    }
                }
                break;
            default:
                break;
        }
    }

    public void OnCollisionStay(Collision other)
    {
        if(other.collider.tag == "Floor")
        {
            grounded = true;
        }
    }

    public void OnCollisionExit(Collision other)
    {
        if (other.collider.tag == "Floor")
        {
            grounded = false;
        }
    }

    //For managing character-weapon collision
    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag.Contains("Weapon"))
        {
            if (other.gameObject.GetComponent<BaseWeaponScript>().canBeCollected)
            {
                StartCoroutine(CollectWeapon(other.gameObject));
                isArmed = true;
            }
        }

        if (other.gameObject.tag == "Floor")
        {
            canExtraJump = true;
        }
    }

    private IEnumerator CollectWeapon(GameObject weapon)
    {
        if(isArmed)
        {
            TossWeapon();
            weaponScript = null;
        }
        equippedWeapon = weapon;
        weaponScript = weapon.GetComponent<BaseWeaponScript>();
        equippedWeapon.GetComponent<Rigidbody>().isKinematic = true;
        weaponScript.canBeCollected = false;

        equippedWeapon.transform.SetParent(weaponSlot.transform);
        equippedWeapon.transform.position = weaponSlot.transform.position;
        equippedWeapon.transform.rotation = Quaternion.Euler(weaponSlot.transform.rotation.eulerAngles * -1);
        equippedWeapon.GetComponent<Collider>().isTrigger = true;

        equippedWeaponName = weaponScript.weaponName;
        equippedWeaponSprite = weaponScript.weaponSprite;

        isArmed = true;
        yield return null;
    }

    public void TossWeapon()
    {
        //called when manually tossing weapon away or when picking up another weapon

        //unparenting weapon
        equippedWeapon.transform.SetParent(null);

        //removing some movement constraints
        equippedWeapon.GetComponent<Rigidbody>().useGravity = true;
        equippedWeapon.GetComponent<Rigidbody>().isKinematic = false;

        //Tossess the weapon away while also telling the weapon not to immediately collide with the character
        StartCoroutine(weaponScript.Flung(gameObject.GetComponent<Collider>()));
        StartCoroutine(weaponScript.PickableCD());
        equippedWeapon.GetComponent<Rigidbody>().AddForce(new Vector3(35, 10, 0), ForceMode.VelocityChange);
        equippedWeapon.GetComponent<Collider>().isTrigger = false;        
        weaponScript.isEquipped = false;
        weaponScript.canBeCollected = false;

        isArmed = false;
    }

    //when hit by a bullet that's moving in the opposite direction, drop speed to zero
    public void GetStopped(float direction)
    {
        if(Mathf.Sign(direction) == Mathf.Sign(internalVel))
        {
            rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.y);
        }
    }
    
    //set how long a character should be stopped upon bullet collision
    public void SetDisablingMovementTime(float duration)
    {   
        if(!canMove)
        {
            duration += duration/2;
        }
        canMove = false;
        moveDisableDuration = duration;             
    }

    #region Health and Lives Methods
    //Makes displayed health stop-lerp to current health
    public IEnumerator UpdateHealth()
    {
        float t = 0;
        int previousHealth = displayedHealth;
        while (t < 1)
        {
            t += Time.deltaTime;
            displayedHealth = (int)Mathf.Lerp(previousHealth, currHealth, 1 - (t - 1) * (t - 1));

            if(t > 1)
            {
                displayedHealth = currHealth;
            }
            yield return null;
        }
    }

    //use this when doing damage
    public void TakeDamage(int damage)
    {
        SetHealth(GetHealth() - damage);
    }

    public IEnumerator CharacterDeath()
    {
        alive = false;
        while (displayedHealth != GetHealth())
        {
            yield return null;
        }
        SetRemainingLives(GetRemainingLives() - 1);

        //Replace line below with | play death animation --> teleport out of sight --> wait a bit --> make it spawn on the spawning platform
        gameObject.SetActive(false);
        yield return new WaitForSeconds(2.5f);        
    }

    //Usual getters/setters
    public int GetHealth()
    {
        return currHealth;
    }

    private void SetHealth(int amount)
    {
        currHealth = amount;
        StartCoroutine("UpdateHealth");
    }

    public int GetRemainingLives()
    {
        return livesLeft;
    }

    public void SetRemainingLives(int amount)
    {
        livesLeft = amount;
    }
    #endregion
}
