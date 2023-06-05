using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class GameRoom
{
    public string id;
    public string status;
    public bool p1status;
    public bool p2status;
    public string player1; // lhs player
    public string player2; // rhs player
    public int sets; // number of sets
    public int goal; // goal for a set 

}
