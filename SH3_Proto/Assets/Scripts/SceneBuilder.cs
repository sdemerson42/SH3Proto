using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBuilder : MonoBehaviour
{
    public GameObject[] rooms;

    //GameObject m_tileManager;

    public void BuildRoom()
    {
        var room = rooms[Random.Range(0, rooms.Length)];
        Instantiate(room, new Vector3(0f, 0f, 0f), Quaternion.identity);

    }

}
