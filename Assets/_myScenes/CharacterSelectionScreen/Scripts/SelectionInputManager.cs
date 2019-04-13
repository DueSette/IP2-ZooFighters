using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SelectionInputManager : MonoBehaviour
{
    [Tooltip("The object that will represent each players' symbol when choosing a character and navigating the character selection menu")]
    public GameObject selector;
    [Tooltip("An array containing all four possible character selectors")]
    public GameObject[] selectors = new GameObject[4];
    public Sprite[] sprites = new Sprite[4];
    public Canvas canvas;
    private AudioSource aud;

    public GameManagerScript gameManager;  // a reference to the game manager

    //true when each joystick's LEFT stick horizontal axis is in idle position. Need this to prevent Update from going crazy
    private bool stick1HIdle = true;
    private bool stick2HIdle = true;
    private bool stick3HIdle = true;
    private bool stick4HIdle = true;

    //same but for the vertical axis of the left stick
    private bool stick1VIdle = true;
    private bool stick2VIdle = true;
    private bool stick3VIdle = true;
    private bool stick4VIdle = true;

    //Used to remove control from joysticks
    private bool j1Suspend = false;
    private bool j2Suspend = false;
    private bool j3Suspend = false;
    private bool j4Suspend = false;

    private void Awake()
    {
        gameManager = GameManagerScript.gmInstance;
        aud = GetComponent<AudioSource>();

        //Instantiates and preps all four character selectors. They are rendered inactive at first, so that if some players don't join it's not a problem
        for (int i = 0; i < selectors.Length; i++)
        {
            selectors[i] = Instantiate(selector, transform.position, Quaternion.identity);
            selectors[i].transform.SetParent(canvas.transform);
            selectors[i].GetComponent<SelectorBehaviour>().SetJoystickNum(i);
            selectors[i].GetComponent<SelectorBehaviour>().SetImage(sprites[i]);
            selectors[i].SetActive(false);
        }
    }

    //blocks controller input for a short duration
    private IEnumerator SuspendControl(int selectorNum)
    {
        switch(selectorNum)
        {
            case 0:
                j1Suspend = true;
                yield return new WaitForSeconds(0.33f);
                j1Suspend = false;
                break;
            case 1:
                j2Suspend = true;
                yield return new WaitForSeconds(0.33f);
                j2Suspend = false;
                break;
            case 2:
                j3Suspend = true;
                yield return new WaitForSeconds(0.33f);
                j3Suspend = false;
                break;
            case 3:
                j4Suspend = true;
                yield return new WaitForSeconds(0.33f);
                j4Suspend = false;
                break;
            default:
                break;
        }
    }

    //ALL INPUTS HERE
    void Update()
    {
        if (gameManager != null)
        {
            //CHARACTER SELECTION INPUTS
            if (gameManager.GetGameState() == GameManagerScript.GameState.charSelect)
            {
                //==================== CHARACTER SELECTION INPUT=========================

                //MOUSE and KEYBOARD - Delete when gamepad support is ready
                #region MOUSE
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    selectors[0].SetActive(true);
                }
                if (selectors[0].GetComponent<SelectorBehaviour>().chosenCharacter == null)
                {
                    if (Input.GetKeyDown(KeyCode.A))
                    {
                        selectors[0].GetComponent<SelectorBehaviour>().SetCursorPos(-1, false);
                    }
                    if (Input.GetKeyDown(KeyCode.D))
                    {
                        selectors[0].GetComponent<SelectorBehaviour>().SetCursorPos(1, false);
                    }
                    if (Input.GetKeyDown(KeyCode.W))
                    {
                        selectors[0].GetComponent<SelectorBehaviour>().SetCursorPos(1, true);
                    }
                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        selectors[0].GetComponent<SelectorBehaviour>().SetCursorPos(-1, true);
                    }
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (selectors[0].activeSelf && selectors[0].GetComponent<SelectorBehaviour>().chosenCharacter == null)
                    {
                        selectors[0].GetComponent<SelectorBehaviour>().CharSelect();
                    }
                }
                if (Input.GetKeyDown(KeyCode.LeftControl))
                {
                    if (selectors[0].GetComponent<SelectorBehaviour>().ready)
                    {
                        selectors[0].GetComponent<SelectorBehaviour>().CharDeselect();
                    }
                    else
                    {
                        selectors[0].SetActive(false);
                    }
                }
                #endregion
                //===========JOYSTICK 1, REMEMBER TO TICK "FLIPPED" IN THE PROJECT INPUT SETTINGS=========
                #region JOY 1
                //ACTIVATING THE SELECTOR, SELECTING AND DESELECTING CHARACTER
                if (!j1Suspend)
                {
                    if (Input.GetKeyDown(KeyCode.Joystick1Button0))
                    {
                        if (selectors[0].activeSelf)
                        {
                            if (selectors[0].GetComponent<SelectorBehaviour>().chosenCharacter == null)
                            {
                                selectors[0].GetComponent<SelectorBehaviour>().CharSelect();
                            }
                        }
                        else
                        {
                            selectors[0].GetComponent<SelectorBehaviour>().CharHover();
                            selectors[0].SetActive(true);
                            StartCoroutine(SuspendControl(0));
                            aud.Play();
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.Joystick1Button1))
                    {
                        if (selectors[0].GetComponent<SelectorBehaviour>().ready)
                        {
                            selectors[0].GetComponent<SelectorBehaviour>().CharDeselect();
                        }
                        else
                        {
                            selectors[0].SetActive(false);
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.Joystick1Button7))
                    {
                        StartCoroutine(gameManager.StartGameplayLoop());
                    }
                    //IDLE FUNCTIONS: IF A STICK IS BETWEEN TWO VALUES, CONSIDER IT IDLE AGAIN
                    if ((Input.GetAxis("LeftJoyHorizontal") < 0.3f) && Input.GetAxis("LeftJoyHorizontal") > -0.3f)
                    {
                        stick1HIdle = true;
                    }
                    if ((Input.GetAxis("LeftJoyVertical") < 0.3f) && Input.GetAxis("LeftJoyVertical") > -0.3f)
                    {
                        stick1VIdle = true;
                    }
                    //RIGHT
                    if (selectors[0].GetComponent<SelectorBehaviour>().chosenCharacter == null && selectors[0].gameObject.activeSelf)
                    {
                        if ((Input.GetAxis("LeftJoyHorizontal") > 0.5f))
                        {
                            if (stick1HIdle)
                            {
                                selectors[0].GetComponent<SelectorBehaviour>().SetCursorPos(1, false);
                                selectors[0].GetComponent<SelectorBehaviour>().CharHover();
                                stick1HIdle = false;
                            }
                        }
                        //LEFT
                        if ((Input.GetAxis("LeftJoyHorizontal") < -0.5f))
                        {
                            if (stick1HIdle)
                            {
                                selectors[0].GetComponent<SelectorBehaviour>().SetCursorPos(-1, false);
                                selectors[0].GetComponent<SelectorBehaviour>().CharHover();
                                stick1HIdle = false;
                            }
                        }
                        //UP
                        if ((Input.GetAxis("LeftJoyVertical") > 1.4f))
                        {
                            if (stick1VIdle)
                            {
                                selectors[0].GetComponent<SelectorBehaviour>().SetCursorPos(1, true);
                                selectors[0].GetComponent<SelectorBehaviour>().CharHover();
                                stick1VIdle = false;
                            }
                        }
                        //DOWN
                        if ((Input.GetAxis("LeftJoyVertical") < -1.4f))
                        {
                            if (stick1VIdle)
                            {
                                selectors[0].GetComponent<SelectorBehaviour>().SetCursorPos(-1, true);
                                selectors[0].GetComponent<SelectorBehaviour>().CharHover();
                                stick1VIdle = false;
                            }
                        }
                    }
                }
                #endregion
                //========= JOYSTICK 2 ==========
                #region JOY 2
                if (!j2Suspend)
                {
                    if (Input.GetKeyDown(KeyCode.Joystick2Button0))
                    {
                        if (selectors[1].activeSelf)
                        {
                            if (selectors[1].GetComponent<SelectorBehaviour>().chosenCharacter == null)
                            {
                                selectors[1].GetComponent<SelectorBehaviour>().CharSelect();
                            }
                        }
                        else
                        {
                            selectors[1].GetComponent<SelectorBehaviour>().CharHover();
                            selectors[1].SetActive(true);
                            StartCoroutine(SuspendControl(1));
                            aud.Play();
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.Joystick2Button3))
                    {
                        if (selectors[1].activeSelf && selectors[1].GetComponent<SelectorBehaviour>().chosenCharacter == null)
                        {
                            selectors[1].GetComponent<SelectorBehaviour>().CharSelect();
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.Joystick2Button1))
                    {
                        if (selectors[1].GetComponent<SelectorBehaviour>().ready)
                        {
                            selectors[1].GetComponent<SelectorBehaviour>().CharDeselect();
                        }
                        else
                        {
                            selectors[1].SetActive(false);
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.Joystick2Button7))
                    {
                        StartCoroutine(gameManager.StartGameplayLoop());
                    }
                    //IDLE FUNCTIONS: IF A STICK IS BETWEEN TWO VALUES, CONSIDER IT IDLE AGAIN
                    if ((Input.GetAxis("LeftJoy2Horizontal") < 0.3f) && Input.GetAxis("LeftJoy2Horizontal") > -0.3f)
                    {
                        stick2HIdle = true;
                    }
                    if ((Input.GetAxis("LeftJoy2Vertical") < 0.3f) && Input.GetAxis("LeftJoy2Vertical") > -0.3f)
                    {
                        stick2VIdle = true;
                    }
                    //RIGHT
                    if (selectors[1].GetComponent<SelectorBehaviour>().chosenCharacter == null && selectors[1].gameObject.activeSelf)
                    {

                        if ((Input.GetAxis("LeftJoy2Horizontal") > 0.5f))
                        {
                            if (stick2HIdle)
                            {
                                selectors[1].GetComponent<SelectorBehaviour>().SetCursorPos(1, false);
                                selectors[1].GetComponent<SelectorBehaviour>().CharHover();
                                stick2HIdle = false;
                            }
                        }
                        //LEFT
                        if ((Input.GetAxis("LeftJoy2Horizontal") < -0.5f))
                        {
                            if (stick2HIdle)
                            {
                                selectors[1].GetComponent<SelectorBehaviour>().SetCursorPos(-1, false);
                                selectors[1].GetComponent<SelectorBehaviour>().CharHover();
                                stick2HIdle = false;
                            }
                        }
                        //UP
                        if ((Input.GetAxis("LeftJoy2Vertical") > 1.4f))
                        {
                            if (stick2VIdle)
                            {
                                selectors[1].GetComponent<SelectorBehaviour>().SetCursorPos(1, true);
                                selectors[1].GetComponent<SelectorBehaviour>().CharHover();
                                stick2VIdle = false;
                            }
                        }
                        //DOWN
                        if ((Input.GetAxis("LeftJoy2Vertical") < -1.4f))
                        {
                            if (stick2VIdle)
                            {
                                selectors[1].GetComponent<SelectorBehaviour>().SetCursorPos(-1, true);
                                selectors[1].GetComponent<SelectorBehaviour>().CharHover();
                                stick2VIdle = false;
                            }
                        }
                    }
                }
                #endregion
                //========= JOYSTICK 3 ========
                #region JOY 3
                if (!j3Suspend)
                {
                    if (Input.GetKeyDown(KeyCode.Joystick3Button0))
                    {
                        if (selectors[2].activeSelf)
                        {
                            if (selectors[2].GetComponent<SelectorBehaviour>().chosenCharacter == null)
                            {
                                selectors[2].GetComponent<SelectorBehaviour>().CharSelect();
                            }
                        }
                        else
                        {
                            selectors[2].GetComponent<SelectorBehaviour>().CharHover();
                            selectors[2].SetActive(true);
                            StartCoroutine(SuspendControl(2));
                            aud.Play();
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.Joystick3Button3))
                    {
                        if (selectors[2].activeSelf && selectors[2].GetComponent<SelectorBehaviour>().chosenCharacter == null)
                        {
                            selectors[2].GetComponent<SelectorBehaviour>().CharSelect();
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.Joystick3Button1))
                    {
                        if (selectors[2].GetComponent<SelectorBehaviour>().ready)
                        {
                            selectors[2].GetComponent<SelectorBehaviour>().CharDeselect();
                        }
                        else
                        {
                            selectors[2].SetActive(false);
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.Joystick3Button7))
                    {
                        StartCoroutine(gameManager.StartGameplayLoop());
                    }

                    //IDLE FUNCTIONS: IF A STICK IS BETWEEN TWO VALUES, CONSIDER IT IDLE AGAIN
                    if ((Input.GetAxis("LeftJoy3Horizontal") < 0.3f) && Input.GetAxis("LeftJoy3Horizontal") > -0.3f)
                    {
                        stick3HIdle = true;
                    }
                    if ((Input.GetAxis("LeftJoy3Vertical") < 0.3f) && Input.GetAxis("LeftJoy3Vertical") > -0.3f)
                    {
                        stick3VIdle = true;
                    }
                    //RIGHT
                    if (selectors[2].GetComponent<SelectorBehaviour>().chosenCharacter == null && selectors[2].gameObject.activeSelf)
                    {
                        if ((Input.GetAxis("LeftJoy3Horizontal") > 0.5f))
                        {
                            if (stick3HIdle)
                            {
                                selectors[2].GetComponent<SelectorBehaviour>().SetCursorPos(1, false);
                                selectors[2].GetComponent<SelectorBehaviour>().CharHover();
                                stick3HIdle = false;
                            }
                        }
                        //LEFT
                        if ((Input.GetAxis("LeftJoy3Horizontal") < -0.5f))
                        {
                            if (stick3HIdle)
                            {
                                selectors[2].GetComponent<SelectorBehaviour>().SetCursorPos(-1, false);
                                selectors[2].GetComponent<SelectorBehaviour>().CharHover();
                                stick3HIdle = false;
                            }
                        }
                        //UP
                        if ((Input.GetAxis("LeftJoy3Vertical") > 1.4f))
                        {
                            if (stick3VIdle)
                            {
                                selectors[2].GetComponent<SelectorBehaviour>().SetCursorPos(1, true);
                                selectors[2].GetComponent<SelectorBehaviour>().CharHover();
                                stick3VIdle = false;
                            }
                        }
                        //DOWN
                        if ((Input.GetAxis("LeftJoy3Vertical") < -1.4f))
                        {
                            if (stick3VIdle)
                            {
                                selectors[2].GetComponent<SelectorBehaviour>().SetCursorPos(-1, true);
                                selectors[2].GetComponent<SelectorBehaviour>().CharHover();
                                stick3VIdle = false;
                            }
                        }
                    }
                }
                #endregion
                //========= JOYSTICK 4 ======
                #region JOY 4
                if (!j4Suspend)
                {
                    if (Input.GetKeyDown(KeyCode.Joystick4Button0))
                    {
                        if (selectors[3].activeSelf)
                        {
                            if (selectors[3].GetComponent<SelectorBehaviour>().chosenCharacter == null)
                            {
                                selectors[3].GetComponent<SelectorBehaviour>().CharSelect();
                            }
                        }
                        else
                        {
                            selectors[3].GetComponent<SelectorBehaviour>().CharHover();
                            selectors[3].SetActive(true);
                            StartCoroutine(SuspendControl(3));
                            aud.Play();
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.Joystick4Button3))
                    {
                        if (selectors[3].activeSelf && selectors[3].GetComponent<SelectorBehaviour>().chosenCharacter == null)
                        {
                            selectors[3].GetComponent<SelectorBehaviour>().CharSelect();
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.Joystick4Button1))
                    {
                        if (selectors[3].GetComponent<SelectorBehaviour>().ready)
                        {
                            selectors[3].GetComponent<SelectorBehaviour>().CharDeselect();
                        }
                        else
                        {
                            selectors[3].SetActive(false);
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.Joystick4Button7))
                    {
                        StartCoroutine(gameManager.StartGameplayLoop());
                    }
                    //IDLE FUNCTIONS: IF A STICK IS BETWEEN TWO VALUES, CONSIDER IT IDLE AGAIN
                    if ((Input.GetAxis("LeftJoy4Horizontal") < 0.3f) && Input.GetAxis("LeftJoy4Horizontal") > -0.3f)
                    {
                        stick4HIdle = true;
                    }
                    if ((Input.GetAxis("LeftJoy4Vertical") < 0.3f) && Input.GetAxis("LeftJoy4Vertical") > -0.3f)
                    {
                        stick4VIdle = true;
                    }

                    if (selectors[3].GetComponent<SelectorBehaviour>().chosenCharacter == null && selectors[3].gameObject.activeSelf)
                    {
                        //RIGHT
                        if ((Input.GetAxis("LeftJoy4Horizontal") > 0.5f))
                        {
                            if (stick4HIdle)
                            {
                                selectors[3].GetComponent<SelectorBehaviour>().SetCursorPos(1, false);
                                selectors[3].GetComponent<SelectorBehaviour>().CharHover();
                                stick4HIdle = false;
                            }
                        }
                        //LEFT
                        if ((Input.GetAxis("LeftJoy4Horizontal") < -0.5f))
                        {
                            if (stick4HIdle)
                            {
                                selectors[3].GetComponent<SelectorBehaviour>().SetCursorPos(-1, false);
                                selectors[3].GetComponent<SelectorBehaviour>().CharHover();
                                stick4HIdle = false;
                            }
                        }
                        //UP
                        if ((Input.GetAxis("LeftJoy4Vertical") > 1.4f))
                        {
                            if (stick4VIdle)
                            {
                                selectors[3].GetComponent<SelectorBehaviour>().SetCursorPos(1, true);
                                selectors[3].GetComponent<SelectorBehaviour>().CharHover();
                                stick4VIdle = false;
                            }
                        }
                        //DOWN
                        if ((Input.GetAxis("LeftJoy4Vertical") < -1.4f))
                        {
                            if (stick4VIdle)
                            {
                                selectors[3].GetComponent<SelectorBehaviour>().SetCursorPos(-1, true);
                                selectors[3].GetComponent<SelectorBehaviour>().CharHover();
                                stick4VIdle = false;
                            }
                        }
                    }
                }
                #endregion              
            }
        }
    }
}
