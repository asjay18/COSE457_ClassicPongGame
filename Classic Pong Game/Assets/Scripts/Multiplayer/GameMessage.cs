using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameMessage
{
    public string action; 
    public string opcode;
    public string message;
    public GameRoom gameRoom;
    public PlayerData playerData;
    public BallData ballData;

    public GameMessage(string actionIn, string opcodeIn)
    {
        action = actionIn;
        opcode = opcodeIn;
    }

    public GameMessage(string actionIn, string opcodeIn, PlayerData playerDataIn)
    {
        action = actionIn;
        opcode = opcodeIn;
        playerData = playerDataIn;
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}
