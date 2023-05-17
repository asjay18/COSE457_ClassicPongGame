using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class WaitForPlayer : MonoBehaviour
{
    public static string loadGameScene;
    public static int loadGameType;

    public TMP_Text waitingText;
    private int i = 0;
    private readonly string[] dots = { "", ".", "..", "..." };

    private bool foundMatch;

    private GameObject nodeObject;

    private void Start()
    {
        foundMatch = false;
        waitingText.text = "Waiting for Other Player";

        Debug.Log("send server waiting player");
        nodeObject = GameObject.Find("Node");
        nodeObject.GetComponent<Node>().FindMatch();

        StartCoroutine(CheckForPlayer());
    }
    public void OnDestroy()
    {
        Debug.Log("send server that i stop waiting!");
        nodeObject.GetComponent<Node>().QuitGame();
    }

    public static void LoadGameSceneHandler(string _name, int _loadType)
    {
        loadGameScene = _name;
        loadGameType = _loadType;
        SceneManager.LoadScene("WaitScene");
    }
    IEnumerator CheckForPlayer()
    {
        yield return null;
        while (!foundMatch)
        {
            yield return null;
            i = (i + 1) % 600;
            waitingText.text = "Waiting for Other Player" + dots[(i/150)];
        }

        // 서버로부터 상대방이 입장했다는 연락을 받으면 scene 로딩 시작!
        AsyncOperation operation = SceneManager.LoadSceneAsync(loadGameScene);
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            yield return null;
            i = (i + 1) % 600;
            waitingText.text = "Found Match! Loading Game" + dots[(i / 150)];
        }
        
    }
    

    public void SetFoundMatch(bool found)
    {
        if (found) foundMatch = true;
        else foundMatch = false;
    }

}
