using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventSystem {

    public static event EventHandler<InputEventArg> OnSwipe;
    public static event EventHandler ModelModified;

    public static void OnSwipeInvoke(object sender, InputEventArg arg)
    {
        OnSwipe.Invoke(null, arg);
    }

    public static void ModelModifiedInvoke(object sender)
    {
        ModelModified.Invoke(null, EventArgs.Empty);
    }

    

}

public class InputEventArg : System.EventArgs
{
    public Directions CurrDirection;
}


