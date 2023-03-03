using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaCounter : MonoBehaviour
{
    public GameObject ManaCrystal1;
    public GameObject ManaCrystal2;
    public GameObject ManaCrystal3;

    BattlePlayer Player;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Paladin J Nordstrom").GetComponent<BattlePlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Player)
        {
            if(Player.Mana == 3)
            {
                ManaCrystal3.SetActive(true);
                ManaCrystal2.SetActive(true);
                ManaCrystal1.SetActive(true);
            }
            else if(Player.Mana == 2)
            {
                ManaCrystal3.SetActive(false);
                ManaCrystal2.SetActive(true);
                ManaCrystal1.SetActive(true);

            }
            else if (Player.Mana == 1)
            {
                ManaCrystal3.SetActive(false);
                ManaCrystal2.SetActive(false);
                ManaCrystal1.SetActive(true);

            }
            else if (Player.Mana == 0)
            {
                ManaCrystal1.SetActive(false);
                ManaCrystal2.SetActive(false);
                ManaCrystal3.SetActive(false);
            }
        }
    }
}
