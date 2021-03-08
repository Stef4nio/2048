using System;
using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngineInternal;
using GameLogic2048;
using Zenject;


public enum GameState
{
    Idle,
    Moving,
    Pause,
    GameOver,
    Win
}

public class GameModel
{

    //TODO: save all the info here in playerPrefs
    public GameState State = GameState.Idle;

    public List<ModelCell> AllCells = new List<ModelCell>();
    public int GameScore = 0;
    public int GameHighScore = 0;
    public int PreviousScore = 0;
    public bool isUndone = false;
    public ModelCell[,] GameField = new ModelCell[Config.FieldHeight, Config.FieldWidth];
    public ModelCell[,] PreviousMoveField = new ModelCell[Config.FieldHeight, Config.FieldWidth];
    public ModelCell[,] TemporaryMoveField = new ModelCell[Config.FieldHeight, Config.FieldWidth];

    [Inject] 
    private CellFactory _cellFactory;



    /// <summary>
    /// Loads previous gameState from playerPrefs
    /// </summary>
    /// <param name="info">An instance of a class with all required data</param>
    public void LoadInfo(InfoContainer info)
    {
        if (info != null)
        {
            AllCells = info.AllCells.ToList();
            GameScore = info.GameScore;
            GameHighScore = info.GameHighScore;
            PreviousScore = info.PreviousScore;
            isUndone = info.IsUndone;
            GameField = info.GameField ?? new ModelCell[Config.FieldHeight, Config.FieldWidth];
            PreviousMoveField = info.PreviousMoveField ?? new ModelCell[Config.FieldHeight, Config.FieldWidth];
            TemporaryMoveField = info.TemporaryMoveField ?? new ModelCell[Config.FieldHeight, Config.FieldWidth];
            _cellFactory.Load(info.CurrentId);
            for (int i = 0; i < Config.FieldHeight; i++)
            {
                for (int j = 0; j < Config.FieldWidth; j++)
                {
                    if (GameField[i, j] != null)
                    {
                        GameField[i, j] = AllCells.Find(c => GameField[i, j].id == c.id);
                    }
                }
            }

            foreach (var cell in AllCells)
            {
                cell.isNew = true;
            }
        }
        //GarbageCollect();
    }



    /// <summary>
    /// Creates a new cell and adds it to the gameModel
    /// </summary>
    /// <param name="_x">X coordinate of a new cell</param>
    /// <param name="_y">Y coordinate of a new cell</param>
    /// <param name="value">Value of a new cell</param>
    /// <param name="doAnimate">Does cell need to be animated at popup</param>
    /// <returns>Reference to a new cell from gameModel</returns>
    public ModelCell CreateAndSetCell(int _x,int _y,int value,bool doAnimate)
    {
        GameField[_y, _x] = _cellFactory.CreateCell(value,doAnimate);
        GameField[_y, _x].x = _x;
        GameField[_y, _x].y = _y;

        RegisterCell(GameField[_y, _x]);

        return GameField[_y, _x];
    }

    /// <summary>
    /// Adds a cell to a cellRegistry
    /// </summary>
    /// <param name="cell">Cell to add to registry</param>
    public void RegisterCell(ModelCell cell)
    {
        if (!AllCells.Contains(cell)&&cell!=null)
        {
            AllCells.Add(cell);
        }
    }

    /// <summary>
    /// Deletes a cell from the registry
    /// </summary>
    /// <param name="id">Id of the cell to be unregistered</param>
    /// <param name="isUndo">Need to be true when called from Undo sequence</param>
    public void UnregisterCell(int id, bool isUndo = false)
    {
        int amount = 0;
        if (isUndo)
        {
            amount = AllCells.RemoveAll(c => c.id == id&&c.isReadyToDestroy);
        }
        else
        {
            amount = AllCells.RemoveAll(c => c.id == id);
        }
        Debug.Log("From AllCells removed " + amount + " elements");
    }

    /// <summary>
    /// Checks for existence of a cell in certain coordinates
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>True if cell exists and false otherwise</returns>
    public bool DoesCellExist(int x, int y)
    {
        return(GameField[y,x]!=null);
    }

    /// <summary>
    /// Sets a whole row to a gameModel
    /// </summary>
    /// <param name="row">An array of cells to be set as a row</param>
    /// <param name="rowPosition">A row coordinate</param>
    public void SetRow(ModelCell[] row, int rowPosition)
    {        
        row.ToList().ForEach(RegisterCell);
        for(int i = 0;i<row.Length;i++)
        {
            GameField[rowPosition, i] = row[i];
            if (GameField[rowPosition, i] != null)
            {
                GameField[rowPosition, i].x = i;
                GameField[rowPosition, i].y = rowPosition;
            }
        }
        //EventSystem.ModelModifiedInvoke(null);
    }

    /// <summary>
    /// Sets a whole row to a previous gameModel
    /// </summary>
    /// <param name="row">An array of cells to be set as a row</param>
    /// <param name="rowPosition">A row coordinate</param>
    public void SetRowToPrevious(ModelCell[] row, int rowPosition)
    {
        for (int i = 0; i < row.Length; i++)
        {
            PreviousMoveField[rowPosition, i] = row[i];
            if (PreviousMoveField[rowPosition, i] != null)
            {
                PreviousMoveField[rowPosition, i].x = i;
                PreviousMoveField[rowPosition, i].y = rowPosition;
            }
        }
        //EventSystem.ModelModifiedInvoke(null);
    }

    /// <summary>
    /// Sets a whole column to a gameModel
    /// </summary>
    /// <param name="column">An array of cells to be set as a column</param>
    /// <param name="columnPosition">A column coordinate</param>
    public void SetColumn(ModelCell[] column, int columnPosition)
    {
        column.ToList().ForEach(RegisterCell);
        for (int i = 0; i < column.Length; i++)
        {
            GameField[i, columnPosition] = column[i];
            if (GameField[i, columnPosition] != null)
            {
                GameField[i, columnPosition].x = columnPosition;
                GameField[i, columnPosition].y = i;
            }
        }
        //EventSystem.ModelModifiedInvoke(null);
    }

    /// <summary>
    /// Sets a whole column to a previous gameModel
    /// </summary>
    /// <param name="column">An array of cells to be set as a column</param>
    /// <param name="columnPosition">A column coordinate</param>
    public void SetColumnToPrevious(ModelCell[] column, int columnPosition)
    {
        for (int i = 0; i < column.Length; i++)
        {
            PreviousMoveField[i, columnPosition] = column[i];
            if (PreviousMoveField[i, columnPosition] != null)
            {
                PreviousMoveField[i, columnPosition].x = columnPosition;
                PreviousMoveField[i, columnPosition].y = i;
            }
        }
        //EventSystem.ModelModifiedInvoke(null);
    }

    /// <summary>
    /// Starts Undo sequence
    /// </summary>
    internal void PrepareForUndo()
    {
        GameScore = PreviousScore;
        for (int i = 0; i < Config.FieldHeight; i++)
        {
            for (int j = 0; j < Config.FieldWidth; j++)
            {
                if (GameField[i, j] != null)
                {
                    AllCells.Find(c => Equals(c, GameField[i, j])).isReadyToDestroy = true;
                    Debug.Log("READY TO DESTROY: " + GameField[i, j].ToString());
                }

                GameField[i, j] = GetCellFromPrevious(j,i);
                if (GameField[i, j] != null)
                {
                    RegisterCell(GameField[i, j]);
                    GameField[i, j].isNew = true;
                }
            }
        }
        isUndone = true;
    }

    /// <summary>
    /// Delete all cells that were marked as ready to be destroyed
    /// </summary>
    private void ClearReadyToDestroyCells()
    {
        AllCells.RemoveAll(x => x.isReadyToDestroy);
    }

    /// <summary>
    /// Finishes the undo sequence
    /// </summary>
    internal void FinalizeUndo()
    {
        ClearReadyToDestroyCells();
        foreach (var modelCell in GameField)
        {
            RegisterCell(modelCell);
        }
    }

    /// <summary>
    /// Returns a cell from previous gameModel
    /// </summary>
    /// <param name="x">X coordinate of a requested cell</param>
    /// <param name="y">Y coordinate of a requested cell</param>
    /// <returns>A cell from the previous gameModel</returns>
    public ModelCell GetCellFromPrevious(int x, int y)
    {
        return PreviousMoveField[y, x];
    }

    /// <summary>
    /// Resets the model
    /// </summary>
    public void Restart()
    {
        GameScore = 0;
        isUndone = false;
        State = GameState.Idle;
        GameField = new ModelCell[Config.FieldHeight,Config.FieldWidth];
        PreviousMoveField = new ModelCell[Config.FieldHeight, Config.FieldWidth];
        TemporaryMoveField = new ModelCell[Config.FieldHeight, Config.FieldWidth];
        AllCells.Clear();
    }

    /// <summary>
    /// Returns a requested column
    /// </summary>
    /// <param name="columnPosition">Column coordinate</param>
    /// <returns>Requested column</returns>
    public ModelCell[] GetColumn(int columnPosition)
    {
        ModelCell[] output = new ModelCell[Config.FieldHeight];
        for (int i = 0; i < Config.FieldHeight; i++)
        {
            output[i] = GameField[i, columnPosition];
        }
        return output;
    }

    /// <summary>
    /// Returns a requested row
    /// </summary>
    /// <param name="rowPosition">Row coordinate</param>
    /// <returns>Requested row</returns>
    public ModelCell[] GetRow(int rowPosition)
    {
        ModelCell[] output = new ModelCell[Config.FieldWidth];
        for (int i = 0; i < Config.FieldWidth; i++)
        {
            output[i] = GameField[rowPosition, i];
        }
        return output;
    }
 

    /// <summary>
    /// Compare current and previous gameModels
    /// </summary>
    /// <returns>True is models are equal, false otherwise</returns>
    public bool AreLastAndCurrentMoveEqual()
    {
        for (int i = 0; i < Config.FieldHeight; i++)
        {
            for (int j = 0; j < Config.FieldWidth; j++)
            {
                if ((PreviousMoveField[i, j]??new ModelCell(0,0,false)).value != (GameField[i, j]??new ModelCell(0,0,false)).value)
                {
                    return false;
                }
            }
        }
        for (int j = 0; j < Config.FieldHeight; j++)
        {
            for (int i = 0; i < Config.FieldWidth; i++)
            {
                PreviousMoveField[j, i] = TemporaryMoveField[j, i] != null ? TemporaryMoveField[j, i].Clone() : null;
            }
        }
        return true;
    }



    /// <summary>
    /// Save the last move of a player before making a new one
    /// </summary>
    public void SavePreviousState()
    {
        PreviousScore = GameScore;
        for (int j = 0; j < Config.FieldHeight; j++)
        {
            for (int i = 0; i < Config.FieldWidth; i++)
            {
                TemporaryMoveField[j, i] = PreviousMoveField[j, i] != null ? PreviousMoveField[j, i].Clone() : null;
            }
        }
        for (int j = 0; j < Config.FieldHeight; j++)
        {
            for (int i = 0; i < Config.FieldWidth; i++)
            {
                PreviousMoveField[j, i] = GameField[j, i] != null ? GameField[j, i].Clone() : null;
            }
        }

        //DebugPanel.Instance.PrintGridBefore(PreviousMoveField);
    }

    /// <summary>
    /// Check if there are empty cells in a gameModel
    /// </summary>
    /// <returns>True if there are no empty cell, false otherwise</returns>
    public bool IsGameModelFilledUp()
    {
        int _filledCells = 0;
        foreach (var cell in GameField)
        {
            if (cell != null)
            {
                _filledCells++;
            }
        }

        return (_filledCells == Config.FieldHeight * Config.FieldWidth);
    }

    /// <summary>
    /// Get a cell from previous gameModel by id
    /// </summary>
    /// <param name="id">Id of a requested cell</param>
    /// <returns>Requested cell from previous gameModel</returns>
    public ModelCell GetPreviousCellById(int id)
    {

        foreach (ModelCell cell in PreviousMoveField)
        {
            if (cell!=null && cell.id == id)
            {
                return cell;
            }
        }

        return null;
        //return PreviousMoveField.Cast<Cell>().FirstOrDefault(c => c.id == id);
    }

    /// <summary>
    /// Return a class instance, containing all the info about current game state
    /// </summary>
    /// <param name="isAlreadyWon">If player has won</param>
    /// <returns>A filled instance with all saved game data</returns>
    public InfoContainer SaveInfo(bool isAlreadyWon)
    {
        return new InfoContainer(AllCells,GameScore,GameHighScore,PreviousScore,isUndone,
            GameField,PreviousMoveField,TemporaryMoveField,_cellFactory.currentID,
            (State == GameState.GameOver), isAlreadyWon);
    }


    /// <summary>
    /// Resets all the offsets and multiplication marks after compression of the field is finished
    /// </summary>
    public void ResetMultiplies()
    {
        foreach (var cell in AllCells)
        {
            cell.isMultiply = false;
            cell.offsetX = 0;
            cell.offsetY = 0;
        }
    }

    /// <summary>
    /// If player has won
    /// </summary>
    /// <returns>True if player has won, false is he hasn't yet</returns>
    public bool IsWon()
    {
        return GameLogic<ModelCell,CellFactory>.IsWon(AllCells);
    }

    /// <summary>
    /// If player has lost
    /// </summary>
    /// <returns>True if player has lost, false is he hasn't yet</returns>
    public bool IsLose()
    {
        return GameLogic<ModelCell,CellFactory>.IsLose(GameField,Config.FieldHeight,Config.FieldWidth);
    }
}

[Serializable]
public class InfoContainer
{
    public bool IsGameover;
    public bool IsAlreadyWon;
    public ModelCell[] AllCells ;
    public int GameScore ;
    public int GameHighScore ;
    public int PreviousScore ;
    public bool IsUndone ;
    public int CurrentId ;
    public ModelCell[,] GameField ;
    public ModelCell[,] PreviousMoveField ;
    public ModelCell[,] TemporaryMoveField ;

    public InfoContainer(List<ModelCell> _allCells,int _GameScore, int _GameHighScore, int _PreviousScore, bool _isUndone, ModelCell[,] _GameField, ModelCell[,] _PreviousMoveField, ModelCell[,] _TemporaryMoveField, int _currentId, bool _isGameover, bool _isAlreadyWon)
    {
        AllCells = (_allCells??new List<ModelCell>()).ToArray();
        GameScore = _GameScore;
        GameHighScore = _GameHighScore;
        PreviousScore = _PreviousScore;
        IsUndone = _isUndone;
        GameField = _GameField;
        PreviousMoveField = _PreviousMoveField;
        TemporaryMoveField = _TemporaryMoveField;
        CurrentId = _currentId;
        IsGameover = _isGameover;
        IsAlreadyWon = _isAlreadyWon;
    }
}
