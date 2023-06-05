using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreUp : MonoBehaviour
{
    public bool rightwall;
    public GameObject scoreManagerObject;

    private Node serverNode;
    private ScoreManager scoreManager;


    void Start()
    {
        scoreManager = scoreManagerObject.GetComponent<ScoreManager>();
        GameObject nodeObject = GameObject.Find("Node");
        serverNode = nodeObject.GetComponent<Node>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (serverNode.playerData.sideNumber == 1)
        {
            if (rightwall)
            {
                scoreManager.plWin();
                serverNode.PlayerScores();
            }
        }
        else
        {
            if (!rightwall)
            {
                scoreManager.prWin();
                serverNode.PlayerScores();
            }
        }
    }
}
