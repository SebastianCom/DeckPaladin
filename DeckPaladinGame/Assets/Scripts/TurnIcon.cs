using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnIcon : MonoBehaviour
{
    // Start is called before the first frame update

    GameObject Player;
    GameObject YourTurn;
    GameObject EnemyTurn;

    void Start()
    {
        Player = GameObject.Find("Paladin J Nordstrom");
        YourTurn = transform.GetChild(0).gameObject;
        EnemyTurn = transform.GetChild(1).gameObject;

        YourTurn.SetActive(false);
        EnemyTurn.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Player.GetComponent<BattlePlayer>().bPlayersTurn)
        {
            YourTurn.SetActive(true);
            EnemyTurn.SetActive(false);
        }
        else
        {
            EnemyTurn.SetActive(true);
            YourTurn.SetActive(false);
        }
    }
}
