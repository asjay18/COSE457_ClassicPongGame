using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreUp : MonoBehaviour
{
    public bool rightwall;
    public GameObject scoreManagerObject;

    private ScoreManager scoreManager;

    void Start()
    {
        scoreManager = scoreManagerObject.GetComponent<ScoreManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (rightwall) scoreManager.plWin();
        else scoreManager.prWin();
    }
}
