using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystem {

    public event EventHandler<InputEventArg> OnSwipe;
    public event EventHandler OnModelModified;
    public event EventHandler OnGameOver;
    public event EventHandler OnWin;
    public event EventHandler OnUndo;
    public event EventHandler OnRestart;
    public event EventHandler OnMovementFinished;
    public event EventHandler<InputEventArg> OnCurrentSwipeDirectionChanged;


    public void OnSwipeInvoke(object sender, InputEventArg arg)
    {
        if (OnSwipe != null)
        {
            OnSwipe.Invoke(null, arg);
        }
    }

    public void OnCurrentSwipeDirectionChangedInvoke(InputEventArg arg, object sender = null)
    {
        if (OnCurrentSwipeDirectionChanged != null)
        {
            OnCurrentSwipeDirectionChanged.Invoke(sender,arg);
        }
    }

    public void ModelModifiedInvoke(object sender = null)
    {
        if (OnModelModified != null)
        {
            OnModelModified.Invoke(null, EventArgs.Empty);
        }
    }

    public void OnGameOverInvoke(object sender = null)
    {
        if (OnGameOver != null)
        {
            OnGameOver.Invoke(null, EventArgs.Empty);
        }
    }

    public void OnWinInvoke(object sender = null)
    {
        if (OnWin != null)
        {
            OnWin.Invoke(null, EventArgs.Empty);
        }
    }

    public void OnUndoInvoke(object sender = null)
    {
        if (OnUndo != null)
        {
            OnUndo.Invoke(null, EventArgs.Empty);
        }
    }

    public void OnRestartInvoke(object sender = null)
    {
        if (OnRestart != null)
        {
            OnRestart.Invoke(null, EventArgs.Empty);
        }
    }

    public void OnMovementFinishedInvoke(object sender = null)
    {
        if (OnMovementFinished != null)
        {
            OnMovementFinished.Invoke(null, EventArgs.Empty);
        }
    }
}

public class InputEventArg : EventArgs
{
    public Direction CurrDirection;
}



