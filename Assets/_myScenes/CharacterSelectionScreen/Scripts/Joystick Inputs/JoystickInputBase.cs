using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickInputBase : MonoBehaviour
{
    public float movSpeedMod;
    public int health;

    public virtual void Start()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void Shoot()
    {
        print("shooting");
    }

    public virtual void Move()
    {

    }

    public virtual void Die()
    {

    }

    public virtual void Respawn()
    {

    }
}
