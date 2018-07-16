using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CellFactory
{

    //TODO:save current ID to playerPrefs
    private static int currentID = 0;

    public static Cell CreateCell(int value,bool doAnimate)
    {
        return new Cell(value, currentID++,doAnimate);
    }
}
