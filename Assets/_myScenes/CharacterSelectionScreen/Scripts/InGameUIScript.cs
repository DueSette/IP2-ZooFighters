using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIScript : MonoBehaviour
{
    public GameObject representedCharacter;
    private BaseCharacterBehaviour charBehaviour;
    public Text livesLeftText;

    public Slider slider;
    public Image fillImage;
    public GameObject equippedWeaponImage;

    public int theHealth;
    public int remainingLives;

    public Color fullHealthColor = Color.green;
    public Color zeroHealthColor = Color.red;

    
    void Start()
    {
        charBehaviour = representedCharacter.GetComponent<BaseCharacterBehaviour>();
        slider.maxValue = charBehaviour.maxHealth;
    }

    //This should update a bar or slider based on its own character's hp.
    //Then also keep tracks of lives left and stuff
    public void UpdateHUD()
    {   
        if(charBehaviour.equippedWeaponSprite != null)
        {
            equippedWeaponImage.GetComponent<Image>().sprite = charBehaviour.equippedWeaponSprite;
            equippedWeaponImage.GetComponent<Image>().color = new Color(255, 255, 255, 255);
        }
        else
        {
            equippedWeaponImage.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }

        UpdateLivesLeft();
        UpdateHPColor();
    }

    private void UpdateLivesLeft()
    {
        remainingLives = charBehaviour.livesLeft;
        livesLeftText.text = remainingLives.ToString() + " Lives Left";
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
}
