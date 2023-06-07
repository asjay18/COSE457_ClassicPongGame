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
    public GameObject youIndicatorText;
    public GameObject gameStartImage;

    public GameObject oponentLeftPanel;
    public GameObject youWonImage;
    public GameObject youLoseImage;
    public GameObject youWonSetImage;
    public GameObject youLoseSetImage;
    public GameObject youWonPanel;
    public GameObject youLosePanel;

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
            player1status.text = "waiting..";
            player2status.text = "u ready?";
            playerBlinker.GetComponent<RectTransform>().anchoredPosition = new Vector2(195,75);
            youIndicatorText.GetComponent<RectTransform>().anchoredPosition = new Vector2(170, -210);
        } else
        {
            player1status.text = "u ready?";
            player2status.text = "waiting..";
            playerBlinker.GetComponent<RectTransform>().anchoredPosition = new Vector2(-195, 75);
            youIndicatorText.GetComponent<RectTransform>().anchoredPosition = new Vector2(-170, -210);
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
        actionBuffer.Enqueue(delegate {
            gameStartImage.SetActive(true);
            StartSet();
        });
        actionBuffer.Enqueue(delegate {
            readyPanel.SetActive(false);
        });        
    }
    public void OponentLeft()
    {
        actionBuffer.Enqueue(delegate {
            StopSet();
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
    IEnumerator WaitFor2Sec()
    {
        yield return new WaitForSecondsRealtime(2.0f);
        serverNode.playerGameRoom.status = "playing";
        leftPaddle.transform.position = new Vector3(-44, 0, 0);
        leftPaddle.SetActive(true);
        rightPaddle.transform.position = new Vector3(44, 0, 0);
        rightPaddle.SetActive(true);
        ball.SetActive(true);
    }

    public void StartSet()
    {
        try
        {
            serverNode.playerGameRoom.status = "set_done";
            ball.SetActive(false);
            leftPaddle.SetActive(false);
            rightPaddle.SetActive(false);

            StartCoroutine(WaitFor2Sec());
        } catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void StopSet()
    {
        try
        {
            serverNode.playerGameRoom.status = "stopped";
            ball.SetActive(false);
            leftPaddle.SetActive(false);
            rightPaddle.SetActive(false);
        }
        catch (Exception e)
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
            scoreSide = 2;
            actionBuffer.Enqueue(delegate {
                scoreManager.prWin();
            });
        }
        else
        {
            scoreSide = 1;
            actionBuffer.Enqueue(delegate {
                scoreManager.plWin();
            });
        }
        actionBuffer.Enqueue(delegate {
            if (!youLoseSetImage.activeInHierarchy)
            {
                youLoseImage.SetActive(true);
            }
        });
    }

    public void HandlePlayerScore()
    {
        scoreSide = player.sideNumber;
        actionBuffer.Enqueue(delegate
        {
            youWonImage.SetActive(true);
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
        scoreSide = player.sideNumber;
        actionBuffer.Enqueue(delegate {
            StartSet();
            youWonImage.SetActive(false);
            youWonSetImage.SetActive(true);
        });
    }

    public void SetLost()
    {
        scoreSide = 3 - player.sideNumber;
        actionBuffer.Enqueue(delegate {
            StartSet(); 
            youLoseSetImage.SetActive(true);
        });
    }

    public void GameWin()
    {
        actionBuffer.Enqueue(delegate {
            StopSet();
            youWonPanel.SetActive(true);
        });
    }

    public void GameLost()
    {
        actionBuffer.Enqueue(delegate {
            StopSet();
            youLosePanel.SetActive(true);
        });
    }
}
