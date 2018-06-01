using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {

    public static int score;
    public Text scoreLabel;

    private void Awake()
    {
        score = 0;
    }

    private void Update()
    {
        scoreLabel.text = score.ToString();
    }
}
