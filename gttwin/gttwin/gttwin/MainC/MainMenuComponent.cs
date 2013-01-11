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



        }


        # region Methods

        /// <summary>
        /// Odpalane przed wyrysowaniem czegokolwiek
        /// </summary>
        public override void Initialize()
        {
            /*
             * Dodawanie informacji o inpucie do managera
             * */
            MenuInputManager = new InputManager();
            MenuInputManager.AddAction("Play");
            MenuInputManager.AddAction("Help");
            MenuInputManager.AddAction("Highscores");

            MenuInputManager.AddAction("Close");
            MenuInputManager["Play"].Add(Keys.Enter);
            MenuInputManager["Close"].Add(Keys.Q);
            MenuInputManager["Help"].Add(Keys.W);
            MenuInputManager["Highscores"].Add(Keys.E);

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

            komunikat = "[Enter] - Graj";

           // Vector2 wymiarKom = contentFont.MeasureString(komunikat);
           // wspNaSrodek = new Vector2((GraphicsDevice.Viewport.TitleSafeArea.Width - wymiarKom.X) / 2, (GraphicsDevice.Viewport.TitleSafeArea.Height - wymiarKom.Y) / 2);
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
            spriteBatch.DrawString(contentFont, komunikat, new Vector2(120,150), Color.Black);
            spriteBatch.DrawString(contentFont, "[w] - Help", new Vector2(120, 170), Color.Black); 
            spriteBatch.DrawString(contentFont, "[e] - Highscores", new Vector2(120, 190), Color.Black);
            spriteBatch.DrawString(contentFont, "[q] - Wyjdz z gry", new Vector2(120, 210), Color.Black);
            spriteBatch.DrawString(contentFont, "Gracz: " + Game1.player.login, new Vector2(10, 50), Color.Yellow);
            spriteBatch.DrawString(contentFont, "Leveli odblokowanych: " + Game1.player.UnlockedLevels.Count.ToString(), new Vector2(30, 80), Color.Yellow);
            // Zamykanie rysowania duszków w danej klatce
            spriteBatch.End();

            //NetControler.SaveScore("admin@admin.com", 10000);
            
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

            if (MenuInputManager["Play"].IsTapped)
            {
                // Dodanie komponentu wyboru ekranu
                //Game.Components.Add(new GameCC(Game));


                // Dodanie komponentu wyboru leveli
                Game.Components.Add(new LevelChooserComponent(Game));
                // Usuniecie komponentu MainMenu
                Game.Components.Remove((IGameComponent)this);

                //Game.Components.Add(new RegisterComponent(Game));

                
            }
            if (MenuInputManager["Close"].IsTapped)
            {
                // TUTAJ JEST EVENT ON EXIT MOZNABY COS ZAPISAC MOZE ??!?!?!?!?
                this.Game.Exit();
            }

            if (MenuInputManager["Help"].IsTapped)
            {
                Game.Components.Add(new HelpView(Game));
                Game.Components.Remove((IGameComponent)this);
            }

            if (MenuInputManager["Highscores"].IsTapped)
            {
                Game.Components.Add(new HighscoreView(Game));
                Game.Components.Remove((IGameComponent)this);
            }

            
        }

        #endregion

        # region Fields
        private InputManager MenuInputManager;
        private Vector2 wspNaSrodek;
        private string komunikat;
        protected SpriteBatch spriteBatch;
        protected SpriteFont headerFont;
        protected SpriteFont contentFont;
        protected ContentManager contentManagerRef;
        # endregion

    }
}
