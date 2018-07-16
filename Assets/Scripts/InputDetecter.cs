using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public enum Direction
{
    Up,
    Down,
    Left,
    Right,
    None
};




public class InputDetecter : MonoBehaviour {
    Vector2 _swipeStart;
    Vector2 _deltaSwipe;
    Vector2 _swipeEnd;
    [SerializeField] private Button _restartPopupButton;
    [SerializeField] private Button _restartYesButton;
    [SerializeField] private Button _restartNoButton;
    [SerializeField] private Button _undoButton;
    [SerializeField] private Image _confirmationPopupPanel;
    public event EventHandler OnPopupButtonClick;
    public event EventHandler OnYesButtonClick;
    public event EventHandler OnNoButtonClick;
    public event EventHandler OnUndoButtonClick;


    // Use this for initialization
    void Start () {
        _restartPopupButton.onClick.AddListener(()=> {
            if (OnPopupButtonClick != null)
            {
                OnPopupButtonClick.Invoke(null,null);
            } });

        _restartYesButton.onClick.AddListener(() =>
        {
            if (OnYesButtonClick != null)
            {
                OnYesButtonClick.Invoke(null, null);
            }
        });

        _restartNoButton.onClick.AddListener(() => {
            if (OnNoButtonClick != null)
            {
                OnNoButtonClick.Invoke(null, null);
            } });

        _undoButton.onClick.AddListener(() =>
        {
            if (OnUndoButtonClick != null)
            {
                OnUndoButtonClick.Invoke(null, null);
            }
        });


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
                _swipeStart = new Vector2(t.position.x, t.position.y);
            }

            if (t.phase == TouchPhase.Ended)
            {
                _swipeStart = new Vector2(t.position.x, t.position.y);

                _deltaSwipe = new Vector2(_swipeEnd.x - _swipeStart.x, _swipeEnd.y - _swipeStart.y);

                if (_deltaSwipe.x > 50 && _deltaSwipe.y > -250f && _deltaSwipe.y < 250f)
                {
                    Debug.Log("Swipe right");
                }

                if (_deltaSwipe.x < -50 && _deltaSwipe.y > -250f && _deltaSwipe.y < 250f)
                {
                    Debug.Log("Swipe left");
                }

                if (_deltaSwipe.y > 50 && _deltaSwipe.x > -250f && _deltaSwipe.x < 250f)
                {
                    Debug.Log("Swipe up");
                }

                if (_deltaSwipe.y < -50 && _deltaSwipe.x > -250f && _deltaSwipe.x < 250f)
                {
                    Debug.Log("Swipe down");
                }
            }
            return;
        */
        if (Input.GetMouseButtonDown(0))
        {
            _swipeStart = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        if (Input.GetMouseButtonUp(0))
        {
            _swipeEnd = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            _deltaSwipe = new Vector2(_swipeEnd.x - _swipeStart.x, _swipeEnd.y - _swipeStart.y);
            float deltaX = Math.Abs(_deltaSwipe.x);
            float deltaY = Math.Abs(_deltaSwipe.y);


            if (deltaY < Config.MinSwipeLenght || deltaX < Config.MinSwipeLenght)
            {
                return;
            }

            if (deltaX > deltaY && _deltaSwipe.x > 0)
            {
                Debug.Log("Swipe right");
               /* if (GameModel.State == GameState.Idle)
                {*/
                    EventSystem.OnSwipeInvoke(this, new InputEventArg() { CurrDirection = Direction.Right });
                /*}
                else
                {
                    Debug.Log("Swipe right DENIED!");
                }*/
                
            }

            if (deltaX > deltaY && _deltaSwipe.x < 0)
            {
                Debug.Log("Swipe left");
                /*if (GameModel.State == GameState.Idle)
                {*/
                    EventSystem.OnSwipeInvoke(this, new InputEventArg() { CurrDirection = Direction.Left });
                /*}
                else
                {
                    Debug.Log("Swipe left DENIED!");
                }
                */
            }

            if (deltaY > deltaX && _deltaSwipe.y > 0)
            {
                Debug.Log("Swipe up");
                /*if (GameModel.State == GameState.Idle)
                {*/
                    EventSystem.OnSwipeInvoke(this, new InputEventArg() { CurrDirection = Direction.Up });
               /* }
                else
                {
                    Debug.Log("Swipe up DENIED!");
                }*/

            }

            if (deltaY > deltaX && _deltaSwipe.y < 0)
            {
                Debug.Log("Swipe down");
                /*if (GameModel.State == GameState.Idle)
                {*/
                    EventSystem.OnSwipeInvoke(this, new InputEventArg() { CurrDirection = Direction.Down });
                /*}
                else
                {
                    Debug.Log("Swipe down DENIED!");
                }*/

            }
        }

    }
}