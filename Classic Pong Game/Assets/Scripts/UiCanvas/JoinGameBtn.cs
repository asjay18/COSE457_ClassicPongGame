using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class JoinGameBtn : MonoBehaviour
{
    public Transform buttonScale;
    Vector3 defaultScale;

    public GameObject inputTextField;
    private TMP_InputField gameCodeInput;
    private string gameCode = null;

    private GameObject nodeObject;
    private Node serverNode;

    // Start is called before the first frame update
    void Start()
    {
        defaultScale = buttonScale.localScale;
        nodeObject = GameObject.Find("Node");
        serverNode = nodeObject.GetComponent<Node>();
        gameCodeInput = inputTextField.GetComponent<TMP_InputField>();
    }

    // Update is called once per frame
    public void OnJoinBtnClick()
    {
        gameCode = gameCodeInput.text;
        serverNode.OpenConnection();
        serverNode.JoinGameWithCode(gameCode);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonScale.localScale = defaultScale * 1.1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonScale.localScale = defaultScale;
    }
}
