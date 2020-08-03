using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBuilder : MonoBehaviour
{
    public int rows;
    public int columns;
    public GameObject wall;
    public GameObject floor;

    GameObject m_tileManager;

    public void BuildRoom()
    {
        m_tileManager = new GameObject("TileManager");

        for (int i = 0; i < columns; ++i)
        {
            for (int j = 0; j < rows; ++j)
            {
                GameObject instance = null;
                if (i == 0 || j == 0 || i == columns - 1 || j == rows - 1)
                {
                    instance = Instantiate(wall, new Vector3(i, j, 0f),
                        Quaternion.identity);
                }
                else instance = Instantiate(floor, new Vector3(i, j, 0f),
                        Quaternion.identity);
                instance.transform.SetParent(m_tileManager.transform);
            }
        }

    }

}
