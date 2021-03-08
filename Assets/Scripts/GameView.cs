using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

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

    [Inject]
    private GameModel _model;
    [Inject] 
    private EventSystem _eventSystem;

    //private CellView[,] Field = new CellView[Config.FieldHeight, Config.FieldWidth];

    private List<CellView> _cellViews = new List<CellView>();
    private int _movingCellsAmount;
    private bool isWon = false;


    // Use this for initialization
    private void Awake ()
    {
        _eventSystem.OnModelModified += RefreshField;
        _eventSystem.OnGameOver += FinishGame;
        _eventSystem.OnWin += OnGameWon;
        GetComponent<InputDetecter>().OnWinContinueClick += ContinuePlaying;
        GetComponent<InputDetecter>().OnYesButtonClick += OnAnswerButtonClick;
        GetComponent<InputDetecter>().OnUndoButtonClick += Undo;
        _scoreText.text = "0";
        SpriteState state = new SpriteState {pressedSprite = _restartButtonNormalHoverStateSprite};
        _restartButton.GetComponent<Button>().spriteState = state;
        _restartButton.GetComponent<Button>().image.sprite = _restartButtonNormalStateSprite;
    }

    /// <summary>
    /// Unlocks the game, after winning, allowing the player to continue his game
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ContinuePlaying(object sender, EventArgs e)
    {
        WinPanel.Disable();
        SpriteState state = new SpriteState {pressedSprite = _restartButtonNormalHoverStateSprite};
        _restartButton.GetComponent<Button>().spriteState = state;
        _restartButton.GetComponent<Button>().image.sprite = _restartButtonNormalStateSprite;
        _model.State = GameState.Idle;
    }

    /// <summary>
    /// Sets a flag if a game is won
    /// </summary>
    /// <param name="sender">Standard sender parameter</param>
    /// <param name="e">Standard eventArgs parameter</param>
    private void OnGameWon(object sender, EventArgs e)
    {
        isWon = true;
    }

    /// <summary>
    /// Congratulates the player for winning the game
    /// </summary>
    public void WinGame()
    {
        _model.State = GameState.Win;
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


    /// <summary>
    /// Reacts to a user confirming his will to restart a game
    /// </summary>
    /// <param name="sender">Standard sender parameter</param>
    /// <param name="e">Standard eventArgs parameter</param>
    private void OnAnswerButtonClick(object sender, EventArgs e)
    {
        Restart();
        GameOverPanel.Disable();
    }


    /// <summary>
    /// Starts the undo sequence
    /// </summary>
    /// <param name="sender">Standard sender parameter</param>
    /// <param name="e">Standard eventArgs parameter</param>
    public void Undo(object sender, EventArgs e)
    {
        if (_model.State == GameState.GameOver)
        {
            GameOverPanel.Disable();
            SpriteState state = new SpriteState {pressedSprite = _restartButtonNormalHoverStateSprite};
            _restartButton.GetComponent<Button>().spriteState = state;
            _restartButton.GetComponent<Button>().image.sprite = _restartButtonNormalStateSprite;
            _model.State = GameState.Idle;
        }
        if (_model.State == GameState.Win)
        {
            WinPanel.Disable();
            SpriteState state = new SpriteState {pressedSprite = _restartButtonNormalHoverStateSprite};
            _restartButton.GetComponent<Button>().spriteState = state;
            _restartButton.GetComponent<Button>().image.sprite = _restartButtonNormalStateSprite;
            _model.State = GameState.Idle;
        }
        if (_model.State == GameState.Idle)
        {
            
            _eventSystem.OnUndoInvoke();
            for (int i = 0; i < _model.AllCells.Count; i++)
            {
                if (_model.AllCells[i].isReadyToDestroy)
                {
                    CellView cellView = GetCellViewById(_model.AllCells[i].id);
                    Debug.Log("<color=\"red\">DESTROY: " + cellView.CellData.ToString() + "</color>");
                    //_model.UnregisterCell(cellView.CellData.id, true);
                    cellView.Kill();
                    _cellViews.RemoveAll(c => c.CellData.id == cellView.CellData.id);
                }
            }
            _model.FinalizeUndo();
            RefreshField();
        }
        else
        {
            Debug.Log("Undo DENIED!");
        }
    }

    /// <summary>
    /// A debug function to prematurely "lose" the game and test game's behaviour
    /// </summary>
    public void ArtificialFinishGame()
    {
        _model.State = GameState.GameOver;
        SpriteState state = new SpriteState {pressedSprite = _restartButtonEmphasizedHoverStateSprite};
        _restartButton.GetComponent<Button>().spriteState = state;
        _restartButton.GetComponent<Button>().image.sprite = _restartButtonEmphasizedStateSprite;
        GameOverPanel.OnGameLost();
    }

    /// <summary>
    /// Finishes a game, showing the player his results and asking him to restart
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FinishGame(object sender, EventArgs e)
    {
        _model.State = GameState.GameOver;
        SpriteState state = new SpriteState {pressedSprite = _restartButtonEmphasizedHoverStateSprite};
        _restartButton.GetComponent<Button>().spriteState = state;
        _restartButton.GetComponent<Button>().image.sprite = _restartButtonEmphasizedStateSprite;
        GameOverPanel.OnGameLost();
    }


    /// <summary>
    /// Refreshes models on a scene to correspond the gameModel
    /// </summary>
    /// <param name="sender">Standard sender parameter</param>
    /// <param name="e">Standard eventArgs parameter</param>
    private void RefreshField(object sender=null, EventArgs e = null)
    {
        //CheckDataAvailability();
        //DebugPanel.Instance.PrintGridCurrent(_model.GameField);
        _scoreText.text = _model.GameScore.ToString();
        _HighScoreText.text = _model.GameHighScore.ToString();
        _movingCellsAmount = 0;
        foreach (ModelCell cell in _model.AllCells)
        {
            if (_model.State == GameState.Idle)
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


            if (_model.State == GameState.Moving)
            {
                if (cell.offsetX != 0 || cell.offsetY != 0)
                {

                    var currCellView = GetCellViewById(cell.id);
                    _movingCellsAmount++;
                    currCellView.Move(OnMovementFinished, _model.GetPreviousCellById(cell.id));
                }
            }
        }

    }


    /// <summary>
    /// Restarts the game
    /// </summary>
    public void Restart()
    {
        SpriteState state = new SpriteState();

        if (_model.State != GameState.Moving)
        {
            foreach (var cellView in _cellViews)
            {
                cellView.Kill();
            }

            _cellViews.Clear();

            state.highlightedSprite = _restartButtonNormalHoverStateSprite;
            _restartButton.GetComponent<Button>().spriteState = state;
            _restartButton.GetComponent<Button>().image.sprite = _restartButtonNormalStateSprite;
            _eventSystem.OnRestartInvoke();
            
        }
        else if(_model.State == GameState.Win)
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
   

    /// <summary>
    /// Fixes the game field after moving the cells
    /// </summary>
    private void OnMovementFinished()
    {
        _movingCellsAmount--;
        if (_movingCellsAmount <= 0)
        {
            _movingCellsAmount = 0;
            _model.State = GameState.Idle;
            RefreshField();

            for (int i = _cellViews.Count-1; i >= 0; i--)
            {
                var cellView = _cellViews[i];
                if (cellView.CellData.isReadyToDestroy)
                {
                    Debug.Log("<color=\"red\">DESTROY: " + cellView.CellData.ToString()+"</color>");
                    _model.UnregisterCell(cellView.CellData.id);
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
            _model.ResetMultiplies();
            _eventSystem.OnMovementFinishedInvoke();
            if (isWon)
            {
                WinGame();
            }
            //DebugPanel.Instance.PrintGridCurrent(GameModel.GameField);
        }
    }

    /// <summary>
    /// Returns a cellView by its id
    /// </summary>
    /// <param name="cellId">An id of a requested cell</param>
    /// <returns>Requested cell view</returns>
    private CellView GetCellViewById(int cellId)
    {
        return _cellViews.FirstOrDefault(c => c.CellData.id == cellId);
    }

 
}
