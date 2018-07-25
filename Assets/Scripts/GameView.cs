using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{
    [SerializeField] private GameOverPanel GameOverPanel;
    [SerializeField] private WinPanel WinPanel;
    [SerializeField] private Transform _gamePanelTransform;
    [SerializeField] private GameObject _cellPrefab;
    [SerializeField] private Button _undoButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Text _scoreText;
    [SerializeField] private Text _HighScoreText;
    [SerializeField] private Sprite[]  _uiCellBackgroundSprites = new Sprite[12];
    [SerializeField] private Sprite _restartButtonNormalStateSprite;
    [SerializeField] private Sprite _restartButtonNormalHoverStateSprite;
    [SerializeField] private Sprite _restartButtonEmphasizedStateSprite;
    [SerializeField] private Sprite _restartButtonEmphasizedHoverStateSprite;



    //private CellView[,] Field = new CellView[Config.FieldHeight, Config.FieldWidth];

    private List<CellView> _cellViews = new List<CellView>();
    private int _movingCellsAmount;
    private bool isWon = false;


    // Use this for initialization
    private void Awake ()
    {
	    EventSystem.OnModelModified += RefreshField;
        EventSystem.OnGameOver += FinishGame;
        EventSystem.OnWin += OnGameWon;
        GetComponent<InputDetecter>().OnWinContinueClick += ContinuePlaying;
        GetComponent<InputDetecter>().OnYesButtonClick += OnAnswerButtonClick;
        GetComponent<InputDetecter>().OnUndoButtonClick += Undo;
        _scoreText.text = "0";
        SpriteState state = new SpriteState {pressedSprite = _restartButtonNormalHoverStateSprite};
        _restartButton.GetComponent<Button>().spriteState = state;
        _restartButton.GetComponent<Button>().image.sprite = _restartButtonNormalStateSprite;
    }

    private void ContinuePlaying(object sender, EventArgs e)
    {
        WinPanel.Disable();
        SpriteState state = new SpriteState {pressedSprite = _restartButtonNormalHoverStateSprite};
        _restartButton.GetComponent<Button>().spriteState = state;
        _restartButton.GetComponent<Button>().image.sprite = _restartButtonNormalStateSprite;
        GameModel.State = GameState.Idle;
    }

    private void OnGameWon(object sender, EventArgs e)
    {
        isWon = true;
    }

    public void WinGame()
    {
        GameModel.State = GameState.Win;
        SpriteState state = new SpriteState {pressedSprite = _restartButtonEmphasizedHoverStateSprite};
        _restartButton.GetComponent<Button>().spriteState = state;
        _restartButton.GetComponent<Button>().image.sprite = _restartButtonEmphasizedStateSprite;
        WinPanel.OnWin();
        isWon = false;
    }

    /*/ private void OnGameOverRestartButtonClick(object sender, EventArgs e)
     {
         Restart();
         GameOverPanel.gameObject.SetActive(false);
     }*/


    private void OnAnswerButtonClick(object sender, EventArgs e)
    {
        Restart();
        GameOverPanel.Disable();
    }


    public void Undo(object sender, EventArgs e)
    {
        if (GameModel.State == GameState.GameOver)
        {
            GameOverPanel.Disable();
            SpriteState state = new SpriteState {pressedSprite = _restartButtonNormalHoverStateSprite};
            _restartButton.GetComponent<Button>().spriteState = state;
            _restartButton.GetComponent<Button>().image.sprite = _restartButtonNormalStateSprite;
            GameModel.State = GameState.Idle;
        }
        if (GameModel.State == GameState.Win)
        {
            WinPanel.Disable();
            SpriteState state = new SpriteState {pressedSprite = _restartButtonNormalHoverStateSprite};
            _restartButton.GetComponent<Button>().spriteState = state;
            _restartButton.GetComponent<Button>().image.sprite = _restartButtonNormalStateSprite;
            GameModel.State = GameState.Idle;
        }
        if (GameModel.State == GameState.Idle)
        {
            
            EventSystem.OnUndoInvoke();
            for (int i = 0; i < GameModel.AllCells.Count; i++)
            {
                if (GameModel.AllCells[i].isReadyToDestroy)
                {
                    CellView cellView = GetCellViewById(GameModel.AllCells[i].id);
                    Debug.Log("<color=\"red\">DESTROY: " + cellView.CellData.ToString() + "</color>");
                    GameModel.UnregisterCell(cellView.CellData.id, true);
                    cellView.Kill();
                    i--;
                    _cellViews.RemoveAll(c => c.CellData.id == cellView.CellData.id);
                }
            }

            RefreshField();
        }
        else
        {
            Debug.Log("Undo DENIED!");
        }
    }

    public void ArtificialFinishGame()
    {
        GameModel.State = GameState.GameOver;
        SpriteState state = new SpriteState {pressedSprite = _restartButtonEmphasizedHoverStateSprite};
        _restartButton.GetComponent<Button>().spriteState = state;
        _restartButton.GetComponent<Button>().image.sprite = _restartButtonEmphasizedStateSprite;
        GameOverPanel.OnGameLost();
    }

    private void FinishGame(object sender, EventArgs e)
    {
        GameModel.State = GameState.GameOver;
        SpriteState state = new SpriteState {pressedSprite = _restartButtonEmphasizedHoverStateSprite};
        _restartButton.GetComponent<Button>().spriteState = state;
        _restartButton.GetComponent<Button>().image.sprite = _restartButtonEmphasizedStateSprite;
        GameOverPanel.OnGameLost();
    }


    private void RefreshField(object sender=null, EventArgs e = null)
    {
        //CheckDataAvailability();
        //DebugPanel.Instance.PrintGridCurrent(GameModel.GameField);
        _scoreText.text = GameModel.GameScore.ToString();
        _HighScoreText.text = GameModel.GameHighScore.ToString();
        _movingCellsAmount = 0;
        foreach (Cell cell in GameModel.AllCells)
        {
            if (GameModel.State == GameState.Idle)
            {
                if (cell.isNew)
                {
                    var cellView = Instantiate(_cellPrefab, _gamePanelTransform).GetComponent<CellView>();
                    cell.isNew = false;
                    if (cell.doAnimate)
                    {
                        cellView.AnimateSpawn();
                    }

                    int spriteNumber = Convert.ToInt32(Math.Log(cell.value, 2))-1;
                    if (spriteNumber > 10)
                    {
                        spriteNumber = 11;
                    }
                    cellView.GetComponent<Image>().sprite =
                        _uiCellBackgroundSprites[spriteNumber];
                    cellView.SetCellData(cell);
                    _cellViews.Add(cellView);
                }
            }


            if (GameModel.State == GameState.Moving)
            {
                if (cell.offsetX != 0 ||
                    cell.offsetY != 0)
                {

                    var currCellView = GetCellViewById(cell.id);
                    _movingCellsAmount++;
                    currCellView.Move(OnMovementFinished);
                }
            }
        }

    }


    public void Restart()
    {
        SpriteState state = new SpriteState();

        if (GameModel.State != GameState.Moving)
        {
            foreach (var cellView in _cellViews)
            {
                cellView.Kill();
            }

            _cellViews.Clear();

            state.highlightedSprite = _restartButtonNormalHoverStateSprite;
            _restartButton.GetComponent<Button>().spriteState = state;
            _restartButton.GetComponent<Button>().image.sprite = _restartButtonNormalStateSprite;
            EventSystem.OnRestartInvoke();
            
        }
        else if(GameModel.State == GameState.Win)
        {
            WinPanel.Disable();
            state.highlightedSprite = _restartButtonNormalHoverStateSprite;
            _restartButton.GetComponent<Button>().spriteState = state;
            _restartButton.GetComponent<Button>().image.sprite = _restartButtonNormalStateSprite;
        }
        else
        {
            Debug.Log("Restart DENIED!!!");
        }
    }
   

    private void OnMovementFinished()
    {
        _movingCellsAmount--;
        if (_movingCellsAmount <= 0)
        {
            _movingCellsAmount = 0;
            GameModel.State = GameState.Idle;
            RefreshField();

            for (int i = _cellViews.Count-1; i >= 0; i--)
            {
                var cellView = _cellViews[i];
                if (cellView.CellData.isReadyToDestroy)
                {
                    Debug.Log("<color=\"red\">DESTROY: " + cellView.CellData.ToString()+"</color>");
                    GameModel.UnregisterCell(cellView.CellData.id);
                    cellView.Kill();
                    _cellViews.RemoveAll(c => c.CellData.id == cellView.CellData.id);
                }
            }

            foreach (var cellView in _cellViews)
            {
                if (cellView.CellData.isMultiply)
                {
                    cellView.AnimateMultiplication();
                }
            }
            GameModel.ResetMultiplies();
            EventSystem.OnMovementFinishedInvoke();
            if (isWon)
            {
                WinGame();
            }
            //DebugPanel.Instance.PrintGridCurrent(GameModel.GameField);
        }
    }


    private CellView GetCellViewById(int cellId)
    {
        return _cellViews.FirstOrDefault(c => c.CellData.id == cellId);
    }

 
}
