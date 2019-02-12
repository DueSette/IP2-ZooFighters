using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SelectorBehaviour : MonoBehaviour
{
    //variables needed to retrieve data from other scripts
    private static GameObject portraitsHolder;      //the object that spawns the portraits
    private CharacterDisplayer charDispl;   //the script attached to the previous object

    public int totalCharactersNum;     //how many portraits are there
    private int cursorPos;             //which portrait the cursor should snap to
    private int joystickNumber;     //the joystick that controls this specific instance of the selector object

    public GameObject chosenCharacter;     //the character that will be spawned when the game starts
    public bool ready = false;     //if the player has locked their character or not

    private void OnEnable()
    {
        if (portraitsHolder == null)
        {
            portraitsHolder = GameObject.Find("PortraitsHolder");
        }
        charDispl = portraitsHolder.GetComponent<CharacterDisplayer>();
        totalCharactersNum = charDispl.characters.Length;
        gameObject.transform.position = portraitsHolder.transform.GetChild(0).transform.position;
        cursorPos = 0;
    }

    private void Update()   //snaps to the appropriate portrait
    {
        if (joystickNumber < 2)
        {
            gameObject.transform.position = portraitsHolder.transform.GetChild(cursorPos).transform.position + new Vector3(joystickNumber * 100, 0, 0);
        }

        else
        {
            gameObject.transform.position = portraitsHolder.transform.GetChild(cursorPos).transform.position + new Vector3((joystickNumber - 2) * 100, -100, 0);
        }
    }

    //accesses the portrait that this selector is currently standing on and calls the SelectCharacter function, also keeping track of what joystick number called this
    public void CharSelect()
    {
        CharacterPortraitScript portraitScript = portraitsHolder.transform.GetChild(cursorPos).gameObject.GetComponent<CharacterPortraitScript>();
        //Instantiates and populates the character reference. Spawning place is placeholder)
        chosenCharacter = Instantiate(portraitScript.SelectCharacter(GetJoystickNum(), cursorPos), new Vector3(-2, -13, 26), Quaternion.identity);

        //character is inactive until the game starts
        chosenCharacter.SetActive(false);

        //ties this character to their specific controller
        AssignJoystick(chosenCharacter);
        ready = true;
    }

    //destroys current instance of selected character and updates UI
    public void CharDeselect()
    {
        //if there is an instantiated character
        if(chosenCharacter != null)
        {
            //destroys it and empties the reference to it
            Destroy(chosenCharacter);
            chosenCharacter = null;
        }
        portraitsHolder.transform.GetChild(cursorPos).gameObject.GetComponent<CharacterPortraitScript>().DeselectCharacter(GetJoystickNum());
        ready = false;
    }

    //when hovering character portraits (called in update)
    public void CharHover()
    {
        portraitsHolder.transform.GetChild(cursorPos).gameObject.GetComponent<CharacterPortraitScript>().HoverCharacter(GetJoystickNum(), cursorPos);
    }

    //assigns an image to this object (the images should be "P1, P2, P3, P4")
    public void SetImage(Sprite sprite)
    {
        gameObject.GetComponent<Image>().sprite = sprite;
    }

    //returns the joystick number tied to this selector
    public int GetJoystickNum()
    {
        return joystickNumber;
    }

    //sets the joystick number tied to this selector
    public void SetJoystickNum(int joyNum)
    {
        joystickNumber = joyNum;
    }

    //returns cursor position (stored as an int)
    public int GetCursorPos()
    {
        return cursorPos;
    }

    //Pass as parameter the portrait number the cursor should snap to
    public void SetCursorPos(int offSet, bool vertical)
    {
        if (vertical)
        {
            cursorPos = GetCursorPos() + offSet * (totalCharactersNum / 2);
        }

        else
        {
            cursorPos = GetCursorPos() + offSet;
        }

        if(cursorPos > totalCharactersNum-1)
        {
            cursorPos -= totalCharactersNum;
        }

        if(cursorPos < 0)
        {
            cursorPos += totalCharactersNum;
        }
    }

    public void OnDisable()
    {
        portraitsHolder.transform.GetChild(cursorPos).gameObject.GetComponent<CharacterPortraitScript>().DeselectCharacter(GetJoystickNum());
    }

    public void AssignJoystick(GameObject character)
    {
        switch (GetJoystickNum())
        {
            case 0:
                {
                    character.AddComponent<JoystickInput1>();
                }
                break;
            case 1:
                {
                    character.AddComponent<JoystickInput2>();
                }
                break;
            case 2:
                {
                    character.AddComponent<JoystickInput3>();
                }
                break;
            case 3:
                {
                    character.AddComponent<JoystickInput4>();
                }
                break;
            default:
                break;
        }
    }
}
