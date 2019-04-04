using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaBulletScript : BulletScript
{
    public override void OnTriggerEnter(Collider collider)
    {
        //if this collides with something THAT IS NEITHER the weapon or the destroyer volume (since they are almost always going to collide with them)
        if (collider.tag != "Weapon" && !collider.name.Contains("Destroyer"))
        {
            //HITTING A CHARACTER
            if (collider.gameObject.GetComponent<BaseCharacterBehaviour>() && !collider.gameObject.GetComponent<BaseCharacterBehaviour>().respawned)
            {
                charScript = collider.gameObject.GetComponent<BaseCharacterBehaviour>();

                charScript.TakeDamage(damage);
                charScript.GetStopped(direction);

                collider.GetComponent<Rigidbody>().AddForce(new Vector3(pushBack.x * direction, pushBack.y, 0), ForceMode.Impulse);
                charScript.SetDisablingMovementTime(stopTargetDuration);
                RaiseSoundEvent(aud.clip);    //sound event
                return;
            }

            else if(collider.tag == "Floor")
            {
                return;
            }
            OnImpact();
        }
    }
}
