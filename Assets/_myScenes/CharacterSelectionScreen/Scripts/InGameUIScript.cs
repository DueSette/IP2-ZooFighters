using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIScript : MonoBehaviour
{
    public GameObject representedCharacter;
    private BaseCharacterBehaviour charBehaviour;

    public Slider slider;
    public Image fillImage;
    public Image equippedWeaponSprite;
    public string equippedWeaponName;

    public int theHealth;
    public int remainingLives;

    private Color fullHealthColor = Color.green;
    private Color zeroHealthColor = Color.red;

    
    void Start()
    {
        charBehaviour = representedCharacter.GetComponent<BaseCharacterBehaviour>();
    }

    //This should update a bar or slider based on its own character's hp.
    //Then also keep tracks of lives left and stuff
    public void UpdateHUD()
    {
        slider.maxValue = charBehaviour.maxHealth;
        slider.value = charBehaviour.displayedHealth;

        if(charBehaviour.equippedWeaponImage != null)
        {
            equippedWeaponSprite = charBehaviour.equippedWeaponImage;
        }
        if(charBehaviour.equippedWeaponName != null)
        {
            equippedWeaponName = charBehaviour.equippedWeaponName;
        }

        remainingLives = charBehaviour.livesLeft;

        fillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, (slider.value / slider.maxValue));
        if(slider.value == 0)
        {
            fillImage.color = Color.black;
        }
    }
}
