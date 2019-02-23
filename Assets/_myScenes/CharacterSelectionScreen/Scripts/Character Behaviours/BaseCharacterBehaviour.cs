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

    public CharacterController controller;
    private GameManagerScript gmScript;

    //Gameplay variables
    public int maxHealth = 100;
    [Tooltip("This represents health but is updated with the UI, meaning more slowly")]
    public int displayedHealth;
    [Tooltip("This is the actual real health of the character, it's updated BEFORE the UI")]
    public int currHealth;

    //Base stats
    public int totalLives;
    public float movSpeed;
    public float jumpPower;
    public float bodyMass;
    private float fallingMass;
    [Tooltip("Set 1 for default damage, go below one for below average damage and viceversa")]
    public float damageMod = 1;

    //Additional variables
    public int livesLeft;
    public bool isArmed = false; 
    public Sprite equippedWeaponSprite;
    public string equippedWeaponName;
    private BaseWeaponScript weaponScript;
    public GameObject weaponSlot;

    public float internalVel;
    [SerializeField]
    private float maxInternalHspeed = 20;
    [SerializeField]
    private float minInternalHspeed = -20;
    public bool canMove = true;

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
        fallingMass = bodyMass * 3;
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
    }

    //Define here all the actual functions (shoot, jump, etc)
    //Also update HP here
    public virtual void Update()
    {
        internalVel = GetComponent<Rigidbody>().velocity.x;

        CheckInput();
        
        //=====COMBAT GAME FUNCTIONS
        if (gmScript.GetGameState() == GameManagerScript.GameState.inGame)
        {
            if (aButton)
            {
                Jump();
            }
            if (bButton)
            {
                print("b pressed");
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

            if (lStickHor != 0)
            {
                Move(lStickHor);
            }

        }
    }

    public bool slowFall;

    public virtual void FixedUpdate()
    {
        if(rb.velocity.y < 0)
        {
            rb.AddForce(Physics.gravity * 3);
        }

        if(!CheckIfGrounded())
        {
            print("notgrounded");

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
        //transform.Translate(new Vector3(1 * Mathf.Sign(stickDirection), 0, 0) * Time.deltaTime * movSpeed, Space.World);
        //can't move after 
        print("movin");
        if (internalVel < maxInternalHspeed && internalVel > minInternalHspeed)
        {
            if (stickDirection != 0 && canMove)
            {
                transform.Translate(new Vector3(1 * Mathf.Sign(stickDirection), 0, 0) * Time.deltaTime * movSpeed, Space.World);
                //GetComponent<Rigidbody>().AddForce(new Vector3(10 * Mathf.Sign(stickDirection), 0, 0) * Time.deltaTime * movSpeed, ForceMode.Impulse);
            }
        }

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
        GetComponent<Rigidbody>().AddForce(new Vector3(0, jumpPower, 0), ForceMode.Impulse);
    }

    private bool CheckIfGrounded()
    {
        RaycastHit hit;
        Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z), Vector3.down, out hit);
        
        if(hit.distance < 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Performs some form of command pattern in relation to the current JoyStick Enum and GameState, checking which buttons have been pressed
    public void CheckInput()
    {
        aButton = false;
        bButton = false;
        xButton = false;
        yButton = false;
        lBumper = false;
        rBumper = false;
        startButton = false;
        lStickHor = 0;
        rStickVer = 0;
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
                    }
                }
                break;
            default:
                break;
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag.Contains("Weapon"))
        {
            CollectWeapon(other.gameObject);
        }
    }

    private void CollectWeapon(GameObject weapon)
    {
         weaponScript = weapon.GetComponent<BaseWeaponScript>();
        //position weapon correctly
        //tell the character everything it needs to know about the current weapon
        weapon.transform.SetParent(weaponSlot.transform);
        weapon.transform.position = weaponSlot.transform.position;
        weapon.GetComponent<Collider>().isTrigger = true;

        equippedWeaponName = weaponScript.weaponName;
        equippedWeaponSprite = weaponScript.weaponSprite;

        isArmed = true;
    }

    public void TossWeapon()
    {
        //called when manually tossing weapon away or when picking up another weapon
    }

    public void GetStopped(float direction)
    {
        if(Mathf.Sign(direction) == Mathf.Sign(internalVel))
        {
            rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.y);
            print("stop");
        }
    }

    public IEnumerator DisableMove(float duration)
    {
        print("disabling movement");
        if (!canMove)
        {
            duration /= 3;
        }
        canMove = false;

        yield return StartCoroutine(CoolDown(duration));
        
        canMove = true;
        print("can move now");
    }

    private IEnumerator CoolDown(float num)
    {
        yield return new WaitForSeconds(num);
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

    public void TakeDamage(int damage)
    {
        SetHealth(GetHealth() - damage);
    }

    //Usual getters/setters
    public int GetHealth()
    {
        return currHealth;
    }

    public void SetHealth(int amount)
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
