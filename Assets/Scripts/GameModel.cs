using System;
using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngineInternal;


public enum GameState
{
    Idle,
    Moving,
    Pause,
    GameOver
}

public static class GameModel
{

    //TODO: save all the info here in playerPrefs
    public static GameState State = GameState.Idle;

    public static List<Cell> AllCells = new List<Cell>();
    public static int GameScore = 0;
    public static int GameHighScore = 0;
    public static int PreviousScore = 0;
    public static bool isUndone = false;
    public static Cell[,] GameField = new Cell[Config.FieldHeight, Config.FieldWidth];
    public static Cell[,] PreviousMoveField = new Cell[Config.FieldHeight, Config.FieldWidth];
    public static Cell[,] TemporaryMoveField = new Cell[Config.FieldHeight, Config.FieldWidth];



    public static void LoadInfo(InfoContainer info)
    {
        if (info != null)
        {
            foreach (var cell in info.AllCells)
            {
                AllCells.Add(cell);
            }
            GameScore = info.GameScore;
            GameHighScore = info.GameHighScore;
            PreviousScore = info.PreviousScore;
            isUndone = info.isUndone;
            GameField = info.GameField ?? new Cell[Config.FieldHeight, Config.FieldWidth];
            PreviousMoveField = info.PreviousMoveField ?? new Cell[Config.FieldHeight, Config.FieldWidth];
            TemporaryMoveField = info.TemporaryMoveField ?? new Cell[Config.FieldHeight, Config.FieldWidth];
            CellFactory.Load(info.currentID);
            foreach (var cell in GameField)
            {
                if (cell != null)
                {
                    cell.isNew = true;
                }
            }
        }
        //GarbageCollect();
    }



    public static Cell CreateAndSetCell(int _x,int _y,int value,bool doAnimate)
    {
        GameField[_y, _x] = CellFactory.CreateCell(value,doAnimate);
        GameField[_y, _x].x = _x;
        GameField[_y, _x].y = _y;

        RegisterCell(GameField[_y, _x]);

        return GameField[_y, _x];
    }

    public static void RegisterCell(Cell cell)
    {
        AllCells.Add(cell);
    }

    public static void UnregisterCell(int id, bool isUndo = false)
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

    private static void GarbageCollect()
    {
        foreach (var cell in AllCells)
        {
            if (Array.IndexOf(GameField, cell) == -1)
            {
                AllCells.Remove(cell);
            }
        }
    }

    public static bool DoesCellExist(int x, int y)
    {
        return(GameField[y,x]!=null);
    }

    public static void SetRow(Cell[] row, int rowPosition)
    {
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

    public static void SetRowToPrevious(Cell[] row, int rowPosition)
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

    public static void SetColumn(Cell[] column, int columnPosition)
    {
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

    internal static void Undo()
    {
        GameScore = PreviousScore;
        for (int i = 0; i < Config.FieldHeight; i++)
        {
            for (int j = 0; j < Config.FieldWidth; j++)
            {
                if (GameField[i, j] != null)
                {
                    AllCells.Find(c => c == GameField[i, j]).isReadyToDestroy = true;
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

    }

    public static Cell GetCellFromPrevious(int x, int y)
    {
        return PreviousMoveField[y, x];
    }

    public static void SetColumnToPrevious(Cell[] column, int columnPosition)
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

    public static void Restart()
    {
        GameHighScore = 0;
        GameScore = 0;
        isUndone = false;
        State = GameState.Idle;
        GameField = new Cell[Config.FieldHeight,Config.FieldWidth];
        PreviousMoveField = new Cell[Config.FieldHeight, Config.FieldWidth];
        TemporaryMoveField = new Cell[Config.FieldHeight, Config.FieldWidth];
        AllCells.Clear();
    }

    public static Cell[] GetColumn(int columnPosition)
    {
        Cell[] output = new Cell[Config.FieldHeight];
        for (int i = 0; i < Config.FieldHeight; i++)
        {
            output[i] = GameField[i, columnPosition];
        }
        return output;
    }

    public static Cell[] GetRow(int rowPosition)
    {
        Cell[] output = new Cell[Config.FieldWidth];
        for (int i = 0; i < Config.FieldWidth; i++)
        {
            output[i] = GameField[rowPosition, i];
        }
        return output;
    }
 

    public static bool AreLastAndCurrentMoveEqual()
    {
        for (int i = 0; i < Config.FieldHeight; i++)
        {
            for (int j = 0; j < Config.FieldWidth; j++)
            {
                if ((PreviousMoveField[i, j]??new Cell(0,0,false)).value != (GameField[i, j]??new Cell(0,0,false)).value)
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



    public static void SavePreviousState()
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

        DebugPanel.Instance.PrintGridBefore(PreviousMoveField);
    }

    public static bool IsGameModelFilledUp()
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

    public static Cell GetPreviousCellById(int id)
    {

        foreach (Cell cell in PreviousMoveField)
        {
            if (cell!=null && cell.id == id)
            {
                return cell;
            }
        }

        return null;
        //return PreviousMoveField.Cast<Cell>().FirstOrDefault(c => c.id == id);
    }

    public static InfoContainer SaveInfo()
    {
        return new InfoContainer(AllCells,GameScore,GameHighScore,PreviousScore,isUndone,GameField,PreviousMoveField,TemporaryMoveField,CellFactory.currentID);
    }


    public static void ResetMultiplies()
    {
        foreach (var cell in AllCells)
        {
            cell.isMultiply = false;
            cell.offsetX = 0;
            cell.offsetY = 0;
        }
    }
}

[Serializable]
public class InfoContainer
{
    public Cell[] AllCells ;
    public int GameScore ;
    public int GameHighScore ;
    public int PreviousScore ;
    public bool isUndone ;
    public int currentID ;
    public Cell[,] GameField ;
    public Cell[,] PreviousMoveField ;
    public Cell[,] TemporaryMoveField ;

    public InfoContainer(List<Cell> _allCells,int _GameScore, int _GameHighScore, int _PreviousScore, bool _isUndone, Cell[,] _GameField, Cell[,] _PreviousMoveField, Cell[,] _TemporaryMoveField, int _currentId)
    {
        AllCells = (_allCells??new List<Cell>()).ToArray();
        GameScore = _GameScore;
        GameHighScore = _GameHighScore;
        PreviousScore = _PreviousScore;
        isUndone = _isUndone;
        GameField = _GameField;
        PreviousMoveField = _PreviousMoveField;
        TemporaryMoveField = _TemporaryMoveField;
        currentID = _currentId;
    }
}
