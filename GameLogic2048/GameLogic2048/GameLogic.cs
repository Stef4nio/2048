using System;
using System.Collections.Generic;

namespace GameLogic2048
{
    public static class GameLogic<Cell, Factory> where Cell : ICell, new() where Factory:ICellFactory<Cell>

    {
    public static Cell[] Compressor(Cell[] row, Factory cellFactory, Direction direction, out int deltaScore)
    {
        int changes;
        deltaScore = 0;
        if (direction == Direction.Down || direction == Direction.Right)
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
                    row[i] = default;


                    changes++;
                    continue;
                }

                if (row[i] != null && row[i - 1] != null &&
                    !(row[i].isMultiply || row[i - 1].isMultiply) &&
                    row[i - 1].value == row[i].value)
                {
                    row[i - 1].isReadyToDestroy = true;
                    row[i].isReadyToDestroy = true;
                    var newCell = cellFactory.CreateCell(row[i].value *= 2, false);

                    //TODO: resolve this dependency in main project
                    //_model.RegisterCell(newCell);

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
                    row[i] = default;
                    //TODO: resolve this dependency in main project
                    //_model.GameScore += row[i - 1].value;
                    deltaScore += row[i - 1].value;
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

    public static bool IsWon(List<Cell> gameField)
    {
        return gameField.Find(c => c.value == 2048) != null;
    }

    public static bool IsLose(Cell[,] gameField, int fieldHeight, int fieldWidth)
    {
        for (int i = 0; i < fieldHeight; i++)
        {
            for (int j = 0; j < fieldWidth; j++)
            {
                int currValue = gameField[i, j].value;
                try
                {
                    if (gameField[i - 1, j].value == currValue)
                    {
                        return false;
                    }
                }
                catch
                {
                }

                try
                {
                    if (gameField[i + 1, j].value == currValue)
                    {
                        return false;
                    }
                }
                catch
                {
                }

                try
                {
                    if (gameField[i, j - 1].value == currValue)
                    {
                        return false;
                    }
                }
                catch
                {
                }

                try
                {
                    if (gameField[i, j + 1].value == currValue)
                    {
                        return false;
                    }
                }
                catch
                {
                }
            }
        }

        return true;
    }
    }
}