using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SelectorBehaviour : MonoBehaviour
{
    //variables needed to retrieve data from other scripts
    private static GameObject portraitsDisplayer;      //the object that spawns the portraits
    private CharacterDisplayer charDispl;   //the script attached to the previous object
    public delegate void SelectorActionDelegate(AudioClip clip);
    public static event SelectorActionDelegate OnSelectorAction;

    [SerializeField] AudioClip[] clips = new AudioClip[2];

    [HideInInspector]
    public int totalCharactersNum;     //how many total portraits are there in the character selection UI
    public int cursorPos;             //which portrait the cursor should snap to
    private int joystickNumber;     //the joystick that controls this specific instance of the selector object

    public GameObject chosenCharacter;     //the character that will be activated when the game starts
    public bool ready = false;     //if the player has locked their character or not

    private void OnEnable()
    {
        if (portraitsDisplayer == null)
        {
            portraitsDisplayer = GameObject.Find("Portraits Displayer");   //CAREFUL WITH NAME SEARCH
        }
        charDispl = portraitsDisplayer.GetComponent<CharacterDisplayer>();
        totalCharactersNum = charDispl.characters.Length;
        gameObject.transform.position = portraitsDisplayer.transform.GetChild(0).transform.position;
        cursorPos = 0;

        StartCoroutine(portraitsDisplayer.transform.GetChild(cursorPos).gameObject.GetComponent<CharacterPortraitScript>().SelectorEnable(GetJoystickNum(), GetCursorPos()));
    }

    private void Update()   //snaps to the appropriate portrait
    {
        if (joystickNumber < 2)
        {
            gameObject.transform.position = portraitsDisplayer.transform.GetChild(cursorPos).transform.position + new Vector3(-15 + (joystickNumber * 115), 10, 0);
        }

        else
        {
            gameObject.transform.position = portraitsDisplayer.transform.GetChild(cursorPos).transform.position + new Vector3(-15 + (joystickNumber - 2) * 115, -110, 0);
        }
    }

    //accesses the portrait that this selector is currently standing on and calls the SelectCharacter function, also keeping track of what joystick number called this
    public void CharSelect()
    {
        CharacterPortraitScript portraitScript = portraitsDisplayer.transform.GetChild(cursorPos).gameObject.GetComponent<CharacterPortraitScript>();
        //Instantiates and populates the character reference.
        chosenCharacter = Instantiate(portraitScript.SelectCharacter(GetJoystickNum(), cursorPos), transform.localPosition, Quaternion.Euler(0, 90, 0));

        //character is inactive until the game starts
        chosenCharacter.SetActive(false);

        //ties this character to their specific controller
        chosenCharacter.GetComponent<BaseCharacterBehaviour>().ReceiveJoystick(GetJoystickNum());
        ready = true;
        OnSelectorAction(clips[1]);
    }

    //destroys current instance of selected character and updates UI
    public void CharDeselect()
    {
        //if there is an instantiated character and it is still not active (meaning the game didn't start)
        if(chosenCharacter != null && !chosenCharacter.activeSelf)
        {
            //destroys it and empties the reference to it
            Destroy(chosenCharacter);
            chosenCharacter = null;
        }

        portraitsDisplayer.transform.GetChild(cursorPos).gameObject.GetComponent<CharacterPortraitScript>().DeselectCharacter(GetJoystickNum());      
        ready = false;
        OnSelectorAction(clips[2]);
    }

    //when hovering character portraits
    public void CharHover()
    {
        portraitsDisplayer.transform.GetChild(cursorPos).gameObject.GetComponent<CharacterPortraitScript>().HoverCharacter(GetJoystickNum(), cursorPos);
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
        OnSelectorAction(clips[0]);
    }

    public void OnDisable()
    {
        portraitsDisplayer.transform.GetChild(cursorPos).gameObject.GetComponent<CharacterPortraitScript>().DeselectCharacter(GetJoystickNum());
        portraitsDisplayer.transform.GetChild(cursorPos).gameObject.GetComponent<CharacterPortraitScript>().SelectorDisable(GetJoystickNum());
    }
}
