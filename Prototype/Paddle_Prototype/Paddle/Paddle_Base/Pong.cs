using System;
using System.Collections.Generic;
using XnaGeometry;

namespace Paddle
{
    public class Pong
    {
        #region Constants

        private const int BALL_RADIUS = 5;
        private const int HEIGHT = 240;
        private const int WIDTH = 320;
        private const int BORDER_WIDTH = WIDTH / 2;
        private const int BORDER_HEIGHT = 5;
        private const int PADDLE_WIDTH = 5;
        private const int PADDLE_HEIGHT = HEIGHT / 10;
        private const int LEFT_GOAL = -WIDTH / 2 - 30;
        private const int RIGHT_GOAL = WIDTH / 2 + 30;
        private const int BALL_SPEED = 10;

        #endregion

        private Random Random { get; set; }
        public GameEntity Ball { get; private set; }
        public GameEntity TopBorder { get; private set; }
        public GameEntity BottomBorder { get; private set; }
        public GameEntity RightPaddle { get; private set; }
        public GameEntity LeftPaddle { get; private set; }
        public IPaddlePlayer PlayerOne { get; set; }
        public IPaddlePlayer PlayerTwo { get; set; }

        #region Helper acccessor

        private double RightPaddleX { get; set; }
        private Vector2 RightPaddleNormal { get; set; }

        private double LeftPaddleX { get; set; }
        private Vector2 LeftPaddleNormal { get; set; }

        private double TopBorderY { get; set; }
        private Vector2 TopBorderNormal { get; set; }

        private double BottomBorderY { get; set; }
        private Vector2 BottomBorderNormal { get; set; }

        #endregion

        public Pong()
        {
            Random = new Random();
            Ball = new GameEntity();
            Ball.Scale = new Vector2(BALL_RADIUS, BALL_RADIUS);
            Ball.Position = Vector2.Zero;
            var angle = Random.NextDouble() * Math.PI * 2;
            Ball.Displacement = new Vector2(Math.Cos(angle), Math.Sin(angle));
            Ball.Displacement.Normalize();

            TopBorder = new GameEntity();
            TopBorder.Scale = new Vector2(BORDER_WIDTH, BORDER_HEIGHT);
            TopBorder.Position = new Vector2(0.0, HEIGHT / 2.0);
            TopBorderY = TopBorder.Position.Y - TopBorder.Scale.Y;
            TopBorderNormal = -1 * Vector2.UnitY;

            BottomBorder = new GameEntity();
            BottomBorder.Scale = new Vector2(BORDER_WIDTH, BORDER_HEIGHT);
            BottomBorder.Position = new Vector2(0.0, -HEIGHT / 2.0);
            BottomBorderY = BottomBorder.Position.Y + BottomBorder.Scale.Y;
            BottomBorderNormal = Vector2.UnitY;

            RightPaddle = new GameEntity();
            RightPaddle.Scale = new Vector2(PADDLE_WIDTH, PADDLE_HEIGHT);
            RightPaddle.Position = new Vector2(WIDTH / 2.0, 0.0);
            RightPaddleX = RightPaddle.Position.X - RightPaddle.Scale.X;
            RightPaddleNormal = -1 * Vector2.UnitX;

            LeftPaddle = new GameEntity();
            LeftPaddle.Scale = new Vector2(PADDLE_WIDTH, PADDLE_HEIGHT);
            LeftPaddle.Position = new Vector2(-WIDTH / 2.0, 0.0);
            LeftPaddleX = LeftPaddle.Position.X + LeftPaddle.Scale.X;
            LeftPaddleNormal = Vector2.UnitX;
        }

        public void Update(double dt)
        {
            if (Ball.Position.X > RIGHT_GOAL)
            {
                PlayerOne.Points++;
                Ball.Position = Vector2.Zero;
            }
            else if (Ball.Position.X < LEFT_GOAL)
            {
                PlayerTwo.Points++;
                Ball.Position = Vector2.Zero;
            }

            bool processed = false;
            double consummed = dt;
            while (consummed > 0.0)
            {
                var displacement = Ball.Displacement * BALL_SPEED * consummed;
                var ballCurrentPosition = Ball.Position;
                var ballTranslation = Matrix.CreateTranslation(displacement.X, displacement.Y, 0.0);
                Vector2 ballNextPosition;
                Vector2.Transform(ref ballCurrentPosition, ref ballTranslation, out ballNextPosition);

                if (ballCurrentPosition.X < LeftPaddleX || ballCurrentPosition.X > RightPaddleX)
                {
                    Ball.Position = ballNextPosition;   // No collision
                    consummed = 0.0;
                }
                else if (ballNextPosition.X < (LeftPaddleX + Ball.Scale.X))
                {
                    processed = ProcessPaddleCollision(LeftPaddle, Ball, LeftPaddleX + Ball.Scale.X, ref consummed, ref displacement);
                    if (!processed)
                    {
                        Ball.Position = ballNextPosition;   // No collision
                        consummed = 0.0;
                    }
                }
                else if (ballNextPosition.X > (RightPaddleX - Ball.Scale.X))
                {
                    processed = ProcessPaddleCollision(RightPaddle, Ball, RightPaddleX - Ball.Scale.X, ref consummed, ref displacement);
                    if (!processed)
                    {
                        Ball.Position = ballNextPosition;   // No collision
                        consummed = 0.0;
                    }
                }
                else if (ballNextPosition.Y > (TopBorderY - Ball.Scale.Y))
                {
                    consummed -= ProcessBorderCollision(Ball, TopBorderY - Ball.Scale.Y, consummed, ref displacement);
                }
                else if (ballNextPosition.Y < (BottomBorderY + Ball.Scale.Y))
                {
                    consummed -= ProcessBorderCollision(Ball, BottomBorderY + Ball.Scale.Y, consummed, ref displacement);
                }
                else
                {
                    Ball.Position = ballNextPosition;   // No collision
                    consummed = 0.0;
                }
            }
        }

        private static bool ProcessPaddleCollision(GameEntity paddle, GameEntity ball, double paddleIntersection, ref double dt, ref Vector2 displacement)
        {
            var ballPosition = ball.Position;
            var t = (paddleIntersection - ballPosition.X) / ball.Displacement.X;
            var y = (ballPosition.Y + ball.Displacement.Y * t);
            var intersectionPoint = new Vector2(paddleIntersection, y);
            // Check with the ball radius
            var processed = (y) < (paddle.Position.Y + paddle.Scale.Y) && (y) > (paddle.Position.Y - paddle.Scale.Y);
            if (processed)
            {
                double distance;
                Vector2.Distance(ref ballPosition, ref intersectionPoint, out distance);
                ball.Position = intersectionPoint;
                var displacementY = ball.Displacement.Y;
                if (ball.Displacement.Y > 0)
                {
                    if (ball.Position.Y < paddle.Position.Y)
                    {
                        // bounce it back
                        displacementY = -1 * ball.Displacement.Y;
                    }
                }
                else
                {
                    if (ball.Position.Y > paddle.Position.Y)
                    {
                        // bounce it back
                        displacementY = -1 * ball.Displacement.Y;
                    }
                }

                ball.Displacement = new Vector2(-1 * ball.Displacement.X, displacementY);
                
                dt -= dt * distance / displacement.Length();
            }

            return processed;
        }

        private static double ProcessBorderCollision(GameEntity ball, double borderIntersection, double dt, ref Vector2 displacement)
        {
            var ballPosition = ball.Position;
            var t = (borderIntersection - ball.Position.Y) / ball.Displacement.Y;
            var x = (ball.Position.X + ball.Displacement.X * t);
            var intersectionPoint = new Vector2(x, borderIntersection);

            double distance;
            Vector2.Distance(ref ballPosition, ref intersectionPoint, out distance);
            ball.Position = intersectionPoint;
            ball.Displacement = new Vector2(ball.Displacement.X, -1 * ball.Displacement.Y);
            return dt * distance / displacement.Length();
        }
    }
}
