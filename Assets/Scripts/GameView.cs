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
        for (int i = 0; i < 4;i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Field[i, j] =(GameObject) Instantiate(Resources.Load("Cell"), new Vector3(j * 253 +160, (3-i) * 253 +581),
                    new Quaternion(0, 0, 0, 0), gamePanelTransform);
            }
        }
	}

    private void RefreshField(object sender, EventArgs e)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Field[i ,j].GetComponent<Text>().text = GameModel.GameField[i, j] != null ? GameModel.GameField[i, j].value.ToString():"";
                //immitate that animations are done
                if (GameModel.GameField[i, j] != null)
                {
                    GameModel.GameField[i, j].isMultiply = false;
                    GameModel.GameField[i, j].offset = 0;
                }
            }
        }
        
    }

    // Update is called once per frame
    void Update () {
		
	}
}
