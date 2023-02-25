using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update

    public int  Health = 25;
    public int  MaxHealth = 25;
    public int  Armor = 0;

    public GameObject HealthBar;

    public BattlePlayer Player;

    bool temp = false;

    public CardType[] AttackList;

    Vector3 StartingPos;

    public CardType CurrentAttack;

    bool bAnimating = false;

    Animator EnemyAnimator;

    float MoveSpeed = 1.0f;
    float StrikeTime = 3.6f;
    float Timer = 0.0f;

    bool bReturning = false;
    bool bDamageApplied = false;
    bool bAttackPicked = false;


    string AnimName = " ";


    void Start()
    {
        HealthBar.GetComponent<Slider>().maxValue = MaxHealth;
        EnemyAnimator = GetComponent<Animator>();
        Player = GameObject.Find("Paladin J Nordstrom").GetComponent<BattlePlayer>();
        StartingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        HealthBar.GetComponent<Slider>().value = Health;

        if (Input.GetKeyDown(KeyCode.P) && !temp)
        {
            Health -= 5;
            Debug.Log("Health = " + Health);
            temp = true;
        }
        else
        {
            temp= false;
        }

        if(!Player.bPlayersTurn)
        {
            Debug.Log("Animating = " + bAnimating);
            
            if(CurrentAttack == null && !bAttackPicked)
                PickAttack();
            
            
            InitiateBattle();

            if (bAnimating)
                HandleAttackAnimation(AnimName);
            else if (bReturning)
                HandleReturn();
        }

    }

    public void TakeDamage(int damage)
    {
        int damageTaken = damage - Armor;
        if (damageTaken > 0) 
        {
            Health -= damageTaken;
        }
    }

    void InitiateBattle()
    {
        if (Player != null)
        {
            Debug.Log("AI Attack Innitated");

            if (CurrentAttack && CurrentAttack.Card == SpecificCard.Strike)
            {
                if (bAnimating == false)
                {
                    if (Vector3.Distance(transform.position, Player.transform.position) >= 1.00f)
                    {
                        transform.position = Vector3.Lerp(transform.position, Player.transform.position, MoveSpeed * Time.deltaTime);

                        EnemyAnimator.SetFloat("ForwardSpeed", 1);
                    }
                    else
                    {
                        EnemyAnimator.SetFloat("ForwardSpeed", 0);
                        Timer = StrikeTime;
                        EnemyAnimator.SetBool("Strike", true);
                        AnimName = "Strike";
                        bAnimating = true;
                        bDamageApplied = true;

                    }
                }
            }
        }
    }

    void PickAttack()
    {
        int index = Random.Range(0, AttackList.Length - 1);

        CurrentAttack = AttackList[index];
        bAttackPicked = true;
    }

    void HandleAttackAnimation(string Anim)
    {
        if (bAnimating)
        {

            if (Timer > 0.0f)
            {
                Timer -= Time.deltaTime;
            }
            else
            {
                EnemyAnimator.SetBool(Anim, false);
                bAnimating = false;
                bReturning = true;
                AnimName = " ";
            }

            if (Timer > 0.0f && Timer <= 1.5f && bDamageApplied)
            {
                Player.GetComponent<BattlePlayer>().TakeDamage(CurrentAttack.GetComponent<CardType>().Damage);
                CurrentAttack = null;
                bDamageApplied = false;
            }
        }
    }

    void HandleReturn()
    {
        if (Vector3.Distance(transform.position, StartingPos) >= 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, StartingPos, MoveSpeed * Time.deltaTime);

            EnemyAnimator.SetFloat("ForwardSpeed", -1);
        }
        else
        {
            // Destroy(TargetedOpponent);
            EnemyAnimator.SetFloat("ForwardSpeed", 0);
            bReturning = false;
            Player.bPlayersTurn = true;
            CurrentAttack = null;
            bAttackPicked = false;

        }
    }
}
