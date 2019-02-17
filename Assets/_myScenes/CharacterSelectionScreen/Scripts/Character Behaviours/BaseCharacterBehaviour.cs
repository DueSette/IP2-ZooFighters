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

    //Analog sticks
    private float lStickHor;
    private float rStickVer;

    //bumpers and triggers
    private bool rBumper;
    private bool lBumper;
    private bool rTrig;

    #endregion

    private enum JoyStick { J1, J2, J3, J4 };
    JoyStick jStick;

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
    public int damageMod = 1;

    //Additional variables
    public int livesLeft;
    public bool isArmed = false; 
    public Image equippedWeaponImage;
    public string equippedWeaponName;

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
    }

    //Define here all the actual functions (shoot, jump, etc)
    //Also update HP here
    public virtual void Update()
    {
        CheckInput();
        if (gmScript.GetGameState() == GameManagerScript.GameState.inGame)
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                StartCoroutine("UpdateHealth");
            }
        }

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
            if (rTrig)
            {
                print("BLAM");
            }

            if(lStickHor > 0.4f || lStickHor < -0.4f)
            {
                Move(lStickHor);
            }
        }
    }

    public virtual void Move(float stickDirection)
    {
        GetComponent<Rigidbody>().AddForce(new Vector3 (2 * Mathf.Sign(stickDirection), 0, 0) * Time.deltaTime * movSpeed, ForceMode.VelocityChange);
    }

    public virtual void Jump()
    {
        GetComponent<Rigidbody>().AddForce(new Vector3(0, jumpPower, 0), ForceMode.Impulse);
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
        lStickHor = 0;
        rStickVer = 0;
        rTrig = false;

        switch (jStick)
        {
            case JoyStick.J1:
                {
                    aButton = Input.GetKeyDown(KeyCode.Joystick1Button0);
                    bButton = Input.GetKeyDown(KeyCode.Joystick1Button1);
                    xButton = Input.GetKeyDown(KeyCode.Joystick1Button2);
                    yButton = Input.GetKeyDown(KeyCode.Joystick1Button3);
                    lBumper = Input.GetKeyDown(KeyCode.Joystick1Button4);
                    rBumper = Input.GetKeyDown(KeyCode.Joystick1Button5);

                    lStickHor = Input.GetAxis("LeftJoyHorizontal");

                    if(Input.GetAxis("J1RT") > 0.4f)
                    {
                        rTrig = true;
                    }
                }
                break;
            case JoyStick.J2:
                {
                    aButton = Input.GetKeyDown(KeyCode.Joystick2Button0);
                    bButton = Input.GetKeyDown(KeyCode.Joystick2Button1);
                    xButton = Input.GetKeyDown(KeyCode.Joystick2Button2);
                    yButton = Input.GetKeyDown(KeyCode.Joystick2Button3);
                    lBumper = Input.GetKeyDown(KeyCode.Joystick2Button4);
                    rBumper = Input.GetKeyDown(KeyCode.Joystick2Button5);

                    if (Input.GetAxis("J2RT") > 0.4f)
                    {
                        rTrig = true;
                    }
                }
                break;
            case JoyStick.J3:
                {
                    aButton = Input.GetKeyDown(KeyCode.Joystick3Button0);
                    bButton = Input.GetKeyDown(KeyCode.Joystick3Button1);
                    xButton = Input.GetKeyDown(KeyCode.Joystick3Button2);
                    yButton = Input.GetKeyDown(KeyCode.Joystick3Button3);
                    lBumper = Input.GetKeyDown(KeyCode.Joystick3Button4);
                    rBumper = Input.GetKeyDown(KeyCode.Joystick3Button5);

                    if (Input.GetAxis("J3RT") > 0.4f)
                    {
                        rTrig = true;
                    }
                }
                break;
            case JoyStick.J4:
                {
                    aButton = Input.GetKeyDown(KeyCode.Joystick4Button0);
                    bButton = Input.GetKeyDown(KeyCode.Joystick4Button1);
                    xButton = Input.GetKeyDown(KeyCode.Joystick4Button2);
                    yButton = Input.GetKeyDown(KeyCode.Joystick4Button3);
                    lBumper = Input.GetKeyDown(KeyCode.Joystick4Button4);
                    rBumper = Input.GetKeyDown(KeyCode.Joystick4Button5);

                    if (Input.GetAxis("J4RT") > 0.4f)
                    {
                        rTrig = true;
                    }
                }
                break;
            default:
                break;
        }
    }

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

    //Usual getter/setter
    public int GetHealth()
    {
        return currHealth;
    }

    public void SetHealth(int amount)
    {
        currHealth = amount;
        StartCoroutine("UpdateHealth");
    }
}
