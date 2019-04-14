using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BreakoutLucasA
{
    // klass för blocken
    class Block : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        GraphicsDevice graphics;
        Texture2D block1;

        int bredd;
        int höjd;
        int posX;
        int posY;


        public Block(Game game, GraphicsDevice graphics, SpriteBatch spriteBatch, int bredd, int höjd, int posX, int posY) : base(game)
        {
            this.spriteBatch = spriteBatch;
            this.graphics = graphics;
            this.höjd = höjd;
            this.bredd = bredd;
            this.posX = posX;
            this.posY = posY;

            block1 = new Texture2D(graphics, 1, 1);
            block1.SetData(new Color[] { Color.White });        
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(block1, new Rectangle(posX, posY, bredd, höjd), Color.Purple);
            spriteBatch.End();
        }
    }
}
