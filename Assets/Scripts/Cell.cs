using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public int id;
    public int value = 0;
    public int offsetX = 0;
    public int offsetY = 0;
    public int x = 0;
    public int y = 0;
    public bool isNew = false;
    public bool isMultiply = false;
    public bool isReadyToDestroy = false;

    public Cell(int val,int _id)
    {
        id = _id;
        isNew = true;
        value = val;
    }

    public Cell Clone()
    {
        var clone = new Cell(0,0);
        clone.id = id;
        clone.value = value;
        clone.offsetX = offsetX;
        clone.offsetY = offsetY;
        clone.x = x;
        clone.y = y;
        clone.isNew = isNew;
        clone.isReadyToDestroy = isReadyToDestroy;
        clone.isMultiply = isMultiply;
        return clone;
    }


    public override string ToString()
    {
        return "ID=" + id + " [" + value + "] x=" + x + " y=" + y + " ox=" + offsetX + " oy=" + offsetY;
    }

}
