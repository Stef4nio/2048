using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogics:MonoBehaviour{
    System.Random rand = new System.Random();

    private void Start()
    {
        EventSystem.OnSwipe += OnSwipe;
        AddRandomCell();
        AddRandomCell();
    }

    private void OnSwipe(object sender, InputEventArg e)
    {   
        for(int i = 0;i <4;i++)
        {
            for (int j = 0; j < 4; j++)
            {
                GameModel.SetCellToPrevious(j,i, GameModel.GetCell(j, i));
            }
        }
        switch(e.CurrDirection)
        {
            case Directions.Up:
                for(int i = 0;i < 4;i++)
                {
                    GameModel.SetColumn(Compressor(GameModel.GetColumn(i),false),i);
                }
                break;

            case Directions.Down:
                for (int i = 0; i < 4; i++)
                {
                    GameModel.SetColumn(Compressor(GameModel.GetColumn(i), true), i);
                }
                break;

            case Directions.Left:
                for (int i = 0; i < 4; i++)
                {
                    GameModel.SetRow(Compressor(GameModel.GetRow(i), false), i);
                }
                break;

            case Directions.Right:
                for (int i = 0; i < 4; i++)
                {
                    GameModel.SetRow(Compressor(GameModel.GetRow(i), true), i);
                }
                break;
        }
        if (GameModel.CompareLastAndCurrentMove())
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    GameModel.SetCell(j,i, GameModel.GetCellFromPrevious(j, i).value);
                }
            }
        }
        else
        {
            AddRandomCell();
        }
    }

    private void AddRandomCell()
    {
        int rndX = 0;
        int rndY = 0;
        bool isSet = false;
        int rndValue = rand.Next(100) <= 80 ? 2 : 4;
        while (!isSet)
        {
            rndX = rand.Next(4);
            rndY = rand.Next(4);
            if(!GameModel.DoesCellExist(rndX,rndY))
            {
                isSet = true;
                GameModel.SetCell(rndX, rndY, rndValue);
            }
        }
    }

    private Cell[] Compressor (Cell[] row, bool isReverse)
    {
        int changes = 0;
        if (isReverse)
        {
            Array.Reverse(row);
        }
        do
        {
            changes = 0;
            for (int i = 1; i < 4; i++)
            {
                if (row[i] != null && row[i - 1] == null)
                {
                    row[i - 1] = row[i];
                    row[i] = null;
                    row[i - 1].offset++;
                    changes++;
                }
                if (row[i] != null && row[i - 1] != null && !(row[i].isMultiply || row[i - 1].isMultiply) && row[i - 1].value == row[i].value)
                {
                    row[i - 1].value *= 2;
                    row[i] = null;
                    row[i - 1].isMultiply = true;
                    row[i - 1].offset++;
                    changes++;
                }
            }
        } while (changes != 0);
        if (isReverse)
        {
            Array.Reverse(row);
        }
        return row;
    }
    
}
