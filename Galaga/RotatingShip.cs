using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Galaga
{

    class RotatingShip
    {
        public static List<RotatingShip> ListOfShips = new List<RotatingShip>();

        public static Texture2D EnemyAndPlayerTexture;

        // position in game
        public Vector2 Position;

        // rotation
        public Vector2 Rotation;
        public float Angle;
        

        // position on texture
        public Rectangle TargetPosition = new Rectangle(0,0,16,16);

        public ShipTypeEnum Type;

        public bool TextureFlippedVertically;
        public bool TextureFlippedHorizontally;

        // scale of texture (1 - not resized)
        public const float Scale = 6;

        public Rectangle Hitbox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, 16 * (int)Scale, 16 * (int)Scale);
            }
        }

        public RotatingShip(float x, float y, ShipTypeEnum type, float angle)
        {
            Position = new Vector2(x, y);
            Type = type;
            Angle = angle;
            SetDirection(AngleToVector(angle));
            TargetPosition.Y = (int)type * (8 + 16);
            ListOfShips.Add(this);
        }
        

        public void SetDirection(Vector2 moveVector)
        {
            Angle = VectorToAngle(moveVector);

            if (Angle > 0)
                TextureFlippedVertically = true;
            else
                TextureFlippedVertically = false;

            if (Math.Abs(Angle) < (Math.PI / 2.0f))
                TextureFlippedHorizontally = true;
            else
                TextureFlippedHorizontally = false;

            // calculate which texture to choose
            float tempAngle = (float)Math.PI - Math.Abs(Angle);
            if(tempAngle > (Math.PI / 2.0f))
            {
                tempAngle = Math.Abs(Angle);
            }
            tempAngle /= (float)(Math.PI / 2.0f);
            tempAngle *= 6.0f;

            int textureIndex = (int)Math.Floor(tempAngle);

            TargetPosition.X = textureIndex * (16+8);
        }

        public void SetTexture(int idx)
        {
            TargetPosition.X = idx * (16 + 8);
        }
        
        public void Draw(SpriteBatch spBatch)
        {

            SpriteEffects effect = SpriteEffects.None;
            if (TextureFlippedHorizontally)
                effect |= SpriteEffects.FlipHorizontally;

            if (TextureFlippedVertically)
                effect |= SpriteEffects.FlipVertically;

            spBatch.Draw(EnemyAndPlayerTexture, Position, TargetPosition, Color.White, 0, Vector2.Zero, Scale, effect, 0);
        }

        protected Vector2 AngleToVector(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        protected float VectorToAngle(Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }
    }
}
