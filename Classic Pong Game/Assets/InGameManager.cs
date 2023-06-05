using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class InGameManager : MonoBehaviour
{
    public GameObject rightSideGameObject;
    private RightSide rightSideManager;

    public int scoreSide;

    public GameObject ball;
    public GameObject rightPaddle;
    public GameObject leftPaddle;

    public GameObject readyPanel;
    public GameObject playerBlinker;
    public TMP_Text player1status;
    public TMP_Text player2status;

    private GameObject nodeObject;
    private Node serverNode;

    private PlayerData player;

    public GameObject oponentLeftPanel;
    public GameObject youWonImage;
    public GameObject youLoseImage;

    public GameObject scoreManagerObj;
    private ScoreManager scoreManager;

    void Start()
    {
        nodeObject = GameObject.Find("Node");
        serverNode = nodeObject.GetComponent<Node>();
        serverNode.ChangeFindIgmObjectFlag();
        player = serverNode.playerData;
        Debug.Log("player.id is " + player.id);

        scoreManager = scoreManagerObj.GetComponent<ScoreManager>();

        scoreSide = 1;
        ball.SetActive(false);
        leftPaddle.SetActive(false);
        rightPaddle.SetActive(false);

        rightSideManager = rightSideGameObject.GetComponent<RightSide>();
        bool isRight = (player.sideNumber == 2);
        Debug.Log(player.sideNumber);
        rightSideManager.ChangePlayerSide(isRight);

        if (isRight)
        {
            player1status.text = "waiting";
            player2status.text = "ready?";
            playerBlinker.GetComponent<RectTransform>().anchoredPosition = new Vector2(200,75);
        } else
        {
            player1status.text = "ready?";
            player2status.text = "waiting";
            playerBlinker.GetComponent<RectTransform>().anchoredPosition = new Vector2(-200, 75);
        }
    }

    private Queue<UnityAction> actionBuffer = new Queue<UnityAction>();
    private void Update()
    {
        if (actionBuffer.Count > 0)
        {
            actionBuffer.Dequeue().Invoke();
        }

    }

    public void Player1Ready()
    {
        actionBuffer.Enqueue(delegate { 
            player1status.text = "READY"; 
        }); 
    }
    public void Player2Ready()
    {
        actionBuffer.Enqueue(delegate {
            player2status.text = "READY";
        });
    }
    public void GameStart()
    {
        // Game Starts in 3..2..1.. (효과 넣고 싶다!!)
        actionBuffer.Enqueue(delegate {
            StartSet();
        });
        actionBuffer.Enqueue(delegate {
            readyPanel.SetActive(false);
        });        
    }
    public void OponentLeft()
    {
        actionBuffer.Enqueue(delegate {
            StartSet();
            //Todo stopset?
        });
        actionBuffer.Enqueue(delegate {
            oponentLeftPanel.SetActive(true);
        });        
    }

    public GameObject GetPlayerRacket()
    {
        if (player.sideNumber == 1) return leftPaddle;
        else return rightPaddle;
    }

    public void StartSet()
    {
        try
        {
            ball.SetActive(false);
            ball.SetActive(true);
            leftPaddle.transform.position = new Vector3(-22, 0, 0);
            leftPaddle.SetActive(true);
            rightPaddle.transform.position = new Vector3(22, 0, 0);
            rightPaddle.SetActive(true);
        } catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void HandleOponentPosition(PlayerData oponentData)
    {
        if (player.sideNumber == 1)
        {
            actionBuffer.Enqueue(delegate {
                rightPaddle.transform.position = new Vector3(oponentData.xPos, oponentData.yPos, 0);
            });            
        }
        else
        {
            actionBuffer.Enqueue(delegate {
                leftPaddle.transform.position = new Vector3(oponentData.xPos, oponentData.yPos, 0);
            });
        }   
    }

    public void HandleOponentScore()
    {
        if (player.sideNumber == 1)
        {
            actionBuffer.Enqueue(delegate {
                scoreManager.prWin();
            });
            scoreSide = 2;
        }
        else
        {
            actionBuffer.Enqueue(delegate {
                scoreManager.plWin();
            });
            scoreSide = 1;
        }
        actionBuffer.Enqueue(delegate {
            StartSet();
        });
    }

    public void HandleOponentHit(BallData ballData)
    {
        actionBuffer.Enqueue(delegate {
            ball.transform.position = new Vector2(ballData.xPos, ballData.yPos);
            ball.GetComponent<Rigidbody2D>().velocity = new Vector2(ballData.xVel, ballData.yVel);
        });        
    }

    public void SetWin()
    {
        // 1초간 you won set!
        actionBuffer.Enqueue(delegate
        {
            youWonImage.SetActive(true);
        });
        scoreSide = player.sideNumber;
        actionBuffer.Enqueue(delegate {
            StartSet();
        });
    }

    public void SetLost()
    {
        actionBuffer.Enqueue(delegate
        {
            youLoseImage.SetActive(true);
        });
        scoreSide = 3 - player.sideNumber;
        actionBuffer.Enqueue(delegate {
            StartSet();
        });
    }
}
