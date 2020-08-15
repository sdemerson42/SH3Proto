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

    SceneBuilder sceneBuilder;
    PlayerLogic playerLogic;

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

        playerLogic = player.GetComponent<PlayerLogic>();

        sceneBuilder = GetComponent<SceneBuilder>();
        // sceneBuilder.BuildRoom();
        sceneBuilder.GridBuild();
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
        if (uiGold != playerLogic.Gold)
        {
            if (uiGold < playerLogic.Gold) ++uiGold;
            else --uiGold;

            playerGoldTally.text = uiGold.ToString();
        }
    }

    public void AddPlayerGold(int value)
    {
        playerLogic.Gold += value;
    }
}
