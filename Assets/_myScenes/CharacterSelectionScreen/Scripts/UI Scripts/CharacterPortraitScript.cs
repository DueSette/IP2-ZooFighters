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
    public VideoClip selectVideo;
    public VideoClip hoverVideo;
    public VideoPlayer videoPlayer;
    public RawImage rawImage;
    public Coroutine waiter;

    [SerializeField] Sprite readyImage;

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

    public void SetClips(VideoClip selectClip, VideoClip hoverClip)
    {
        selectVideo = selectClip;
        hoverVideo = hoverClip;
    }

    //Shows the selected character in the SelectedPortraits object, keeping track of what joystick called this
    //e.g: if joystick 0 calls the function, it's going to access the first portrait of the array
    public GameObject SelectCharacter(int joystickNum, int characterToSpawn)
    {
        //returns the gameobject that will be spawned
        //Setting up the introductory videoclip
        videoPlayer = selectedCharactersPanels[joystickNum].transform.GetChild(0).gameObject.GetComponent<VideoPlayer>();
        videoPlayer.clip = selectVideo;

        waiter = StartCoroutine(Wait(joystickNum));
        videoPlayer.Play();

        //Turn on the "Ready" image
        selectedCharactersPanels[joystickNum].transform.GetChild(2).gameObject.GetComponent<Image>().enabled = true;

        return characters[characterToSpawn];
    }

    private IEnumerator Wait(int joystickNum)
    {
        rawImage = selectedCharactersPanels[joystickNum].transform.GetChild(0).gameObject.GetComponent<RawImage>();
        yield return new WaitForSeconds(0.2f);
        rawImage.enabled = true;
    }

    public IEnumerator SelectorEnable(int joystickNum, int cursorPos)
    {
        rawImage = selectedCharactersPanels[joystickNum].gameObject.GetComponent<RawImage>();
        rawImage.enabled = false;
        yield return new WaitForSeconds(0.2f);
        HoverCharacter(joystickNum, cursorPos);

        selectedCharactersPanels[joystickNum].transform.GetChild(3).gameObject.GetComponent<Image>().enabled = false;
    }

    //deselects the character (ACTS ON THE CHILD NUMBER 0 OF SELECTED BANNER)
    public void DeselectCharacter(int joystickNum)
    {
        if (waiter != null)
            StopCoroutine(waiter);

        selectedCharactersPanels[joystickNum].transform.GetChild(0).gameObject.GetComponent<VideoPlayer>().clip = null;
        videoPlayer = selectedCharactersPanels[joystickNum].transform.GetChild(0).gameObject.GetComponent<VideoPlayer>();
        if (videoPlayer != null)
        {
            rawImage = selectedCharactersPanels[joystickNum].transform.GetChild(0).gameObject.GetComponent<RawImage>();
           
            rawImage.enabled = false;
            videoPlayer.Stop();
        }

        //Turn off the "ready" image
        selectedCharactersPanels[joystickNum].transform.GetChild(2).gameObject.GetComponent<Image>().enabled = false;
    }

    //HAPPENS WHEN SELECTOR IS DISABLED (HAPPENS ON THE PARENT SELECTED BANNER)
    public void SelectorDisable(int joystickNum)
    {
        selectedCharactersPanels[joystickNum].gameObject.GetComponent<VideoPlayer>().clip = null;
        if (rawImage != null)
        {
            rawImage = selectedCharactersPanels[joystickNum].gameObject.GetComponent<RawImage>();
            rawImage.enabled = false;
        }
        if(videoPlayer != null)
            videoPlayer.Stop();

        selectedCharactersPanels[joystickNum].transform.GetChild(3).gameObject.GetComponent<Image>().enabled = true;
    }

    //called when hovering over a portrait while not locked onto any character
    public void HoverCharacter(int joystickNum, int characterToDisplay)
    {
        videoPlayer = selectedCharactersPanels[joystickNum].gameObject.GetComponent<VideoPlayer>();
        rawImage = selectedCharactersPanels[joystickNum].gameObject.GetComponent<RawImage>();
        videoPlayer.clip = hoverVideo;

        if (videoPlayer.clip == hoverVideo && videoPlayer.isPlaying)
            videoPlayer.Stop();

        videoPlayer.Play();
        rawImage.enabled = true;
        videoPlayer = selectedCharactersPanels[joystickNum].transform.GetChild(0).gameObject.GetComponent<VideoPlayer>();
        videoPlayer.clip = selectVideo;
    }
}
