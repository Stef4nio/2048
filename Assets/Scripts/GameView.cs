using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{
    [SerializeField] private Text[] _testTexts = new Text[16];
    [SerializeField] private Text[] _IDsTexts = new Text[16];
    [SerializeField] private Transform _gamePanelTransform;
    [SerializeField] private GameObject _cellPrefab;

    private CellView[,] Field = new CellView[Config.FieldHeight, Config.FieldWidth];

	// Use this for initialization
    private void Awake () {
	    EventSystem.ModelModified += RefreshField;
    }


    private void RefreshField(object sender, EventArgs e)
    {
        CheckDataAvailability();
        for (var i = 0; i < Config.FieldHeight; i++)
        {
            for (var j = 0; j < Config.FieldWidth; j++)
            {
                _testTexts[i * 4 + j].text = (GameModel.GameField[i, j] ?? new Cell(0, 0)).value.ToString();
                _IDsTexts[i * 4 + j].text =
                    GameModel.GameField[i, j] != null ?GameModel.GameField[i, j].id.ToString() : "N/A";
            }
        }
        /* for (int i = 0; i < Config.FieldHeight; i++)
         {
             for (int j = 0; j < Config.FieldWidth; j++)
             {
                 Cell currentCell = GameModel.GetCell(j, i);
                 if (Field[i, j] == null && currentCell != null)
                 {
                     var cellView = Instantiate(_cellPrefab, _gamePanelTransform).GetComponent<CellView>();
                     cellView.CellData = currentCell;
                     cellView.ChangeText();
                     cellView.SetPosition(new Vector3(cellView.CellData.x * (cellView.Width+Config.CellViewSpacing), cellView.CellData.y * (cellView.Height + Config.CellViewSpacing)));
                     Field[i, j] = cellView;
                 }
                 else if (Field[i, j] != null && currentCell == null)
                 {
                     Field[i, j].Kill();
                     Field[i, j] = null;
                 }
                 else if(Field[i, j] != null && currentCell != null)
                 {
                     Field[i, j].ChangeText();
                 }


                 //immitate that animations are done
                 if (currentCell != null)
                 {
                     currentCell.isMultiply = false;
                     currentCell.offset = 0;
                 }
             }
         }*/

        foreach (var cell in GameModel.GameField)
        {
            if (cell == null)
            {
                continue;
            }
                if (cell.isNew)
                {
                    var cellView = Instantiate(_cellPrefab, _gamePanelTransform).GetComponent<CellView>();
                    cell.isNew = false;
                    cellView.CellData = cell;
                    cellView.ChangeText();
                    cellView.SetPosition(new Vector3(cellView.CellData.x * (cellView.Width + Config.CellViewSpacing),
                        -cellView.CellData.y* (cellView.Height + Config.CellViewSpacing)));
                    Field[cellView.CellData.y,cellView.CellData.x] = cellView;
                }

                if (cell.offset > 0)
                {
                    /*if (Field[cell.y, cell.x] != null && Field[cell.y, cell.x].CellData == null)
                    {
                        Field[cell.y, cell.x].Kill();
                        Field[cell.y, cell.x] = null;
                    }*/
                    var currCellView = GetCellViewByData(cell);
                    Field[cell.y, cell.x] = currCellView;
                    Field[cell.y, cell.x].Move();
                }
           
        }


    }

    private CellView GetCellViewByData(Cell data)
    {
        foreach (var View in Field)
        {
            if (View == null)
            {
                continue;
            }

            if (View.CellData == data)
            {
                return View;
            }
        }

        return null;
    }

    // Update is called once per frame
    private void Update () {
		
	}

    private void CheckDataAvailability()
    {
        List<Cell> killedCells = GameModel.GetKilledCells();
        if (killedCells == null)
        {
            return;
        }
        foreach (var cell in killedCells)
        {
            //Clear this cellView from Field[] here!!!
            GetCellViewByData(cell).Kill();
        }
    }
}
