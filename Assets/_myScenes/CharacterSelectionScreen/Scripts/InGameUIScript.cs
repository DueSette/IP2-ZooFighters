using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIScript : MonoBehaviour
{
    public GameObject representedCharacter;
    private BaseCharacterBehaviour charBehaviour;
    
    public GameObject charAvatar;
    public Sprite[] charBackgrounds = new Sprite[4];
    public GameObject charBackground;
    public Slider slider;
    public Image fillImage;
    public GameObject[] lifePoints = new GameObject[6];
    [HideInInspector]
    public GameObject equippedWeaponImage;

    [HideInInspector]
    public int theHealth;
    public Color fullHealthColor = Color.green;
    public Color zeroHealthColor = Color.red;

    void Start()
    {
        charBehaviour = representedCharacter.GetComponent<BaseCharacterBehaviour>();
        slider.maxValue = charBehaviour.maxHealth;
    }

    //This should update a bar or slider based on its own character's hp.
    //Then also keep tracks of lives left and stuff
    public void UpdateHUD(int elapsedTime)
    {
        SetAvatar();
        if (charBehaviour.equippedWeaponSprite != null)
        {
            equippedWeaponImage.GetComponent<Image>().sprite = charBehaviour.equippedWeaponSprite;
            equippedWeaponImage.GetComponent<Image>().color = new Color(255, 255, 255, 255);
        }
        else
        {
            equippedWeaponImage.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }

        UpdateHPColor();
        if (elapsedTime % 90 == 0)
        {
            SetLifePoints();
        }
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
        int i = 0;
        foreach (GameObject point in lifePoints)
        {
            if (i < charBehaviour.livesLeft)
                point.SetActive(true);
            else
                point.SetActive(false);
            i++;
        }
    }

    public void SetAvatar()
    {
        charAvatar.GetComponent<Image>().sprite = representedCharacter.GetComponent<BaseCharacterBehaviour>().characterSprite;
    }

    public void SetBackground(int playerNum)
    {
        charBackground.GetComponent<Image>().sprite = charBackgrounds[playerNum];
    }
}
