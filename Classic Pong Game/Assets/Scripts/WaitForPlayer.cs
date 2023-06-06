using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class WaitForPlayer : MonoBehaviour
{
    public static string loadGameScene;
    public static int loadGameType;
    public static string roomCode;

    public TMP_Text waitingText;
    public TMP_Text roomNumberText;
    private int i = 0;
    private readonly string[] dots = { "", ".", "..", "..." };

    private static bool foundMatch;
    public static string roomNumber = "";

    private Node nodeObject;

    private void Start()
    {
        foundMatch = false;
        waitingText.text = "WAITING FOR OTHER PLAYER";
        if (loadGameType == 1)
        {
            roomNumberText.text = "ROOM CODE IS ..";
        } 
        else
        {
            roomNumberText.text = "";
        }
        nodeObject = GameObject.Find("Node").GetComponent<Node>();
        nodeObject.FindMatch(loadGameType, roomCode);

        StartCoroutine(CheckForPlayer());
    }

    private void Update()
    {
        if (roomNumber == "") return;
        roomNumberText.text = "ROOM CODE IS '" + roomNumber + "'";
        roomNumber = "";
    }

    public static void LoadGameSceneHandler(string _name, int _loadType, string _roomCode)
    {
        loadGameScene = _name;
        loadGameType = _loadType;
        roomCode = _roomCode;
    }

    IEnumerator CheckForPlayer()
    {
        yield return null;
        while (!foundMatch)
        {
            yield return null;
            i = (i + 1) % 400;
            waitingText.text = "WAITING FOR OTHER PLAYER" + dots[(i/100)];
        }

        Debug.Log(foundMatch);
        // 서버로부터 상대방이 입장했다는 연락을 받으면 scene 로딩 시작!
        AsyncOperation operation = SceneManager.LoadSceneAsync(loadGameScene);
        operation.allowSceneActivation = true;
        while (!operation.isDone)
        {
            yield return null;
            i = (i + 1) % 400;
            waitingText.text = "FOUND MATCH! LOADING GAME" + dots[(i / 100)];
        }
        if (operation.isDone)
        {
            operation.allowSceneActivation = true;
        }
        
    }
    
    public static void SetFoundMatch(bool found)
    {
        if (found) foundMatch = true;
        else foundMatch = false;
    }

    public static void SetRoomNumber(string roomNumberIn)
    {
        roomNumber = roomNumberIn;
    }

}
