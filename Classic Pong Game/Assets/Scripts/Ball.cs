using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float speed = 30.0f;

    public GameObject inGameManagerGameObject;
    private InGameManager inGameManager;

    private Node serverNode;

    float hitFactor(Vector2 ballPos, Vector2 paddlePos, float paddleHeight)
    {
        return (ballPos.y - paddlePos.y / paddleHeight);
    }

    void Start()
    {
        inGameManager = inGameManagerGameObject.GetComponent<InGameManager>();
        
        GameObject nodeObject = GameObject.Find("Node");
        serverNode = nodeObject.GetComponent<Node>();
    }

    private void OnEnable()
    {
        if (inGameManager == null) return;
        gameObject.transform.position = new Vector3(0, 0, -1);
        if (inGameManager.scoreSide == 1)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.left * speed;
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.right * speed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Paddle_R")
        {
            if (serverNode.playerData.sideNumber == 2)
            {
                float y = hitFactor(transform.position, collision.transform.position, collision.collider.bounds.size.y);
                Vector2 dir = new Vector2(-1, y).normalized;

                Vector2 newVelocity = 1.05f * speed * dir;
                GetComponent<Rigidbody2D>().velocity = newVelocity;

                serverNode.PlayerHits(
                    new BallData(
                        gameObject.transform.position.x,
                        gameObject.transform.position.y,
                        newVelocity.x, 
                        newVelocity.y
                    )
                );
            }
        }
        else if (collision.gameObject.name == "Paddle_L")
        {
            if (serverNode.playerData.sideNumber == 1)
            {
                float y = hitFactor(transform.position, collision.transform.position, collision.collider.bounds.size.y);
                Vector2 dir = new Vector2(1, y).normalized;

                Vector2 newVelocity = 1.05f * speed * dir;
                GetComponent<Rigidbody2D>().velocity = newVelocity;
            }
                
        }
    }
}
