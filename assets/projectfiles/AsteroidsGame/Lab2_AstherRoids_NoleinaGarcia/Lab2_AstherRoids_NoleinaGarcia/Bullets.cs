// /////////////////////////////////////////////////////////////////////////////
// Lab02 - Astheroids
// March 17 2018
// By Noleina Garcia for CMPE2800: Advanced C# Programming
//
// This class is dedicated to making the bullets for the spaceship. It makes
// the polypath for it and also colors the drawing it.
// /////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Lab2_AstherRoids_NoleinaGarcia
{
    public class Bullets : ShapeBase
    {
        protected GraphicsPath b_gp = new GraphicsPath();

        public Bullets(PointF pos)
            : base(pos)
        {
            b_gp = MakePolyPath(_cfRadius - 5, 20, 0);
        }

        //Scale, Rotate, Translate
        //does each change on the shape one at a time in this order so that the shape doesn't get convoluted
        public override GraphicsPath GetPath(Rectangle r)
        {
            GraphicsPath bullet = (GraphicsPath) b_gp.Clone();

            Matrix mat = new Matrix();

            //Makes the bullet smaller
            mat.Reset();
            mat.Scale(0.2f, 0.2f);
            bullet.Transform(mat);

            //Rotates the bullet
            mat.Reset();
            mat.Rotate(_dRotation);
            bullet.Transform(mat);

            //Shoots the bullet in the direction that the ship is pointing at
            mat.Reset();
            mat.Translate(_position.X, _position.Y);
            bullet.Transform(mat);

            return bullet;
        }

        public override void Animation(Size clientsize)
        {
            //Increments the rotation
            if (TurnRight)
            {
                _dRotation += 5;
            }
            if (TurnLeft)
            {
                _dRotation -= 5;
            }

            //Formula to get the position of the tip of the spaceship
            _position.X += (float)(Math.Sin((180 + _dRotation) * (Math.PI / 180.0)) * 20);
            _position.Y += (float)(Math.Cos(_dRotation * (Math.PI / 180.0)) * 20);

            //if the bullets have hit the wall then kill it and dont let it wrap around like the asteroids and spaceship
            if (_position.X < 0 || (_position.X >= clientsize.Width) || (_position.Y < 0) || (_position.Y >= clientsize.Height))
            {
                IsMarkedForDeath = true;
            }
        }

        //Colors the bullets in Thistle
        public override void ColoredRender(Graphics gr, Rectangle r)
        {
            gr.FillPath(new SolidBrush(Color.Thistle), GetPath(r));
        }
    }
}
