using System.Collections;
using System.Collections.Generic;
using GameLogic2048;
using UnityEngine;

public class ModelCell:ICell
{
    public int id { get; }
    public int value { get; set; }
    public int offsetX { get; set; }
    public int offsetY { get; set; }
    public bool isMultiply { get; set; }
    public bool isReadyToDestroy { get; set; }
    public bool isNew { get; set; }
    public int x = 0;
    public int y = 0;
    public bool doAnimate = false;

    public ModelCell()
    {
        id = 0;
        value = 0;
        offsetX = 0;
        offsetY = 0;
        isMultiply = false;
        isNew = true;
        isReadyToDestroy = false;
    }

    public ModelCell(int val,int _id, bool DoAnimate)
    {
        id = _id;
        isNew = true;
        value = val;
        doAnimate = DoAnimate;
    }

    public ModelCell Clone()
    {
        var clone = new ModelCell(value,id,false);
        clone.offsetX = offsetX;
        clone.offsetY = offsetY;
        clone.x = x;
        clone.y = y;
        clone.isNew = isNew;
        clone.isReadyToDestroy = isReadyToDestroy;
        clone.isMultiply = isMultiply;
        return clone;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        ModelCell operand = obj as ModelCell;
        return operand?.id==id;
    }

    public override int GetHashCode()
    {
        return id;
    }

    public override string ToString()
    {
        return "ID=" + id + " [" + value + "] x=" + x + " y=" + y + " ox=" + offsetX + " oy=" + offsetY;
    }
    
}
