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

    public GameObject igmObject;
    private InGameManager inGameManager;

    public void Start()
    {
        plScoreText.text = "0";
        prScoreText.text = "0";
        plRoundText.text = "0";
        prRoundText.text = "0";

        inGameManager = igmObject.GetComponent<InGameManager>();
    }
    public void plWin()
    {
        plScore += 1;
        if (plScore == 11)
        {
            plScore = 0;
            prScore = 0;
            prScoreText.text = prScore.ToString();
            plRound += 1;
            plRoundText.text = plRound.ToString();
        }
        plScoreText.text = plScore.ToString();
        inGameManager.StartSet();
        if (plRound == 2)
        {
            inGameManager.StopSet();
        }
    }
    public void prWin()
    {
        prScore += 1;
        if (prScore == 11)
        {
            prScore = 0;
            plScore = 0;
            plScoreText.text = plScore.ToString();
            prRound += 1;
            prRoundText.text = prRound.ToString();
        }
        prScoreText.text = prScore.ToString();
        inGameManager.StartSet();
        if (prRound == 2)
        {
            inGameManager.StopSet();
        }
    }
}
