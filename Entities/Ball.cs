using System;
using CocosSharp;

namespace Test
{
    public class Ball : CCNode
    {
        CCSprite ballSprite;
        //Getter Setter 
        public float VelocityX
        {
            get;
            set;
        }

        public float VelocityY
        {
            get;
            set;
        }

        public Ball() : base()
        {
            ballSprite = new CCSprite("BigBall");

            // Making the Sprite be centered makes
            // positioning easier.
            ballSprite.AnchorPoint = CCPoint.AnchorMiddle;
            this.AddChild(ballSprite);

            //Starts our frame logic
            this.Schedule(ApplyVelocity);
        }

        //Our frame by frame action
        void ApplyVelocity(float time)
        {
            PositionX += VelocityX*time;
            PositionY += VelocityY*time;
        }

    }
}



   

       

       
