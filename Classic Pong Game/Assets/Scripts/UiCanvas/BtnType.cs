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

    private void Start()
    {
        defaultScale = buttonScale.localScale;
    }
    public void OnBtnClick()
    {
        switch (currentType)
        {
            case BTNType.PlayGame:
                WaitForPlayer.LoadGameSceneHandler("InGameScene", 0);
                Debug.Log("새 게임 시작하기");
                break;
            case BTNType.GoToMain:
                SceneManager.LoadScene("mainScene");
                Debug.Log("대기 상태 해제");
                break;
            case BTNType.Exit:
                Application.Quit();
                Debug.Log("나가기");
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
