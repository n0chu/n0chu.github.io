// /////////////////////////////////////////////////////////////////////////////
// Lab02 - Astheroids
// March 17 2018
// By Noleina Garcia for CMPE2800: Advanced C# Programming
//
// Collects all the data coming from the controller and the keyboard which
// then gets used in this program to attach certain actions to certain events
// in the game.
// /////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace Lab2_AstherRoids_NoleinaGarcia
{
    //All the set controls for the Asteroids game
    public struct Shimon
    {
        public bool TurnClockwise, TurnCounterClockwise, PewPew, Gogo, Shield, Pause, Restart;

        public Shimon(bool clockwise, bool counterclockwise, bool fire, bool thrust, bool s, bool p, bool r)
        {
            TurnClockwise = clockwise;                           //Turns the spaceship to the right
            TurnCounterClockwise = counterclockwise;             //Turns the spaceship to the left
            PewPew = fire;                                       //Activates the guns on the spaceship and lets it fire the asteroids
            Gogo = thrust;                                       //Moves the spaceship forwards
            Shield = s;                                          //Activates a shield to protect the ship (was not implemented in this code)
            Pause = p;                                           //Pauses the game
            Restart = r;                                         //Restarts the game
        }
    }

    public class InputAbstraction
    {
        private GamePadState gps;
        protected Shimon s;
        private bool LastPauseState = false;                     //stores the last state of the pause button

        public Shimon SPublic { private set { s = value; } get { return s; } }    //lets the form have access to all the controls of the game without letting it change the code

        public InputAbstraction()
        {
            s = new Shimon();
            Thread xboxthread = new Thread(Xboxy);               //Dedicated to get input from the controller, therefore it needs to be on at all times to be ready to receive data
            xboxthread.IsBackground = true;
            xboxthread.Start();
        }



        public void InputKeyUp(object sender, KeyEventArgs e)
        {
            //for keyboard
            //If a controller is not connected then proceed to unset all the events according to which key was let go
            if (!GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                switch (e.KeyCode)
                {
                    //Thrust: Moves the ship forwards
                    case System.Windows.Forms.Keys.W:
                    case System.Windows.Forms.Keys.Up:

                        s.Gogo = false;
                        break;
                    //Turns the spaceship clockwise
                    case System.Windows.Forms.Keys.D:
                    case System.Windows.Forms.Keys.Right:

                        s.TurnClockwise = false;
                        break;
                    //Turns the spaceship counterclockwise
                    case System.Windows.Forms.Keys.A:
                    case System.Windows.Forms.Keys.Left:

                        s.TurnCounterClockwise = false;
                        break;
                    //Restarts the game
                    case System.Windows.Forms.Keys.R:

                        s.Restart = false;
                        break;
                    //Lets the spaceship shoot the asteroids
                    case System.Windows.Forms.Keys.Space:

                        s.PewPew = false;
                        break;
                    //Disarms the shields around the spaceship
                    case System.Windows.Forms.Keys.ShiftKey:

                        s.Shield = false;
                        break;
                }
            }
        }

        public void InputKeyDown(object sender, KeyEventArgs e)
        {
            //for keyboard
            //If a controller is not connected then proceed to set all the events according to which key is being pressed
            if (!GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                switch (e.KeyCode)
                {
                    //Thrust: Moves the ship forwards
                    case System.Windows.Forms.Keys.W:
                    case System.Windows.Forms.Keys.Up:

                        s.Gogo = true;
                        break;
                    //Turns the spaceship clockwise
                    case System.Windows.Forms.Keys.D:
                    case System.Windows.Forms.Keys.Right:

                        s.TurnClockwise = true;
                        break;
                    //Turns the spaceship counterclockwise
                    case System.Windows.Forms.Keys.A:
                    case System.Windows.Forms.Keys.Left:

                        s.TurnCounterClockwise = true;
                        break;
                    //Pauses the game
                    case System.Windows.Forms.Keys.Escape:
                    case System.Windows.Forms.Keys.P:

                        //There is only one event for this control because the pause function must be able to toggle
                        if (s.Pause.Equals(false))
                        {
                            s.Pause = true;
                        }
                        else if (s.Pause.Equals(true))
                        {
                            s.Pause = false;
                        }
                        break;
                    //Restarts the game
                    case System.Windows.Forms.Keys.R:

                        s.Restart = true;
                        break;
                    //Lets the spaceship shoot the asteroids
                    case System.Windows.Forms.Keys.Space:

                        s.PewPew = true;
                        break;
                    //Activates the shields around the spaceship
                    case System.Windows.Forms.Keys.ShiftKey:

                        s.Shield = true;
                        break;
                }
            }
        }

        public void Xboxy()
        {
            while (true)
            {
                gps = GamePad.GetState(PlayerIndex.One);

                //for controller
                //If a controller is connected then ignore all the keyboard events and proceed to set/unset all the events according to which button is being pressed
                if (gps.IsConnected)
                {
                    //Turn CounterClockwise
                    if (gps.IsButtonDown(Buttons.DPadLeft))
                    {
                        s.TurnCounterClockwise = true;
                    }
                    if (gps.IsButtonUp(Buttons.DPadLeft))
                    {
                        s.TurnCounterClockwise = false;
                    }
                    //Turn Clockwise
                    if (gps.IsButtonDown(Buttons.DPadRight))
                    {
                        s.TurnClockwise = true;
                    }
                    if (gps.IsButtonUp(Buttons.DPadRight))
                    {
                        s.TurnClockwise = false;
                    }
                    //Fire
                    if (gps.IsButtonDown(Buttons.RightTrigger))
                    {
                        s.PewPew = true;
                    }
                    if (gps.IsButtonUp(Buttons.RightTrigger))
                    {
                        s.PewPew = false;
                    }
                    //Thurst
                    if (gps.IsButtonDown(Buttons.LeftTrigger))
                    {
                        s.Gogo = true;
                    }
                    if (gps.IsButtonUp(Buttons.LeftTrigger))
                    {
                        s.Gogo = false;
                    }
                    //Shield
                    if (gps.IsButtonDown(Buttons.B))
                    {
                        s.Shield = true;
                    }
                    if (gps.IsButtonUp(Buttons.B))
                    {
                        s.Shield = false;
                    }
                    //Pause
                    if (gps.IsButtonDown(Buttons.Start))
                    {
                        if (!LastPauseState)
                        {
                            s.Pause = !s.Pause;
                        }

                        LastPauseState = true;
                    }
                    else
                    {
                        LastPauseState = false;
                    }
                    //Restart
                    if (gps.IsButtonDown(Buttons.Back))
                    {
                        s.Restart = true;
                    }
                }

                Thread.Sleep(20);
            }
        }
    }
}
