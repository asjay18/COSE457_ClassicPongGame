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
                Debug.Log("�� ���� �����ϱ�");
                break;
            case BTNType.PlayGameWithFriendHOST:
                WaitForPlayer.LoadGameSceneHandler("InGameScene", 1, "");
                SceneManager.LoadScene("WaitScene");
                Debug.Log("ģ���� �� ���� �����ϱ�");
                break;
            case BTNType.PlayGameWithFriendCLIENT1:
                SceneManager.LoadScene("CodeInputScene");
                Debug.Log("ģ�����ӿ� �����ϱ�");
                break;
            case BTNType.PlayGameWithFriendCLIENT2:
                //SceneManager.LoadScene("codeInputScene");
                Debug.Log("ģ�����ӿ� �����ϱ�");
                break;
            case BTNType.GoToMain:
                SceneManager.LoadScene("mainScene");
                Debug.Log("��� ���� ����");
                serverNode.QuitGame();
                break;
            case BTNType.Exit:
                Application.Quit();
                Debug.Log("������");
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
