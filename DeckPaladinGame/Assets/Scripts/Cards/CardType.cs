using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SpecificCard
{
    Strike,
    Evade,
    Stun
}

public class CardType : MonoBehaviour
{
    public SpecificCard Card;
    public Vector3 HandPosition;
    public Outline CardOutline;
    // Start is called before the first frame update
    void Start()
    {
        HandPosition= transform.position;
        CardOutline = GetComponent<Outline>();
        CardOutline.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
