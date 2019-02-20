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
    public Sprite equippedWeaponSprite;
    public string equippedWeaponName;

    public int theHealth;
    public int remainingLives;

    public Color fullHealthColor = Color.green;
    public Color zeroHealthColor = Color.red;

    
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

        if(charBehaviour.equippedWeaponSprite != null)
        {
            equippedWeaponSprite = charBehaviour.equippedWeaponSprite;
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
