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
    public float stopTargetDuration; //stun length

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
        aud = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10 && !hitPeople.Contains(other.gameObject))
        {
            BaseCharacterBehaviour charScript = other.gameObject.GetComponent<BaseCharacterBehaviour>();
            //IF SLAPPING (UNARMED)
            if (!parentScript.isArmed)
            {
                parentScript.PauseFrames(0.1f);
                charScript.TakeDamage(10);

                other.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(slapPushBack * Mathf.Sign(transform.parent.rotation.y), 0, 0), ForceMode.Impulse);

                charScript.SetDisablingMovementTime(stopTargetDuration);
                charScript.SetStun(stopTargetDuration);

                hitPeople.Add(other.gameObject);
                aud.clip = audioClips[0];
                aud.Play();
            }
            //IF ARMED (MELEE WEAPON)
            else if (parentScript.meleeWeaponScript)
            {

                parentScript.PauseFrames(0.2f);
                charScript.TakeDamage((int)(parentScript.meleeWeaponScript.weaponDamage * parentScript.damageMod));
                other.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(parentScript.meleeWeaponScript.pushBack.x * Mathf.Sign(transform.parent.rotation.y), parentScript.meleeWeaponScript.pushBack.y, 0), ForceMode.Impulse);

                charScript.SetDisablingMovementTime(stopTargetDuration);

                hitPeople.Add(other.gameObject);
                aud.clip = audioClips[1];
                aud.Play();
            }
        }
    }

    private void OnDisable()
    {
        hitPeople.Clear();
        Start();
    }
}
