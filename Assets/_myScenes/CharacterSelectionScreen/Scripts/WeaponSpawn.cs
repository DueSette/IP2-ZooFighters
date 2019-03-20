using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawn : MonoBehaviour
{
    public GameObject[] weaponsToSpawn = new GameObject[1];
    public bool canSpawn = true;
    public float timeBetweenSpawns = 3;

    private Vector3 transf;
    private GameManagerScript gm;

    private float maxSpawnTime = 24;
    [SerializeField]
    private List<GameObject> inGameWeapons = new List<GameObject>();

    // Start is called before the first frame update
    void OnEnable()
    {
        gm = GameManagerScript.gmInstance;
        StartCoroutine(SetSpawn(1));
    }

    int ticks = 30; //needed to make update make less per-frame calculations
    //adjusts weapon spawn rate according to how many players are there in the game
    void Update()
    {
        if (Time.frameCount % ticks == 0)
        {
            maxSpawnTime = 24;
            foreach (GameObject character in gm.inGameChars)
            {
                if (character != null)
                {
                    BaseCharacterBehaviour charScript = character.GetComponent<BaseCharacterBehaviour>();
                    if (charScript.GetRemainingLives() < 0)
                    {
                        maxSpawnTime -= 8;
                    }
                }
                else
                {
                    maxSpawnTime -= 8;
                }
            }

            if (UpdateWeaponList() < gm.CountCharacters())
            {
                Spawn();
                print("aa");
            }

            if(UpdateWeaponList() >= gm.CountCharacters() * 2.5f)
            {
                canSpawn = false;
            }
            else
            {
                canSpawn = true;
            }
        }
    }

    //Counting how many weapons are in the play field
    private int UpdateWeaponList()
    {
        for (int i = 0; i < inGameWeapons.Count; i++)
        {
            if (inGameWeapons[i] == null || !inGameWeapons[i].activeSelf)
            {
                inGameWeapons.Remove(inGameWeapons[i]);
            }
        }
        return inGameWeapons.Count;
    }

    private static void AlterValue(ref float val, float amount)
    {
        val += amount;
    }

    //Tells the program to spawn a weapon while also deciding when to spawn the next wave
    private IEnumerator SetSpawn(float time)
    {
        yield return new WaitForSeconds(time);
        timeBetweenSpawns = Random.Range(2, maxSpawnTime);  //sets up the initial value of TimeBetweenSpawns
        AlterValue(ref timeBetweenSpawns, UpdateWeaponList()); //for every weapon in the existing weapons list, add 1 second

        Spawn();
        StartCoroutine(SetSpawn(timeBetweenSpawns));
    }

    //Does the actual spawning, choosing a location between two limits and a weapon among the array of existing weapons
    private void Spawn()
    {
        int num = Random.Range(0, weaponsToSpawn.Length);

        transf = PickLocation();

        GameObject newWeapon = Instantiate(weaponsToSpawn[num], transf, Quaternion.Euler(0, 90, 0));
        newWeapon.name = weaponsToSpawn[num].name;
        inGameWeapons.Add(newWeapon);
    }

    private Vector3 PickLocation()
    {
        Vector3 newLocation = new Vector3(Random.Range(-52, 52), transform.position.y, transform.position.z);

        if (CheckLocation(newLocation))
            return newLocation;
        else
           return PickLocation();
    }

    private bool CheckLocation(Vector2 loc)
    {
        if (Physics.Raycast(new Vector3(loc.x, loc.y, 0), Vector3.down, 150))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}