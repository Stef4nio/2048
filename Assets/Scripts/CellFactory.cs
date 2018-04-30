using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CellFactory{

    public static Cell CreateCell(int value)
    {
        return new Cell(value);
    }
}
