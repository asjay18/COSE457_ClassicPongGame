using System;

[Serializable]
public class BallData
{
    public float xPos;
    public float yPos;
    public float xVel;
    public float yVel;

    public BallData(float xPosIn, float yPosIn, float xVelIn, float yVelIn)
    {
        xPos = xPosIn;
        yPos = yPosIn;
        xVel = xVelIn;
        yVel = yVelIn;
    }
}
