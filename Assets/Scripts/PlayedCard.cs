using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayedCard : MonoBehaviour {
    //public Transform target = null;
    private ScoreGroup target = null;
    public CardType card = CardType.Null;
    //private List<IListensPlayedCard> callbackOnCardPlayed = new List<IListensPlayedCard>();
    private Animator anim;
    // Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}

    public void setTarget(ScoreGroup targetToSet)
    {
        target = targetToSet;
    }

    /*public void listenToCardPlay(IListensPlayedCard listener)
    {
        callbackOnCardPlayed.Add(listener);
    }

    private void onCardPlayed()
    {
        foreach (IListensPlayedCard listener in callbackOnCardPlayed)
        {
            listener.CardPlayed(this);
        }
        callbackOnCardPlayed.Clear();
    }*/

	// Update is called once per frame
	void Update () {
		if(target != null)
        {
            anim.SetBool("isReadyToMove",true);
            Vector3 moveTowards = target.transform.position + target.positionOfNextCard;
            transform.position = Vector3.MoveTowards(transform.position, moveTowards, Time.deltaTime*12);
            if (transform.position == moveTowards)
            {
                target.CardInPlace(this);
                target = null;
            }
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, target.rotation, Time.deltaTime * 12);
        }
	}

    public void ApplyCard(CardType cardToApply,Texture2D texture)
    {
        SkinnedMeshRenderer renderer = GetComponent<SkinnedMeshRenderer>();
        renderer.material.mainTexture = texture;
        card = cardToApply;
    }
}
