using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeObjectScript : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> hitPeople = new List<GameObject>();
    private GameObject parentCharacter;
    private BaseCharacterBehaviour parentScript;

    public float slapPushBack;
    public float slapStopTargetDuration; //stun length

    public AudioClip[] audioClips;
    public AudioSource aud;

    // Start is called before the first frame update
    void Awake()
    {
        parentCharacter = transform.parent.gameObject;
        aud = GetComponent<AudioSource>();
    }

    void Start()
    {
        hitPeople.Add(transform.parent.gameObject);
        parentScript = parentCharacter.GetComponent<BaseCharacterBehaviour>();
    }

    private void OnTriggerEnter(Collider other)
    {
		if (other.gameObject.layer == 10 && !hitPeople.Contains(other.gameObject) && !other.gameObject.GetComponent<BaseCharacterBehaviour>().respawned && other.gameObject.GetComponent<BaseCharacterBehaviour>().GetHealth() > 0)
		{
			BaseCharacterBehaviour charScript = other.gameObject.GetComponent<BaseCharacterBehaviour>();
			//IF SLAPPING (UNARMED)
			if (!parentScript.isArmed)
			{
				parentScript.PauseFrames(0.1f);
				charScript.TakeDamage(10);

				other.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(slapPushBack * Mathf.Sign(transform.parent.rotation.y), 0, 0), ForceMode.Impulse);
				other.gameObject.GetComponent<Animator>().SetTrigger("Staggered");
				other.gameObject.GetComponent<Animator>().SetLayerWeight(9, 1);
				charScript.SetDisablingMovementTime(slapStopTargetDuration);
				charScript.SetStun(slapStopTargetDuration);

				hitPeople.Add(other.gameObject);
				aud.clip = audioClips[0];
				aud.Play();
			}
			//IF ARMED (MELEE WEAPON)
			else if (parentScript.meleeWeaponScript)
			{
				parentScript.PauseFrames(0.2f);
				charScript.TakeDamage((int)(parentScript.meleeWeaponScript.weaponDamage * parentScript.damageMod));
				other.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(parentScript.meleeWeaponScript.pushback.x * Mathf.Sign(transform.parent.rotation.y), parentScript.meleeWeaponScript.pushback.y, 0), ForceMode.Impulse);

				//==== FIX HERE (SHOULD VARY BASED ON THE WEAPON)
				charScript.SetDisablingMovementTime(slapStopTargetDuration);

				hitPeople.Add(other.gameObject);

				other.gameObject.GetComponent<Animator>().SetLayerWeight(9, 0.6f);
				if (parentScript.equippedWeapon.tag == "Lightsaber")				
					aud.clip = audioClips[Random.Range(3, 5)];
				
				else
					aud.clip = audioClips[1];
				aud.Play();
			}
		}
		//If wielding a baseball bat and hitting a grenade
		else if (parentScript.equippedWeapon != null && parentScript.equippedWeapon.CompareTag("BaseballBat") && other.gameObject.CompareTag("Thrown") && !hitPeople.Contains(other.gameObject))
		{
			parentScript.PauseFrames(0.2f);

			other.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(parentScript.meleeWeaponScript.pushback.x / 1.7f * Mathf.Sign(transform.parent.rotation.y),
				parentScript.meleeWeaponScript.pushback.y / 3, 0),
				ForceMode.Impulse);
			other.gameObject.GetComponent<GrenadeScript>().ticking = true;

			hitPeople.Add(other.gameObject);
			aud.clip = audioClips[1];
			aud.Play();
		}
	}

	protected float GetStoppingDurationByWeapon(string weaponTag)
	{
		switch (weaponTag)
		{
			case "BaseballBat":
				return slapStopTargetDuration / 1.45f;
			case "LightSaber":
				return slapStopTargetDuration / 4;
			case "SlapObject":
				return slapStopTargetDuration;				
			default:
				return 0;				
		}
	}

    private void OnDisable()
    {
        hitPeople.Clear();
        Start();
    }
}
