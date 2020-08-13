using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomThing : MonoBehaviour
{
    public GameObject[] objects;

    void Awake()
    {
        Instantiate(objects[Random.Range(0, objects.Length)], transform.position,
            Quaternion.identity);
        Destroy(gameObject);
    }


}
