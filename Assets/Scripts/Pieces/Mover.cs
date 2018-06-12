using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Callback function type for when moveRequest is completed, is called with Mover object
 **/
public delegate void MoveCompleteCallback(GameObject movedObject);

/**
 * Request to Mover MonoBehavior object to a destination.
 **/
public class MoveRequest
{
    ///<summary>Creates a Request to Mover MonoBehavior object to a destination.</summary> 
    ///<param name="destination">The desintation to move to</param>
    ///<param name="moveParent">The GameObject to parent to after the move is completed</param>
    ///<param name="callback">The callback to call after the function is done</param>
    ///<param name="speed">The speed in which to move (multiplied by TimeDelta)</param>
    public MoveRequest(Vector3 destination, GameObject moveParent, MoveCompleteCallback callback, int speed = 12)
    {
        _destination = destination;
        _moveParent = moveParent;
        _callback = callback;
        _speed = speed;
    }
    Vector3 _destination;
    MoveCompleteCallback _callback;
    GameObject _moveParent;
    int _speed;

    /// <summary>
    /// The Destination To Move To
    /// </summary>
    /// <returns>Destination</returns>
    public Vector3 Destination() { return _destination; }
    /// <summary>
    /// The callback to call after the function is done
    /// </summary>
    /// <returns>Callback</returns>
    public MoveCompleteCallback Callback() { return _callback; }
    /// <summary>
    /// The GameObject to parent to after the move is completed 
    /// </summary>
    /// <returns>MoveParent</returns>
    public GameObject MoveParent() { return _moveParent; }
    /// <summary>
    /// The speed in which to move (multiplied by TimeDelta)
    /// </summary>
    /// <returns>speed</returns>
    public int Speed() { return _speed; }
}

public class Mover : MonoBehaviour {
    protected MoveRequest _moveRequest = null;
    protected bool pause = false;
    protected bool onBeforeMoveRan = false;
    /// <summary>
    /// Sets a Move Request for this Mover. if this is not paused, it will start moving right away.
    /// </summary>
    /// <param name="moveRequest">The Request to Move To</param>
    public void SetMoveRequest(MoveRequest moveRequest)
    {
        _moveRequest = moveRequest;
    }
    /// <summary>
    /// Pauses this Mover. Use Play() to resume Moving
    /// </summary>
    public void Pause()
    {
        pause = true;
    }
    /// <summary>
    /// Resumes This Mover. If this Mover is not paused, has no effect.
    /// </summary>
    public void Resume()
    {
        pause = false;
    }
    /// <summary>
    /// Function called once before a move has started (for each moverequest)
    /// </summary>
    protected virtual void OnBeforeMove()
    {

    }
    /// <summary>
    /// Function to Move. This function must be in your Update
    /// </summary>
    protected void Move()
    {
        if (_moveRequest != null && !pause)
        {
            if (!onBeforeMoveRan)
            {
                OnBeforeMove();
                onBeforeMoveRan = true;
            }
            transform.position = Vector3.MoveTowards(transform.position, _moveRequest.Destination(), Time.deltaTime * _moveRequest.Speed());
            if (transform.position == _moveRequest.Destination())
            {
                MoveRequest completedRequest = _moveRequest;
                _moveRequest = null;
                onBeforeMoveRan = false;
                this.transform.SetParent(completedRequest.MoveParent().transform);
                completedRequest.Callback()(this.gameObject);
            }
        }
    }
}
