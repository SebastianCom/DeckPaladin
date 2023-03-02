using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public enum EnemyState
{
    Picking,
    Attacking,
    Animating,
    Returning,
    Stunned,
    None
}
public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    public EnemyState m_EnemyState = EnemyState.None;
    public int  Health = 25;
    public int  MaxHealth = 25;
    public int  Armor = 0;

    public GameObject HealthBar;

    public BattlePlayer Player;

    public CardType[] AttackList;

    Vector3 StartingPos;

    public CardType CurrentAttack;


    Animator EnemyAnimator;

    float MoveSpeed = 1.0f;
    float StrikeTime = 3.6f;
    float Timer = 0.0f;

    bool bDamageApplied = false;


    string AnimName = " ";


    void Start()
    {
        m_EnemyState = EnemyState.None;
        HealthBar.GetComponent<Slider>().maxValue = MaxHealth;
        EnemyAnimator = GetComponent<Animator>();
        Player = GameObject.Find("Paladin J Nordstrom").GetComponent<BattlePlayer>();
        StartingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();


        switch (m_EnemyState)
        {
            case EnemyState.Picking:
                PickAttack();
                break;

            case EnemyState.Attacking:
                InitiateBattle();
                break;

            case EnemyState.Animating:
                HandleAttackAnimation(AnimName);
                break;

            case EnemyState.Returning:
                HandleReturn();
                break;

            case EnemyState.Stunned:
                if (!Player.bPlayersTurn)
                {
                    Player.HandleStartTurn();
                    SetEnemyState(EnemyState.None);
                }
                break;

            case EnemyState.None:
                if (!Player.bPlayersTurn)
                    SetEnemyState(EnemyState.Picking);
                break;
        }

    }

    public void TakeDamage(int damage)
    {
        int damageTaken = damage - Armor;
        Armor -= damage;
        if (damageTaken > 0)
        {
            Health -= damageTaken;
        }

        if (Armor < 0.0f)
        {
            Armor = 0;
        }
    }

    void InitiateBattle()
    {
        if (Player != null)
        {
            Debug.Log("AI Attack Innitated");

            if (CurrentAttack && CurrentAttack.moveType == MoveType.Damage)
            {
                HandleMove(CurrentAttack.gameObject.name, StrikeTime);
            }
            else if(CurrentAttack && CurrentAttack.moveType == MoveType.Buff)
            {
                AnimName = CurrentAttack.gameObject.name;
                EnemyAnimator.SetBool(AnimName, true);
                Timer = 1.0f;
                SetEnemyState(EnemyState.Animating);
                bDamageApplied = true;
            }
        }
    }

    private void HandleMove(string Attack, float AnimTime)
    {
        if (Vector3.Distance(transform.position, Player.transform.position) >= 1.00f)
        {
            transform.position = Vector3.Lerp(transform.position, Player.transform.position, MoveSpeed * Time.deltaTime);

            EnemyAnimator.SetFloat("ForwardSpeed", 1);
        }
        else
        {
            EnemyAnimator.SetFloat("ForwardSpeed", 0);
            Timer = AnimTime;
            EnemyAnimator.SetBool(Attack, true);
            AnimName = Attack;
            bDamageApplied = true;
            SetEnemyState(EnemyState.Animating);

        }
    }

    void PickAttack()
    {
        int index = Random.Range(0, AttackList.Length - 1);

        CurrentAttack = AttackList[index];

        SetEnemyState(EnemyState.Attacking);
    }

    void HandleAttackAnimation(string Anim)
    {

        if (Timer > 0.0f)
        {
            Timer -= Time.deltaTime;
        }
        else
        {

            if (Anim == "Evade" && bDamageApplied)
            {
                Armor += CurrentAttack.GetComponent<CardType>().Armor;
                UpdateUI();
            }

            EnemyAnimator.SetBool(Anim, false);
            AnimName = " ";
            SetEnemyState(EnemyState.Returning);
        }

        if (Timer > 0.0f && Timer <= 1.5f && bDamageApplied && CurrentAttack.GetComponent<CardType>().moveType ==  MoveType.Damage)
        {
            Player.GetComponent<BattlePlayer>().TakeDamage(CurrentAttack.GetComponent<CardType>().Damage);
            CurrentAttack = null;
            bDamageApplied = false;
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

            EnemyAnimator.SetFloat("ForwardSpeed", 0);
            Player.HandleStartTurn();
            CurrentAttack = null;
            SetEnemyState(EnemyState.None);

        }
    }

    public void UpdateUI()
    {
        HealthBar.GetComponent<Slider>().value = Health;
        HealthBar.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = Armor.ToString();
        HealthBar.transform.GetChild(3).GetComponent<Text>().text = Health.ToString();
    }

    public void SetEnemyState(EnemyState state)
    {
        m_EnemyState = state;

        switch (m_EnemyState)
        {
            case EnemyState.Picking:
                break;

            case EnemyState.Attacking:
                break;

            case EnemyState.Animating:
                break;

            case EnemyState.Returning:
                break;

            case EnemyState.Stunned:
                break;

            case EnemyState.None:
                break;
        }
    }
}
