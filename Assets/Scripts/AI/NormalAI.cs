using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class NormalAI : BaseAI, IDisposable
{
    public NormalAI(ComputerPlayer ourPlayer) : base(ourPlayer) 
    {
        counter = new CardCounter(ourPlayer);
    }

    private CardCounter counter = null;
    public override CardType ChooseCard(List<CardType> pack)
    {
        counter.InitPack(pack);
        Dictionary<CardType, float> valuePerCardType = new Dictionary<CardType, float>();
        foreach (CardType card in pack.OrderByDescending(r => r))
        {
            if (!valuePerCardType.ContainsKey(card))
            {
                if (ScoreCard.RollValues.ContainsKey(card) && valuePerCardType.Keys.Any(r => ScoreCard.RollValues.ContainsKey(r)))
                {
                    //If we already have a rolls score then give this 
                    //We go through the cards descending specifically so we only value the highest roll available
                    //(Why would the player play a smaller roll?)
                    valuePerCardType[card] = 0.0f;
                    continue;
                }
                valuePerCardType[card] = GetValueForCard(card);
            }
        }
        //Grab the top values. There could be more than one card type here
        var topValues = valuePerCardType.GroupBy(r=>r.Value).OrderByDescending(r=>r.Key).First().ToList();
        //randomly select a card type from the top values
        return topValues.ElementAt(UnityEngine.Random.Range(0, topValues.Count)).Key;
    }

    /// <summary>
    /// Gets an active scoregroup if it exists, but only if the card can be played on it (aka open score groups)
    /// </summary>
    /// <typeparam name="T">The Type of Scoregroup</typeparam>
    /// <param name="card">The Card to check</param>
    /// <returns>null or a given scoregroup</returns>
    private T GetActiveScoreGroup<T>(CardType card) where T: ScoreGroup
    {
        foreach(T group in this._player.GetComponentsInChildren<T>())
        {
            if (group.CanPlayOnGroup(card))
            {
                return group;
            }
        }
        return null;
    }

    /// <summary>
    /// Calculates the approximate value of a card from the perspective of this turn
    /// Can take into account future cards played on this card
    /// </summary>
    /// <param name="card">The card to calculate</param>
    /// <returns>Approx value of card as a float</returns>
    private float GetValueForCard(CardType card)
    {
        switch (card)
        {
            case CardType.Roll_Single:
            case CardType.Roll_Double:
            case CardType.Roll_Triple:
                {
                    //get our current roll score
                    int oldRollsScore = this._player.scoreCard.GetRollsPotentialScore();
                    //Ask the scorecard to get us a predictor based on the new card value
                    IRollScorer scorer = this._player.scoreCard.MakeRollsPredictor(card);
                    List<IRollScorer> scorers = new List<IRollScorer> { scorer };
                    //We want to add the opponents in because roll scores are relative to the other players
                    //These Predictor scorers are read-only, meaning that pushing them through the UpdateRollScores()
                    //function does not update the players' actual scores
                    foreach (IRollScorer opScorer in Deck.Instance().GetOpponentPredictorScorers(this._player))
                    {
                        scorers.Add(opScorer);
                    }
                    //calculate the new potential scores
                    scorers.UpdateRollScores();
                    float value = scorer.GetRollsPotentialScore() - oldRollsScore;
                    if (UnityEngine.Random.Range(0,1.0f) > 0.5f)
                    {
                        //account for multiple people playing rolls on the same turn
                        //without this the AI would always play rolls on the first turn
                        //clobbering each other
                        value *= 0.5f;
                    }
                    return value;
                }
            case CardType.Dumpling:
                {
                    //dumplings are based on how many you've played already
                    int maxPossiblePlayable = counter.MaxCopiesPlayableThisRound(card);
                    //value of the first card played (presumably this turn's card)
                    int startingCard = 1;
                    int count = 0;
                    float value = 0.0f;
                    ScoreGroup sg = GetActiveScoreGroup<DumplingScoreGroup>(card);
                    if (sg != null)
                    {
                        startingCard += sg.CardCount();
                    }
                    //used to calculate the cards that can be played and their total value
                    int countto = maxPossiblePlayable + startingCard - 1;
                    //dumplings aren't worth anything passed 5
                    if (countto > 5) countto = 5;
                    for (int i = startingCard; i <= countto; i++)
                    {
                        value += i;
                        count++;
                    }
                    return count == 0 ? 0 : value / (float)count;
                }
            case CardType.Nigiri_Squid:
            case CardType.Nigiri_Salmon:
            case CardType.Nigiri_Egg:
                {
                    //If there's a wasabi, we're playing on it
                    WasabiScoreGroup sg = GetActiveScoreGroup<WasabiScoreGroup>(card);
                    if (sg)
                    {
                        return ScoreCard.NigiriValues[card] * 3.0f;
                    }
                    //Otherwise just value of current nigiri
                    return ScoreCard.NigiriValues[card];
                }
            case CardType.Wasabi:
                {
                    foreach (var nigiri in ScoreCard.NigiriValues.OrderByDescending(r=>r.Value))
                    {
                        //check card counter for every type of nigiri, starting with the most valued.
                        //divide potential value by 2, as it would take 2 cards to get this potential score
                        if (counter.MaxCopiesPlayableThisRound(nigiri.Key) > 0)
                        {
                            return (nigiri.Value * 3.0f) / 2;
                        }
                    }
                    return 0.0f;
                }
            case CardType.Sashimi:
                {
                    int sashimiRemaining = 3;
                    SashimiScoreGroup sg = GetActiveScoreGroup<SashimiScoreGroup>(card);
                    //If we've already started, count the sashimi we already have
                    if (sg)
                    {
                        sashimiRemaining -= sg.CardCount();
                    }
                    //make sure we will be able to play sashimi after this
                    if (counter.MaxCopiesPlayableThisRound(card) >= sashimiRemaining)
                    {
                        return 10.0f / sashimiRemaining;
                    }
                    //We won't be able to finish this sashimi, don't bother
                    return 0.0f;
                }
            case CardType.Tempura:
                {
                    int tempuraRemaining = 2;
                    TempuraScoreGroup sg = GetActiveScoreGroup<TempuraScoreGroup>(card);
                    //If we've already started, count the sashimi we already have
                    if (sg)
                    {
                        tempuraRemaining -= sg.CardCount();
                    }
                    //make sure we will be able to play tempura after this
                    if (counter.MaxCopiesPlayableThisRound(card) >= tempuraRemaining)
                    {
                        return 5.0f / tempuraRemaining;
                    }
                    //We won't be able to finish this tempura, don't bother
                    return 0.0f;
                }
            case CardType.Pudding:
                {
                    //get our current pudding score
                    int oldPuddingScore = this._player.scoreCard.GetPuddingPotentialScore();
                    //Ask the scorecard to get us a predictor based on the new card value
                    IPuddingScorer scorer = this._player.scoreCard.MakePuddingPredictor(1);
                    List<IPuddingScorer> scorers = new List<IPuddingScorer>() { scorer };
                    //We want to add the opponents in because pudding scores are relative to the other players
                    //These Predictor scorers are read-only, meaning that pushing them through the UpdatePuddingScores()
                    //function does not update the players' actual scores
                    foreach (IPuddingScorer opScorer in Deck.Instance().GetOpponentPredictorScorers(this._player))
                    {
                        scorers.Add(opScorer);
                    }
                    //calculate new potential scores
                    scorers.UpdatePuddingScores();
                    //we don't need to do since AI clobbering puddings doesn't appear to be a problem
                    return scorer.GetPuddingPotentialScore() - oldPuddingScore;
                }
        default:
            {
                return 0.0f;
            }
        }
    }

    public override void Dispose()
    {
        counter.Dispose();
    }
}
