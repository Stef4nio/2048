using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(InputDetecter))]
public class GameController:MonoBehaviour{
    System.Random _rand;
    private bool isAlreadyWon;
    
    [Inject]
    private GameModel _model;
    [Inject] 
    private EventSystem _eventSystem;


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
        _eventSystem.OnCurrentSwipeDirectionChanged += OnSwipe;
        _eventSystem.OnRestart += OnRestart;
        _eventSystem.OnUndo += OnUndo;
        //PlayerPrefs.DeleteAll();
        InfoContainer container =
            JsonConvert.DeserializeObject<InfoContainer>(PlayerPrefs.GetString(Config.PlayerInfoKey));
        _model.LoadInfo(container);
        Init();
        isAlreadyWon = container.IsAlreadyWon;
        if (container.IsGameover)
        {
            _eventSystem.OnGameOverInvoke();
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
        _eventSystem.ModelModifiedInvoke();
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
        _eventSystem.ModelModifiedInvoke();

        if (_model.IsWon() && !isAlreadyWon)
        {
            Win();
        }
        if (_model.IsGameModelFilledUp())
        {
            if (_model.IsLose())
            {
                _eventSystem.OnGameOverInvoke();
            }
        }

    }

    private void Win()
    {
        _eventSystem.OnWinInvoke();
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
        }        
    }
    

    public bool IsEqual(ModelCell[,] array1, ModelCell[,] array2)
    {
        return array1.IsEqual(array2);
    }

    //TODO: get this thing the fuck out of here and even put it in another file
    private ModelCell[] Compressor (ModelCell[] row, Direction direction)
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
