using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //Bumpers and triggers
    private bool rBumper;
    private bool lBumper;
    private bool rTrig;

    #endregion

    #region General Data
    enum JoyStick { J1, J2, J3, J4 };
    JoyStick jStick;

    private enum CharacterStates { Unarmed, Melee, Pistol, Rifle };
    CharacterStates charState;

    private GameManagerScript gmScript;
    public Animator anim;

    //Gameplay variables
    public Sprite characterSprite;
    public int maxHealth = 100;
    [Tooltip("This represents health but is updated with the UI, meaning more slowly")]
    public int displayedHealth;
    [Tooltip("This is the actual real health of the character, it's updated BEFORE the UI")]
    public int currentHealth;
    public int livesLeft;

    //Base stats
    public bool alive = true;
    public int totalLives;
    public float movSpeed;
    public float jumpPower;
    public float bodyMass;
    [Tooltip("Set 1 for default damage, go below one for below average damage and vice versa")]
    public float damageMod = 1;

    //Weapon related variables
    public bool isArmed = false;
    public Vector2 flingPower;
    private GameObject equippedWeapon;
    public Sprite equippedWeaponSprite;
    public RangedWeaponScript rangedWeaponScript;
    public MeleeWeaponScript meleeWeaponScript;
    //public MeleeWeaponScript
    [Tooltip("The game object that the weapon will be childed too")]
    public GameObject weaponSlot;
    public GameObject meleeObject;

    public bool rTrigReleased = true;
    public bool canSlap = true;

    //Weapons list data
    public GameObject[] weaponArray;
    public GameObject equippedWeaponInventory;

    protected Dictionary<string, GameObject> weaponDictionary = new Dictionary<string, GameObject>();

    //Physics related variables
    protected bool slowFall;
    [SerializeField]
    protected bool canMove = true;
    [SerializeField]
    protected bool stunned;
    protected bool grounded = true;
    protected bool canExtraJump = true;
    protected bool drag = false;

    protected bool slapping = false;
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
        print("joystick num \"" + joyNum + "\" selected its character");

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
        rb = GetComponent<Rigidbody>();
        rb.mass = bodyMass;
        anim = GetComponent<Animator>();
        anim.SetBool("Unarmed", true);

        //equippedWeaponInventory.SetActive(false); //this might come in handy but is unnecessary as of now

        //Inventory system initialisation
        foreach (GameObject weapon in weaponArray)
        {
            if (weapon != null)
            {
                weapon.SetActive(false);
                weaponDictionary.Add(weapon.name, weapon);
            }
        }
    }

    //Define here all the actual commands (shoot, jump, etc)
    //Also update HP here
    public virtual void Update()
    {
        anim.SetBool("isGrounded", grounded);

        CheckInput();

        //Checking if character is still alive
        if (GetHealth() <= 0 && alive)
        {
            StartCoroutine(CharacterDeath());
        }

        //Movement disabling/enabling algorithm
        if (!canMove)
        {
            moveDisableTimeElapsed += Time.deltaTime;
            if (moveDisableTimeElapsed >= moveDisableDuration)
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
                if (grounded && !stunned)
                {
                    Jump();
                }
                else if (canExtraJump && !stunned)
                {
                    Jump();
                    canExtraJump = false;
                }
            }
            if (bButton && isArmed && !stunned)
            {
                if (rangedWeaponScript != null && rangedWeaponScript.ammo < 1)
                {
                    StartCoroutine(TossWeaponEmpty());
                }
                else
                {
                    StartCoroutine(TossWeapon());
                }
            }
            if (xButton && isArmed && !stunned)
            {
                StartCoroutine(TossWeaponEmpty());
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
                rTrigReleased = false;

                //SLAPPING
                if (!isArmed && canSlap && !anim.GetBool("Slapping") && !stunned)
                {
                    print("slapped");
                    canSlap = false;
                    if (grounded)
                    {
                        slapping = true;
                        StartCoroutine(Slap(false));
                    }
                    else
                    {
                        StartCoroutine(Slap(true));
                    }
                }

                //SHOOTING
                else if (isArmed && rangedWeaponScript != null && rangedWeaponScript.canShoot && rangedWeaponScript.ammo > 0 && !stunned)
                {
                    rangedWeaponScript.Fire(damageMod, Mathf.Sign(transform.rotation.y));
                    anim.SetTrigger("Shoot");
                }
                //SWINGING MELEE
                else if (isArmed && meleeWeaponScript != null && meleeWeaponScript.canSwing && !stunned)
                {
                    StartCoroutine(MeleeHit());
                }
            }
            else
            {
                rTrigReleased = true;
                //anim.SetBool("IsSlapping", false);
            }

            if (lStickHor != 0 && canMove && !slapping)
            {
                Move(lStickHor);
            }
            else
            {
                anim.SetBool("isRunning", false);
                anim.SetBool("isIdle", true);
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
            if (slowFall && rb.velocity.y > 0)
            {
                rb.AddForce(Physics.gravity);
            }
            else
            {
                rb.AddForce(Physics.gravity * 2.5f);
            }
        }

    }

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

                        if (Input.GetKeyDown(KeyCode.Joystick1Button0))
                        {
                            slowFall = true;
                        }
                        if (Input.GetKeyUp(KeyCode.Joystick1Button0))
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

    //Happens when calling RT while unarmed
    private IEnumerator Slap(bool inAir)
    {
        canSlap = false;
        anim.SetBool("IsSlapping", true);

        if (!inAir)
            slapping = true;

        yield return new WaitForSeconds(0.20f);
        meleeObject.SetActive(true);
        yield return new WaitForSeconds(0.35f);
        meleeObject.SetActive(false);
        //anim.SetBool("IsSlapping", false);

        //yield return new WaitUntil(() => rTrigReleased == true);
        slapping = false;
        anim.SetBool("IsSlapping", false);
        yield return new WaitForSeconds(0.5f);
        canSlap = true;
    }

    private IEnumerator MeleeHit()
    {
        //anim.SetBool("swinging", true);
        meleeWeaponScript.Swing();  //Just sets the cooldown
        anim.SetBool("IsSlapping", true);
        yield return new WaitForSeconds(0.20f);
        if (meleeObject != null)
            meleeObject.SetActive(true);

        yield return new WaitForSeconds(0.40f);

        if (meleeObject != null)
            meleeObject.SetActive(false);
        //anim.SetBool("swinging", false);
        anim.SetBool("IsSlapping", false);

        yield return null;
    }

    //private IEnumerator 

    public virtual void Move(float stickDirection)
    {
        transform.Translate(new Vector3(1 * Mathf.Sign(stickDirection), 0, 0) * Time.deltaTime * movSpeed, Space.World);
        anim.SetBool("isRunning", true);
        anim.SetBool("isIdle", false);


        if (Mathf.Sign(stickDirection) == rb.velocity.x)
        {
            drag = true;
        }

        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 90 * Mathf.Sign(stickDirection), transform.rotation.z));
    }

    public virtual void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(new Vector3(0, jumpPower, 0), ForceMode.Impulse);
        anim.SetTrigger("Jump");
        grounded = false;
    }

    public void OnCollisionStay(Collision other)
    {
        if (other.collider.tag == "Floor" && other.transform.position.y < transform.position.y)
        {
            grounded = true;
        }
    }

    public void OnCollisionExit(Collision other)
    {
        if (other.collider.tag == "Floor")
        {
            StartCoroutine(CoyoteTime());
        }
    }

    //It's literally just coyote time
    private IEnumerator CoyoteTime()
    {
        yield return new WaitForSeconds(0.1f);
        grounded = false;
    }

    //For managing CHARACTER-WEAPON collisions
    public void OnTriggerStay(Collider coll)
    {
        //Colliding with a weapon
        if (coll.gameObject.layer == 11 && !isArmed)
        {
            if (coll.gameObject.GetComponent<RangedWeaponScript>() != null && coll.gameObject.GetComponent<RangedWeaponScript>().canBeCollected)
            {
                ActivateWeapon(coll.name);
                StartCoroutine(CollectWeapon(coll.gameObject));

                isArmed = true;
                anim.SetBool("Unarmed", false);
                anim.SetBool("Melee", false);
                anim.SetBool("Rifle", true);
            }

            else if (coll.gameObject.GetComponent<MeleeWeaponScript>() != null && coll.gameObject.GetComponent<MeleeWeaponScript>().canBeCollected)
            {
                ActivateWeapon(coll.name);
                StartCoroutine(CollectWeapon(coll.gameObject));

                isArmed = true;
                anim.SetBool("Unarmed", true);
                anim.SetBool("Melee", false);   //ofc this should be true but animations are not ready yet
                anim.SetBool("Rifle", false);
            }
        }
    }

    private void ActivateWeapon(string weaponName)
    {
        if (!weaponDictionary.ContainsKey("my" + weaponName))
        {
            print("don't have any " + "my" + weaponName + " here");
        }
        else
        {
            weaponDictionary["my" + weaponName].SetActive(true);
            equippedWeaponInventory = weaponDictionary["my" + weaponName];
        }
    }

    //For managing CHARACTER-GROUND collisions
    public void OnCollisionEnter(Collision other)
    {
        //when touching a platform (but not with the "head" of the character)
        if (other.gameObject.tag == "Floor" && other.gameObject.transform.position.y < transform.position.y)
        {
            canExtraJump = true;
        }
    }

    //Manages the process of picking up a weapon from the ground
    private IEnumerator CollectWeapon(GameObject weapon)
    {       
        equippedWeapon = weapon;

        if (weapon.GetComponent<RangedWeaponScript>())
        {
            rangedWeaponScript = weapon.GetComponent<RangedWeaponScript>();
            rangedWeaponScript.isEquipped = true;
            rangedWeaponScript.canBeCollected = false;
            rangedWeaponScript.weaponHolderCollider = GetComponent<Collider>();
            equippedWeaponSprite = rangedWeaponScript.weaponSprite;
        }

        else if (weapon.GetComponent<MeleeWeaponScript>())
        {
            meleeWeaponScript = weapon.GetComponent<MeleeWeaponScript>();
            meleeWeaponScript.isEquipped = true;
            meleeWeaponScript.canBeCollected = false;
            meleeWeaponScript.weaponHolderCollider = GetComponent<Collider>();
            equippedWeaponSprite = meleeWeaponScript.weaponSprite;
        }

        equippedWeapon.GetComponent<Rigidbody>().isKinematic = true;

        //Reset their velocity, rotational velocity and rotation. Looks strange but prevents a bug that would otherwise pop every time.
        equippedWeapon.GetComponent<Rigidbody>().velocity = Vector3.zero;
        equippedWeapon.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        equippedWeapon.transform.rotation = Quaternion.Euler(Vector3.zero);

        equippedWeapon.transform.SetParent(weaponSlot.transform);
        equippedWeapon.transform.position = weaponSlot.transform.position;
        equippedWeapon.transform.rotation = Quaternion.Euler(weaponSlot.transform.rotation.eulerAngles * -1);

        equippedWeapon.GetComponent<MeshRenderer>().enabled = false;

        equippedWeaponInventory.SetActive(true);
        isArmed = true;
        yield return null;
    }

    private void Flush()
    {
        if (equippedWeapon.GetComponent<RangedWeaponScript>())
        {
            StartCoroutine(rangedWeaponScript.Flung(gameObject.GetComponent<Collider>()));
            rangedWeaponScript.weaponHolderCollider = null;

            rangedWeaponScript.isEquipped = false;
            rangedWeaponScript.canBeCollected = false;
        }

        else if (equippedWeapon.GetComponent<MeleeWeaponScript>())
        {
            StartCoroutine(meleeWeaponScript.Flung(gameObject.GetComponent<Collider>()));
            meleeWeaponScript.weaponHolderCollider = null;

            meleeWeaponScript.isEquipped = false;
            meleeWeaponScript.canBeCollected = false;
        }
    }

    //Called when manually tossing a non-empty weapon away or when picking up another weapon
    public IEnumerator TossWeapon()
    {
        yield return new WaitForSeconds(0.085f);
        //UI info
        equippedWeaponSprite = null;

        //unparenting weapon
        equippedWeapon.transform.SetParent(null);

        //removing some movement constraints
        equippedWeapon.GetComponent<Rigidbody>().isKinematic = false;
        equippedWeapon.transform.rotation = Quaternion.Euler(equippedWeapon.transform.rotation.x, 90, equippedWeapon.transform.rotation.z); //resetting rotation

        //Tossess the weapon away while also telling the weapon not to immediately collide with the character
        Flush();
        equippedWeapon.GetComponent<Rigidbody>().AddForce(new Vector3(flingPower.x * Mathf.Sign(transform.rotation.y), flingPower.y, 0), ForceMode.VelocityChange);
        equippedWeapon.GetComponent<Rigidbody>().AddTorque(transform.right * 27 * Mathf.Sign(transform.rotation.y), ForceMode.VelocityChange);

        if (equippedWeapon.GetComponent<RangedWeaponScript>())
            rangedWeaponScript = null;
        else
            meleeWeaponScript = null;

        isArmed = false;

        equippedWeaponInventory.SetActive(false);
        equippedWeapon.GetComponent<MeshRenderer>().enabled = true;

        //Animator stuff
        anim.SetBool("Unarmed", true);
        anim.SetBool("Melee", false);
        anim.SetBool("Rifle", false);
        anim.SetTrigger("Throw");
    }

    //Called when tossing a weapon with no ammo, which executes a power throw that can damage people
    public IEnumerator TossWeaponEmpty()
    {
        yield return new WaitForSeconds(0.1f);

        if (equippedWeapon != null)
        {
            //UI info
            equippedWeaponSprite = null;

            //unparenting weapon and updating physics constraints
            equippedWeapon.transform.SetParent(null);
            equippedWeapon.GetComponent<Rigidbody>().isKinematic = false;
            equippedWeapon.GetComponent<Rigidbody>().useGravity = false;
            equippedWeapon.transform.rotation = Quaternion.Euler(equippedWeapon.transform.rotation.x, 90, equippedWeapon.transform.rotation.z); //resetting rotation

            Flush();

            equippedWeaponInventory.SetActive(false);

            equippedWeapon.GetComponent<MeshRenderer>().enabled = true;
            equippedWeapon.GetComponent<Rigidbody>().AddForce(new Vector3(80 * Mathf.Sign(transform.rotation.y), 0, 0), ForceMode.VelocityChange);
            equippedWeapon.GetComponent<Rigidbody>().AddTorque(transform.right * 27 * Mathf.Sign(transform.rotation.y), ForceMode.VelocityChange);

            if (equippedWeapon.GetComponent<RangedWeaponScript>())
            {
                rangedWeaponScript.actAsBullet = true;
                rangedWeaponScript = null;
            }
            else
            {
                meleeWeaponScript.actAsBullet = true;
                meleeWeaponScript = null;
            }
            isArmed = false;

            //Animator stuff
            anim.SetBool("Unarmed", true);
            anim.SetBool("Melee", false);
            anim.SetBool("Rifle", false);

            anim.SetTrigger("Throw");
        }
    }

    //when hit by a bullet that's moving in the opposite direction, drop speed to zero
    public void GetStopped(float direction)
    {
        if (Mathf.Sign(direction) == Mathf.Sign(transform.rotation.y))
        {
            print("get stopped method called");
            rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.y);
        }
    }

    //set how long a character should be stopped upon bullet collision
    public void SetDisablingMovementTime(float duration)
    {
        if (!canMove)
        {
            duration += duration / 2;
        }
        canMove = false;
        moveDisableDuration = duration;
    }

    public IEnumerator SetStun(float time)
    {
        stunned = true;
        yield return new WaitForSeconds(time);
        stunned = false;
    }

    #region Health and Lives Methods

    Coroutine updateHealth = null;

    //Makes displayed health stop-lerp to current health
    public IEnumerator UpdateHealth()
    {
        float t = 0;
        int previousHealth = displayedHealth;
        while (t < 1)
        {
            t += Time.deltaTime;
            displayedHealth = (int)Mathf.Lerp(previousHealth, currentHealth, 1 - (t - 1) * (t - 1));

            if (t > 1)
            {
                displayedHealth = currentHealth;
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
        StartCoroutine(CameraScript.instance.SetShakeTime(0.5f, 6, 1.5f));
        alive = false;
        equippedWeaponSprite = null;
        while (displayedHealth != GetHealth())
        {
            yield return null;
        }
        SetRemainingLives(GetRemainingLives() - 1);

        //DEACTIVATION PROCESS
        if (isArmed)
        {
            isArmed = false;
            anim.SetBool("Unarmed", true);
            anim.SetBool("Melee", false);
            anim.SetBool("Rifle", false);

            equippedWeaponInventory.SetActive(false);
            Destroy(equippedWeapon);
        }
        //anim.SetBool("Dead", true);

        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rb.isKinematic = true;

        //Play death animation, when it's over execute lines below
        //GetComponent<MeshRenderer>().enabled = false;        //this is okay to turn off cubes but models are more complex than this
        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(1.5f);
        if (GetRemainingLives() > -1)
        {
            StartCoroutine(CharacterRespawn());
        }
    }

    public IEnumerator CharacterRespawn()
    {
        SetHealth(maxHealth);

        StartCoroutine(UpdateHealth());
        //position on a respawn platform
        //GetComponent<MeshRenderer>().enabled = true;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        transform.position = new Vector3(-2, 20, 0); //this is temporary
        //make the platform appear on screen
        alive = true;
        //maybe give a jump boost
        //have the platform lerp away/deactivate
        yield return null;
    }

    //Usual getters/setters
    public int GetHealth()
    {
        return currentHealth;
    }

    private void SetHealth(int amount)
    {
        if (updateHealth != null)
        {
            StopCoroutine(updateHealth);
        }
        currentHealth = amount;
        updateHealth = StartCoroutine(UpdateHealth());
    }

    public int GetRemainingLives()
    {
        return livesLeft;
    }

    public void SetRemainingLives(int amount)
    {
        livesLeft = amount;
    }

    public void PauseFrames(float time)
    {
        StartCoroutine(SkipFrames(time));
    }

    private IEnumerator SkipFrames(float time)
    {
        print("pause");
        GetComponent<Animator>().speed = 0.1f;
        yield return new WaitForSeconds(time);
        GetComponent<Animator>().speed = 1;
        print("exit pause");
    }
    #endregion
}
