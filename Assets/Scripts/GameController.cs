using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using GameLogic2048;
using Zenject;

[RequireComponent(typeof(InputDetecter))]
public class GameController:MonoBehaviour{
    System.Random _rand;
    private bool isAlreadyWon;
    
    [Inject]
    private GameModel _model;
    [Inject] 
    private EventSystem _eventSystem;
    [Inject] 
    private CellFactory _cellFactory;


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
        int deltaScore = 0;
        if (e.CurrDirection == Direction.Up || e.CurrDirection == Direction.Down)
        {
            for (int i = 0; i < Config.FieldHeight; i++)
            {
                ModelCell[] column;
                column =  GameLogic<ModelCell,CellFactory>.Compressor(_model.GetColumn(i), _cellFactory,
                    e.CurrDirection.ToGameLogicDirection(), out deltaScore);
                _model.GameScore += deltaScore;
                Debug.LogError($"Delta score: {deltaScore}");
                _model.SetColumn(column,i);
            }
        }
        else if(e.CurrDirection != Direction.None)
        {
            for (int i = 0; i < Config.FieldWidth; i++)
            {
                ModelCell[] row;
                
                row = GameLogic<ModelCell,CellFactory>.Compressor(_model.GetRow(i), _cellFactory,
                    e.CurrDirection.ToGameLogicDirection(), out deltaScore);
                _model.GameScore += deltaScore;
                Debug.LogError($"Delta score: {deltaScore}");
                _model.SetRow(row,i);
            }
        }
        _model.GameScore += deltaScore;
        
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
            _model.PrepareForUndo();
        }        
    }

    private void saveInfo()
    {
        string info = JsonConvert.SerializeObject(_model.SaveInfo(isAlreadyWon));
        Debug.Log(info);
        PlayerPrefs.SetString(Config.PlayerInfoKey,info);
    }



}
