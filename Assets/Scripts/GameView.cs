using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameView : MonoBehaviour {

    [SerializeField]
    Text[] Field = new Text[16];

	// Use this for initialization
	void Awake () {
		for(int i = 0; i<Field.Length;i++)
        {
            Field[i].text = "";
        }
        EventSystem.ModelModified += RefreshField;
	}

    private void RefreshField(object sender, EventArgs e)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Field[i * 4 + j].text = GameModel.GameField[i, j] != null ? GameModel.GameField[i, j].value.ToString():"";
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
