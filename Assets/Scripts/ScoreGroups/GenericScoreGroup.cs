using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericScoreGroup : ScoreGroup {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override bool CanPlayOnGroup(CardType card)
    {
        //These are cards we don't stack
        return false;
    }

}
