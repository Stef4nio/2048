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


    /// <summary>
    /// Invokes an onSwipe event
    /// </summary>
    /// <param name="sender">Standard sender parameter</param>
    /// <param name="arg">An event argument with swipe direction</param>
    public void OnSwipeInvoke(object sender, InputEventArg arg)
    {
        if (OnSwipe != null)
        {
            OnSwipe.Invoke(null, arg);
        }
    }

    /// <summary>
    /// If currentSwipe direction is changed (a new sequential swipe is loaded from a buffer) invokes a corresponding
    /// event
    /// </summary>
    /// <param name="arg">SAn event argument with swipe direction</param>
    /// <param name="sender">Standard sender parameter</param>
    public void OnCurrentSwipeDirectionChangedInvoke(InputEventArg arg, object sender = null)
    {
        if (OnCurrentSwipeDirectionChanged != null)
        {
            OnCurrentSwipeDirectionChanged.Invoke(sender,arg);
        }
    }

    /// <summary>
    /// If there were some changes in gameModel, this event is invoked
    /// </summary>
    /// <param name="sender">An event argument with swipe direction</param>
    public void ModelModifiedInvoke(object sender = null)
    {
        if (OnModelModified != null)
        {
            OnModelModified.Invoke(null, EventArgs.Empty);
        }
    }

    /// <summary>
    /// If player loses, this event is invoked
    /// </summary>
    /// <param name="sender"></param>
    public void OnGameOverInvoke(object sender = null)
    {
        if (OnGameOver != null)
        {
            OnGameOver.Invoke(null, EventArgs.Empty);
        }
    }

    /// <summary>
    /// If player wins, this event is invoked
    /// </summary>
    /// <param name="sender"></param>
    public void OnWinInvoke(object sender = null)
    {
        if (OnWin != null)
        {
            OnWin.Invoke(null, EventArgs.Empty);
        }
    }

    /// <summary>
    /// If player presses Undo button, this event is invoked
    /// </summary>
    /// <param name="sender"></param>
    public void OnUndoInvoke(object sender = null)
    {
        if (OnUndo != null)
        {
            OnUndo.Invoke(null, EventArgs.Empty);
        }
    }

    /// <summary>
    /// If player presses Restart button, this event is invoked
    /// </summary>
    /// <param name="sender"></param>
    public void OnRestartInvoke(object sender = null)
    {
        if (OnRestart != null)
        {
            OnRestart.Invoke(null, EventArgs.Empty);
        }
    }

    /// <summary>
    /// If all animations are finished, this event is invoked
    /// </summary>
    /// <param name="sender"></param>
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



