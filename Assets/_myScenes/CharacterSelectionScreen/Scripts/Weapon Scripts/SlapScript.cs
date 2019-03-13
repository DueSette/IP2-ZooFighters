using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlapScript : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> slappedPeople = new List<GameObject>();

    public float pushBack;
    public float stopTargetDuration; //stun length

    // Start is called before the first frame update
    void Awake()
    {
        slappedPeople.Add(transform.parent.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        print(other.name);
        if (other.gameObject.layer == 10 && !slappedPeople.Contains(other.gameObject))
        {
            BaseCharacterBehaviour charScript = other.gameObject.GetComponent<BaseCharacterBehaviour>();

            charScript.TakeDamage(10);

            other.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(pushBack * Mathf.Sign(transform.parent.rotation.y), 0, 0), ForceMode.Impulse);

            charScript.SetDisablingMovementTime(stopTargetDuration);
            charScript.SetStun(stopTargetDuration);

            slappedPeople.Add(other.gameObject);
        }
    }

    private void OnDisable()
    {
        slappedPeople.Clear();
        Awake();
    }
}
