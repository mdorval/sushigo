using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Used for predicting how much a pudding would be worth for AI
/// </summary>
class PredictPuddingScoreCard : IPuddingScorer
{
    private readonly int _puddingCount = 0;
    private int _puddingPotentialScore = 0;

    public PredictPuddingScoreCard(int puddingCount)
    {
        _puddingCount = puddingCount;
    }

    public int Puddings()
    {
        return _puddingCount;
    }

    public void SetPuddingPotentialScore(int puddingScore)
    {
        _puddingPotentialScore = puddingScore;
    }

    public int GetPuddingPotentialScore()
    {
        return _puddingPotentialScore;
    }
}
