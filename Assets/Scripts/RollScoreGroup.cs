using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollScoreGroup : ScoreGroup {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override bool CanPlayOnGroup(CardType card)
    {
        return card == CardType.Roll_Double || card == CardType.Roll_Single || card == CardType.Roll_Triple;
    }
}
