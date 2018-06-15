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
    [SerializeField] private Transform _gamePanelTransform;
    [SerializeField] private GameObject _cellPrefab;
    [SerializeField] private Button _undoButton;
    [SerializeField] private Text _scoreText;
    [SerializeField] private Text _HighScoreText;


    //private CellView[,] Field = new CellView[Config.FieldHeight, Config.FieldWidth];

    private List<CellView> _cellViews = new List<CellView>();
    private int _movingCellsAmount;


    // Use this for initialization
    private void Awake ()
    {
	    EventSystem.OnModelModified += RefreshField;
        EventSystem.OnGameOver += FinishGame;
        EventSystem.OnUndo += Undo;
        _scoreText.text = "0";
    }

    private void Undo(object sender, EventArgs e)
    {
        if (GameModel.State == GameState.Idle)
        {
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

    private void FinishGame(object sender, EventArgs e)
    {
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

            GameModel.ResetMultiplies();

           //DebugPanel.Instance.PrintGridCurrent(GameModel.GameField);
        }
    }

    private CellView GetCellViewById(int cellId)
    {
        return _cellViews.FirstOrDefault(c => c.CellData.id == cellId);
    }

 
}
