using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkMore : MonoBehaviour
{
    private float timeCount = 0.0f;
    public Transform imageScale;
    Vector3 defaultScale;

    private Color defaultColor;

    private void Start()
    {
        defaultScale = imageScale.localScale;
        defaultColor = gameObject.GetComponent<RawImage>().color;
    }

    // Update is called once per frame
    private void Update()
    {
            timeCount += Time.deltaTime;
            if (timeCount < 0.5f)
            {
                gameObject.GetComponent<RawImage>().color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 1 - timeCount);
            }
            else
            {
                gameObject.GetComponent<RawImage>().color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, timeCount);
                if (timeCount > 1.0f)
                {
                    timeCount = 0.0f;

                }
            }
            imageScale.localScale = defaultScale * (1 + (1 - timeCount)/10);
    }
}
