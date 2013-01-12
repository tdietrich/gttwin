using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace gttwin.MainC
{
    /// <summary>
    /// 
    /// Autor: Tomasz Dietrich i Michał Czwarnowski
    /// </summary>
    class HelpView : DrawableGameComponent
    {

        public HelpView(Game game)
            : base(game)
        {

        }

        # region methods

        public override void Initialize()
        {
            MyInputManager = new InputManager();
            MyInputManager.AddAction("Back");

            MyInputManager["Back"].Add(Keys.Escape);

            spriteBatch = new SpriteBatch(Game.GraphicsDevice);


            base.Initialize();
        }

        protected override void LoadContent()
        {
            HeaderFont = Game.Content.Load<SpriteFont>("font");
            titleFont = this.Game.Content.Load<SpriteFont>("titleFont");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            MyInputManager.Update();


            if (MyInputManager["Back"].IsTapped)
            {
                Game.Components.Add(new MainMenuComponent(Game));

                Game.Components.Remove((IGameComponent)this);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CadetBlue);

            spriteBatch.Begin();
                spriteBatch.DrawString(titleFont, "Pomoc", new Vector2(280, 10), Color.White);
                spriteBatch.DrawString(HeaderFont, "Pomoc ma na celu wprowadzic gracza w zasady i sposob", new Vector2(10, 150), Color.Black);
                spriteBatch.DrawString(HeaderFont, "dzialania gry. Ale nie tutaj. Tu trzeba sobie radzic!", new Vector2(10, 180), Color.Black);
            spriteBatch.End();

            base.Draw(gameTime);
        }


#endregion

        # region fields
        private InputManager MyInputManager;
        private SpriteFont HeaderFont;
        private SpriteBatch spriteBatch;
        private Texture2D sratatat;
        private SpriteFont titleFont;
        #endregion


    }
}
