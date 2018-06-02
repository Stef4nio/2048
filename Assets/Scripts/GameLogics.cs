using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class GameLogics:MonoBehaviour{
    System.Random _rand;

    private void Awake()
    {
        var seed = Environment.TickCount;
        
        seed = 5;
        _rand = new System.Random(seed);
        Debug.Log("SEED=" + seed);
    }

    private void Start()
    {
        EventSystem.OnSwipe += OnSwipe;
        AddRandomCell();
        AddRandomCell();
        EventSystem.ModelModifiedInvoke();
        
    }


    private void OnSwipe(object sender, InputEventArg e)
    {
        GameModel.SavePreviousState();

        switch(e.CurrDirection)
        {
            case Directions.Up:
                for (int i = 0; i < Config.FieldHeight; i++)
                {
                    GameModel.SetColumn(Compressor(GameModel.GetColumn(i), e.CurrDirection),i);
                }
                break;

            case Directions.Down:
                for (int i = 0; i < Config.FieldHeight; i++)
                {
                    GameModel.SetColumn(Compressor(GameModel.GetColumn(i), e.CurrDirection), i);
                }
                break;

            case Directions.Left:
                for (int i = 0; i < Config.FieldWidth; i++)
                {
                    GameModel.SetRow(Compressor(GameModel.GetRow(i), e.CurrDirection), i);
                }
                break;

            case Directions.Right:
                for (int i = 0; i < Config.FieldWidth; i++)
                {
                    GameModel.SetRow(Compressor(GameModel.GetRow(i), e.CurrDirection), i);
                }
                break;
        }

        if (GameModel.isGameModelFilledUp() && IsLose())
        {
            EventSystem.OnGameOverInvoke();
        }

        if (!GameModel.CompareLastAndCurrentMove())
        {
            AddRandomCell();
            GameModel.State = GameState.Moving;
        }



        EventSystem.ModelModifiedInvoke();
    }


    public void Undo()
    {
        for (int i = 0; i < Config.FieldWidth; i++)
        {
            GameModel.SetRow(GameModel.GetRowToPrevious(i), i);
        }

        GameModel.State = GameState.Moving;
        EventSystem.ModelModifiedInvoke();
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
            if(!GameModel.DoesCellExist(rndX,rndY))
            {
                isSet = true;
                GameModel.CreateAndSetCell(rndX, rndY, rndValue);
            }
        }
    }


    private bool IsLose()
    {
        Cell[,] tempCells = new Cell[Config.FieldHeight,Config.FieldWidth];
        tempCells = (Cell[,]) GameModel.GameField.Clone();
        for (int i = 0; i < Config.FieldHeight; i++)
        {
            GameModel.SetColumn(Compressor(GameModel.GetColumn(i),Directions.Up), i);
        }

        for (int i = 0; i < Config.FieldHeight; i++)
        {
            GameModel.SetColumn(Compressor(GameModel.GetColumn(i), Directions.Down), i);
        }

        for (int i = 0; i < Config.FieldHeight; i++)
        {
            GameModel.SetColumn(Compressor(GameModel.GetColumn(i), Directions.Left), i);
        }

        for (int i = 0; i < Config.FieldHeight; i++)
        {
            GameModel.SetColumn(Compressor(GameModel.GetColumn(i),Directions.Right), i);
        }

        return GameModel.IsEqual(tempCells,GameModel.GameField);
    }

    private Cell[] Compressor (Cell[] row, Directions directions)
    {
        int changes;
        if (directions==Directions.Down || directions == Directions.Right)
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
                    switch (directions)
                    {
                        case Directions.Up:
                            row[i].offsetY--;
                            break;
                        case Directions.Down:
                            row[i].offsetY++;
                            break;
                        case Directions.Left:
                            row[i].offsetX--;
                            break;
                        case Directions.Right:
                            row[i].offsetX++;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("directions", directions, null);
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
                    var newCell = CellFactory.CreateCell(row[i].value *= 2);
                    GameModel.RegisterCell(newCell);

                    newCell.isMultiply = true;

                    switch (directions)
                    {
                        case Directions.Up:
                            row[i].offsetY--;
                            break;
                        case Directions.Down:
                            row[i].offsetY++;
                            break;
                        case Directions.Left:
                            row[i].offsetX--;
                            break;
                        case Directions.Right:
                            row[i].offsetX++;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("directions", directions, null);
                    }

                    row[i - 1] = newCell;
                    row[i] = null;

                    changes++;
                }
            }
        } while (changes != 0);

        if (directions == Directions.Down || directions == Directions.Right)
        {
            Array.Reverse(row);
        }
        return row;
    }
    
}
