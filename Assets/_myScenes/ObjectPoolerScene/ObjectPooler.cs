using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable] //serializing the class makes it show up on the inspector
    public struct Pool
    {
        public string tag; //pool name
        public GameObject prefab;   //the thing it contains (prefabs)
        [Tooltip("How many do you want to instantiate?")]
        public int size;    //its size - tells it when to start reusing objects instead of spawning new ones
    }

    #region Singleton
    //quick static function to have access to this class
    public static ObjectPooler instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion  

    [Tooltip("All the different types of objects (bullets / platforms), anything that spawns")]
    public List<Pool> pools;    
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)    //for each item that we are going to call "pool" in the List<T> named "pools"...
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();     //...create a Queue of GameObjects

            for(int i = 0; i < pool.size; i++)  //then, for as many times as we decided (size is set in the inspector!)
            {
                GameObject obj = Instantiate(pool.prefab); //set an object called obj as a new obj, which is instantiated on the moment
                obj.SetActive(false);   //deactivate obj
                obj.transform.SetParent(gameObject.transform);
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

        GameObject objectToSpawn = poolDictionary[tag].Dequeue(); //pulls out element of the queue
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(objectToSpawn); //puts it at the end of the queue

        return objectToSpawn;
    }  
}
