using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using UnityEngine.Video;

public class CharacterPortraitScript : MonoBehaviour
{
    [Tooltip("Assign children of this object")]
    public GameObject button;   //Assign all the children of the Character Object
    public GameObject portrait;
    public GameObject text;
    public VideoClip introVid;
    public VideoPlayer videoPlayer;

    [Tooltip("the object that contains the four panels representing the player's selected character")]
    public static GameObject selectedPortraits;
    public GameObject[] selectedCharactersPanels = new GameObject[4];   //the four panels where the selected characters appear
    [Tooltip("Manually fill this array with all the actual characters, complete with model, scripts and everything")]
    public GameObject[] characters;

    private void Awake()
    {
        if (selectedPortraits == null)
        {
            selectedPortraits = GameObject.Find("Selected Banners"); //BE CAREFUL WITH NAME SEARCH
        }

        for (int i = 0; i < 4; i++)
        {
            //assigns each member of the array the reference to each SelectedPortrait gameobject
            selectedCharactersPanels[i] = selectedPortraits.transform.GetChild(i).gameObject;
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

    public void SetClip(VideoClip clip)
    {
        introVid = clip;
    }

    //Shows the selected character in the SelectedPortraits object, keeping track of what joystick called this
    //e.g: if joystick 0 calls the function, it's going to access the first portrait of the array
    public GameObject SelectCharacter(int joystickNum, int characterToSpawn)
    {     
        //returns the gameobject that will be spawned
        selectedCharactersPanels[joystickNum].GetComponent<Image>().color = Color.white;
        selectedCharactersPanels[joystickNum].GetComponent<Image>().sprite = portrait.GetComponent<Image>().sprite;

        //Setting up the introductory videoclip
        videoPlayer = selectedCharactersPanels[joystickNum].transform.GetChild(0).gameObject.GetComponent<VideoPlayer>();
        videoPlayer.clip = introVid;
        videoPlayer.gameObject.SetActive(true);

        return characters[characterToSpawn];
    }

    //deselects the character
    public void DeselectCharacter(int joystickNum)
    {
        selectedCharactersPanels[joystickNum].GetComponent<Image>().sprite = null;
        selectedCharactersPanels[joystickNum].GetComponent<Image>().color = Color.white - new Color(0, 0, 0, 0.42f);

        if(videoPlayer != null)
            videoPlayer.gameObject.SetActive(false);
    }

    //called when hovering over a portrait while not locked onto any character
    public void HoverCharacter(int joystickNum, int characterToDisplay)
    {
        selectedCharactersPanels[joystickNum].GetComponent<Image>().color = Color.white - new Color(0, 0, 0, 0.42f);
        selectedCharactersPanels[joystickNum].GetComponent<Image>().sprite = this.portrait.GetComponent<Image>().sprite;
    }
}
