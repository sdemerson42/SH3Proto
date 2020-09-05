using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static public GameManager instance;

    public GameObject player;
    public Text playerGoldTally;

    SceneBuilder m_sceneBuilder;
    PlayerLogic m_playerLogic;
    WorldMapBuilder m_worldMapBuilder;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        m_playerLogic = player.GetComponent<PlayerLogic>();

        m_sceneBuilder = GetComponent<SceneBuilder>();
        m_worldMapBuilder = GetComponent<WorldMapBuilder>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_worldMapBuilder.BuildMap();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        // Gold - Just a test

        int uiGold = int.Parse(playerGoldTally.text);
        if (uiGold != m_playerLogic.Gold)
        {
            if (uiGold < m_playerLogic.Gold) ++uiGold;
            else --uiGold;

            playerGoldTally.text = uiGold.ToString();
        }
    }

    public void AddPlayerGold(int value)
    {
        m_playerLogic.Gold += value;
    }
}
