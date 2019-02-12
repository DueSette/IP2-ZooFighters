using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickInput1 : JoystickInputBase
{
    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            Shoot();
            
        }
    }
}
