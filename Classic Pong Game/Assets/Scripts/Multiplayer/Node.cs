using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using WebSocketSharp;

public class Node : MonoBehaviour
{
    public GameObject nodeObject;

    private WebSocket _webSocket;
    private string _webSocketDns = "ws://15.164.171.153:3333";

    public GameObject playerRacket;
    public GameRoom playerGameRoom;
    public PlayerData playerData;

    private bool findIgmObjectFlag = false;
    private GameObject igmObject;
    private InGameManager inGameManager;

    private bool intentionalClose = false;

    public const string ResponseFoundMatchOp = "6";
    public const string PlayerReadyOp = "7";
    public const string PlayerReadyResOp = "8";

    public const string StartGameOp = "10";
    public const string PlayingOp = "11";

    public const string PlayerMovementOp = "30";
    public const string OponentMovementOp = "31";

    public const string PlayerScoresOp = "40";
    public const string OponentScoresOp = "41";

    public const string PlayerHitsOp = "50";
    public const string OponentHitsOp = "51";

    public const string OponentLeftOp = "90";

    public const string SetEndOp = "98";
    public const string GameEndOp = "99";

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    private void SetupWsCallbacks()
    {
        _webSocket.OnOpen += (sender, openArgs) =>
        {
            Debug.Log("Connection open!");
            intentionalClose = false;
        };
        _webSocket.OnClose += (sender, closeArgs) =>
        {
            Debug.Log("Connection closed!");
        };
        _webSocket.OnMessage += (sender, messageArgs) =>
        {
            ProcessReceivedMessage(messageArgs.Data);
        };
        _webSocket.OnError += (sender, error) =>
        {
            Debug.Log("Error! " + error.Message);
            Debug.Log(error);
        };
    }
    public void FindMatch()
    {
        // 서버에 연결
        _webSocket = new WebSocket("ws://localhost:3333");
        SetupWsCallbacks();
        _webSocket.Connect();

        _webSocket.OnMessage += (sender, messageArgs) =>
        {
            if (messageArgs.IsText)
            {
                if (playerData.id == "")
                {
                    PlayerData tempPlayerData = JsonUtility.FromJson<PlayerData>(messageArgs.Data);
                    Debug.Log(tempPlayerData.id);
                    playerData.id = tempPlayerData.id;
                }
                return;
            }
        };
    }

    public void ChangeFindIgmObjectFlag()
    {
        findIgmObjectFlag = true;
    }
    public void Update()
    {
        if (_webSocket == null) return;

        if (findIgmObjectFlag)
        {
            findIgmObjectFlag = false;
            igmObject = GameObject.Find("InGameManager");
            inGameManager = igmObject.GetComponent<InGameManager>();
        }

        if (playerRacket == null) return;

        if (playerGameRoom != null && playerGameRoom.status == "playing" && playerData.id != "")
        {
            playerData.xPos = playerRacket.transform.position.x;
            playerData.yPos = playerRacket.transform.position.y;

            GameMessage playerDataMessage = new GameMessage("OnMessage", PlayerMovementOp, playerData);
            SendWebSocketMessage(JsonUtility.ToJson(playerDataMessage));
        }
    }

    public void SendWebSocketMessage(string message)
    {
        if (_webSocket.ReadyState == WebSocketState.Open)
        {
            // Sending plain text
            _webSocket.Send(message);
        }
    }
    private void ProcessReceivedMessage(string message)
    {
        GameMessage gameMessage = JsonUtility.FromJson<GameMessage>(message);

        if (gameMessage.opcode == ResponseFoundMatchOp)
        {
            handleMatchFound(gameMessage);
        }
        else if (gameMessage.opcode == OponentMovementOp)
        {
            handleOponentPosition(gameMessage.playerData);
        }
        else if (gameMessage.opcode == PlayerReadyResOp)
        {
            handleMatchReady(gameMessage);
        }
        else if (gameMessage.opcode == OponentScoresOp)
        {
            handleOponentScore();
        }
        else if (gameMessage.opcode == OponentHitsOp)
        {
            handleOponentHit(gameMessage.ballData);
        }
        else if (gameMessage.opcode == StartGameOp)
        {
            handleStartGame();
        }
        else if (gameMessage.opcode == OponentLeftOp)
        {
            handleEndGame();
        }
        else if (gameMessage.opcode == GameEndOp)
        {
            Debug.Log("you won~");
            QuitGame();
            // show back to menu, quit option page
            // show confetti
        }
        else if (gameMessage.opcode == SetEndOp)
        {
            if (gameMessage.message == "PLAYER 1")
            {
                if (playerData.sideNumber == 1)
                {
                    inGameManager.SetWin();
                }
                else
                {
                    inGameManager.SetLost();
                }
            }
            else
            {
                if (playerData.sideNumber == 1)
                {
                    inGameManager.SetLost();
                }
                else
                {
                    inGameManager.SetWin();
                }
            }
        }
    }
    public void QuitGame()
    {
        intentionalClose = true;
        _webSocket.Close();
    }

    public void PlayerIsReady()
    {
        GameMessage playerReadyMes = new GameMessage("OnMessage", PlayerReadyOp);
        SendWebSocketMessage(JsonUtility.ToJson(playerReadyMes));
    }

    public void PlayerScores()
    {
        GameMessage playerScoresMes = new GameMessage("OnMessage", PlayerScoresOp);
        SendWebSocketMessage(JsonUtility.ToJson(playerScoresMes));
    }

    public void PlayerHits(BallData ballData)
    {
        GameMessage playerHitsMes = new GameMessage("OnMessage", PlayerHitsOp);
        playerHitsMes.ballData = ballData;
        SendWebSocketMessage(JsonUtility.ToJson(playerHitsMes));
    }

    private void handleMatchFound(GameMessage gameMessage)
    {
        try
        {
            GameRoom gameRoom = gameMessage.gameRoom;
            if (gameRoom.player1 == playerData.id) playerData.sideNumber = 1;
            else playerData.sideNumber = 2;

            WaitForPlayer.SetFoundMatch(true);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("Game can't start...");
        }
    }

    private void handleMatchReady(GameMessage gameMessage)
    {
        try
        {
            GameRoom gameRoom = gameMessage.gameRoom;
            if (gameRoom.p1status) inGameManager.Player1Ready();
            if (gameRoom.p2status) inGameManager.Player2Ready();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private void handleOponentScore()
    {
        inGameManager.HandleOponentScore();
    }

    private void handleOponentHit(BallData ballData)
    {
        inGameManager.HandleOponentHit(ballData);
    }

    private void handleStartGame()
    {
        try 
        {
            playerGameRoom.status = "playing";
            inGameManager.GameStart();
            playerRacket = inGameManager.GetPlayerRacket();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }       
        
    }

    private void handleEndGame()
    {
        playerGameRoom.status = "gameover";
        inGameManager.OponentLeft();
    }

    private void handleOponentPosition(PlayerData oponentData)
    {
        inGameManager.HandleOponentPosition(oponentData);
    }

}
