using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventSystem {

    public static event EventHandler<InputEventArg> OnSwipe;
    public static event EventHandler OnModelModified;
    public static event EventHandler OnGameOver;
    public static event EventHandler OnWin;
    public static event EventHandler OnUndo;
    public static event EventHandler OnRestart;
    public static event EventHandler OnMovementFinished;
    public static event EventHandler<InputEventArg> OnCurrentSwipeDirectionChanged;


    public static void OnSwipeInvoke(object sender, InputEventArg arg)
    {
        if (OnSwipe != null)
        {
            OnSwipe.Invoke(null, arg);
        }
    }

    public static void OnCurrentSwipeDirectionChangedInvoke(InputEventArg arg, object sender = null)
    {
        if (OnCurrentSwipeDirectionChanged != null)
        {
            OnCurrentSwipeDirectionChanged.Invoke(sender,arg);
        }
    }

    public static void ModelModifiedInvoke(object sender = null)
    {
        if (OnModelModified != null)
        {
            OnModelModified.Invoke(null, EventArgs.Empty);
        }
    }

    public static void OnGameOverInvoke(object sender = null)
    {
        if (OnGameOver != null)
        {
            OnGameOver.Invoke(null, EventArgs.Empty);
        }
    }

    public static void OnWinInvoke(object sender = null)
    {
        if (OnWin != null)
        {
            OnWin.Invoke(null, EventArgs.Empty);
        }
    }

    public static void OnUndoInvoke(object sender = null)
    {
        if (OnUndo != null)
        {
            OnUndo.Invoke(null, EventArgs.Empty);
        }
    }

    public static void OnRestartInvoke(object sender = null)
    {
        if (OnRestart != null)
        {
            OnRestart.Invoke(null, EventArgs.Empty);
        }
    }

    public static void OnMovementFinishedInvoke(object sender = null)
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



