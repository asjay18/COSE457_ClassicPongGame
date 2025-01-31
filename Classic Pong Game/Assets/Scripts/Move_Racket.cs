using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Racket : MonoBehaviour
{
    public bool isOnRight;
    public float speed = 30.0f;

    public GameObject rightSideGameObject;
    private RightSide rightSideVariable;

    private Node serverNode;

    void Start()
    {
        rightSideVariable = rightSideGameObject.GetComponent<RightSide>();
        serverNode = GameObject.Find("Node").GetComponent<Node>();
    }
    void FixedUpdate()
    {
        if(rightSideVariable.isPlayerOnRight == isOnRight)
        {
            float v = Input.GetAxisRaw("Vertical");
            float h = Input.GetAxisRaw("Horizontal");
            GetComponent<Rigidbody2D>().velocity = new Vector2(h, v) * speed;
            serverNode.ChangePlayerMoveFlag();
        }   
    }
}
