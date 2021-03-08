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

    /// <summary>
    /// Default cell constructor
    /// </summary>
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

    /// <summary>
    /// Cell constructor
    /// </summary>
    /// <param name="val">Cells value</param>
    /// <param name="_id">Cells id</param>
    /// <param name="DoAnimate">Does cell need to be animated on popup</param>
    public ModelCell(int val,int _id, bool DoAnimate)
    {
        id = _id;
        isNew = true;
        value = val;
        doAnimate = DoAnimate;
    }

    /// <summary>
    /// Cell deep copy
    /// </summary>
    /// <returns>A new cell, identical to current</returns>
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

    /// <summary>
    /// Checks cells for equality
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        ModelCell operand = obj as ModelCell;
        return operand?.id==id;
    }

    /// <summary>
    /// Return cells id, as its hash
    /// </summary>
    /// <returns>Cells hash</returns>
    public override int GetHashCode()
    {
        return id;
    }

    /// <summary>
    /// Converts the cell into a string
    /// </summary>
    /// <returns>A string containing info about the cell</returns>
    public override string ToString()
    {
        return "ID=" + id + " [" + value + "] x=" + x + " y=" + y + " ox=" + offsetX + " oy=" + offsetY;
    }
    
}
