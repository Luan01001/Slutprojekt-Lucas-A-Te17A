using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BreakoutLucasA;
using System.Collections.Generic;
using System;

namespace BreakoutLucasA
{
    
    public class Breakout : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D bakgrund;
        Vector2 bakgrundpos = new Vector2(0, 0);
        Texture2D paddel;
        Vector2 paddelpos = new Vector2(440, 660);
        Texture2D boll;
        Vector2 bollpos = new Vector2(400, 200);
        Rectangle bollhitbox, paddelhitbox;

        // en ny lista med block
        List<Block> Brickor;

        int skärmbredd;
        int skärmhöjd;
        int blockhöjd = 30;
        int blockbredd = 100;
        int blockrader = 6;
        

        Random slumppos = new Random();

        Vector2 hastighet;



        public Breakout()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // ändrar storlek på rutan
            graphics.PreferredBackBufferWidth = 1080;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();
        }
              
               
       
        protected override void Initialize()
        {
           

            base.Initialize();
        }

     
        protected override void LoadContent()
        {
           
            spriteBatch = new SpriteBatch(GraphicsDevice);
            bakgrund = Content.Load<Texture2D>("bakgrund");
            paddel = Content.Load<Texture2D>("paddel");
            boll = Content.Load<Texture2D>("boll");

            skärmbredd = GraphicsDevice.Viewport.Width;
            skärmhöjd = GraphicsDevice.Viewport.Height;

            hastighet.X = 4;
            hastighet.Y = 4;

            Brickor = new List<Block>();

            SlumpLoad();
            BlockSkapare();
           
        }

        public void BlockSkapare()
        {
              // de olika kolumnerna som blocken kommer vara i
            for (int i = 0; i < skärmbredd / blockbredd; i++)
            {
                //// de olika raderna som blocken kommer vara i
                for (int j = 1; j < blockrader + 1; j++)
                {
                    Brickor.Add(new Block(this, GraphicsDevice, spriteBatch, blockbredd, blockhöjd, i * blockbredd + i, j * blockhöjd + j));
                }
            }

            foreach (var Block in Brickor)
            {
                Components.Add(Block);
            }
        }
       
        protected override void UnloadContent()
        {
           
        }

       
        protected override void Update(GameTime gameTime)
        {
            KeyboardState kstate = Keyboard.GetState();
            if (kstate.IsKeyDown(Keys.Right)) 
                paddelpos.X += 4;   
            if (kstate.IsKeyDown(Keys.Left))
                paddelpos.X -= 4;

            // hindrar paddeln att åka utanför fönstret
            if (paddelpos.X > Window.ClientBounds.Width - paddel.Width)
                paddelpos.X = Window.ClientBounds.Width - paddel.Width;
            if (paddelpos.X < 0)
                paddelpos.X = 0;

            

            // gör så att bollen studsar mot väggarna (vänstersida och ovansidan)
            if (bollpos.X <= 0)
                hastighet.X = -hastighet.X;
            if (bollpos.Y <= 0)
                hastighet.Y = -hastighet.Y;

            // gör så att bollen studsar mot väggarna (högersidan och undersidan)
            // undersidan är tillfällig, det gör det dock lättare för att testa spelet
            if (bollpos.X + boll.Width >= skärmbredd)
                hastighet.X = -hastighet.X;
            if (bollpos.Y + boll.Height >= skärmhöjd)
                hastighet.Y = -hastighet.Y;

            //skapar hitboxarna till bollen och paddeln
            bollhitbox = new Rectangle((int)bollpos.X, (int) bollpos.Y, 26, 26);
            paddelhitbox = new Rectangle((int)paddelpos.X, (int) paddelpos.Y , 155, 26);

            // kollisionen, skickar tillbaka bollen i motsatt riktning
            if (paddelhitbox.Intersects(bollhitbox))
            {
                 hastighet.Y = -hastighet.Y;
            }
            

            bollpos.X = bollpos.X + hastighet.X;
            bollpos.Y = bollpos.Y + hastighet.Y;

            base.Update(gameTime);
        }

        void SlumpLoad()
        {
            // gör att bollen genereras åt olika håll
            int random = slumppos.Next(0, 2);
            //riktningar:
            // sydöst
            if(random == 0)
            {
                hastighet.X = 3;
                hastighet.Y = 3;
            }
            // sydväst
            if (random == 1)
            {
                hastighet.X = -3;
                hastighet.Y = 3;
            }
           
        }

       
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Purple);
            spriteBatch.Begin();
            spriteBatch.Draw(bakgrund, bakgrundpos, Color.White);
            spriteBatch.Draw(paddel, paddelpos, Color.White);
            spriteBatch.Draw(boll, bollpos, Color.White);
            spriteBatch.End();
         

            base.Draw(gameTime);
        }
    }
}
