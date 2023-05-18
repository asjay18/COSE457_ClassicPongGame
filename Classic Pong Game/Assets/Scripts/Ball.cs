using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float speed = 30.0f;

    public GameObject rightSideGameObject;
    private RightSide rightSideVariable;

    public GameObject inGameManagerGameObject;
    private InGameManager inGameManager;

    float hitFactor(Vector2 ballPos, Vector2 paddlePos, float paddleHeight)
    {
        return (ballPos.y - paddlePos.y / paddleHeight);
    }

    void Start()
    {
        rightSideVariable = rightSideGameObject.GetComponent<RightSide>();
        inGameManager = inGameManagerGameObject.GetComponent<InGameManager>();

        if (rightSideVariable.isPlayerOnRight)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.right * speed;
            inGameManager.SendBallVel(Vector2.right * speed);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!rightSideVariable.isPlayerOnRight) return;

        Vector2 newVelocity = new Vector2();
        if (collision.gameObject.name == "Paddle_R")
        {
            float y = hitFactor(transform.position, collision.transform.position, collision.collider.bounds.size.y);
            Vector2 dir = new Vector2(-1, y).normalized;

            newVelocity = dir * speed * 1.05f;
            GetComponent<Rigidbody2D>().velocity = newVelocity;
        }
        if (collision.gameObject.name == "Paddle_L")
        {
            float y = hitFactor(transform.position, collision.transform.position, collision.collider.bounds.size.y);
            Vector2 dir = new Vector2(1, y).normalized;

            newVelocity = dir * speed * 1.05f;
            GetComponent<Rigidbody2D>().velocity = newVelocity;
        }
        inGameManager.SendBallVel(newVelocity);
    }
}
