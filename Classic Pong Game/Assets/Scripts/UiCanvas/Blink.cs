using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blink : MonoBehaviour
{
    private bool enableFlag = false;
    private int blinkNum = 0;
    private float timeCount = 0.0f;

    // Update is called once per frame
    private void Update()
    {
        if (enableFlag)
        {
            timeCount += Time.deltaTime;
            if (timeCount < 0.5f)
            {
                gameObject.GetComponent<RawImage>().color = new Color(1, 1, 1, 1 - timeCount);
            }
            else
            {
                gameObject.GetComponent<RawImage>().color = new Color(1, 1, 1, timeCount);
                if (timeCount > 1.0f)
                {
                    timeCount = 0.0f;
                    blinkNum++;

                    if (blinkNum > 2)
                    {
                        enableFlag = false;
                        gameObject.SetActive(false);
                    }
                }
            }
        }
    }
    private void OnEnable()
    {
        enableFlag = true; 
        blinkNum = 0;
        timeCount = 0.0f;
    }
}
