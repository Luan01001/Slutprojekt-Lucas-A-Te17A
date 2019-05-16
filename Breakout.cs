using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BreakoutLucasA {

    /// <summary>
    /// Viktigaste klassen
    /// </summary>
    public class Breakout : Game {

        Random r = new Random();

        //Vid vilken tid större plattan ska tas bort
        bool SkaHaStorPaddel = false;
        double Störreplattatills;
        Texture2D paddelStor;
        double TidKvaravStorPaddle;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //bakgrundernas, gameover & endscreen texture
        Texture2D bakgrund;
        Texture2D bakgrund2;
        Texture2D bakgrund3;
        Texture2D bakgrund4;
        Texture2D bakgrund5;
        Texture2D bakgrund6;
        Texture2D gameover;
        Texture2D endscreen;

        Vector2 bakgrundpos = new Vector2(0, 0);
        Texture2D paddel;
        Vector2 paddelpos = new Vector2(900, 660); //paddelns startposition
        Texture2D boll;
        Vector2 bollpos = new Vector2(945, 300); // bollens startposition
        Rectangle bollhitbox, paddelhitbox;
        SpriteFont Text;

        // Poäng och Nivå
        private int _poäng;
        int poängFörNästaNivå = 7400;
        int nivå = 1;
        

        
        int liv = 3;

        private PoängManager _poängManager;


        // en ny lista med block
        List<Block> Brickor;

        //skärmkonfiguration
        int skärmbredd;
        int skärmhöjd;

        //Block
        int blockhöjd = 15;
        int blockbredd = 50;
        int blockrader = 12;

     
        int underkant = 690; // y koordinat, om bollen rör där så stängs spelet ner

        Vector2 hastighet;

        Random slumppos = new Random(); //random funktion som gör att bollen slumpmässigt åker åt olika håll när den spawnar





        public Breakout() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // ändrar storlek på spelfönstret
            graphics.PreferredBackBufferWidth = 1880;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();
        }



        protected override void Initialize() {


            base.Initialize();
        }


        protected override void LoadContent() {

            spriteBatch = new SpriteBatch(GraphicsDevice);
            bakgrund = Content.Load<Texture2D>("bakgrund");
            bakgrund2 = Content.Load<Texture2D>("bakgrund2");
            bakgrund3 = Content.Load<Texture2D>("bakgrund3");
            bakgrund4 = Content.Load<Texture2D>("bakgrund4");
            bakgrund5 = Content.Load<Texture2D>("bakgrund5");
            bakgrund6 = Content.Load<Texture2D>("bakgrund6");
            gameover = Content.Load<Texture2D>("gameover");
            endscreen = Content.Load<Texture2D>("endscreen"); // slutbilden som visas när spelaren har förstört alla block

            paddel = Content.Load<Texture2D>("paddel"); //paddeln som spelaren styr
            boll = Content.Load<Texture2D>("boll"); // bollen 
            paddelStor = Content.Load<Texture2D>("paddelStor"); // bonuseffekten (stor paddel)

            // sätter skärmens höjd och bredd i en variabel så att det blir lättare att använda för att bland annat hindra paddeln från att åka utanför spelfönstret
            skärmbredd = GraphicsDevice.Viewport.Width;
            skärmhöjd = GraphicsDevice.Viewport.Height;


            Brickor = new List<Block>(); //lista med block

            SlumpLoad();
            BlockSkapare();

            _poängManager = PoängManager.Load();

            Text = Content.Load<SpriteFont>("Ubuntu32"); // laddar in text
        }

        public void BlockSkapare() {
            // de olika kolumnerna som blocken kommer vara i
            for (int i = 0; i < skärmbredd / blockbredd; i++) {
                //// de olika raderna som blocken kommer vara i
                for (int j = 1; j < blockrader + 1; j++) {
                    Brickor.Add(new Block(this, GraphicsDevice, spriteBatch, blockbredd, blockhöjd, i * blockbredd + i, j * blockhöjd + j)); // sätter egenskaperna för blocket
                }
            }

            foreach (var Block in Brickor) {
                Components.Add(Block);
            }
        }

        protected override void UnloadContent() {

        }


        protected override void Update(GameTime gameTime) {
            // flyttar paddeln i x-led när piltangenterna <- och -> trycks ner
            KeyboardState kstate = Keyboard.GetState();
            if (kstate.IsKeyDown(Keys.Right))
                paddelpos.X += 8;
            if (kstate.IsKeyDown(Keys.Left))
                paddelpos.X -= 8;

            // hindrar paddeln att åka utanför fönstret
            if (paddelpos.X > Window.ClientBounds.Width - paddel.Width && !SkaHaStorPaddel)
                paddelpos.X = Window.ClientBounds.Width - paddel.Width;
            if (paddelpos.X < 0)
                paddelpos.X = 0;

            // förhindrar den stora paddeln att åka utanför fönstret
            if (paddelpos.X > 1700 && SkaHaStorPaddel)
            {
                paddelpos.X = 1700;
            }


            // gör så att bollen studsar mot väggarna (vänstersida och ovansidan)
            if (bollpos.X <= 0)
                hastighet.X = -hastighet.X;
            if (bollpos.Y <= 0)
                hastighet.Y = -hastighet.Y;

            // gör så att bollen studsar mot väggarna (högersidan)
            if (bollpos.X + boll.Width >= skärmbredd)
                hastighet.X = -hastighet.X;


            //skapar hitboxarna till bollen och paddeln
            bollhitbox = new Rectangle((int)bollpos.X, (int)bollpos.Y, 22, 22);

            //ändrar hitboxen på paddlen beroende på om spelaren har fått "powerup" stor paddle
            if (SkaHaStorPaddel) {
                paddelhitbox = new Rectangle((int)paddelpos.X, (int)paddelpos.Y, 180, 26);
            } else {
                paddelhitbox = new Rectangle((int)paddelpos.X, (int)paddelpos.Y, 114, 26);
            }




            // kollisionen, skickar tillbaka bollen i motsatt riktning
            if (paddelhitbox.Intersects(bollhitbox)) {
                hastighet.Y = -hastighet.Y;
            }

            // avlutar spelet om bollen hamnar under paddeln
            if (bollpos.Y > underkant) {
                _poängManager.Add(new BreakoutLucasA.Poäng() {
                    Spelare = "Test",
                    Värde = _poäng,
                }
               );
                PoängManager.Save(_poängManager);

                liv--;

             

                if (liv > 0)
                {
                    // Om spelaren har fler liv än 0 så återställs bollens pos och fart
                    SlumpLoad();

                }
                
            }
             // om spelaren har 0 liv kvar och F trycks ner så avslutas spelet
            if (liv == 0 && kstate.IsKeyDown(Keys.F))
                {
                    Exit();
                }


            bollpos.X = bollpos.X + hastighet.X;
            bollpos.Y = bollpos.Y + hastighet.Y;

            for (int i = 0; i < Brickor.Count; i++) {
                if (bollhitbox.Intersects(Brickor[i].getbrickHitbox())) // om bollen nuddar en bricka så vänder den samt blocket försvinner, samt spelaren får 100 poäng
                {
                    Brickor[i].flyttablock();
                    hastighet.Y *= -1;
                    _poäng += 100;

                    //Ger spelaren en random chans att få en "Powerup" (större paddle)
                    if (r.Next(1, 101) < 10) {
                        Störreplattatills = gameTime.TotalGameTime.TotalSeconds + 10;
                    }
                }
            }

            //Kolla om spelaren ska ha stor paddle.
            if (gameTime.TotalGameTime.TotalSeconds < Störreplattatills) {
                SkaHaStorPaddel = true;
                //Räknar ut tid kvar för att visa till spelaren.
                TidKvaravStorPaddle = Math.Round(Störreplattatills - gameTime.TotalGameTime.TotalSeconds);
            } else {
                SkaHaStorPaddel = false;
                TidKvaravStorPaddle = 0;
            }



            //Har hand om nivå system
            if(_poäng >= poängFörNästaNivå)
            {
                //sätter ytterligate poäng som behövs fär nästa nivå.
                poängFörNästaNivå = _poäng + 7400;               
                nivå++;
                //Öka bollens hastighet för att öka svårighetsgraden för varje nivå. öka beroende på vilket håll den redan åker emot
                if (hastighet.X > 0) { hastighet.X++; } else { hastighet.X--; }
                if (hastighet.Y > 0) { hastighet.Y++; } else { hastighet.Y--; }
            }
              
            

            base.Update(gameTime);
        }

        void SlumpLoad() {
            bollpos = new Vector2(945, 300);
            paddelpos = new Vector2(900, 660);
            // gör att bollen genereras åt olika håll
            int random = slumppos.Next(0, 2);
            //riktningar:
            // sydöst
            if (random == 0) {
                hastighet.X = 3;
                hastighet.Y = 3;
            }
            // sydväst
            if (random == 1) {
                hastighet.X = -3;
                hastighet.Y = 3;
            }

        }


        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Purple);
            spriteBatch.Begin();
            // ritar ut en ny bakgrund för varje ny nivå som uppnås
            spriteBatch.Draw(bakgrund, bakgrundpos, Color.White);
            if (nivå == 2)
            {
                spriteBatch.Draw(bakgrund2, bakgrundpos, Color.White); 
            }
            if (nivå == 3)
            {
                spriteBatch.Draw(bakgrund3, bakgrundpos, Color.White);
            }
            if (nivå == 4)
            {
                spriteBatch.Draw(bakgrund4, bakgrundpos, Color.White);
            }
            if (nivå == 5)
            {
                spriteBatch.Draw(bakgrund5, bakgrundpos, Color.White);
            }
            if (nivå == 6)
            {
                spriteBatch.Draw(bakgrund6, bakgrundpos, Color.White);
            }
            if (liv == 0)
            {
                spriteBatch.Draw(gameover, bakgrundpos, Color.White);
                // flyttar bort paddeln och bollen från skärmen
                paddelpos = new Vector2(3000, 5000);
                bollpos = new Vector2(3000, 400);

                // bollen stannar
                hastighet.X = 0;
                hastighet.Y = 0;
            }

            if (_poäng >= 44400) // när spelaren förstör alla block så klarar spelaren spelet, paddeln och bollen försvinner och stannar.
            {
                spriteBatch.Draw(endscreen, bakgrundpos, Color.White); //ritar ut slutskärmsbilden

                // flyttar bort paddeln och bollen från skärmen
                paddelpos = new Vector2(3000, 5000);
                bollpos = new Vector2(3000, 400);
     
                // bollen stannar
                hastighet.X = 0; 
                hastighet.Y = 0;
            }




            // Rita ut stor respektive liten paddle så att den är lika stor som hitboxen
            if (SkaHaStorPaddel) {
                spriteBatch.Draw(paddelStor, paddelpos, Color.White);
            } else {
                spriteBatch.Draw(paddel, paddelpos, Color.White);
            }


            spriteBatch.Draw(boll, bollpos, Color.White); // ritar ut bollen
            spriteBatch.DrawString(Text, "Score: " + _poäng.ToString(), new Vector2(100, 700), Color.White); // la till poäng (texten och position)
            spriteBatch.DrawString(Text, "Tid kvar av stor paddle: " + TidKvaravStorPaddle, new Vector2(200, 700), Color.White); // skriv ut tid kvar tills stor paddle försvinner
            spriteBatch.DrawString(Text, "Niva: " + nivå, new Vector2(400, 700), Color.White); //Skriver ut nivå
            spriteBatch.DrawString(Text, "Liv: " + liv, new Vector2(500, 700), Color.White); //Skriver ut liv
            spriteBatch.DrawString(Text, "Highscore:\n " + string.Join("\n", _poängManager.Highscore.Select(c => c.Spelare + ": " + c.Värde).ToArray()), new Vector2(100, 400), Color.White); // ritar ut highscore och genom att använda string.Join så bryts raden för varje highscore nivå.

            // varje bricka ritas ut med hjälp av draw funktionen i Block klassen, samt att position uppdateras här (för att spara plats)
            foreach (var item in Brickor) {
                item.Draw(spriteBatch);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
