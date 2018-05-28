using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventSystem {

    public static event EventHandler<InputEventArg> OnSwipe;
    public static event EventHandler OnModelModified;

    public static void OnSwipeInvoke(object sender, InputEventArg arg)
    {
        if (OnSwipe != null)
        {
            OnSwipe.Invoke(null, arg);
        }
    }

    public static void ModelModifiedInvoke(object sender)
    {
        if (OnModelModified != null)
        {
            OnModelModified.Invoke(null, EventArgs.Empty);
        }
    }
}

public class InputEventArg : EventArgs
{
    public Directions CurrDirection;
}



