// /////////////////////////////////////////////////////////////////////////////
// Lab02 - Astheroids
// March 17 2018
// By Noleina Garcia for CMPE2800: Advanced C# Programming
//
// This program is a simple game where the goal is to destroy all asteroids in
// the spaceship's path by shooting them all. The player gains lives every time
// a factor of 1000 is added to their score, but the player also loses a life
// when the spaceship is hit by a rock.
// /////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Lab2_AstherRoids_NoleinaGarcia
{
    public partial class Form1 : Form
    {
        InputAbstraction me = new InputAbstraction();                    //
        List<ShapeBase> AllShapes = new List<ShapeBase>();               //will hold the spaceship and all the asteroids in a list
        List<Bullets> ammo = new List<Bullets>();                        //will hold all the bullets on the screen
        BufferedGraphicsContext bgc = new BufferedGraphicsContext();     //used to bind a back-buffer to the primary surface, spec size to create as client size
        BufferedGraphics bg;                                             //also used to bind a back-buffer to the primary surface, spec size to create as client size
        PointF SpaceShipPoint = new PointF();                            //stores the middle of the client rectangle which becomes the spawn point for the spaceship
        static Random rnd = new Random();                                //random number generating variable
        bool fireState, restartState = false;                            //both booleans are to restrict the event to only happen once while its corresponding button is pressed down
        bool startscreen = true;                                         //boolean that will run the title screen when the game is initially booted up
        int LifeCounter = 3;                                             //stores the amount of lives the player has
        int LevelCounter = 1;                                            //stores what level the user is on
        int GameScore = 0;                                               //stores the total and current score the player has
        int LastGameScore = 0;                                           //stores the previous score of the player

        public Form1()
        {
            InitializeComponent();

            //Combines and calls the Key Events call in our class instead of in here
            KeyUp += me.InputKeyUp;
            KeyDown += me.InputKeyDown;

            //Starts the game
            RestartGame();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // clear the back-buffer
            bg.Graphics.Clear(Color.MistyRose);

            //Loads Title Screen
            if (startscreen)
            {
                //Writes the welcome screen and the instructions to the controls
                bg.Graphics.DrawString("Welcome to Pastel Astheroids!", new Font(FontFamily.GenericMonospace, 15), new SolidBrush(Color.PaleVioletRed), 250, 0);
                bg.Graphics.DrawString("Press R on the keyboard or back on the console to Start!", new Font(FontFamily.GenericMonospace, 15), new SolidBrush(Color.PaleVioletRed), 50, 30);
                bg.Graphics.DrawString("To move right press DRight on the controller or d on the keyboard", new Font(FontFamily.GenericMonospace, 15), new SolidBrush(Color.PaleVioletRed), 40, 50);
                bg.Graphics.DrawString("To move left press DLeft on the controller or a on the keyboard", new Font(FontFamily.GenericMonospace, 15), new SolidBrush(Color.PaleVioletRed), 40, 70);
                bg.Graphics.DrawString("To fire press Rtrig on the controller or spacebar on the keyboard", new Font(FontFamily.GenericMonospace, 15), new SolidBrush(Color.PaleVioletRed), 40, 90);
                bg.Graphics.DrawString("To thrust press Ltrig on the controller or w on the keyboard", new Font(FontFamily.GenericMonospace, 15), new SolidBrush(Color.PaleVioletRed), 40, 110);
                bg.Graphics.DrawString("To pause press Start on the controller or p and escape on the keyboard", new Font(FontFamily.GenericMonospace, 15), new SolidBrush(Color.PaleVioletRed), 40, 130);
                bg.Graphics.DrawString("To restart press Back on the controller or R on the keyboard", new Font(FontFamily.GenericMonospace, 15), new SolidBrush(Color.PaleVioletRed), 40, 150);

                //After it is shown then it will never show up again
                if (me.SPublic.Restart)
                {
                    startscreen = false;
                }
            }
            //Main Game Loop
            else if ((LifeCounter > 0) && (AllShapes.Count > 1) && (LevelCounter < 4))
            {
                //If the game isn't paused then apply all the changes to the spaceship, bullets and asteroids
                if (!me.SPublic.Pause)
                {
                    //Turn all of the shapes to unpaused
                    AllShapes.ForEach(q => q.IsPaused = false);
                    //AllShapes.ForEach(q => q.AnimationTick(ClientSize));

                    //Ticks all the shapes including the bullets
                    AllShapes.ForEach(q => q.Tick(ClientRectangle, ClientSize));
                    ammo.ForEach(q => q.Tick(ClientRectangle, ClientSize));

                    //Thrust
                    if (me.SPublic.Gogo)
                    {
                        AllShapes[0].ThrustOn = true;
                    }
                    else
                    {
                        AllShapes[0].ThrustOn = false;
                    }
                    //Fire
                    if (me.SPublic.PewPew)
                    {
                        //Only lets the player shoot once everytime the button is still pressed down
                        if (fireState)
                        {
                            //runs the code for the spaceship to fire its bullet
                            AllShapes[0].Fire = true;
                            
                            //Makes a bullet if there aren't 8 bullets on the screen presently
                            Bullets temp = new Bullets(AllShapes[0].Positionn);
                            temp.Rotation = AllShapes[0].Rotation;

                            if (ammo.Count < 8)
                            {
                                ammo.Add(temp);
                            }
                        }
                        fireState = false;
                    }
                    else
                    {
                        AllShapes[0].Fire = false;
                        fireState = true;
                    }
                    //Restarts the game
                    if (me.SPublic.Restart)
                    {
                        //AllShapes[0].Restart = true;

                        if (restartState)
                        {
                            GameScore = 0;
                            RestartGame();
                        }
                        restartState = false;
                    }
                    else
                    {
                        //AllShapes[0].Restart = false;
                        restartState = true;
                    }
                    //Turn Right
                    if (me.SPublic.TurnClockwise)
                    {
                        AllShapes[0].TurnRight = true;
                    }
                    else
                    {
                        AllShapes[0].TurnRight = false;
                    }
                    //Turn Left
                    if (me.SPublic.TurnCounterClockwise)
                    {
                        AllShapes[0].TurnLeft = true;
                    }
                    else
                    {
                        AllShapes[0].TurnLeft = false;
                    }

                    //Checks if any of the asteroids have collided with the spaceship
                    for (int i = 1; i < AllShapes.Count; i++)
                    {
                        Region r1 = new Region(AllShapes[0].GetPath(ClientRectangle)); //asteroid
                        Region r3 = new Region(AllShapes[i].GetPath(ClientRectangle)); //spaceship
                        r1.Intersect(r3);

                        //If they intersect then take a life away
                        if (!r1.IsEmpty(bg.Graphics))
                        {
                            //Shows the effect of taking damage
                            AllShapes[i].IsClose = true;
                            AllShapes[0].IsClose = true;

                            //Puts the Spaceship back to the spawn point
                            if (LifeCounter > 0)
                            {
                                LifeCounter--;
                                AllShapes[0].Positionn = SpaceShipPoint;
                            }
                        }
                    }

                    //Check if any bullets have hit an asteroid
                    for (int i = 1; i < AllShapes.Count; i++)
                    {
                        for (int j = 0; j < ammo.Count; j++)
                        {
                            //If the shapes are in close proximity of each other then check if they intersect
                            if (i < AllShapes.Count && j < ammo.Count)
                            {
                                if (ShapeBase.MirrorGetDistance(AllShapes[i], ammo[j], ClientRectangle) < AllShapes[0].Rockradius)
                                {
                                    Region r1 = new Region(AllShapes[i].GetPath(ClientRectangle)); //asteroid
                                    Region r2 = new Region(ammo[j].GetPath(ClientRectangle));      //bullet

                                    r1.Intersect(r2);

                                    //If they intersect then. check if..
                                    if (!r1.IsEmpty(bg.Graphics))
                                    {
                                        //If the asteroid is in its normal state (the state it spawned in) then store the position the large asteroid was at,
                                        //and split it to two new and smaller asteroids in the same spot, then remove the original
                                        //Add 5 to the user's score
                                        if (AllShapes[i].p_AsteroidState.Equals(AsteroidState.Large))
                                        {
                                            PointF p = AllShapes[i].Positionn;

                                            GameScore += 5;
                                            AllShapes.Add(new Asteroids(p, AsteroidState.Medium, 7.0));
                                            AllShapes.Add(new Asteroids(p, AsteroidState.Medium, 7.0));
                                            //AllShapes.Remove(AllShapes[i]);
                                            //ammo.Remove(ammo[j]);
                                            AllShapes.RemoveAt(i);

                                            if (j < ammo.Count)
                                            {
                                                ammo.RemoveAt(j);
                                            }

                                            //AllShapes[i].IsMarkedForDeath = true;
                                            //ammo[j].IsMarkedForDeath = true;
                                        }
                                        //If the asteroid is in its first split state then store the position the medium asteroid was at,
                                        //and split it to two new and smaller asteroids in the same spot, then remove the original
                                        //Add 10 to the user's score
                                        else if (AllShapes[i].p_AsteroidState.Equals(AsteroidState.Medium))
                                        {
                                            PointF p = AllShapes[i].Positionn;

                                            GameScore += 10;
                                            AllShapes.Add(new Asteroids(p, AsteroidState.Small, 4.0));
                                            AllShapes.Add(new Asteroids(p, AsteroidState.Small, 4.0));
                                            //AllShapes.Remove(AllShapes[i]);
                                            //ammo.Remove(ammo[j]);
                                            AllShapes.RemoveAt(i);

                                            if (j < ammo.Count)
                                            {
                                                ammo.RemoveAt(j);
                                            }

                                            //AllShapes[i].IsMarkedForDeath = true;
                                            //ammo[j].IsMarkedForDeath = true;
                                        }
                                        //If the asteroid is in its second split state then store the position the small asteroid was at then remove it
                                        //Add 15 to the user's score
                                        else if (AllShapes[i].p_AsteroidState.Equals(AsteroidState.Small))
                                        {
                                            GameScore += 15;
                                            //AllShapes.Remove(AllShapes[i]);
                                            //ammo.Remove(ammo[j]);
                                            AllShapes.RemoveAt(i);

                                            if (j < ammo.Count)
                                            {
                                                ammo.RemoveAt(j);
                                            }
                                            //AllShapes[i].IsMarkedForDeath = true;
                                            //ammo[j].IsMarkedForDeath = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                // draw the shape by using bg.Graphics
                //else if the game is paused then freeze everything
                else
                {
                    AllShapes.ForEach(q => q.IsPaused = true);
                }

                //Everytime the player gains 1000 points onto his score he gets another life back
                if (GameScore > 0 && (GameScore - LastGameScore) % 1000 == 0)
                {
                    if (LastGameScore == 0)
                    {
                        LastGameScore = GameScore;
                        LifeCounter += 1;
                    }
                    else
                    {
                        if (GameScore - LastGameScore > 0)
                        {
                            LastGameScore = GameScore;
                            LifeCounter += 1;
                        }
                    }
                }

                //Render all the new changes on the spaceship, asteroids and bullets
                //Remove everything that is dead and turn everything that is blue back to pink
                ammo.ForEach(q => q.Render(bg.Graphics, ClientRectangle));
                AllShapes.ForEach(q => q.Render(bg.Graphics, ClientRectangle));
                AllShapes.ForEach(q => q.IsClose = false);
                AllShapes.RemoveAll(q => q.IsMarkedForDeath);
                ammo.RemoveAll(q => q.IsMarkedForDeath);

                //Status bars at the top left of the screen
                bg.Graphics.DrawString($"Level {LevelCounter}", new Font(FontFamily.GenericMonospace, 15), new SolidBrush(Color.PaleVioletRed), 0, 0);
                bg.Graphics.DrawString($"Life:{LifeCounter}", new Font(FontFamily.GenericMonospace, 15), new SolidBrush(Color.PaleVioletRed), 130, 0);
                bg.Graphics.DrawString($"Bullets:{8 - ammo.Count}", new Font(FontFamily.GenericMonospace, 15), new SolidBrush(Color.PaleVioletRed), 250, 0);
                bg.Graphics.DrawString($"Score:{GameScore}", new Font(FontFamily.GenericMonospace, 15), new SolidBrush(Color.PaleVioletRed), 400, 0);
            }
            //If the user hasn't died and hasn't won yet then level them up and add more asteroids
            else if (AllShapes.Count < 2 && LevelCounter < 4)
            {
                LevelCounter++;
                LevelUp();
            }
            //Once the player has completeled all levels then they have won the game
            else if (LevelCounter > 3)
            {
                if (me.SPublic.Restart)
                {
                    GameScore = 0;
                    RestartGame();
                }

                //Shows the stats of the player
                bg.Graphics.DrawString("You Win!", new Font(FontFamily.GenericMonospace, 50), new SolidBrush(Color.Red), SpaceShipPoint.X - 200, SpaceShipPoint.Y - 40);
                bg.Graphics.DrawString($"Final Score:{GameScore}", new Font(FontFamily.GenericMonospace, 50), new SolidBrush(Color.Red), SpaceShipPoint.X - 300, SpaceShipPoint.Y - 80);
                bg.Graphics.DrawString($"Press R to Play Again", new Font(FontFamily.GenericMonospace, 50), new SolidBrush(Color.Red), SpaceShipPoint.X - 450, SpaceShipPoint.Y - 125);
            }
            //If they ran out of lives then it's game over
            else
            {
                bg.Graphics.Clear(Color.Black);

                if (me.SPublic.Restart)
                {
                    GameScore = 0;
                    RestartGame();
                }

                //Shows the stats to the player
                bg.Graphics.DrawString("Game Over", new Font(FontFamily.GenericMonospace, 50), new SolidBrush(Color.Red), SpaceShipPoint.X - 200, SpaceShipPoint.Y - 40);
                bg.Graphics.DrawString($"Final Score:{GameScore}", new Font(FontFamily.GenericMonospace, 50), new SolidBrush(Color.Red), SpaceShipPoint.X - 300, SpaceShipPoint.Y - 80);
                bg.Graphics.DrawString($"Press R to Play Again", new Font(FontFamily.GenericMonospace, 50), new SolidBrush(Color.Red), SpaceShipPoint.X - 450, SpaceShipPoint.Y - 125);
            }
            // draw the shape by using bg.Graphics
            //ammo.ForEach(q => q.Render(bg.Graphics, ClientRectangle));
            //AllShapes.ForEach(q => q.Render(bg.Graphics, ClientRectangle));
            //AllShapes.RemoveAll(q => q.IsMarkedForDeath);
            //ammo.RemoveAll(q => q.IsMarkedForDeath);

            // flip the back-buffer to the primary surface
            bg.Render();
        }

        //Resizes the client when the window has been changed in size
        private void Form1_Resize(object sender, EventArgs e)
        {
            bg?.Dispose();
            bg = bgc.Allocate(CreateGraphics(), ClientRectangle);

            SpaceShipPoint = new PointF(ClientRectangle.Width / 2, ClientRectangle.Height / 2);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            bg = bgc.Allocate(CreateGraphics(), ClientRectangle);
        }

        //Resets the score, life counter and level back to normal
        public void RestartGame()
        {
            GameScore = 0;

            LifeCounter = 3;

            LevelCounter = 1;

            LevelUp();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //clear the back - buffer
            bg.Graphics.Clear(Color.MistyRose);



            bg.Render();
        }

        //When the player completes a level then add more asteroids according to the level the player is on
        //and bring the spaceship to the middle of the screen again
        public void LevelUp()
        {
            //Gets rid of all the previous asteroids and spaceship and makes a new one
            AllShapes.Clear();

            SpaceShipPoint = new PointF(ClientRectangle.Width / 2, ClientRectangle.Height / 2);

            AllShapes.Add(new SpaceShip(SpaceShipPoint));

            while (AllShapes.Count < (5 * LevelCounter))
            {
                AllShapes.Add(new Asteroids(new PointF((float)rnd.NextDouble() * ClientRectangle.Width, (float)rnd.NextDouble() * ClientRectangle.Height), AsteroidState.Large, 10.0));
            }
        }
    }
}
