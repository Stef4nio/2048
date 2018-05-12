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
    [SerializeField] private Transform _gamePanelTransform;
    [SerializeField] private GameObject _cellPrefab;

    private CellView[,] Field = new CellView[Config.FieldHeight, Config.FieldWidth];

	// Use this for initialization
	void Awake () {
	    EventSystem.ModelModified += RefreshField;
	}

    private void RefreshField(object sender, EventArgs e)
    {
        for (int i = 0; i < Config.FieldHeight; i++)
        {
            for (int j = 0; j < Config.FieldWidth; j++)
            {
                _testTexts[i * 4 + j].text = (GameModel.GameField[i, j]??new Cell(0)).value.ToString();
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

            CellView currCellView = GetCellViewByData(cell);
            if (cell.isNew)
            {
                var cellView = Instantiate(_cellPrefab, _gamePanelTransform).GetComponent<CellView>();
                cell.isNew = false;
                cellView.CellData = cell;
                cellView.ChangeText();
                cellView.SetPosition(new Vector3(cellView.CellData.x * (cellView.Width + Config.CellViewSpacing), cellView.CellData.y * (cellView.Height + Config.CellViewSpacing)));
                Field[cellView.CellData.y, cellView.CellData.x] = cellView;
            }

            if (cell.offset > 0)
            {
                if (Field[cell.y, cell.x] != null && Field[cell.y, cell.x].CellData == null)
                {
                    Field[cell.y, cell.x].Kill();
                    Field[cell.y, cell.x] = null;
                }
                
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
    void Update () {
		
	}
}
