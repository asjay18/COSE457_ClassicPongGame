using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public int plScore = 0;
    public int prScore = 0;
    public int plRound = 0;
    public int prRound = 0;

    public TextMeshProUGUI plScoreText;
    public TextMeshProUGUI prScoreText;
    public TextMeshProUGUI plRoundText;
    public TextMeshProUGUI prRoundText;

    public void Start()
    {
        plScoreText.text = "0";
        prScoreText.text = "0";
        plRoundText.text = "0";
        prRoundText.text = "0";
    }
    public void plWin()
    {
        plScore += 1;
        if (plScore == 12)
        {
            plScore = 0;
            plRound += 1;
            plRoundText.text = plRound.ToString();

        }
        plScoreText.text = plScore.ToString();
        if (plRound == 3)
        {
            endGame(1);
        }
    }
    public void prWin()
    {
        prScore += 1;
        if (prScore == 12)
        {
            prScore = 0;
            prRound += 1;
            prRoundText.text = prRound.ToString();
        }
        prScoreText.text = prScore.ToString();
        if (prRound == 3)
        {
            endGame(2);
        }
    }

    public void endGame(int winner)
    {
        // end game?
    }
}
