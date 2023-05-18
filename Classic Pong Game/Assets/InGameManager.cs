using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    public GameObject rightSideGameObject;
    private RightSide rightSideManager;

    public GameObject ball;
    private static Rigidbody2D ballRigidBody;

    private GameObject nodeObject;
    private Node serverNode;

    void Start()
    {
        nodeObject = GameObject.Find("Node");
        serverNode = nodeObject.GetComponent<Node>();

        rightSideManager = rightSideGameObject.GetComponent<RightSide>();
        bool isRight = serverNode.CheckIsRight();
        Debug.Log(isRight);
        rightSideManager.ChangePlayerSide(isRight);


        ballRigidBody = ball.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void SetBallPos(Vector2 _inVelocity)
    {
        ballRigidBody.velocity = _inVelocity;
    }

    public void SendBallVel(Vector2 _outVelocity)
    {
        GameMessage velocityMessage = new GameMessage("OnMessage", "20", JsonUtility.ToJson(_outVelocity));
        serverNode.SendWebSocketMessage(JsonUtility.ToJson(velocityMessage));
    }
}
