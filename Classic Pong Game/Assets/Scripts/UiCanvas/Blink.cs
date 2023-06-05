using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    private int blinkNum = 0;
    private float timeCount = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        blinkNum = 0;
        timeCount = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeCount < 0.5f)
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1 - timeCount);
        } else
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, timeCount);
            if (timeCount > 1.0f)
            {
                timeCount = 0.0f;
                blinkNum++;

                if (blinkNum > 4) Destroy(this);
            }
        }
    }
}
