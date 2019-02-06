using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;

public class CharacterScript : MonoBehaviour
{
    [Tooltip("Assign children of this object")]
    public GameObject button;   //Assign all the children of the Character Object
    public GameObject portrait;
    public GameObject text;

    [Tooltip("the object that contains the four panels representing the player's selected character")]
    public GameObject selectedPortraits;
    public GameObject[] selectedCharacters = new GameObject[4];

    private void Awake()
    {
        selectedPortraits = GameObject.Find("Selected Portraits");

        for (int i = 0; i < 4; i++)
        {
            //assigns each member of the array the reference to each SelectedPortrait gameobject
            selectedCharacters[i] = selectedPortraits.transform.GetChild(i).gameObject;
        }        
    }

    public void SetImage(Sprite sprite)
    {
       portrait.GetComponent<Image>().sprite = sprite;
    }

    public void SetName(string name)
    {
        text.GetComponent<Text>().text = name;
    }

    public void SelectCharacter(int joystickNum)
    {
        //shows the selected character in the SelectedPortraits object, keeping track of what joystick called this
        //e.g: if joystick 0 calls the function, it's going to access the first portrait of the array
        selectedCharacters[joystickNum].GetComponent<Image>().sprite = this.portrait.GetComponent<Image>().sprite;
    }
}
