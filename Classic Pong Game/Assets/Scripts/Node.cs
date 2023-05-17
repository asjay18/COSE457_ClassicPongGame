using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using WebSocketSharp;

public class Node : MonoBehaviour
{
    public GameObject nodeObject;

    private WebSocket _webSocket;
    private string _webSocketDns = "ws://43.201.68.58:3333";

    private bool intentionalClose = false;

    public const string RequestFindMatchOp = "5";
    public const string ResponseFoundMatchOp = "6";
    public const string PlayingOp = "11";

    public const string YouWonOp = "91";
    public const string YouLostOp = "92";
    
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
            Debug.Log("OnMessage!");
            Debug.Log(messageArgs);
            ProcessReceivedMessage(messageArgs.Data);
        };
        _webSocket.OnError += (sender, error) =>
        {
            Debug.Log("Error! " + error);
        };
    }
    public void FindMatch()
    {
        // 서버에 연결
        _webSocket = new WebSocket(_webSocketDns);
        SetupWsCallbacks();
        _webSocket.Connect();

        GameMessage findMatchRequest = new GameMessage("OnMessage", RequestFindMatchOp);
        SendWebSocketMessage(JsonUtility.ToJson(findMatchRequest));
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
        Debug.Log(message);

        GameMessage gameMessage = JsonUtility.FromJson<GameMessage>(message);

        if (gameMessage.opcode == ResponseFoundMatchOp)
        {
            Debug.Log("you can play!");
            try
            {
                var playerId = gameMessage.uuid;
                GameRoom gameRoom = JsonUtility.FromJson<GameRoom>(gameMessage.gameRoom);
                if (gameRoom.player1 == playerId) PlayerPrefs.SetString("PlayerSide", "left");
                else PlayerPrefs.SetString("PlayerSide", "right");

                WaitForPlayer.SetFoundMatch(true);
            } catch (Exception e) {
                Debug.Log(e);
                Debug.Log("Game can't start...");
            }
        }
        else if (gameMessage.opcode == YouWonOp)
        {
            Debug.Log("you won~");
            QuitGame();
            // show back to menu, quit option page
            // show confetti
        }
        else if (gameMessage.opcode == YouLostOp)
        {
            Debug.Log("you lost~");
            QuitGame();
            // show back to menu, quit option page
        }
    }
    public void QuitGame()
    {
        intentionalClose = true;
        _webSocket.Close();
    }

}
