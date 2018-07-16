using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeManager:MonoBehaviour
{
    private Direction CurrentSwipeDirection = Direction.None;
    private Direction UpcomingSwipeDirection = Direction.None;

    void Awake()
    {
        EventSystem.OnSwipe += OnSwipe;
        EventSystem.OnMovementFinished += OnMovementFinished;
    }

    private void OnMovementFinished(object sender, EventArgs e)
    {
        CurrentSwipeDirection = UpcomingSwipeDirection;
        UpcomingSwipeDirection = Direction.None;
        EventSystem.OnCurrentSwipeDirectionChangedInvoke(new InputEventArg{CurrDirection = CurrentSwipeDirection});
        CurrentSwipeDirection = Direction.None;
    }

    private void OnSwipe(object sender, InputEventArg e)
    {
        if (GameModel.State == GameState.Idle)
        {
            CurrentSwipeDirection = e.CurrDirection;
            EventSystem.OnCurrentSwipeDirectionChangedInvoke(new InputEventArg { CurrDirection = CurrentSwipeDirection });
        }
        else if (GameModel.State == GameState.Moving)
        {
            UpcomingSwipeDirection = e.CurrDirection;
        }
    }
}
