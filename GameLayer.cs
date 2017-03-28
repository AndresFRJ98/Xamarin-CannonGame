using System;
using System.Collections.Generic;
using CocosSharp;
using Microsoft.Xna.Framework;

namespace Test
{
    public class GameLayer : CCLayerColor
    {

        // Define a label variable
        CCLabel label;
        CCLabel hiLabel;

        CCSprite cannonSprite;
        Ball ballSprite;
        CCSprite duckSprite;

        //Direction variable
        int dir = 1;
        
        //Indicator for if our ball is in the scene
        bool active = false;

        //Score and hiscore variables.

        int score = 0;
        int hiscore = 0;

        public GameLayer() : base(CCColor4B.Black)
        {

            // create and initialize a Label
            label = new CCLabel("Hello CocosSharp", "Fonts/MarkerFelt", 22, CCLabelFormat.SpriteFont);

            // add the label as a child to this Layer
            AddChild(label);

            //hiScore Label
            hiLabel = new CCLabel("Hello CocosSharp", "Fonts/MarkerFelt", 22, CCLabelFormat.SpriteFont);
            AddChild(hiLabel);


            duckSprite = new CCSprite("duck");
            AddChild(duckSprite);

            cannonSprite = new CCSprite("CannonSprite");
            AddChild(cannonSprite);

            Schedule(RunGameLogic);

        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            // Use the bounds to layout the positioning of our drawable assets
            var bounds = VisibleBoundsWorldspace;

            // position the label on the center of the screen
            label.Position = bounds.Center;

            //OUR CODE 
            hiLabel.Position = bounds.Center;
            hiLabel.PositionY -= 100;

            // Position sprites

            cannonSprite.PositionX = bounds.MidX;
            cannonSprite.PositionY = 100;

            duckSprite.PositionX = 500;
            duckSprite.PositionY = 980;


            // Register for touch events
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesBegan = OnTouchesEnded;
            AddEventListener(touchListener, this);
        }

        void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count > 0)
            {
                // Perform touch handling here

                // Rotate cannon

                //Part one 
                float differenceY = touches[0].Location.Y - cannonSprite.PositionY;
                float differenceX = touches[0].Location.X - cannonSprite.PositionX;

                //Part two
                float angleInDegrees = -1 * CCMathHelper.ToDegrees(
                (float)System.Math.Atan2(differenceY, differenceX));
                angleInDegrees += 90;
               
                //Part three
                cannonSprite.RunAction(new CCRotateTo(0, angleInDegrees));

                //Part four
                CCAudioEngine.SharedEngine.PlayEffect("CannonFire");

                //Create and shoot ball

                if (active)
                {
                    ballSprite.RemoveFromParent(true);
                    //Resets score
                    score = 0;

                }

                ballSprite = new Ball();
                AddChild(ballSprite);
                active = true;
                ballSprite.VelocityX = 0;
                ballSprite.VelocityY = 0;
                ballSprite.Position = cannonSprite.Position;

                //Changing 650 alters velocity. 
                CCVector2 velocity = new CCVector2(0, 650);
                RotateVector(ref velocity, angleInDegrees);
                ballSprite.VelocityX = velocity.X;
                ballSprite.VelocityY = velocity.Y;


            }
        }

        void RunGameLogic(float frameTimeInSeconds)
        {
            //Increases speed by current score
            duckSprite.PositionX += (3+score) * dir ;

            // Duck edges 

            float duckRight = duckSprite.BoundingBoxTransformedToParent.MaxX;
            float duckLeft = duckSprite.BoundingBoxTransformedToParent.MinX;
            
            // Screen edges

            float screenRight = VisibleBoundsWorldspace.MaxX;
            float screenLeft = VisibleBoundsWorldspace.MinX;
            float screenTop = VisibleBoundsWorldspace.MaxY;
            float screenBot = VisibleBoundsWorldspace.MinY;

            

            bool turn = (duckRight > screenRight) || (duckLeft < (screenLeft));

            if (turn)
            {
                dir *= -1;
                if (dir < 0)
                {
                    duckSprite.FlipX = false;
                }
                else
                {
                    duckSprite.FlipX = true;
                }

            }

            //Score system
            label.Text = "Score: " + score;
            hiLabel.Text = "Highscore: " + hiscore;

            if (active)
            {
                bool scored = ballSprite.BoundingBoxTransformedToParent.IntersectsRect
                (duckSprite.BoundingBoxTransformedToParent);

                if (scored)
                {
                    ballSprite.Position = cannonSprite.Position;
                    ballSprite.RemoveFromParent(true);
                    active = false;
                    ballSprite.VelocityX = 0;
                    ballSprite.VelocityY = 0;
                    CCAudioEngine.SharedEngine.PlayEffect("score");
                    score++;
                }

                if (score >= hiscore)
                {
                    hiscore = score;
                }

                bool onscreen = ballSprite.BoundingBoxTransformedToParent.IntersectsRect(VisibleBoundsWorldspace);

                if (onscreen == false)
                {
                    ballSprite.RemoveFromParent(true);
                    active = false;
                    CCAudioEngine.SharedEngine.PlayEffect("splash");
                    score = 0;
                }
            }

            




        }

        void RotateVector(ref CCVector2 vector, float cocosSharpDegrees)
        {
            // Invert the rotation to get degrees as is normally
            // used in math (counterclockwise)
            float mathDegrees = -cocosSharpDegrees;

            // Convert the degrees to radians, as the System.Math
            // object expects arguments in radians
            float radians = CCMathHelper.ToRadians(mathDegrees);

            // Calculate the "up" and "right" vectors. This is essentially
            // a 2x2 matrix that we'll use to rotate the vector
            float xAxisXComponent = (float)System.Math.Cos(radians);
            float xAxisYComponent = (float)System.Math.Sin(radians);
            float yAxisXComponent = (float)System.Math.Cos(radians + CCMathHelper.Pi / 2.0f);
            float yAxisYComponent = (float)System.Math.Sin(radians + CCMathHelper.Pi / 2.0f);

            // Store the original vector values which will be used
            // below to perform the final operation of rotation.
            float originalX = vector.X;
            float originalY = vector.Y;

            // Use the axis values calculated above (the matrix values)
            // to rotate and assign the vector.
            vector.X = originalX * xAxisXComponent + originalY * yAxisXComponent;
            vector.Y = originalX * xAxisYComponent + originalY * yAxisYComponent;
        }
    }
}

