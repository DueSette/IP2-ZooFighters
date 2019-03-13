using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CharacterDisplayer : MonoBehaviour
{
    [System.Serializable] //serializing the structure makes it show up on the inspector
    public struct Character
    {
        public string name;
        public Sprite portrait;
        public Sprite selectedImage;
        public VideoClip introClip;
    }

    public float appearSpeed;   //how long between each character portrait instancing

    public Character[] characters;  //an array of Character (the structure)
    public GameObject charObject;   //the game object that will contain all the UI elements (image, name)
    private float portraitSize;     //the width of each sprite

    //keep "ShowCharacters()" on Awake, or it will conflict with SelectorBehaviour because it will not find the children of this.gameObject
    private void Awake()
    {
        portraitSize = charObject.transform.GetChild(1).gameObject.GetComponent<Image>().sprite.texture.width + 35;
        StartCoroutine("ShowCharacters");
    }

    private IEnumerator ShowCharacters()    //generates the clickable character portraits
    {
        for (int i = 0; i < characters.Length; i++)
        {
            CharacterPortraitScript chScript;
            
            if (i > characters.Length/2 - 1)    //basically, if half of the characters have already been displayed, displays the others in the row below
            {
                GameObject charObj = Instantiate(charObject, transform.position + new Vector3(-(portraitSize * characters.Length / 2) + (portraitSize * i), -175, 0), Quaternion.identity);
                chScript = charObj.GetComponent<CharacterPortraitScript>();               
                charObj.transform.SetParent(this.transform);                                            
            }

            else
            {              
                GameObject charObj = Instantiate(charObject, transform.position + new Vector3((portraitSize * i), 0, 0), Quaternion.identity);
                chScript = charObj.GetComponent<CharacterPortraitScript>();
                charObj.transform.SetParent(this.transform);
            }

            chScript.SetImage(characters[i].portrait);
            chScript.SetName(characters[i].name);

            chScript.SetClip(characters[i].introClip);
            yield return new WaitForSeconds(appearSpeed);
        }
        yield return null;
    }
}
