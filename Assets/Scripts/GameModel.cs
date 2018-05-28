using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;


public enum GameState
{
    Idle,
    Moving
}

public static class GameModel
{


    public static GameState State = GameState.Idle;

    public static List<Cell> AllCells = new List<Cell>();

    public static Cell[,] GameField = new Cell[Config.FieldHeight, Config.FieldWidth];
    public static Cell[,] PreviousMoveField = new Cell[Config.FieldHeight, Config.FieldWidth];


    public static Cell CreateAndSetCell(int _x,int _y,int value)
    {
        GameField[_y, _x] = CellFactory.CreateCell(value);
        GameField[_y, _x].x = _x;
        GameField[_y, _x].y = _y;

        RegisterCell(GameField[_y, _x]);

        return GameField[_y, _x];
    }

    public static void RegisterCell(Cell cell)
    {
        AllCells.Add(cell);
    }

    public static void UnregisterCell(int id)
    {
        int amount = AllCells.RemoveAll(c => c.id == id);
        Debug.Log("From AllCells removed "+amount+" elements");
    }

    public static bool DoesCellExist(int x, int y)
    {
        return(GameField[y,x]!=null);
    }


    public static Cell GetCellFromPrevious(int x, int y)
    {
        return GameField[y, x];
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

    public static bool CompareLastAndCurrentMove()
    {
        for (int i = 0; i < Config.FieldHeight; i++)
        {
            for (int j = 0; j < Config.FieldWidth; j++)
            {
                if ((PreviousMoveField[i, j]??new Cell(0,0)).value != (GameField[i, j]??new Cell(0,0)).value)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public static List<Cell> GetKilledCells()
    {
        List<Cell> killedCells = new List<Cell>();
        foreach (var previousCell in PreviousMoveField)
        {
            if (previousCell == null)
            {
                continue;
            }
            int rejects = 0;
            foreach (var cell in GameField)
            {
                if (cell == null)
                {
                    rejects++;
                    continue;
                }

                if (previousCell.id != cell.id)
                {
                    rejects++;
                }
            }

            if (rejects == Config.FieldHeight * Config.FieldWidth)
            {
                killedCells.Add(previousCell);
            }
        }

        if (killedCells.Count == 0)
        {
            return null;
        }

        return killedCells;
    }

    public static void SavePreviousState()
    {
        for (int j = 0; j < Config.FieldHeight; j++)
        {
            for (int i = 0; i < Config.FieldWidth; i++)
            {
                PreviousMoveField[j, i] = GameField[j, i]!=null ? GameField[j, i].Clone() : null;
            }
        }

        DebugPanel.Instance.PrintGridBefore(PreviousMoveField);
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
