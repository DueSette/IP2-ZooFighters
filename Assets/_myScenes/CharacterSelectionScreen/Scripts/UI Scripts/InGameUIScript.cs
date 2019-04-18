using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUIScript : MonoBehaviour
{
    public GameObject representedCharacter;
    private BaseCharacterBehaviour charBehaviour;
    
    public GameObject charAvatar;
    public Sprite[] charPointers = new Sprite[4];
    public GameObject charPointer;
    public Slider slider;
    public Image fillImage;
    public GameObject[] lifePoints = new GameObject[6];
    [HideInInspector]
    public GameObject equippedWeaponImage;
    public TextMeshProUGUI grenadeAmount;

    [HideInInspector]
    public int theHealth;
    public Color fullHealthColor = Color.green;
    public Color zeroHealthColor = Color.red;

    void Start()
    {
        charBehaviour = representedCharacter.GetComponent<BaseCharacterBehaviour>();
        slider.maxValue = charBehaviour.maxHealth;
        charBehaviour.LifeLossEvent += SetLifePoints;
    }

    //This should update a bar or slider based on its own character's hp.
    //Then also keep tracks of lives left and stuff
    public void UpdateHUD()
    {
        if (charBehaviour.equippedWeaponSprite != null)
        {
            equippedWeaponImage.GetComponent<Image>().sprite = charBehaviour.equippedWeaponSprite;
            equippedWeaponImage.GetComponent<Image>().color = new Color(255, 255, 255, 255);
        }
        else
        {
            equippedWeaponImage.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }
        grenadeAmount.text = charBehaviour.grenades.ToString();
        UpdateHPColor();       
    }
    
    private void UpdateHPColor()
    {
        //tells the slider what value it should be (the player's displayed hp)
        slider.value = charBehaviour.displayedHealth;

        //sets the hp bar colour based on the HP left percentage
        fillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, (slider.value / slider.maxValue));
        if(slider.value == 0)
        {
            fillImage.color = Color.black;
        }
    }

    public void ResetLifePoints()
    {
        foreach (GameObject point in lifePoints)
        {
            point.SetActive(true);
        }
    }

    private void SetLifePoints()
    {
        GetComponent<Animator>().SetTrigger("LifeLost");
    }

    public void SetAvatarAndBackground(int playerNum)
    {
        charAvatar.GetComponent<Image>().sprite = representedCharacter.GetComponent<BaseCharacterBehaviour>().characterSprite;
        charPointer.GetComponent<Image>().sprite = charPointers[playerNum];
    }
}
