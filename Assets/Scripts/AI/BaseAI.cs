using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAI: IDisposable {
    /// <summary>
    /// Creates a New AI
    /// </summary>
    /// <param name="ourPlayer">Player for this AI</param>
    public BaseAI(ComputerPlayer ourPlayer)
    {
        _player = ourPlayer;
    }

    protected ComputerPlayer _player;
    protected LinkedList<Player> _otherPlayers;
    /// <summary>
    /// Picks the best card to play from the given pack
    /// </summary>
    /// <param name="pack">A Pack of Cards</param>
    /// <returns>A specific card to play</returns>
    public abstract CardType ChooseCard(List<CardType> pack);
    public abstract void Dispose();
}
