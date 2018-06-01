using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayedCard : Mover {
    public CardType card = CardType.Null;
    private Animator anim;
    // Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}

    protected override void OnBeforeMove()
    {
        anim.SetBool("isReadyToMove", true);
    }

    void Update () {
        Move();
	}

    public void ApplyCard(CardType cardToApply,Texture2D texture)
    {
        SkinnedMeshRenderer renderer = GetComponent<SkinnedMeshRenderer>();
        renderer.material.mainTexture = texture;
        card = cardToApply;
    }
}
