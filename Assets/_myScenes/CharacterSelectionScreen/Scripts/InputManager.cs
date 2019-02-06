using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [Tooltip("The object that will represent each players' symbol when choosing a character and navigating the character selection menu")]
    public GameObject selector;
    [Tooltip("An array containing all four possible character selectors")]
    public GameObject[] selectors = new GameObject[4];
    public Sprite[] sprites = new Sprite[4];
    public Canvas canvas;

    //true when each joystick's left stick horizontal axis is in idle position. Need this to prevent Update from going crazy
    private bool stick1HIdle = true;
    private bool stick2HIdle = true;
    private bool stick3HIdle = true;
    private bool stick4HIdle = true;

    //same but for the vertical axis
    private bool stick1VIdle = true;
    private bool stick2VIdle = true;
    private bool stick3VIdle = true;
    private bool stick4VIdle = true;

    void Start()
    {   
        //Instantiates and preps all four character selectors. They are rendered inactive at first, so that if some players don't join it's not a problem
        for (int i = 0; i < selectors.Length; i++)
        {
            selectors[i] = Instantiate(selector, transform.position, Quaternion.identity);
            selectors[i].transform.SetParent(canvas.transform);
            selectors[i].GetComponent<SelectorBehaviour>().setJoystickNum(i);
            selectors[i].GetComponent<SelectorBehaviour>().SetImage(sprites[i]);
            selectors[i].SetActive(false);
        }
    }

    void Update()
    {
        #region CharacterSelectionInputs
        //MOUSE
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            selectors[0].SetActive(true);
        }
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (selectors[0].activeSelf)
            {
                selectors[0].GetComponent<SelectorBehaviour>().CharSelect();
            }
        }

        //JOYSTICK 1, REMEMBER TO TICK "FLIPPED" IN THE PROJECT SETTINGS
        if(Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            selectors[1].SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button3))
        {
            if (selectors[1].activeSelf)
            {
                selectors[1].GetComponent<SelectorBehaviour>().CharSelect();
            }
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
        if ((Input.GetAxis("LeftJoyHorizontal") > 0.5f))
        {
            if (stick1HIdle)
            {
                selectors[1].GetComponent<SelectorBehaviour>().SetCursorPos(1, false);
                stick1HIdle = false;
            }
        }
        //LEFT
        if ((Input.GetAxis("LeftJoyHorizontal") < -0.5f))
        {
            if (stick1HIdle)
            {
                selectors[1].GetComponent<SelectorBehaviour>().SetCursorPos(-1, false);
                stick1HIdle = false;
            }
        }
        //UP
        if ((Input.GetAxis("LeftJoyVertical") > 0.4f))
        {
            if (stick1VIdle)
            {
                selectors[1].GetComponent<SelectorBehaviour>().SetCursorPos(1, true);
                stick1VIdle = false;
            }
        }
        //DOWN
        if ((Input.GetAxis("LeftJoyVertical") < -0.4f))
        {
            if (stick1VIdle)
            {
                selectors[1].GetComponent<SelectorBehaviour>().SetCursorPos(-1, true);
                stick1VIdle = false;
            }
        }
        #endregion
    }
}
