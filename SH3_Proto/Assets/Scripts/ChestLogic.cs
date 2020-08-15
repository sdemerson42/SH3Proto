using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestLogic : MonoBehaviour
{

    private int m_minGold = 10;
    private int m_maxGold = 50;
    private int m_gold = 0;

    // Start is called before the first frame update
    void Awake()
    {
        m_gold = Random.Range(m_minGold, m_maxGold + 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        {
            if (collision.gameObject.tag == "Player")
            {
                GameManager.instance.AddPlayerGold(m_gold);
                Destroy(gameObject);
            }
        }
    }

}
