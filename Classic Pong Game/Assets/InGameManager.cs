using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    public GameObject rightSideGameObject;
    private RightSide rightSideManager;

    void Start()
    {
        rightSideManager = rightSideGameObject.GetComponent<RightSide>();
        string playerSide = PlayerPrefs.GetString("PlayerSide");
        if (playerSide == "left") rightSideManager.ChangePlayerSide(false); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
