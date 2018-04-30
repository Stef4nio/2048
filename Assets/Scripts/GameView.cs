using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{

    [SerializeField] private Transform gamePanelTransform;

    GameObject[,] Field = new GameObject[4,4];

	// Use this for initialization
	void Awake () {
	    EventSystem.ModelModified += RefreshField;
        /*for (int i = 0; i < 4;i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Field[i, j] =(GameObject) Instantiate(Resources.Load("Cell"), new Vector3(j * 253 +40, (3-i) * 253 +701),
                    new Quaternion(0, 0, 0, 0), gamePanelTransform);
            }
        }*/
	}

    private void RefreshField(object sender, EventArgs e)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Cell currentCell = GameModel.GetCell(j, i);
                if (Field[i, j] == null&& currentCell != null)
                {
                    Field[i, j] = (GameObject)Instantiate(Resources.Load("Cell"), new Vector3(j * 253 + 40, i * 253 + 701),
                        new Quaternion(0, 0, 0, 0), gamePanelTransform);
                    Field[i, j].GetComponent<Text>().text = GameModel.GetCell(j, i).value.ToString();
                }
                else if (Field[i, j] != null && currentCell == null)
                {
                    Destroy(Field[i, j]);
                    Field[i, j] = null;
                }
                else if(Field[i, j] != null && currentCell != null)
                {
                    Field[i, j].GetComponent<Text>().text = currentCell.value.ToString();
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
