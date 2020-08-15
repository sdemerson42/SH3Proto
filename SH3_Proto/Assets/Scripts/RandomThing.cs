using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomThing : MonoBehaviour
{
    public GameObject[] objects;

    void Awake()
    {
        GameObject thing = objects[Random.Range(0, objects.Length)];
        if (thing != null) 
            Instantiate(thing, transform.position, Quaternion.identity);
        
        Destroy(gameObject);
    }


}
