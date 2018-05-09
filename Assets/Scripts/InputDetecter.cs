using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public enum Directions
{
    Up,
    Down,
    Left,
    Right
};


public class InputDetecter : MonoBehaviour {
    Vector2 swipeStart;
    Vector2 deltaSwipe;
    Vector2 swipeEnd;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Swipe();
	}

    private void Swipe()
    {

        /*if (Input.touches.Length > 0)
        {
            Touch t = new Touch();
            if (t.phase == TouchPhase.Began)
            {
                swipeStart = new Vector2(t.position.x, t.position.y);
            }

            if (t.phase == TouchPhase.Ended)
            {
                swipeStart = new Vector2(t.position.x, t.position.y);

                deltaSwipe = new Vector2(swipeEnd.x - swipeStart.x, swipeEnd.y - swipeStart.y);

                if (deltaSwipe.x > 50 && deltaSwipe.y > -250f && deltaSwipe.y < 250f)
                {
                    Debug.Log("Swipe right");
                }

                if (deltaSwipe.x < -50 && deltaSwipe.y > -250f && deltaSwipe.y < 250f)
                {
                    Debug.Log("Swipe left");
                }

                if (deltaSwipe.y > 50 && deltaSwipe.x > -250f && deltaSwipe.x < 250f)
                {
                    Debug.Log("Swipe up");
                }

                if (deltaSwipe.y < -50 && deltaSwipe.x > -250f && deltaSwipe.x < 250f)
                {
                    Debug.Log("Swipe down");
                }
            }
            return;
        */
        if (Input.GetMouseButtonDown(0))
        {
            swipeStart = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        if (Input.GetMouseButtonUp(0))
        {
            swipeEnd = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            deltaSwipe = new Vector2(swipeEnd.x - swipeStart.x, swipeEnd.y - swipeStart.y);

            if (deltaSwipe.x > 50 && deltaSwipe.y > -250f && deltaSwipe.y < 250f)
            {
                EventSystem.OnSwipeInvoke(this, new InputEventArg() { CurrDirection = Directions.Right });
                Config.CurrentDirection = Directions.Right;
                Debug.Log("Swipe right");
            }

            if (deltaSwipe.x < -50 && deltaSwipe.y > -250f && deltaSwipe.y < 250f)
            {
                EventSystem.OnSwipeInvoke(this, new InputEventArg() { CurrDirection = Directions.Left });
                Config.CurrentDirection = Directions.Left;
                Debug.Log("Swipe left");
            }

            if (deltaSwipe.y > 50 && deltaSwipe.x > -250f && deltaSwipe.x < 250f)
            {
                EventSystem.OnSwipeInvoke(this, new InputEventArg() { CurrDirection = Directions.Up });
                Config.CurrentDirection = Directions.Up;
                Debug.Log("Swipe up");
            }

            if (deltaSwipe.y < -50 && deltaSwipe.x > -250f && deltaSwipe.x < 250f)
            {
                EventSystem.OnSwipeInvoke(this, new InputEventArg() { CurrDirection = Directions.Down });
                Config.CurrentDirection = Directions.Down;
                Debug.Log("Swipe down");
            }
        }

    }
}