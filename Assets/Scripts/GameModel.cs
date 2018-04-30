using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameModel {

    public static Cell[,] GameField = new Cell[4,4];
    public static Cell[,] PreviousMoveField = new Cell[4, 4];


    public static void AddCell(int x,int y,int value)
    {
        GameField[y, x] = CellFactory.CreateCell(value);
        EventSystem.ModelModifiedInvoke(null);
    }

    public static Cell GetCell(int x, int y)
    {
        return GameField[y, x] ?? null;
    }

    public static void SetRow(Cell[] row, int rowPosition)
    {
        for(int i = 0;i<4;i++)
        {
            GameField[rowPosition, i] = row[i];
        }
        EventSystem.ModelModifiedInvoke(null);
    }

    public static void SetColumn(Cell[] column, int columnPosition)
    {
        for (int i = 0; i < 4; i++)
        {
            GameField[i, columnPosition] = column[i];
        }
        EventSystem.ModelModifiedInvoke(null);
    }

    public static Cell[] GetColumn(int columnPosition)
    {
        Cell[] output = new Cell[4];
        for (int i = 0; i < 4; i++)
        {
            output[i] = GameField[i, columnPosition];
        }
        return output;
    }

    public static Cell[] GetRow(int rowPosition)
    {
        Cell[] output = new Cell[4];
        for (int i = 0; i < 4; i++)
        {
            output[i] = GameField[rowPosition, i];
        }
        return output;
    }
}
