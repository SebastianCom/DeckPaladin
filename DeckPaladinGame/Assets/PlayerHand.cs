using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public GameObject Slots;
    public GameObject Deck;
    public GameObject DiscardPile;
    
    private List<GameObject> HandSlots = new List<GameObject>(); 
    private List<GameObject> CardDeck = new List<GameObject>(); 
    private List<GameObject> CurrentHand = new List<GameObject>();
    public List<GameObject> PlayedCards = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Slots.transform.childCount; i++)
        {
            HandSlots.Add(Slots.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < Deck.transform.childCount; i++)
        {
            CardDeck.Add(Deck.transform.GetChild(i).gameObject);
        }

        DrawCards();

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            DrawCards();
        }

    }

    public void DrawCards()
    {
        foreach(GameObject Slot in HandSlots) 
        {
            if(Slot.GetComponent<SlotProperties>().bFull)
            {
                continue;
            }
            else 
            {
                if(CardDeck.Count <= 0)
                {
                    FillDeck();
                }

                GameObject currentCard = CardDeck[Random.Range(0, CardDeck.Count)];
                CurrentHand.Add(currentCard);
                CardDeck.Remove(currentCard);
                currentCard.transform.position = Slot.transform.position;
                currentCard.GetComponent<CardType>().HandPosition= Slot.transform.position;
                Slot.GetComponent<SlotProperties>().bFull = true;
                currentCard.GetComponent<CardType>().HandSlot = Slot;
            }
        }
    }
    
    public void PlayCard(GameObject card)
    {
        CurrentHand.Remove(card);
        PlayedCards.Add(card);
        card.GetComponent<CardType>().HandPosition = DiscardPile.transform.position;
        card.transform.position = DiscardPile.transform.position;
        card.GetComponent<CardType>().CardOutline.enabled = false;
        card.GetComponent<CardType>().HandSlot.GetComponent<SlotProperties>().bFull = false;
        card.GetComponent<CardType>().HandSlot = null;

    }

    private void FillDeck()
    {
        for (int i = 0; i < PlayedCards.Count; i++)
        {
            CardDeck.Add(PlayedCards[i]);
        }

        PlayedCards.Clear();
    }
}
