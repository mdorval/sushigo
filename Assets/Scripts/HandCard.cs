using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class HandCard : MonoBehaviour {
    private Animator anim;
    private CardType mycard = CardType.Null;
    private HumanPlayer myplayer;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update() {
		
	}

    public void ApplyCard(CardType card, Texture2D texture, HumanPlayer player)
    {
        myplayer = player;
        SkinnedMeshRenderer renderer = GetComponent<SkinnedMeshRenderer>();
        renderer.material.mainTexture = texture;
        mycard = card;
    }

    void OnMouseEnter()
    {
        anim.SetBool("isHovering", true);
        transform.Translate(new Vector3(0, 0, -0.01f));
    }

    void OnMouseExit()
    {
        anim.SetBool("isHovering", false);
        transform.Translate(new Vector3(0, 0, 0.01f));
    }

    void OnMouseDown()
    {
        myplayer.PlayCard(this.mycard);
    }
}
