﻿using Microsoft.Xna.Framework;
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
        Vector2 paddelpos = new Vector2(900, 660);
        Texture2D boll;
        Vector2 bollpos = new Vector2(945, 300);
        Rectangle bollhitbox, paddelhitbox;
        SpriteFont Text;
        

        // en ny lista med block
        List<Block> Brickor;

        int skärmbredd;
        int skärmhöjd;
        int blockhöjd = 15;
        int blockbredd = 50;
        int blockrader = 12;
        int poäng = 0; // startpoäng
        int underkant = 690; // y koordinat, om bollen rör där så stängs spelet ner

        Vector2 hastighet;

        Random slumppos = new Random(); //random funktion som gör att bollen slumpmässigt åker åt olika håll när den spawnar

        

    
        
        public Breakout()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // ändrar storlek på rutan
            graphics.PreferredBackBufferWidth = 1880;
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

            Text = Content.Load<SpriteFont>("Ubuntu32"); // laddar in text
        }

        public void BlockSkapare()
        {
              // de olika kolumnerna som blocken kommer vara i
            for (int i = 0; i < skärmbredd / blockbredd; i++)
            {
                //// de olika raderna som blocken kommer vara i
                for (int j = 1; j < blockrader + 1; j++)
                {
                    Brickor.Add(new Block(this, GraphicsDevice, spriteBatch, blockbredd, blockhöjd, i * blockbredd + i, j * blockhöjd + j)); // sätter egenskaperna på blocket
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
            // flyttar paddeln i x-led när piltangenterna <- och -> trycks när
            KeyboardState kstate = Keyboard.GetState();
            if (kstate.IsKeyDown(Keys.Right)) 
                paddelpos.X += 8;   
            if (kstate.IsKeyDown(Keys.Left))
                paddelpos.X -= 8;

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

            // gör så att bollen studsar mot väggarna (högersidan)
            if (bollpos.X + boll.Width >= skärmbredd)
                hastighet.X = -hastighet.X;
            

            //skapar hitboxarna till bollen och paddeln
            bollhitbox = new Rectangle((int)bollpos.X, (int) bollpos.Y, 22, 22);
            paddelhitbox = new Rectangle((int)paddelpos.X, (int) paddelpos.Y , 114, 26);

            // ökar hastigheten när spelaren får ett visst antal poäng
            if (poäng > 2000)
            {   
                if (hastighet.Y == 3)
                {
                    hastighet.Y = 5;
                }
            }

            // kollisionen, skickar tillbaka bollen i motsatt riktning
            if (paddelhitbox.Intersects(bollhitbox))
            {
                 hastighet.Y = -hastighet.Y;
            }

            // avlutar spelet om bollen hamnar under paddeln
            if(bollpos.Y > underkant)
            {
                Exit();
            }       
            
 
            bollpos.X = bollpos.X + hastighet.X;
            bollpos.Y = bollpos.Y + hastighet.Y;

            for (int i = 0; i < Brickor.Count; i++)
            {
                if (bollhitbox.Intersects(Brickor[i].getbrickHitbox())) // om bollen nuddar en bricka så vänder den samt blocket försvinner, samt spelaren får 100 poäng
                {
                    Brickor[i].flyttablock();
                    hastighet.Y *= -1;
                    poäng += 100;
                }
            }
     

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
            spriteBatch.DrawString(Text, "Score: "+poäng.ToString(), new Vector2(100, 600), Color.White); // la till poäng (texten och psoition)
         
            // varje bricka ritas ut med hjälp av draw funktionen i Block klassen, samt att position uppdateras här (för att spara plats)
            foreach (var item in Brickor)
            {
                item.Draw(spriteBatch);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
