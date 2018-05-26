using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

public class HandCard : MonoBehaviour, IPointerClickHandler {
    private Animator anim;
    private CardType mycard = CardType.Null;
    private HumanPlayer myplayer;
    private Deck mydeck;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update() {
		
	}

    public void ApplyCard(CardType card, Sprite texture, HumanPlayer player)
    {
        myplayer = player;
        mydeck = player.GetComponentInParent<Deck>();
        Image image = GetComponent<Image>();
        image.sprite = texture;
        mycard = card;
    }

    void OnDestroy()
    {
        Destroy(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        myplayer.PlayCard(this.mycard);
    }
}
