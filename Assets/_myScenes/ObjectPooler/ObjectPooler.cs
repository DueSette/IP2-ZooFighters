using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable] //serializing the class makes it show up on the inspector
    public struct Pool
    {
        public string tag; //pool tag
        public GameObject prefab;   //the thing it contains (prefabs)
        public int size;    //its size - tells it when to start reusing objects instead of spawning new ones
    }

    #region Singletonz 
    //quick static function to have access to this class
    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion  

    public List<Pool> pools;    
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)    //for each item that we are going to call "pool" in the List<T> named "pools"...
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();     //...create a Queue of GameObjects

            for(int i = 0; i < pool.size; i++)  //then, for as many times as we set size to be (size is set in the inspector!)
            {
                GameObject obj = Instantiate(pool.prefab);  //set an object called obj as a new obj, which is instantiated on the moment
                obj.SetActive(false);   //deactivate obj
                objectPool.Enqueue(obj);    //put it in queue
            }

            poolDictionary.Add(pool.tag, objectPool); //add the newly created objectpool to the dictionary (previously empty), its name is going to be the content of the pool
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.Log(tag + " doesn't exist in the dictionary");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue(); //pulls out the first element of the queue
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
    
}
