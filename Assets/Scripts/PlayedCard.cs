using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayedCard : MonoBehaviour {
    public CardType card = CardType.Null;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ApplyCard(CardType cardToApply,Texture2D texture)
    {
        SkinnedMeshRenderer renderer = GetComponent<SkinnedMeshRenderer>();
        renderer.material.mainTexture = texture;
        card = cardToApply;
    }
}
