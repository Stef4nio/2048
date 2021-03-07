using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(InputDetecter))]
public class GameLogics:MonoBehaviour{
    System.Random _rand;
    private bool isAlreadyWon;
    
    [Inject]
    private GameModel _model;


    private void Awake()
    {
        var seed = Environment.TickCount;
        GetComponent<InputDetecter>().OnPopupButtonClick += OnRestartButtonClick;
        GetComponent<InputDetecter>().OnNoButtonClick += OnAnswerButtonClick;
        GetComponent<InputDetecter>().OnYesButtonClick += OnAnswerButtonClick;

        //seed = 5;
        _rand = new System.Random(seed);
        Debug.Log("SEED=" + seed);
    }

    private void OnAnswerButtonClick(object sender, EventArgs e)
    {
        EnableControls();
    }


    private void Start()
    {
        EventSystem.OnCurrentSwipeDirectionChanged += OnSwipe;
        EventSystem.OnRestart += OnRestart;
        EventSystem.OnUndo += OnUndo;
        //PlayerPrefs.DeleteAll();
        InfoContainer container =
            JsonConvert.DeserializeObject<InfoContainer>(PlayerPrefs.GetString(Config.PlayerInfoKey));
        _model.LoadInfo(container);
        Init();
        isAlreadyWon = container.IsAlreadyWon;
        if (container.IsGameover)
        {
            EventSystem.OnGameOverInvoke();
        }
    }

    private void OnRestartButtonClick(object sender,EventArgs e)
    {
        if (_model.State != GameState.GameOver)
        {
            DisableControls();
        }
        saveInfo();
    }

    private void OnRestart(object sender = null, EventArgs e = null)
    {
        _model.Restart();
        Init();
    }

    public void Init()
    {
        if (_model.AllCells.Count == 0)
        {
            AddRandomCell();
            AddRandomCell();
        }
        EventSystem.ModelModifiedInvoke();
    }

    private void OnSwipe(object sender, InputEventArg e)
    {
        _model.SavePreviousState();

        switch(e.CurrDirection)
        {
            case Direction.Up:
                for (int i = 0; i < Config.FieldHeight; i++)
                {
                    _model.SetColumn(Compressor(_model.GetColumn(i), e.CurrDirection),i);
                }
                break;

            case Direction.Down:
                for (int i = 0; i < Config.FieldHeight; i++)
                {
                    _model.SetColumn(Compressor(_model.GetColumn(i), e.CurrDirection), i);
                }
                break;

            case Direction.Left:
                for (int i = 0; i < Config.FieldWidth; i++)
                {
                    _model.SetRow(Compressor(_model.GetRow(i), e.CurrDirection), i);
                }
                break;

            case Direction.Right:
                for (int i = 0; i < Config.FieldWidth; i++)
                {
                    _model.SetRow(Compressor(_model.GetRow(i), e.CurrDirection), i);
                }
                break;
        }

        if (!_model.AreLastAndCurrentMoveEqual())
        {
            AddRandomCell();
            _model.State = GameState.Moving;
            _model.isUndone = false;
        }

        if (_model.GameScore > _model.GameHighScore)
        {
            _model.GameHighScore = _model.GameScore;
        }
        saveInfo();
        EventSystem.ModelModifiedInvoke();

        if (isWon() && !isAlreadyWon)
        {
            Win();
        }
        if (_model.IsGameModelFilledUp())
        {
            if (IsLose())
            {
                EventSystem.OnGameOverInvoke();
            }
        }

    }

    private void Win()
    {
        EventSystem.OnWinInvoke();
        isAlreadyWon = true;
    }

    private void AddRandomCell()
    {
        int rndX = 0;
        int rndY = 0;
        bool isSet = false;
        int rndValue = _rand.Next(100) <= 80 ? 2 : 4;
        while (!isSet)
        {
            rndX = _rand.Next(Config.FieldWidth);
            rndY = _rand.Next(Config.FieldHeight);
            if(!_model.DoesCellExist(rndX,rndY))
            {
                isSet = true;
                _model.CreateAndSetCell(rndX, rndY, rndValue,true);
            }
        }
    }

    public void DisableControls()
    {
        _model.State = GameState.Pause;
    }

    public void EnableControls()
    {
        _model.State = GameState.Idle;
    }

    private void OnUndo(object sender = null, EventArgs e = null)
    {
        if (!_model.isUndone)
        {
            _model.Undo();
            _model.isUndone = true;
        }        
    }

    //TODO move to gamemodel
    private bool isWon()
    {
        return _model.AllCells.Find(c => c.value == 2048) != null;
    }

    //TODO move to gamemodel
    private bool IsLose()
    {
        Cell[,] tempCells =(Cell[,]) _model.GameField.Clone();
        for (int i = 0; i < Config.FieldHeight; i++)
        {
            for (int j = 0; j < Config.FieldWidth; j++)
            {
                int _currValue = tempCells[i, j].value;
                try
                {
                    if (tempCells[i - 1, j].value == _currValue)
                    {
                        return false;
                    }
                }catch{}
                try
                {
                    if (tempCells[i + 1, j].value == _currValue)
                    {
                        return false;
                    }
                } catch{}
                try
                {
                    if (tempCells[i, j - 1].value == _currValue)
                    {
                        return false;
                    }
                }catch{}
                try
                {
                    if (tempCells[i, j + 1].value == _currValue)
                    {
                        return false;
                    }
                }catch {}
            }
        }
        return true;
    }

    //TODO move to gamemodel or even overload the operator or write extension method
    public bool IsEqual(Cell[,] array1, Cell[,] array2)
    {
        for (int i = 0; i < array1.GetLength(0); i++)
        {
            for (int j = 0; j < array1.GetLength(1); j++)
            {
                if (array1[i, j] == null && array2[i, j] == null)
                {
                    continue;
                }

                if (array1[i, j] == null ^ array2[i, j] == null)
                {
                    return false;
                }

                if (array1[i, j].id != array2[i, j].id)
                {
                    return false;
                }
            }
        }
        return true;
    }

    //TODO: get this thing the fuck out of here and even put it in another file
    private Cell[] Compressor (Cell[] row, Direction direction)
    {
        int changes;
        if (direction==Direction.Down || direction == Direction.Right)
        {
            Array.Reverse(row);
        }

        do
        {
            changes = 0;
            for (int i = 1; i < row.Length; i++)
            {
                if (row[i] != null && row[i - 1] == null)
                {
                    switch (direction)
                    {
                        case Direction.Up:
                            row[i].offsetY--;
                            break;
                        case Direction.Down:
                            row[i].offsetY++;
                            break;
                        case Direction.Left:
                            row[i].offsetX--;
                            break;
                        case Direction.Right:
                            row[i].offsetX++;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("direction", direction, null);
                    }

                    row[i - 1] = row[i];
                    row[i] = null;

                   
                    changes++;
                    continue;
                }

                if (row[i] != null && row[i - 1] != null && 
                    !(row[i].isMultiply || row[i - 1].isMultiply) && 
                    row[i - 1].value == row[i].value)
                {
                    row[i - 1].isReadyToDestroy = true;
                    row[i].isReadyToDestroy = true;
                    Debug.Log("READY TO DESTROY: "+ row[i].ToString());
                    Debug.Log("READY TO DESTROY: "+ row[i - 1].ToString());
                    var newCell = CellFactory.CreateCell(row[i].value *= 2,false);
                    _model.RegisterCell(newCell);

                    newCell.isMultiply = true;

                    switch (direction)
                    {
                        case Direction.Up:
                            row[i].offsetY--;
                            break;
                        case Direction.Down:
                            row[i].offsetY++;
                            break;
                        case Direction.Left:
                            row[i].offsetX--;
                            break;
                        case Direction.Right:
                            row[i].offsetX++;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("direction", direction, null);
                    }

                    row[i - 1] = newCell;
                    row[i] = null;
                    _model.GameScore += row[i - 1].value;
                    changes++;
                }
            }
        } while (changes != 0);

        if (direction == Direction.Down || direction == Direction.Right)
        {
            Array.Reverse(row);
        }


        return row;
    }

    private void saveInfo()
    {
        string info = JsonConvert.SerializeObject(_model.SaveInfo(isAlreadyWon));
        Debug.Log(info);
        PlayerPrefs.SetString(Config.PlayerInfoKey,info);
    }



}
