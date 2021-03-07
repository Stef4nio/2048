using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SwipeManager:MonoBehaviour
{
    private Direction CurrentSwipeDirection = Direction.None;
    private Direction UpcomingSwipeDirection = Direction.None;

    [Inject]
    private GameModel _model;
    [Inject] 
    private EventSystem _eventSystem;
    
    void Awake()
    {
        _eventSystem.OnSwipe += OnSwipe;
        _eventSystem.OnMovementFinished += OnMovementFinished;
        
        PlayerPrefs.DeleteAll();
    }

    private void OnMovementFinished(object sender, EventArgs e)
    {
        CurrentSwipeDirection = UpcomingSwipeDirection;
        UpcomingSwipeDirection = Direction.None;
        _eventSystem.OnCurrentSwipeDirectionChangedInvoke(new InputEventArg{CurrDirection = CurrentSwipeDirection});
        CurrentSwipeDirection = Direction.None;
    }

    private void OnSwipe(object sender, InputEventArg e)
    {
        if (_model.State == GameState.Idle)
        {
            CurrentSwipeDirection = e.CurrDirection;
            _eventSystem.OnCurrentSwipeDirectionChangedInvoke(new InputEventArg { CurrDirection = CurrentSwipeDirection });
        }
        else if (_model.State == GameState.Moving)
        {
            UpcomingSwipeDirection = e.CurrDirection;
        }
    }
}
