using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SelectorBehaviour : MonoBehaviour
{
    //variables needed to retrieve data from other scripts
    public GameObject portraitsHolder;      //the object that spawns the portraits
    private CharacterDisplayer charDispl;   //the script attached to the previous object
    public int characterCount;     //how many portraits are there
    private int cursorPos;         //which portrait the cursor should snap to
    private int joystickNumber;

    private void OnEnable()
    {
        portraitsHolder = GameObject.Find("PortraitsHolder");
        charDispl = portraitsHolder.GetComponent<CharacterDisplayer>();
        characterCount = charDispl.characters.Length;
        gameObject.transform.position = portraitsHolder.transform.GetChild(0).transform.position;
        cursorPos = 0;
    }

    private void Update()   //snaps to the appropriate portrait
    {
        gameObject.transform.position = portraitsHolder.transform.GetChild(cursorPos).transform.position;
    }

    public void SetImage(Sprite sprite)
    {
        gameObject.GetComponent<Image>().sprite = sprite;
    }

    //returns the joystick number tied to this selector
    public int getJoystickNum()
    {
        return joystickNumber;
    }

    //sets the joystick number tied to this selector
    public void setJoystickNum(int joyNum)
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
            cursorPos = GetCursorPos() + offSet * (characterCount / 2);
        }

        else
        {
            cursorPos = GetCursorPos() + offSet;
        }

        if(cursorPos > characterCount-1)
        {
            cursorPos -= characterCount;
        }

        if(cursorPos < 0)
        {
            cursorPos += characterCount;
        }
    }

    //accesses the portrait that this is currently standing on and calls the SelectCharacter function, also keeping track of what joystick number called this
    public void CharSelect()
    {
        portraitsHolder.transform.GetChild(cursorPos).gameObject.GetComponent<CharacterScript>().SelectCharacter(getJoystickNum());
    }
}
