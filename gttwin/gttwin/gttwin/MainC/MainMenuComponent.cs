using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace gttwin.MainC
{
    /// <summary>
    /// Komponent głównego menu, dziedzczy po glownej klasie XNA rysowalnego komponentu,
    /// w tej chwili tylkpo tekst i mozliwosc przejscia do gry
    /// </summary>
    class MainMenuComponent : DrawableGameComponent
    {
        public MainMenuComponent(Game game)
            : base(game)
        {

            /*
             * Dodawanie informacji o inpucie do managera
             * */
            MenuInputManager = new InputManager();
            MenuInputManager.AddAction("Play");
            MenuInputManager["Play"].Add(Keys.Enter);


        }


        # region methods

        /// <summary>
        /// Odpalane przed wyrysowaniem czegokolwiek
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }


        /// <summary>
        /// Ładowanie graficznych kontentów i ogolnie wszstkich
        /// </summary>
        protected override void LoadContent()
        {
            

            spriteBatch = new SpriteBatch(this.GraphicsDevice);
            // ladowanie fontu z assetow
            contentFont = Game.Content.Load<SpriteFont>("font");

            komunikat = "Wcisnij ENTER aby przejsc do gry!";

            Vector2 wymiarKom = contentFont.MeasureString(komunikat);
            wspNaSrodek = new Vector2((GraphicsDevice.Viewport.TitleSafeArea.Width - wymiarKom.X) / 2, (GraphicsDevice.Viewport.TitleSafeArea.Height - wymiarKom.Y) / 2);
            base.LoadContent();
        }
        
        /// <summary>
        /// Rysowanie
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {

            base.Draw(gameTime);

            
            spriteBatch.Begin();
            // Rysowanie tekstu
            spriteBatch.DrawString(contentFont, komunikat, wspNaSrodek, Color.Black);
            // Zamykanie rysowania duszków w danej klatce
            spriteBatch.End();


            
        }


        /// <summary>
        /// Update logiki
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            //Sprawdzanie inputa

            MenuInputManager.Update();

            if (MenuInputManager["Play"].IsDown)
            {
                // Dodanie komponentu gry
                Game.Components.Add(new GameCC(Game));

                // Usuniecie komponentu MainMenu
                Game.Components.Remove((IGameComponent)this);
            }
            
        }

        #endregion
        private InputManager MenuInputManager;
        private Vector2 wspNaSrodek;
        private string komunikat;
        protected SpriteBatch spriteBatch;
        protected SpriteFont headerFont;
        protected SpriteFont contentFont;
        protected ContentManager contentManagerRef;


    }
}
