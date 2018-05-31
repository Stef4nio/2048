using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventSystem {

    public static event EventHandler<InputEventArg> OnSwipe;
    public static event EventHandler OnModelModified;
    public static event EventHandler OnGameOver;

    public static void OnSwipeInvoke(object sender, InputEventArg arg)
    {
        if (OnSwipe != null)
        {
            OnSwipe.Invoke(null, arg);
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
        if (OnModelModified != null)
        {
            OnGameOver.Invoke(null, EventArgs.Empty);
        }
    }
}

public class InputEventArg : EventArgs
{
    public Directions CurrDirection;
}



