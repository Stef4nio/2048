using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CellFactory
{
    private static int currentID = 0;

    public static Cell CreateCell(int value)
    {
        return new Cell(value, currentID++);
    }
}
