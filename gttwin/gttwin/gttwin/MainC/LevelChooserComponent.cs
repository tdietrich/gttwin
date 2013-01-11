using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace gttwin.MainC
{

    class LevelChooserComponent : DrawableGameComponent
    {

        /// <summary>
        /// Konstruktor, tu odbywa się między innymi przypisanie akcji inputa.
        /// </summary>
        /// <param name="game"></param>
        public LevelChooserComponent(Game game)
            : base(game)
        {


            // Sprite Batch do rysowania
            spriteBatch = new SpriteBatch(game.GraphicsDevice);

            // Aktywna pozycja w 'menu' wyboru
            activePositionNumber = 1;

            
        }

        #region methods

        /// <summary>
        /// Inicjalizacja
        /// </summary>
        public override void Initialize()
        {
            /*
             * Tutaj odbywa się dodanie poziomów do gry
             * */
            LevelsList = new LinkedList<Level>();
            //LevelsList.AddLast(new Level(3, 200));

            for (float x = 0; x < 10; x++)
            {
                // Wysoksc linii do osiagniecia w danym levelu
                float a = (float)Math.Round((5 - (x*0.1f)),2);

                // Szer platformy
                float b = (float)Math.Round((200 - 5 * x),2);

                
                if (Game1.player.IsLevelUnlocked((int)(x + 1)))
                    LevelsList.AddLast(new Level(a,b,false));
                else
                    LevelsList.AddLast(new Level(a, b));

            }
            

            // Ustawienie currenta na pierwszy lewel
            currentChosenLevel = LevelsList.First;


            MyInputManager = new InputManager();

            // Przesunięcie w lewo
            MyInputManager.AddAction("Left");

            // W prawo
            MyInputManager.AddAction("Right");

            // Wybranie danego levelu
            MyInputManager.AddAction("EnterLevel");

            // Powrót do mainMenu
            MyInputManager.AddAction("Back");

            MyInputManager["Back"].Add(Keys.Escape);
            MyInputManager["EnterLevel"].Add(Keys.Enter);
            MyInputManager["Left"].Add(Keys.Left);
            MyInputManager["Right"].Add(Keys.Right);

            // Ohydny sposób na BUG, który łapie entera z poprzedniego komponentu
            System.Threading.Thread.Sleep(500);

            base.Initialize();
        }

        /// <summary>
        /// Załaduj kontent
        /// </summary>
        protected override void LoadContent()
        {
            // załadowanie czcionki
            font = this.Game.Content.Load<SpriteFont>("levelSpriteFont");
            firstSpritePosition = new Vector2(50, 80);

            base.LoadContent();
        }

        /// <summary>
        /// Zupdatuj sie
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            MyInputManager.Update();

            // Obsługa przyciśnięcia w lewo...

            if (MyInputManager["Left"].IsTapped)
            {
                if (activePositionNumber > 1)
                {

                    if (currentChosenLevel.Previous != null)
                    {
                        // Jeżeli nastepny level nie jest zablokowany
                        if (!currentChosenLevel.Previous.Value.IsLocked())
                        {
                            activePositionNumber -= 1;
                            currentChosenLevel = currentChosenLevel.Previous;
                        }
                    }
                }


            }

            // Obsługa przyciśnięcia w prawo
            if (MyInputManager["Right"].IsTapped)
            {
                if (activePositionNumber <= LevelsList.Count-1)
                {
                    if (currentChosenLevel.Next != null)
                    {
                        if (!currentChosenLevel.Next.Value.IsLocked())
                        {
                            activePositionNumber += 1;
                            currentChosenLevel = currentChosenLevel.Next;
                        }
                    }
                }

            }

            if (MyInputManager["EnterLevel"].IsTapped)
            {
                
                // Dodanie komponentu gry z info o wybranym levelu
                this.Game.Components.Add(new GameCC(this.Game,currentChosenLevel.Value,activePositionNumber));

                
                // Usunięcie tego komponentu
                this.Game.Components.Remove((IGameComponent)this);

            }

            if (MyInputManager["Back"].IsTapped)
            {

                // Dodanie komponentu gry z info o wybranym levelu
                this.Game.Components.Add(new MainMenuComponent(this.Game));


                // Usunięcie tego komponentu
                this.Game.Components.Remove((IGameComponent)this);

            }



            base.Update(gameTime);
        }


        /// <summary>
        /// Rysuj
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            uint x = 0;

            Vector2 pos = new Vector2(50, 80);

            foreach (Level l in LevelsList)
            {
                spriteBatch.Begin();

                string num = (x + 1).ToString();

                // Jeżeli ta pozycja ma byc podswietlana
                if ((x + 1) == activePositionNumber)
                {
                    spriteBatch.DrawString(font, num, pos, Color.White);
                }

                // Jeżeli nie
                else
                {
                    if (LevelsList.ElementAt<Level>((int)x).IsLocked())
                    {
                        spriteBatch.DrawString(font, num, pos, Color.Gray);
                    }
                    else
                        spriteBatch.DrawString(font, num, pos, Color.Black);
                }

                spriteBatch.End();
                x += 1;
                pos.X += 20;

            }
            base.Draw(gameTime);
        }

        #endregion


        #region properities
        private InputManager MyInputManager;
        private LinkedList<Level> LevelsList;
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private Vector2 firstSpritePosition;
        private uint activePositionNumber;
        private LinkedListNode<Level> currentChosenLevel;
        #endregion
    }
}
