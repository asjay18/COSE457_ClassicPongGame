using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightSide : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isPlayerOnRight;

    public void ChangePlayerSide(bool isOnRight)
    {
        isPlayerOnRight = isOnRight;
    }
}
