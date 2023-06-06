using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class BtnType : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public BTNType currentType;
    public Transform buttonScale;
    Vector3 defaultScale;

    private GameObject nodeObject;
    private Node serverNode;

    private void Start()
    {
        defaultScale = buttonScale.localScale;
        nodeObject = GameObject.Find("Node");
        serverNode = nodeObject.GetComponent<Node>();
    }
    public void OnBtnClick()
    {
        switch (currentType)
        {
            case BTNType.PlayGame:
                WaitForPlayer.LoadGameSceneHandler("InGameScene", 0, "");
                SceneManager.LoadScene("WaitScene");
                Debug.Log("새 게임 시작하기");
                break;
            case BTNType.PlayGameWithFriendHOST:
                WaitForPlayer.LoadGameSceneHandler("InGameScene", 1, "");
                SceneManager.LoadScene("WaitScene");
                Debug.Log("친구와 새 게임 시작하기");
                break;
            case BTNType.PlayGameWithFriendCLIENT1:
                SceneManager.LoadScene("CodeInputScene");
                Debug.Log("친구게임에 참가하기");
                break;
            case BTNType.PlayGameWithFriendCLIENT2:
                //SceneManager.LoadScene("codeInputScene");
                Debug.Log("친구게임에 참가하기");
                break;
            case BTNType.GoToMain:
                SceneManager.LoadScene("mainScene");
                Debug.Log("대기 상태 해제");
                serverNode.QuitGame();
                break;
            case BTNType.Exit:
                Application.Quit();
                Debug.Log("나가기");
                break;
            case BTNType.Ready:
                serverNode.PlayerIsReady();
                break;
        }
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
