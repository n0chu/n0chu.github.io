// /////////////////////////////////////////////////////////////////////////////
// Lab02 - Astheroids
// March 17 2018
// By Noleina Garcia for CMPE2800: Advanced C# Programming
//
// This class is responsible for designing and managing asteroid behavior.
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
    class Asteroids : ShapeBase
    {
        GraphicsPath _modelGraphicsPath;

        //Makes the rock shape by making its polypath
        public Asteroids(PointF pos, AsteroidState _as, double sizechange = 0)
            : base(pos)
        {
            //Gives a random number of sides from 6 - 12, and a 25% radius variance
            _splitratio = (sizechange * 10.0) / 100.0;

            _modelGraphicsPath = MakePolyPath(_cfRadius * _splitratio, _random.Next(6, 13), 0.25);
            _dXSpeed = (float)(_random.NextDouble() * 3 - 1.5);
            _dYSpeed = (float)(_random.NextDouble() * 3 - 1.5);
            asteroidrockstate = _as;
        }

        //Scale, Rotate, Translate
        //Draw the asteroid and does each change on the shape one at a time in this order so that the shape doesn't get convoluted
        public override GraphicsPath GetPath(Rectangle r)
        {
            GraphicsPath copy = (GraphicsPath)_modelGraphicsPath.Clone();

            Matrix mat = new Matrix();

            //Only lets the asteroids rotate when the game isn't paused
            if (!IsPaused)
            {
                mat.Rotate(_dRotation++);
                copy.Transform(mat);
            }
            else
            {
                mat.Rotate(_dRotation);
                copy.Transform(mat);
            }

            //Moves the asteroid to the proper position in the client rectangle
            mat.Reset();
            mat.Translate(_position.X, _position.Y);
            copy.Transform(mat);

            //If the original asteroid is starting to disappear into a wall then draw copies of that asteriod on all the different directions and add it to the path
            if (_position.X < _cfRadius || _position.Y < _cfRadius || (r.Width - _position.X) < _cfRadius || (r.Height - _position.Y) < _cfRadius)                          //Checks if the asteroid has hit a wall
            {
                GraphicsPath mirror1 = (GraphicsPath)copy.Clone();
                mat.Reset();
                mat.Translate(-r.Width, r.Height);
                mirror1.Transform(mat);

                GraphicsPath mirror2 = (GraphicsPath)copy.Clone();
                mat.Reset();
                mat.Translate(0, r.Height);
                mirror2.Transform(mat);

                GraphicsPath mirror3 = (GraphicsPath)copy.Clone();
                mat.Reset();
                mat.Translate(r.Width, r.Height);
                mirror3.Transform(mat);

                GraphicsPath mirror4 = (GraphicsPath)copy.Clone();
                mat.Reset();
                mat.Translate(-r.Width, 0);
                mirror4.Transform(mat);

                GraphicsPath mirror5 = (GraphicsPath)copy.Clone();
                mat.Reset();
                mat.Translate(r.Width, 0);
                mirror5.Transform(mat);

                GraphicsPath mirror6 = (GraphicsPath)copy.Clone();
                mat.Reset();
                mat.Translate(-r.Width, -r.Height);
                mirror6.Transform(mat);

                GraphicsPath mirror7 = (GraphicsPath)copy.Clone();
                mat.Reset();
                mat.Translate(0, -r.Height);
                mirror7.Transform(mat);

                GraphicsPath mirror8 = (GraphicsPath)copy.Clone();
                mat.Reset();
                mat.Translate(r.Width, -r.Height);
                mirror8.Transform(mat);

                copy.AddPath(mirror1, false);
                copy.AddPath(mirror2, false);
                copy.AddPath(mirror3, false);
                copy.AddPath(mirror4, false);
                copy.AddPath(mirror5, false);
                copy.AddPath(mirror6, false);
                copy.AddPath(mirror7, false);
                copy.AddPath(mirror8, false);
            }

            return copy;
        }

        public override void Animation(Size clientsize)
        {
            //Applies the new changes to the position so the asteroids can move
            _position.X += _dXSpeed;
            _position.Y += _dYSpeed;
        }

        public override void ColoredRender(Graphics gr, Rectangle r)
        {
            //If nothing has hit the asteroid then continue to draw it as pink, otherwise make it blue (momentarily)
            if (!IsClose)
            {
                gr.FillPath(new SolidBrush(Color.LightCoral), GetPath(r));
            }
            else
            {
                gr.FillPath(new SolidBrush(Color.SkyBlue), GetPath(r));
            }
        }
    }
}
