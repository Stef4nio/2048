﻿using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public static class GameModel {

    private static Cell[,] GameField = new Cell[Config.FieldHeight, Config.FieldWidth];
    private static Cell[,] PreviousMoveField = new Cell[Config.FieldHeight, Config.FieldWidth];


    public static void SetCell(int x,int y,int value)
    {
        GameField[y, x] = CellFactory.CreateCell(value);
        EventSystem.ModelModifiedInvoke(null);
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
        GameField[y, x] = value;
        EventSystem.ModelModifiedInvoke(null);
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
        }
        EventSystem.ModelModifiedInvoke(null);
    }

    public static void SetColumn(Cell[] column, int columnPosition)
    {
        for (int i = 0; i < column.Length; i++)
        {
            GameField[i, columnPosition] = column[i];
        }
        EventSystem.ModelModifiedInvoke(null);
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

                if ((PreviousMoveField[i, j] ?? new Cell(0)).value != (GameField[i, j] ?? new Cell(0)).value)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
