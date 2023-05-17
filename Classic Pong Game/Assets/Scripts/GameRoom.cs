using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class GameRoom
{
    public string player1; // lhs player
    public string player2; // rhs player
    public int sets; // number of sets
    public int goal; // goal for a set
    public Tuple<int, int> gameScore; // score of sets
    public Tuple<int, int> setScore; // score for a set

    

}
