using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameMessage
{
    public string uuid; // player id
    public string opcode; // operation code
    public string message; // sending message
    public string gameRoom; // gameRoom{player1: any; player2: any; sets: number; goal: number; gameScore: number; setScore }
    public string action; 

    public GameMessage(string actionIn, string opcodeIn)
    {
        action = actionIn;
        opcode = opcodeIn;
    }

    public GameMessage(string actionIn, string opcodeIn, string messageIn)
    {
        action = actionIn;
        opcode = opcodeIn;
        message = messageIn;
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}
