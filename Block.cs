﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BreakoutLucasA
{
    /// <summary>
    /// Klass som hanterar blocken som genereras in i spelet
    /// </summary>
    class Block : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        GraphicsDevice graphics;
        Texture2D block1;

        //variabler för blocken
        int bredd;
        int höjd;
        int posX;
        int posY;
        Rectangle hitbox;

        public Block(Game game, GraphicsDevice graphics, SpriteBatch spriteBatch, int bredd, int höjd, int posX, int posY) : base(game)
        {
            this.spriteBatch = spriteBatch;
            this.graphics = graphics;
            this.höjd = höjd;
            this.bredd = bredd;
            this.posX = posX;
            this.posY = posY;
            
            block1 = new Texture2D(graphics, 1, 1); // skapar block1 texture
            block1.SetData(new Color[] { Color.White });
            hitbox = new Rectangle(posX, posY, bredd, höjd); // skapar hitboxen för ett block
        }
        
        public Rectangle getbrickHitbox() { return hitbox; }

        // flyttar rutorna utanför skärmen (gör att blocken försvinner)
        public void flyttablock() { this.posX = 3000; hitbox = new Rectangle(posX, posY, bredd, höjd); }


        public void Draw(SpriteBatch spriteBatch)
        {
           
            spriteBatch.Draw(block1, hitbox, Color.Purple); // ritar ut lila block
          
        }
    }
}
