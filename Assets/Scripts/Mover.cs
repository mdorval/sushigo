using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void MoveCompleteCallback(GameObject movedObject);

public class MoveRequest
{
    public MoveRequest(Vector3 destination, GameObject moveParent, MoveCompleteCallback callback, int speed = 12)
    {
        _destination = destination;
        _moveParent = moveParent;
        _callback = callback;
        _speed = speed;
    }
    ~MoveRequest() { }
    Vector3 _destination;
    MoveCompleteCallback _callback;
    GameObject _moveParent;
    int _speed;
    public Vector3 Destination() { return _destination; }
    public MoveCompleteCallback Callback() { return _callback; }
    public GameObject MoveParent() { return _moveParent; }
    public int Speed() { return _speed; }
}

public class Mover : MonoBehaviour {
    protected MoveRequest _moveRequest = null;
    protected bool pause = false;

    public void SetMoveRequest(MoveRequest moveRequest)
    {
        _moveRequest = moveRequest;
    }
    public void Pause()
    {
        pause = true;
    }
    public void Play()
    {
        pause = false;
    }
    protected virtual void OnBeforeMove()
    {

    }
    protected void Move()
    {
        if (_moveRequest != null && !pause)
        {
            OnBeforeMove();
            transform.position = Vector3.MoveTowards(transform.position, _moveRequest.Destination(), Time.deltaTime * _moveRequest.Speed());
            if (transform.position == _moveRequest.Destination())
            {
                MoveRequest completedRequest = _moveRequest;
                _moveRequest = null;
                this.transform.SetParent(completedRequest.MoveParent().transform);
                completedRequest.Callback()(this.gameObject);
            }
        }
    }
}
