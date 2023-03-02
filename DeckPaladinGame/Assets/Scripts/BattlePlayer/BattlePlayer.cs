using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerState
{
    CardPlay,
    Attacking,
    Animating,
    Returning
}

public class BattlePlayer : MonoBehaviour
{
    // Start is called before the first frame update

    public PlayerState m_PlayerState = PlayerState.CardPlay;

    Vector3 ScreenPos;
    Vector3 MousePos;
    Vector3 StartingPos;


    GameObject CurrentCard;
    GameObject PlayedCard;

    GameObject[] Opponents;
    GameObject TargetedOpponent;


    Animator PlayerAnimator = null;

    public GameObject HealthBar;


    float MoveSpeed = 1.0f;
    float StrikeTime = 3.6f;
    float Timer = 0.0f;

    bool bCardPlayed = false;
    bool bDamageApplied = false;

    public bool bPlayersTurn = true;

    string AnimName = " ";

    public int Health = 25;
    public int MaxHealth = 25;
    public int Armor = 0;

    public PlayerHand playerHand;

    public int Mana;
    public int MaxMana = 3;

    bool bSkipNextTurn = false;

    void Start()
    {
        HealthBar.GetComponent<Slider>().maxValue = MaxHealth;
        StartingPos = transform.position;
        PlayerAnimator= GetComponent<Animator>();
        Mana = MaxMana;
    }

    // Update is called once per frame
    void Update()
    {


        switch (m_PlayerState)
        {
            case PlayerState.CardPlay:
                HandleCardPlay();
                break;

            case PlayerState.Attacking:
                InitiateBattle();
                break;

            case PlayerState.Animating:
                HandleAttackAnimation(AnimName);
                break;

            case PlayerState.Returning:
                HandleReturn();
                break;
        }

        UpdateUI();
    }

    private void HandleCardPlay()
    {
        if(bPlayersTurn)
        {
            if (Input.GetMouseButton(0))
            {
                RaycastHit CardHit = new RaycastHit();

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out CardHit))
                {
                    if (CardHit.transform.gameObject.GetComponent<CardType>())
                    {
                        CurrentCard = CardHit.transform.gameObject;

                        ScreenPos = Input.mousePosition;

                        ScreenPos.z = Camera.main.nearClipPlane + 2.5f;

                        MousePos = Camera.main.ScreenToWorldPoint(ScreenPos);

                        CardHit.transform.position = MousePos;

                        RaycastHit AreaHovered = new RaycastHit();

                        if (Physics.Raycast(CurrentCard.transform.position, -CurrentCard.transform.up, out AreaHovered))
                        {
                            if (AreaHovered.transform.gameObject.tag == "Play Area")
                            {
                                CurrentCard.GetComponent<CardType>().CardOutline.enabled = true;
                            }
                            else
                            {
                                CurrentCard.GetComponent<CardType>().CardOutline.enabled = false;
                            }
                        }
                        else
                        {
                            CurrentCard.GetComponent<CardType>().CardOutline.enabled = false;
                        }

                        Debug.DrawLine(CurrentCard.transform.position, CurrentCard.transform.position + (-CurrentCard.transform.up * 5));
                    }
                }
            }
            else
            {

                if (CurrentCard != null)
                {

                    RaycastHit AreaReleased = new RaycastHit();

                    if (Physics.Raycast(CurrentCard.transform.position, -CurrentCard.transform.up, out AreaReleased))
                    {
                        if (AreaReleased.transform.gameObject.tag == "Play Area")
                        {
                            if(CurrentCard.GetComponent<CardType>().ManaCost <= Mana)
                            {
                                Mana -= CurrentCard.GetComponent<CardType>().ManaCost;
                                Debug.Log(CurrentCard.name + " played");
                                PlayedCard = CurrentCard;
                                playerHand.PlayCard(CurrentCard);
                                SetPlayerState(PlayerState.Attacking);

                                return;
                            }
                        }
                    }
                }
                if (!bCardPlayed && CurrentCard)
                {
                    CurrentCard.transform.position = CurrentCard.GetComponent<CardType>().HandPosition;
                    CurrentCard.GetComponent<CardType>().CardOutline.enabled = false;
                }
            }
        }
    }

    public void SetPlayerState(PlayerState state)
    {
        m_PlayerState = state;
    }

    void InitiateBattle()
    {
        if (PlayedCard != null) 
        {
            Debug.Log("Attack Innitated");

            Opponents = GameObject.FindGameObjectsWithTag("Opponent");

            CardType playedCard = PlayedCard.GetComponent<CardType>();

            if (Opponents.Length == 1)
            {
                TargetedOpponent = Opponents[0];
            }

            if(playedCard.moveType == MoveType.Damage) 
            {
                HandleMove(playedCard.Card.ToString(), StrikeTime);
            }
            else if(playedCard.moveType == MoveType.Buff)
            {
                AnimName = playedCard.Card.ToString();
                PlayerAnimator.SetBool(AnimName, true);
                Timer = 1.0f;
                SetPlayerState(PlayerState.Animating);
                bDamageApplied = true;
            }
            else if (playedCard.moveType == MoveType.Debuff)
            {
                HandleMove(playedCard.Card.ToString(), StrikeTime - 0.5f);
            }
        }
    }

    private void HandleMove(string Attack, float AnimTime)
    {
        if (Vector3.Distance(transform.position, TargetedOpponent.transform.position) >= 1.00f)
        {
            transform.position = Vector3.Lerp(transform.position, TargetedOpponent.transform.position, MoveSpeed * Time.deltaTime);

            PlayerAnimator.SetFloat("ForwardSpeed", 1);
        }
        else
        {
            PlayerAnimator.SetFloat("ForwardSpeed", 0);
            Timer = AnimTime;
            PlayerAnimator.SetBool(Attack, true);
            AnimName = Attack;
            bDamageApplied = true;
            SetPlayerState(PlayerState.Animating);

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

        if(Armor < 0.0f)
        {
            Armor = 0;
        }
    }

    void HandleAttackAnimation(string Anim)
    {
        
        if(Timer > 0.0f && Timer <= 1.5f && bDamageApplied)
        {
            if(CurrentCard.GetComponent<CardType>().moveType == MoveType.Damage)
            {
                TargetedOpponent.GetComponent<Enemy>().TakeDamage(CurrentCard.GetComponent<CardType>().Damage);
                bDamageApplied = false;
            }
        }
        
        if(Timer > 0.0f) 
        {
            Timer -= Time.deltaTime;
        }
        else
        {

            if (Anim == "Evade" && bDamageApplied)
            {
                Armor += CurrentCard.GetComponent<CardType>().Armor;
                UpdateUI();
            }
            else if (CurrentCard.GetComponent<CardType>().moveType == MoveType.Debuff)
            {
                if (CurrentCard.GetComponent<CardType>().Card == SpecificCard.Stun)
                    bSkipNextTurn= true;

                bDamageApplied = false;
            }

            PlayerAnimator.SetBool(Anim, false);
            CurrentCard = null;
            PlayedCard = null;
            SetPlayerState(PlayerState.Returning);
            AnimName = " ";
        }
        
    }

    void HandleReturn()
    {
        if (Vector3.Distance(transform.position, StartingPos) >= 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, StartingPos, MoveSpeed * Time.deltaTime);

            PlayerAnimator.SetFloat("ForwardSpeed", -1);
        }
        else
        {

            PlayerAnimator.SetFloat("ForwardSpeed", 0);
            SetPlayerState(PlayerState.CardPlay);

            if(bSkipNextTurn) 
            {
                TargetedOpponent.GetComponent<Enemy>().SetEnemyState(EnemyState.Stunned);
                bSkipNextTurn = false;
            }

            if(Mana <= 0)
                bPlayersTurn = false;

            TargetedOpponent= null;

        }
    }

    public void HandleStartTurn()
    {
        Mana = MaxMana;
        bPlayersTurn = true;
        playerHand.DrawCards();
        SetPlayerState(PlayerState.CardPlay);
    }

    public void UpdateUI()
    {
        HealthBar.GetComponent<Slider>().value = Health;
        HealthBar.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = Armor.ToString();
        HealthBar.transform.GetChild(3).GetComponent<Text>().text = Health.ToString();
    }

}
