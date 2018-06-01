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

    /// <summary>
    /// Applies Card to this PlayedCard Object
    /// </summary>
    /// <param name="cardToApply">The card to apply</param>
    /// <param name="texture"></param>
    public void ApplyCard(CardInfo cardToApply)
    {
        SkinnedMeshRenderer renderer = GetComponent<SkinnedMeshRenderer>();
        renderer.material.mainTexture = cardToApply.Texture();
        card = cardToApply.type;
    }
}
