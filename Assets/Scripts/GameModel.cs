using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public static class GameModel {

    public static Cell[,] GameField = new Cell[Config.FieldHeight, Config.FieldWidth];
    public static Cell[,] PreviousMoveField = new Cell[Config.FieldHeight, Config.FieldWidth];


    public static void SetCell(int _x,int _y,int value)
    {
        GameField[_y, _x] = CellFactory.CreateCell(value);
        GameField[_y, _x].x = _x;
        GameField[_y, _x].y = _y;
        //EventSystem.ModelModifiedInvoke(null);
    }

    public static bool DoesCellExist(int x, int y)
    {
        return(GameField[y,x]!=null);
    }

    public static Cell GetCell(int x, int y)
    {
        return GameField[y, x];
    }

    public static void SetCellToPrevious(int x, int y, Cell value)
    {
        PreviousMoveField[y, x] = value;
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
}
