using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NigiriScoreGroup : ScoreGroup {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override bool CanPlayOnGroup(CardType card)
    {
        //Don't stack nigiri - nigiri should be layed down side by side
        //Nigiri can go on wasabi but not on each other
        return false;
    }

}
