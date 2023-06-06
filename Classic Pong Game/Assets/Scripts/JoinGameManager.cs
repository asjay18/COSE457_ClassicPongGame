using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;

public class JoinGameManager : MonoBehaviour
{
    public TMP_Text errorMessageText;
    private Node serverNode;

    private void Start()
    {
        serverNode = GameObject.Find("Node").GetComponent<Node>();
        serverNode.ChangeFindJgmObjectFlag();
        errorMessageText.text = "";
    }


    private Queue<UnityAction> actionBuffer = new Queue<UnityAction>();
    private void Update()
    {
        if (actionBuffer.Count > 0)
        {
            actionBuffer.Dequeue().Invoke();
        }
    }

    public void SetJoinError(string messageFromServer)
    {
        actionBuffer.Enqueue(delegate {
            errorMessageText.text = messageFromServer;
        });
    }

    public void LoadWaitScene()
    {
        actionBuffer.Enqueue(delegate {
            SceneManager.LoadScene("WaitScene");
        });
    }
}
