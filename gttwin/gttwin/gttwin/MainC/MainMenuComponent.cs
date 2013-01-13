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
             * 
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

            play = this.Game.Content.Load<Texture2D>("play");
            quit = this.Game.Content.Load<Texture2D>("quit");
            help = this.Game.Content.Load<Texture2D>("help");
            highscores = this.Game.Content.Load<Texture2D>("highscores");
            spriteBatch = new SpriteBatch(this.GraphicsDevice);
            // ladowanie fontu z assetow
            contentFont = Game.Content.Load<SpriteFont>("font");
            titleFont = this.Game.Content.Load<SpriteFont>("titleFont");
            
            komunikat = "Graj!";

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
            spriteBatch.DrawString(contentFont, komunikat, new Vector2(100,270), Color.White);
            spriteBatch.Draw(play, new Rectangle(20, 250, (int)(play.Width * 0.2), (int)(play.Height * 0.2)), Color.White);
            spriteBatch.Draw(help, new Rectangle(20, 320, (int)(play.Width * 0.2), (int)(play.Height * 0.2)), Color.White);
            spriteBatch.Draw(highscores, new Rectangle(20, 390, (int)(play.Width * 0.2), (int)(play.Height * 0.2)), Color.White);
            spriteBatch.Draw(quit, new Rectangle(20, 460, (int)(play.Width * 0.2), (int)(play.Height * 0.2)), Color.White);
            spriteBatch.DrawString(contentFont, "Pomoc", new Vector2(100, 340), Color.White); 
            spriteBatch.DrawString(contentFont, "Najlepsze wyniki", new Vector2(100, 410), Color.White);
            spriteBatch.DrawString(contentFont, "Wyjdz z gry", new Vector2(100, 480), Color.White);
            spriteBatch.DrawString(contentFont, "Gracz: " + Game1.player.login, new Vector2(10, 140), Color.Yellow);
            spriteBatch.DrawString(titleFont, "Gravity Tetris Tower", new Vector2(60, 10), Color.White);
            spriteBatch.DrawString(contentFont, "Leveli odblokowanych: " + Game1.player.UnlockedLevels.Count.ToString(), new Vector2(10, 170), Color.Yellow);
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
        private SpriteFont titleFont;
        protected SpriteFont contentFont;
        protected ContentManager contentManagerRef;
        private Texture2D play;
        private Texture2D quit;
        private Texture2D help;
        private Texture2D highscores;
        # endregion

    }
}
