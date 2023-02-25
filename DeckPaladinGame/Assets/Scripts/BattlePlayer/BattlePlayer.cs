using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class BattlePlayer : MonoBehaviour
{
    // Start is called before the first frame update

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

    bool bAnimating = false;
    bool bReturning = false;
    bool bCardPlayed = false;
    bool bAttacking = false;
    bool bDamageApplied = false;

    public bool bPlayersTurn = true;

    string AnimName = " ";

    public int Health = 25;
    public int MaxHealth = 25;
    public int Armor = 0;

    void Start()
    {
        HealthBar.GetComponent<Slider>().maxValue = MaxHealth;
        StartingPos = transform.position;
        PlayerAnimator= GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        HealthBar.GetComponent<Slider>().value = Health;
        HandleCardPlay();


        if (bAttacking)
            InitiateBattle();

        if(bAnimating)
            HandleAttackAnimation(AnimName);
        else if(bReturning)
            HandleReturn();
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
                            Debug.Log(CurrentCard.name + " played");
                            PlayedCard = CurrentCard;
                            CurrentCard.SetActive(false);
                            bAttacking = true;


                        }
                    }
                }
                if (!bCardPlayed && CurrentCard)
                {
                    CurrentCard.transform.position = CurrentCard.GetComponent<CardType>().HandPosition;
                }
            }
        }
    }

    void InitiateBattle()
    {
        if (PlayedCard != null) 
        {
            Debug.Log("Attack Innitated");

            Opponents = GameObject.FindGameObjectsWithTag("Opponent");

            if(Opponents.Length == 1)
            {
                TargetedOpponent = Opponents[0];
            }

            if(PlayedCard.GetComponent<CardType>().Card == SpecificCard.Strike) 
            {
                if(bAnimating == false) 
                {
                    if (Vector3.Distance(transform.position, TargetedOpponent.transform.position) >= 1.00f)
                    {
                        transform.position = Vector3.Lerp(transform.position, TargetedOpponent.transform.position, MoveSpeed * Time.deltaTime);

                        PlayerAnimator.SetFloat("ForwardSpeed", 1);
                    }
                    else 
                    {
                        PlayerAnimator.SetFloat("ForwardSpeed", 0);
                        Timer = StrikeTime;
                        PlayerAnimator.SetBool("Strike", true);
                        AnimName = "Strike";
                        bAnimating = true;
                        bDamageApplied = true;

                    }
                }
            }
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

    void HandleAttackAnimation(string Anim)
    {
        if(bAnimating)
        {
           
            if(Timer > 0.0f) 
            {
                Timer -= Time.deltaTime;
            }
            else
            {
                PlayerAnimator.SetBool(Anim, false);
                Destroy(CurrentCard);
                PlayedCard = null;
                bAttacking = false;
                bAnimating= false;
                bReturning = true;
                AnimName = " ";
            }
            
            if(Timer > 0.0f && Timer <= 1.5f && bDamageApplied)
            {
                TargetedOpponent.GetComponent<Enemy>().TakeDamage(CurrentCard.GetComponent<CardType>().Damage);
                bDamageApplied = false;
            }
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
            // Destroy(TargetedOpponent);
            PlayerAnimator.SetFloat("ForwardSpeed", 0);
            bReturning = false;
            bPlayersTurn = false;
            TargetedOpponent= null;

        }
    }

}
