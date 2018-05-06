using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{

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
                Cell currentCell = GameModel.GetCell(j, i);
                if (Field[i, j] == null && currentCell != null)
                {
                    var cellView = Instantiate(_cellPrefab, _gamePanelTransform).GetComponent<CellView>();
                    cellView.Cell = currentCell;
                    cellView.ChangeText();
                    cellView.SetPosition(new Vector3(cellView.Cell.x * (cellView.Width+Config.CellViewSpacing), cellView.Cell.y * (cellView.Height + Config.CellViewSpacing)));
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
        }
        
    }

    // Update is called once per frame
    void Update () {
		
	}
}
