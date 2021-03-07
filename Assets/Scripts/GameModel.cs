using System;
using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngineInternal;
using GameLogic2048;
using Zenject;


public enum GameState
{
    Idle,
    Moving,
    Pause,
    GameOver,
    Win
}

public class GameModel
{

    //TODO: save all the info here in playerPrefs
    public GameState State = GameState.Idle;

    public List<ModelCell> AllCells = new List<ModelCell>();
    public int GameScore = 0;
    public int GameHighScore = 0;
    public int PreviousScore = 0;
    public bool isUndone = false;
    public ModelCell[,] GameField = new ModelCell[Config.FieldHeight, Config.FieldWidth];
    public ModelCell[,] PreviousMoveField = new ModelCell[Config.FieldHeight, Config.FieldWidth];
    public ModelCell[,] TemporaryMoveField = new ModelCell[Config.FieldHeight, Config.FieldWidth];

    [Inject] 
    private CellFactory _cellFactory;



    public void LoadInfo(InfoContainer info)
    {
        if (info != null)
        {
            AllCells = info.AllCells.ToList();
            GameScore = info.GameScore;
            GameHighScore = info.GameHighScore;
            PreviousScore = info.PreviousScore;
            isUndone = info.IsUndone;
            GameField = info.GameField ?? new ModelCell[Config.FieldHeight, Config.FieldWidth];
            PreviousMoveField = info.PreviousMoveField ?? new ModelCell[Config.FieldHeight, Config.FieldWidth];
            TemporaryMoveField = info.TemporaryMoveField ?? new ModelCell[Config.FieldHeight, Config.FieldWidth];
            _cellFactory.Load(info.CurrentId);
            for (int i = 0; i < Config.FieldHeight; i++)
            {
                for (int j = 0; j < Config.FieldWidth; j++)
                {
                    if (GameField[i, j] != null)
                    {
                        GameField[i, j] = AllCells.Find(c => GameField[i, j].id == c.id);
                    }
                }
            }

            foreach (var cell in AllCells)
            {
                cell.isNew = true;
            }
        }
        //GarbageCollect();
    }



    public ModelCell CreateAndSetCell(int _x,int _y,int value,bool doAnimate)
    {
        GameField[_y, _x] = _cellFactory.CreateCell(value,doAnimate);
        GameField[_y, _x].x = _x;
        GameField[_y, _x].y = _y;

        RegisterCell(GameField[_y, _x]);

        return GameField[_y, _x];
    }

    public void RegisterCell(ModelCell cell)
    {
        if (!AllCells.Contains(cell)&&cell!=null)
        {
            AllCells.Add(cell);
        }
    }

    public void UnregisterCell(int id, bool isUndo = false)
    {
        int amount = 0;
        if (isUndo)
        {
            amount = AllCells.RemoveAll(c => c.id == id&&c.isReadyToDestroy);
        }
        else
        {
            amount = AllCells.RemoveAll(c => c.id == id);
        }
        Debug.Log("From AllCells removed " + amount + " elements");
    }

    public bool DoesCellExist(int x, int y)
    {
        return(GameField[y,x]!=null);
    }

    public void SetRow(ModelCell[] row, int rowPosition)
    {        
        row.ToList().ForEach(RegisterCell);
        for(int i = 0;i<row.Length;i++)
        {
            GameField[rowPosition, i] = row[i];
            if (GameField[rowPosition, i] != null)
            {
                GameField[rowPosition, i].x = i;
                GameField[rowPosition, i].y = rowPosition;
            }
        }
        //EventSystem.ModelModifiedInvoke(null);
    }

    public void SetRowToPrevious(ModelCell[] row, int rowPosition)
    {
        for (int i = 0; i < row.Length; i++)
        {
            PreviousMoveField[rowPosition, i] = row[i];
            if (PreviousMoveField[rowPosition, i] != null)
            {
                PreviousMoveField[rowPosition, i].x = i;
                PreviousMoveField[rowPosition, i].y = rowPosition;
            }
        }
        //EventSystem.ModelModifiedInvoke(null);
    }

    public void SetColumn(ModelCell[] column, int columnPosition)
    {
        column.ToList().ForEach(RegisterCell);
        for (int i = 0; i < column.Length; i++)
        {
            GameField[i, columnPosition] = column[i];
            if (GameField[i, columnPosition] != null)
            {
                GameField[i, columnPosition].x = columnPosition;
                GameField[i, columnPosition].y = i;
            }
        }
        //EventSystem.ModelModifiedInvoke(null);
    }

    internal void PrepareForUndo()
    {
        GameScore = PreviousScore;
        for (int i = 0; i < Config.FieldHeight; i++)
        {
            for (int j = 0; j < Config.FieldWidth; j++)
            {
                if (GameField[i, j] != null)
                {
                    AllCells.Find(c => Equals(c, GameField[i, j])).isReadyToDestroy = true;
                    Debug.Log("READY TO DESTROY: " + GameField[i, j].ToString());
                }

                GameField[i, j] = GetCellFromPrevious(j,i);
                if (GameField[i, j] != null)
                {
                    RegisterCell(GameField[i, j]);
                    GameField[i, j].isNew = true;
                }
            }
        }
        isUndone = true;
    }

    private void ClearReadyToDestroyCells()
    {
        AllCells.RemoveAll(x => x.isReadyToDestroy);
    }

    internal void FinalizeUndo()
    {
        ClearReadyToDestroyCells();
        foreach (var modelCell in GameField)
        {
            RegisterCell(modelCell);
        }
    }
    
    public ModelCell GetCellFromPrevious(int x, int y)
    {
        return PreviousMoveField[y, x];
    }

    public void SetColumnToPrevious(ModelCell[] column, int columnPosition)
    {
        for (int i = 0; i < column.Length; i++)
        {
            PreviousMoveField[i, columnPosition] = column[i];
            if (PreviousMoveField[i, columnPosition] != null)
            {
                PreviousMoveField[i, columnPosition].x = columnPosition;
                PreviousMoveField[i, columnPosition].y = i;
            }
        }
        //EventSystem.ModelModifiedInvoke(null);
    }

    public void Restart()
    {
        GameScore = 0;
        isUndone = false;
        State = GameState.Idle;
        GameField = new ModelCell[Config.FieldHeight,Config.FieldWidth];
        PreviousMoveField = new ModelCell[Config.FieldHeight, Config.FieldWidth];
        TemporaryMoveField = new ModelCell[Config.FieldHeight, Config.FieldWidth];
        AllCells.Clear();
    }

    public ModelCell[] GetColumn(int columnPosition)
    {
        ModelCell[] output = new ModelCell[Config.FieldHeight];
        for (int i = 0; i < Config.FieldHeight; i++)
        {
            output[i] = GameField[i, columnPosition];
        }
        return output;
    }

    public ModelCell[] GetRow(int rowPosition)
    {
        ModelCell[] output = new ModelCell[Config.FieldWidth];
        for (int i = 0; i < Config.FieldWidth; i++)
        {
            output[i] = GameField[rowPosition, i];
        }
        return output;
    }
 

    public bool AreLastAndCurrentMoveEqual()
    {
        for (int i = 0; i < Config.FieldHeight; i++)
        {
            for (int j = 0; j < Config.FieldWidth; j++)
            {
                if ((PreviousMoveField[i, j]??new ModelCell(0,0,false)).value != (GameField[i, j]??new ModelCell(0,0,false)).value)
                {
                    return false;
                }
            }
        }
        for (int j = 0; j < Config.FieldHeight; j++)
        {
            for (int i = 0; i < Config.FieldWidth; i++)
            {
                PreviousMoveField[j, i] = TemporaryMoveField[j, i] != null ? TemporaryMoveField[j, i].Clone() : null;
            }
        }
        return true;
    }



    public void SavePreviousState()
    {
        PreviousScore = GameScore;
        for (int j = 0; j < Config.FieldHeight; j++)
        {
            for (int i = 0; i < Config.FieldWidth; i++)
            {
                TemporaryMoveField[j, i] = PreviousMoveField[j, i] != null ? PreviousMoveField[j, i].Clone() : null;
            }
        }
        for (int j = 0; j < Config.FieldHeight; j++)
        {
            for (int i = 0; i < Config.FieldWidth; i++)
            {
                PreviousMoveField[j, i] = GameField[j, i] != null ? GameField[j, i].Clone() : null;
            }
        }

        //DebugPanel.Instance.PrintGridBefore(PreviousMoveField);
    }

    public bool IsGameModelFilledUp()
    {
        int _filledCells = 0;
        foreach (var cell in GameField)
        {
            if (cell != null)
            {
                _filledCells++;
            }
        }

        return (_filledCells == Config.FieldHeight * Config.FieldWidth);
    }

    public ModelCell GetPreviousCellById(int id)
    {

        foreach (ModelCell cell in PreviousMoveField)
        {
            if (cell!=null && cell.id == id)
            {
                return cell;
            }
        }

        return null;
        //return PreviousMoveField.Cast<Cell>().FirstOrDefault(c => c.id == id);
    }

    public InfoContainer SaveInfo(bool isAlreadyWon)
    {
        return new InfoContainer(AllCells,GameScore,GameHighScore,PreviousScore,isUndone,
            GameField,PreviousMoveField,TemporaryMoveField,_cellFactory.currentID,
            (State == GameState.GameOver), isAlreadyWon);
    }


    public void ResetMultiplies()
    {
        foreach (var cell in AllCells)
        {
            cell.isMultiply = false;
            cell.offsetX = 0;
            cell.offsetY = 0;
        }
    }

    public bool IsWon()
    {
        return GameLogic<ModelCell,CellFactory>.IsWon(AllCells);
    }

    public bool IsLose()
    {
        return GameLogic<ModelCell,CellFactory>.IsLose(GameField,Config.FieldHeight,Config.FieldWidth);
    }
}

[Serializable]
public class InfoContainer
{
    public bool IsGameover;
    public bool IsAlreadyWon;
    public ModelCell[] AllCells ;
    public int GameScore ;
    public int GameHighScore ;
    public int PreviousScore ;
    public bool IsUndone ;
    public int CurrentId ;
    public ModelCell[,] GameField ;
    public ModelCell[,] PreviousMoveField ;
    public ModelCell[,] TemporaryMoveField ;

    public InfoContainer(List<ModelCell> _allCells,int _GameScore, int _GameHighScore, int _PreviousScore, bool _isUndone, ModelCell[,] _GameField, ModelCell[,] _PreviousMoveField, ModelCell[,] _TemporaryMoveField, int _currentId, bool _isGameover, bool _isAlreadyWon)
    {
        AllCells = (_allCells??new List<ModelCell>()).ToArray();
        GameScore = _GameScore;
        GameHighScore = _GameHighScore;
        PreviousScore = _PreviousScore;
        IsUndone = _isUndone;
        GameField = _GameField;
        PreviousMoveField = _PreviousMoveField;
        TemporaryMoveField = _TemporaryMoveField;
        CurrentId = _currentId;
        IsGameover = _isGameover;
        IsAlreadyWon = _isAlreadyWon;
    }
}
