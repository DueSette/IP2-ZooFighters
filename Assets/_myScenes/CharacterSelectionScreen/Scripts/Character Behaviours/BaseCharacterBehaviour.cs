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

    //Bumpers and triggers
    private bool rBumper;
    private bool lBumperDown;
    private bool lBumperHold;
    private bool lBumperUp;
    private bool rTrig;

    #endregion

    #region General Data
    enum JoyStick { J1, J2, J3, J4 };
    JoyStick jStick;

    private enum CharacterStates { Unarmed, Melee, Pistol, Rifle };
    CharacterStates charState;
    private enum CharTypes { Kangaroo, Cheetah };
    [SerializeField] CharTypes charType;

    private GameManagerScript gmScript;
    public Animator anim;
    public Image characterPointer;
    public Sprite[] pointers = new Sprite[4];

    //Gameplay variables
    public int grenades;
    public Sprite characterSprite;
    public int maxHealth = 100;
    [Tooltip("This represents health but is updated with the UI, meaning more slowly")]
    public int displayedHealth;
    [Tooltip("This is the actual real health of the character, it's updated BEFORE the UI")]
    public int currentHealth;
    public int livesLeft;

    public delegate void LifeLoss();
    public event LifeLoss LifeLossEvent;
    public delegate void SoundDelegate(AudioClip clip);
    public static event SoundDelegate SoundEvent;

    //Base stats
    public bool alive = true;
    public int totalLives;
    public float movSpeed;
    public float jumpPower;
    [Range(25, 85)]
    public float bombTossPower = 25;
    private bool chargingBomb = false;
    public float bodyMass;
    [Tooltip("Set 1 for default damage, go below one for below average damage and vice versa")]
    public float damageMod = 1;
    public float originalMoveSpeed;
    public float slowedMoveSpeed;

    //Weapon related variables
    public bool isArmed = false;
    public Vector2 flingPower;
    [HideInInspector]
    public GameObject equippedWeapon;
    [HideInInspector]
    public Sprite equippedWeaponSprite;
    [HideInInspector]
    public RangedWeaponScript rangedWeaponScript;
    [HideInInspector]
    public MeleeWeaponScript meleeWeaponScript;
    //public MeleeWeaponScript
    [Tooltip("The game object that the weapon will be childed too")]
    public GameObject weaponSlot;
    public GameObject meleeObject;
    public Transform grenadeThrower;
    public Animator charPlasmaAnim;

    public bool canSlap = true;

    public bool respawned = false;

    //Respawn Platform Prefab
    public GameObject respawnPlatform;

    //Weapons list data
    public GameObject[] weaponArray;
    public GameObject equippedWeaponInventory;

    protected Dictionary<string, GameObject> weaponDictionary = new Dictionary<string, GameObject>();

    //Physics related variables
    protected bool slowFall;
    [SerializeField]
    protected bool canMove = true;
    [SerializeField]
    protected bool stunned = false;
    protected bool grounded = true;
    [HideInInspector]
    public bool canExtraJump = true;
    protected bool coyoteOverride = false;
    protected bool drag = false;

    protected bool slapping = false;
    [HideInInspector]
    public float moveDisableDuration = 0;
    [HideInInspector]
    public float moveDisableTimeElapsed = 0;

    public AudioClip[] audioClips;
    AudioSource aud;

    Rigidbody rb;
    [SerializeField]
    SkinnedMeshRenderer outfit;
    [SerializeField]
    Material[] outfitType = new Material[4];
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
                    //jStick = (JoyStick)joyNum;
                }
                break;
            default:
                break;
        }
        characterPointer.sprite = pointers[joyNum];
        characterPointer.enabled = true;
    }

    public virtual void Start()
    {
        gmScript = GameManagerScript.gmInstance;
        rb = GetComponent<Rigidbody>();
        rb.mass = bodyMass;
        aud = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        anim.SetBool("Unarmed", true);

        originalMoveSpeed = movSpeed;
        slowedMoveSpeed = movSpeed / 2;

        //Sets outfit color to the color associated with the joystick
        DefineMaterial(charType);

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
            { }

            //TOSSING GRENADE
            if (lBumperDown && !chargingBomb && grenades > 0 && !stunned)
            {
                chargingBomb = true;
                SoundEvent(audioClips[5]);
            }
            else if (lBumperHold && chargingBomb)
            {
                if (bombTossPower <= 80)
                    bombTossPower += Time.deltaTime * 35;
            }
            else if (lBumperUp && chargingBomb)
            {
                grenades--;
                ThrowGrenade(bombTossPower);
                bombTossPower = 25;
            }

            if (startButton)
            {
                StartCoroutine(gmScript.TogglePause());
            }
            if (rTrig)
            {
                //SLAPPING
                if (!isArmed && canSlap && !anim.GetBool("IsSlapping") && !stunned)
                {
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
                else if (isArmed && rangedWeaponScript != null && rangedWeaponScript.canShoot && !stunned)
                {
                    if (rangedWeaponScript.canShoot) //when there's ammo left
                    {
                        rangedWeaponScript.Fire(damageMod, Mathf.Sign(transform.rotation.y));
                        // anim.SetTrigger("Shoot");
                    }
                }

                //SWINGING MELEE
                else if (isArmed && meleeWeaponScript != null && meleeWeaponScript.canSwing && !stunned)
                {
                    StartCoroutine(MeleeHit());
                }
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

    //Physics
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
                rb.AddForce(Physics.gravity * 2);
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
                        lBumperDown = Input.GetKeyDown(KeyCode.Joystick1Button4);
                        lBumperHold = Input.GetKey(KeyCode.Joystick1Button4);
                        lBumperUp = Input.GetKeyUp(KeyCode.Joystick1Button4);
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
                        lBumperDown = Input.GetKeyDown(KeyCode.Joystick2Button4);
                        lBumperHold = Input.GetKey(KeyCode.Joystick2Button4);
                        lBumperUp = Input.GetKeyUp(KeyCode.Joystick2Button4);
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
                        lBumperDown = Input.GetKeyDown(KeyCode.Joystick3Button4);
                        lBumperHold = Input.GetKey(KeyCode.Joystick3Button4);
                        lBumperUp = Input.GetKeyUp(KeyCode.Joystick3Button4);
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
                        lBumperDown = Input.GetKeyDown(KeyCode.Joystick4Button4);
                        lBumperHold = Input.GetKey(KeyCode.Joystick4Button4);
                        lBumperUp = Input.GetKeyUp(KeyCode.Joystick4Button4);
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
        grounded = false;

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(new Vector3(0, jumpPower, 0), ForceMode.Impulse);

        StartCoroutine(CoyoteOverride());
        anim.SetTrigger("Jump");
        SoundEvent(audioClips[3]);
    }

    //For managing air jump availability
    public void OnCollisionEnter(Collision other)
    {
        //when touching a platform (but not with the "head" of the character)
        if (other.gameObject.tag.Contains("Floor") && other.GetContact(0).point.y < transform.position.y)
        {
            canExtraJump = true;
        }
    }

    //grounded checking
    public void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag.Contains("Floor") && other.GetContact(0).point.y < transform.position.y)
        {
            grounded = true;

        }
    }

    //Coyote time activation
    public void OnCollisionExit(Collision other)
    {
        if (other.collider.tag.Contains("Floor") && !coyoteOverride)
        {
            StartCoroutine(CoyoteTime());
        }
        else if (coyoteOverride)
        {
            grounded = false;
        }
    }

    //It's literally just coyote time
    private IEnumerator CoyoteTime()
    {
        yield return new WaitForSeconds(0.15f);
        grounded = false;
    }

    private IEnumerator CoyoteOverride()
    {
        coyoteOverride = true;
        yield return new WaitForSeconds(0.05f);
        coyoteOverride = false;
    }

    //For managing CHARACTER-WEAPON collisions
    public void OnTriggerStay(Collider coll)
    {
        //Colliding with a weapon
        if (coll.gameObject.layer == 11 && !isArmed && alive)
        {
            //Ranged weapon
            if (coll.gameObject.GetComponent<RangedWeaponScript>() != null && coll.gameObject.GetComponent<RangedWeaponScript>().canBeCollected)
            {
                ActivateWeapon(coll.name);
                StartCoroutine(CollectWeapon(coll.gameObject));

                isArmed = true;
                anim.SetBool("Unarmed", false);
                anim.SetBool("Melee", false);
                anim.SetBool("Rifle", true);
                anim.SetBool("Melee2", false);

            }

            //Melee weapon
            else if (coll.gameObject.GetComponent<MeleeWeaponScript>() != null && coll.gameObject.GetComponent<MeleeWeaponScript>().canBeCollected)
            {
                ActivateWeapon(coll.name);
                StartCoroutine(CollectWeapon(coll.gameObject));

                isArmed = true;
                anim.SetBool("Rifle", false);

                if (coll.gameObject.tag == "Lightsaber")
                {
                    anim.SetBool("Unarmed", false);
                    anim.SetBool("Melee", false);
                    anim.SetBool("Melee2", true);
                }
                else
                {
                    anim.SetBool("Unarmed", false);
                    anim.SetBool("Melee", true);
                    anim.SetBool("Melee2", false);
                }
            }
        }
        //Grenade
        else if (coll.gameObject.layer == 13 && coll.gameObject.GetComponent<GrenadePickUpScript>() != null && grenades < 9)
        {
            Destroy(coll.transform.parent.gameObject);
            grenades = grenades + 3 <= 9 ? grenades + 3 : 9;
            SoundEvent(audioClips[7]);
        }
    }

    //Happens when calling RT while unarmed
    private IEnumerator Slap(bool inAir)
    {
        canSlap = false;
        anim.SetBool("IsSlapping", true);

        if (!inAir)
            slapping = true; //remove these if you want to have people move while slapping

        yield return new WaitForSeconds(0.20f);
        meleeObject.SetActive(true);
        SoundEvent(meleeObject.GetComponent<MeleeObjectScript>().audioClips[2]);
        yield return new WaitForSeconds(0.35f);
        meleeObject.SetActive(false);
        //anim.SetBool("IsSlapping", false);

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
        if (equippedWeapon.tag == "Lightsaber")
        {
            yield return new WaitForSeconds(0.18f);
        }
        else if (equippedWeapon.tag == "BaseballBat")
        {
            yield return new WaitForSeconds(0.18f);
        }

        yield return new WaitForSeconds(0.20f);
        if (meleeObject != null)
            meleeObject.SetActive(true);

        anim.SetBool("IsSlapping", false);

        yield return new WaitForSeconds(0.33f);

        if (meleeObject != null)
            meleeObject.SetActive(false);

        yield return null;
    }

    private void ThrowGrenade(float power)
    {
        chargingBomb = false;
        GameObject grenade = ObjectPooler.instance.SpawnFromPool("Grenade", grenadeThrower.position, Quaternion.identity);
        GrenadeScript gScript = grenade.GetComponent<GrenadeScript>();

        gScript.grenadeSpeed.x = power;
        gScript.direction = Mathf.Sign(transform.rotation.y);
        gScript.shooterCollider = GetComponent<Collider>();
        gScript.AfterEnable();
        SoundEvent(audioClips[4]);
    }

    #region Weapon Specific Functions

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

            if (rangedWeaponScript.anim != null)
                rangedWeaponScript.anim.enabled = false;

            equippedWeaponSprite = rangedWeaponScript.weaponSprite;

            if (weapon.tag == "Asparagun")
                SoundEvent(audioClips[2]);
            else if (weapon.tag == "PlasmaRifle")
            {
                SoundEvent(audioClips[6]);
                rangedWeaponScript.GetComponent<PlasmaGunScript>().ToggleEmissionObjects(false);
            }
        }

        else if (weapon.GetComponent<MeleeWeaponScript>())
        {
            meleeWeaponScript = weapon.GetComponent<MeleeWeaponScript>();
            meleeWeaponScript.isEquipped = true;
            meleeWeaponScript.canBeCollected = false;
            meleeWeaponScript.weaponHolderCollider = GetComponent<Collider>();
            equippedWeaponSprite = meleeWeaponScript.weaponSprite;
            SoundEvent(meleeWeaponScript.audioClips[0]);
            if (weapon.tag == "Lightsaber")
            {
                weapon.GetComponent<Animator>().enabled = true;
                meleeWeaponScript.GetComponent<AudioSource>().loop = true;
                meleeWeaponScript.GetComponent<AudioSource>().Play();
            }
        }

        equippedWeapon.GetComponent<Rigidbody>().isKinematic = true;

        //Reset their velocity, rotational velocity and rotation. Looks strange but prevents a bug that would otherwise pop every time.
        equippedWeapon.GetComponent<Rigidbody>().velocity = Vector3.zero;
        equippedWeapon.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        equippedWeapon.transform.rotation = Quaternion.Euler(Vector3.zero);

        equippedWeapon.transform.SetParent(weaponSlot.transform);
        equippedWeapon.transform.position = weaponSlot.transform.position;
        equippedWeapon.transform.rotation = Quaternion.Euler(weaponSlot.transform.rotation.eulerAngles * -1);


        if (equippedWeapon.GetComponent<MeshRenderer>())
            equippedWeapon.GetComponent<MeshRenderer>().enabled = false;
        else
        {
            MeshRenderer[] renderers = equippedWeapon.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer rend in renderers)
            {
                rend.enabled = false;
            }
        }

        equippedWeaponInventory.SetActive(true);
        isArmed = true;

        yield return null;
    }

    //Activates weapon from dictionary
    private void ActivateWeapon(string weaponName)
    {
        if (!weaponDictionary.ContainsKey("my" + weaponName))
        {
            print("don't have any my" + weaponName + " here");
        }
        else
        {
            weaponDictionary["my" + weaponName].SetActive(true);
            equippedWeaponInventory = weaponDictionary["my" + weaponName];
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
        if (equippedWeapon.GetComponent<MeshRenderer>())
            equippedWeapon.GetComponent<MeshRenderer>().enabled = true;
        else
        {
            MeshRenderer[] renderers = equippedWeapon.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer rend in renderers)
            {
                rend.enabled = true;
            }
        }
        /*
        if (equippedWeapon.tag == "PlasmaRifle")
            rangedWeaponScript.anim.enabled = true;
            */
        if (equippedWeapon.tag == "Lightsaber")
        {
            equippedWeapon.GetComponent<Animator>().SetTrigger("Off");
            SoundEvent(equippedWeapon.GetComponent<MeleeWeaponScript>().audioClips[6]);
        }

        //Animator stuff
        anim.SetBool("Unarmed", true);
        anim.SetBool("Melee", false);
        anim.SetBool("Melee2", false);
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

            Flush();

            //unparenting weapon and updating physics constraints
            equippedWeapon.transform.SetParent(null);
            equippedWeapon.GetComponent<Rigidbody>().isKinematic = false;
            equippedWeapon.GetComponent<Rigidbody>().useGravity = false;
            equippedWeapon.transform.rotation = Quaternion.Euler(equippedWeapon.transform.rotation.x, 90, equippedWeapon.transform.rotation.z); //resetting rotation            

            equippedWeaponInventory.SetActive(false);

            if (equippedWeapon.GetComponent<MeshRenderer>())
                equippedWeapon.GetComponent<MeshRenderer>().enabled = true;
            else
            {
                MeshRenderer[] renderers = equippedWeapon.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer rend in renderers)
                {
                    rend.enabled = true;
                }
            }

            equippedWeapon.GetComponent<Rigidbody>().AddForce(new Vector3(80 * Mathf.Sign(transform.rotation.y), 0, 0), ForceMode.VelocityChange);

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
            anim.SetBool("Melee2", false);
            /*
            if(equippedWeapon.tag == "PlasmaRifle")
                rangedWeaponScript.anim.enabled = true;
                */
            if (equippedWeapon.tag == "Lightsaber")
            {
                SoundEvent(equippedWeapon.GetComponent<MeleeWeaponScript>().audioClips[5]);
                equippedWeapon.GetComponent<Rigidbody>().AddTorque(transform.up * 50 * Mathf.Sign(transform.rotation.x), ForceMode.VelocityChange);
            }
            else
            {
                SoundEvent(audioClips[1]);
                equippedWeapon.GetComponent<Rigidbody>().AddTorque(transform.right * 27 * Mathf.Sign(transform.rotation.y), ForceMode.VelocityChange);
            }
            anim.SetTrigger("Throw");
        }
    }

    //Prepares the weapon when it is about to be thrown
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
    #endregion

    //when hit by a bullet that's moving in the opposite direction, drop speed to zero
    public void GetStopped(float direction)
    {
        if (Mathf.Sign(direction) == Mathf.Sign(transform.rotation.y))
        {
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
        if (gmScript.GetGameState() == GameManagerScript.GameState.inGame)
        {
            SetHealth(GetHealth() - damage);
            anim.SetTrigger("GetRekt");
        }
    }

    public IEnumerator CharacterDeath()
    {
        StartCoroutine(CameraScript.instance.SetShakeTime(0.5f, 6, 1.5f));
        SoundEvent(audioClips[0]);
        alive = false;
        anim.SetTrigger("Died");
        equippedWeaponSprite = null;
        while (displayedHealth != GetHealth())
        {
            yield return null;
        }
        SetRemainingLives(GetRemainingLives() - 1);
        LifeLossEvent();

        //DEACTIVATION PROCESS
        if (isArmed)
        {
            isArmed = false;
            anim.SetBool("Unarmed", true);
            anim.SetBool("Melee", false);
            anim.SetBool("Rifle", false);
            anim.SetBool("Melee2", false);

            equippedWeaponInventory.SetActive(false);
            Destroy(equippedWeapon);
        }

        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rb.isKinematic = true;

        //Play death animation, when it's over execute lines below
        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(0.3f);
        if (GetRemainingLives() > 0)
        {
            StartCoroutine(CharacterRespawn());
        }
        else
        {
            transform.position = new Vector3(100, 100, 0);
            grenades = 0;
            anim.SetBool("CanRespawn", false);
        }
    }

    public IEnumerator CharacterRespawn()
    {
        transform.position = new Vector3(100, 100, 0); //teleport the character away from view for a while
        SetHealth(maxHealth);
        yield return StartCoroutine(UpdateHealth()); //wait until health bar is visually recharged

        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        yield return new WaitForSeconds(0.27f);

        transform.position = gmScript.respawnLocations[(int)jStick].position;
        gmScript.respawnPlatforms[(int)jStick].SetActive(true);

        grenades = 2;
        canExtraJump = true;
        alive = true;
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

    #endregion

    public void PauseFrames(float time)
    {
        StartCoroutine(SkipFrames(time));
    }

    private IEnumerator SkipFrames(float time)
    {
        GetComponent<Animator>().speed = 0.1f;
        yield return new WaitForSeconds(time);
        GetComponent<Animator>().speed = 1;
    }

    private void DefineMaterial(CharTypes type)
    {
        Material[] mats = outfit.materials;

        switch (type)
        {
            case CharTypes.Kangaroo:
                mats[0] = outfitType[(int)jStick];
                break;
            case CharTypes.Cheetah:
                 mats[1] = outfitType[(int)jStick];
                break;
            default:
                break;
        }
        outfit.materials = mats;
    }
    #region Miro's Additions
    /*
    void OnParticleCollision(GameObject Darts)
    {
        TakeDamage(2);
    }

    void OnTriggerEnter(Collider heart)
    {
        if (heart.name == "Heart")
        {
            SetHealth(100);
        }
    }
    */
    #endregion
}
