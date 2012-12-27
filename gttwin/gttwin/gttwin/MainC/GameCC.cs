using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using gtt;
using gtt.MainC;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;


namespace gttwin.MainC
{

    /// <summary>
    /// Klasa głowna gry - odpowiednik GameCC w wersji WP
    /// </summary>
    class GameCC 
        : DrawableGameComponent
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="game"></param>
        public GameCC(Game game)
            :base(game)
        {
            this.gameref = game;

        }

        # region Fields

        private Game gameref;
        /// <summary>
        /// Świat fizyczny
        /// </summary>
        public World world;


        /// <summary>
        /// Lista bloków leżących na platformie
        /// </summary>
        protected List<Block> blocksOnPlatform;

        /// <summary>
        /// Tekstury temporalne
        /// </summary>
        protected Texture2D tex;
        protected Texture2D tex2;
        protected Texture2D tex3;

        /// <summary>
        /// Widok
        /// </summary>
        public DebugViewXNA debugView;

        /// <summary>
        /// Ciało - podłoga
        /// </summary>
        protected Body _floor;

        /// <summary>
        /// Ciało platforma
        /// </summary>
        protected Body _platform;

        /// <summary>
        /// Sprite Podłogi
        /// </summary>
        protected Sprite _floorS;

        /// <summary>
        /// Sprite platformy
        /// </summary>
        protected Sprite _platformS;

        /// <summary>
        /// Licznik do 'wypluwania' klocków
        /// </summary>
        protected float counter;

        private SpriteBatch spriteBatch;

        /// <summary>
        /// Struktura trzymające settingsy gry
        /// </summary>
        public static GameSettings Settings;

        /// <summary>
        /// Zmienna mówiąca czy blok wylosowany 'leci', czy został postawiony.
        /// </summary>
        protected bool blockOnHisWay;

        /// <summary>
        /// Blok który w tym momencie spada.
        /// </summary>
        protected Block CurrentBlock;

        /// <summary>
        /// Linie pokazujące wysokość wieży
        /// </summary>
        public List<LevelLine> LevelLines;

        /// <summary>
        /// Zmienna trzyma y środka ciała położonego najwyzej
        /// </summary>
        public float highestBodyPosition { private set; get; }

        /// <summary>
        /// Efekt basic do rysowania
        /// </summary>
        private BasicEffect basicEffect;


        /// <summary>
        /// manager inputa
        /// </summary>
        private InputManager GameInputManager;


        private Thread InputThread;
        #endregion

        # region Level Specific vars

        protected int floorHeight;
        protected int platformHeight;
        protected int platformWidth;



        #endregion

        /// <summary>
        /// Initialization
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            /**
             * 
             * TUTAJ ODBYWA SIE PRZYPISANIE CONTROLSOW
             * 
             * */

            GameInputManager = new InputManager();
            GameInputManager.AddAction("BackToMenu");
            GameInputManager.AddAction("Pause");
            GameInputManager["BackToMenu"].Add(Keys.Escape);
            GameInputManager["Pause"].Add(Keys.P);

            
            floorHeight = 15;
            platformHeight = 40;
            platformWidth = 250;
            // Ustawienie defaultowych danych, w calej grze dane są z tej zmiennej brane
            Settings = new GameSettings(OptionsHandler.blocksBounciness, OptionsHandler.blocksFriction, 0.12f, new Vector2(0.0f, 1.0f),
                                        new Vector2(this.GraphicsDevice.Viewport.Width / 2, 40));

            // Standardowy "Efekt" do rysowania prymitywów w XNA
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter
               (0, GraphicsDevice.Viewport.Width,     // left, right
                GraphicsDevice.Viewport.Height, 0,    // bottom, top
                0, 1);                                         // near, far plane


            // Get the content manager from the application
            spriteBatch = new SpriteBatch(this.GraphicsDevice);
            LevelLines = new List<LevelLine>();



            // Dodanie linii levelu.
            LevelLines.Add(new LevelLine(250, GraphicsDevice));
            LevelLines.Add(new LevelLine(500, GraphicsDevice));



            // Inicjalizacja świata
            if (world == null)
            {
                world = new World(Settings.gravity);
            }
            else
            {
                world.Clear();
            }

            if (debugView == null)
            {
                debugView = new DebugViewXNA(world);
                debugView.RemoveFlags(FarseerPhysics.DebugViewFlags.Controllers);
                debugView.RemoveFlags(FarseerPhysics.DebugViewFlags.Joint);

                debugView.LoadContent(GraphicsDevice, Game.Content);
            }

            // Tworzenie podlogi
            _floor = BodyFactory.CreateRectangle(world,
                                                ConvertUnits.ToSimUnits(GraphicsDevice.Viewport.Width),
                                                ConvertUnits.ToSimUnits(floorHeight),
                                                10f);
            //_floor.Position = ConvertUnits.ToSimUnits(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height - 50);
            _floor.Position = ConvertUnits.ToSimUnits(GraphicsDevice.Viewport.Width/2, GraphicsDevice.Viewport.Height - (floorHeight-10));
            _floor.BodyType = BodyType.Static;
            _floor.IsStatic = true;
            _floor.Restitution = 0.1f;
            _floor.Friction = 2.5f;

            // Tworzenie platformy
            _platform = BodyFactory.CreateRectangle(world,
                                                ConvertUnits.ToSimUnits(platformWidth),
                                                ConvertUnits.ToSimUnits(platformHeight),
                                                10f);
            //_platform.Position = ConvertUnits.ToSimUnits(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height - 100);
            _platform.Position = ConvertUnits.ToSimUnits(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height - (floorHeight-10) - (platformHeight-12) );
            _platform.BodyType = BodyType.Static;
            _platform.IsStatic = true;
            _platform.Restitution = 0.1f;
            _platform.Friction = 5.0f;

            // Żaden blok nie spada
            blockOnHisWay = false;


            // lista bloków leżących na platformie
            blocksOnPlatform = new List<Block>();

        }

        /// <summary>
        /// Load content
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();



            // TODO: use this.content to load your game content here
            // Tymczasowe textury, syf i mogiła, na razie nie używane, ale nie wywalać.
           //tex = contentManager.Load<Texture2D>("ApplicationIcon");
            tex2 = Game.Content.Load<Texture2D>("asdawdas");
            tex3 = Game.Content.Load<Texture2D>("floor");

           
            //OurBlock = new Block(BLOCKTYPES.Z_SHAPE, ref world, tex);
           _floorS = new Sprite(tex3);
           _platformS = new Sprite(tex2);
        }

        /// <summary>
        /// nakładka na protected powyższy zeby mozna bylo wywolac z zewnatrz...
        /// </summary>
        public void LoadOurContent()
        {
            this.LoadContent();

        }
        /// <summary>
        /// redraw
        /// </summary>
        /// <param name="gameTime"></param>

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            /*spriteBatch.Draw(_floorS.Texture, ConvertUnits.ToDisplayUnits(_floor.Position), null,
                                           Color.White, _floor.Rotation,
                                           _floorS.Origin, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(_platformS.Texture, ConvertUnits.ToDisplayUnits(_platform.Position), null,
                                       Color.White, _platform.Rotation,
                                       _platformS.Origin, 1f, SpriteEffects.None, 0f);*/
            //spriteBatch.Draw(tex, ConvertUnits.ToDisplayUnits(rectangles.Position), null,
            //                           Color.White, rectangles.Rotation,
            //                           rectangleSprite.Origin + offset, 1f, SpriteEffects.None, 0f);
            


            // Rysuj linie poziomów
            DrawLevelLines();

            /*
             * RYSOWANIE
             */


            var projection = Matrix.CreateOrthographicOffCenter(0f,
                ConvertUnits.ToSimUnits(GraphicsDevice.Viewport.Width),
                ConvertUnits.ToSimUnits(GraphicsDevice.Viewport.Height), 0f, 0f,
                1f);

            // Tutaj bedzie przesuwanie ekranu do góry - vector3
            Vector3 trans = new Vector3(0, highestBodyPosition * 0.05f, 0);


            var view = Matrix.CreateTranslation(trans) * projection;


            debugView.RenderDebugData(ref view);

            // Draws user hud
            //DrawHud();

            // Tutaj rysujemy interfejs silverlightowy, ale zamieniony/przerenderowany na 
            // Xna texture.
            //spriteBatch.Draw(elementRenderer.Texture, Vector2.Zero, Color.White);



            spriteBatch.End();

            //OurBlock.Draw(gameTime);
            // Rysowanie odbywa się w klasie GamePage.xaml.cs wlasciwie

        }

        /// <summary>
        /// Przygotowana funkcja do rysowania interfejsu uzytkownika
        /// </summary>
        private void DrawHud()
        {
            
        }

        private void DrawLevelLines()
        {
            basicEffect.CurrentTechnique.Passes[0].Apply();

            // TODO:
            //  - Dać tu foricza, że jeżeli linii jeszcze nie widać to nie rysuj!!!!!!!!!!!!
            foreach (LevelLine line in LevelLines)
            {
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, line.vertices, 0, 1);
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Jeżeli wątek do sprawdzania inputa jest nullem, stwórz go i każ sprawdzić inputa
            if (InputThread == null)
                InputThread = new Thread(UpdateInput);

            // Jeżeli poprzedni blok został usytuowany
            if (!blockOnHisWay)
            {
                // Losowanie randomowego typu bloku
                var random = new Random();
                var type = random.Next(1, 7);

                // Losowanie randomowego obrotu klocka, od 0 do 360
                float rot = (float)random.Next(0, 360);

                // Stworzenie bloku = jednoznacze z dodaniem go do świata
                CurrentBlock = new Block(GraphicsDevice, ref world, tex, (BLOCKTYPES)type, rot);

                CurrentBlock.myBody.IgnoreGravity = true;
                CurrentBlock.myBody.LinearVelocity = new Vector2(0f, 0.6f);
                blocksOnPlatform.Add(CurrentBlock);
                blockOnHisWay = true;
            }

            // Aktualizacja fizyki
            //world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds*0.001f);
            world.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f, (1f / 3f)));


            // Usunięcie Boxów które wypadły za świat - to nie ebdzie wlasciwie potrzebne bo jezeli
            // Coś wypadnie za świat to koniec gry

            foreach (var box in from box in world.BodyList
                                let pos = ConvertUnits.ToDisplayUnits(box.Position)
                                where pos.Y > GraphicsDevice.Viewport.Height
                                select box)
            {
                world.RemoveBody(box);
            }


            // Thread SprawdzaczKolizji = new Thread(UpdateBlockCollision);
            // SprawdzaczKolizji.Start();

            // Sprawdzanie kolizji spadającego klocka
            UpdateBlockCollision();

            // Zmiana pozycji kamery ze względu na wysokość wieży
            UpdateHighestBodyPos();


            UpdateInput();

            //Pętla odczytujaca gesty z ekranu
           /* while (TouchPanel.IsGestureAvailable)
            {
                //odczyt gestu z ekranu
                GestureSample gesture = TouchPanel.ReadGesture();

                //switch sprawdzający który z gestów z Initialize jest wykonywany
                switch (gesture.GestureType)
                {
                    case GestureType.VerticalDrag:
                        if (CurrentBlock != null) CurrentBlock.myBody.Rotation += 0.01f * gesture.Delta.Y;
                        break;
                    case GestureType.HorizontalDrag:
                        if (CurrentBlock != null) CurrentBlock.myBody.Position += 0.01f * gesture.Delta;
                        break;
                }

            }*/
            float frameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        }

        /// <summary>
        /// Delegat dla wątku oddzielnego który sprawdza inputa
        /// </summary>
        private void UpdateInput()
        {
            GameInputManager.Update();


            if (GameInputManager["BackToMenu"].IsTapped)
            {
                Game.Components.Add(new MainMenuComponent(Game));

                Game.Components.Remove((IGameComponent)this);

            }

            if (GameInputManager["Pause"].IsTapped)
            {
                
                
            }

        }

        /// <summary>
        /// Ustawienie pozycji najwyzszego klocka
        /// </summary>
        private void UpdateHighestBodyPos()
        {
            /**
             * 
             * TODO:
             *  Ogarnąć sposób obliczania przesunięcia kamery i przesunięcia punktu respawnu klocków
             *  
             * **/

            foreach (Block b in blocksOnPlatform)
            {

                if (b.myBody.LinearVelocity == Vector2.Zero)
                {
                    if (b.heightChecked == false)
                    {
                        highestBodyPosition += b.myBody.Position.Y;
                        Settings.spawnPoint.Y -= highestBodyPosition * 2;
                        b.heightChecked = true;
                    }
                }
            }


        }

        /// <summary>
        /// Sprawdzanie kolizji spadającego klocka
        /// </summary>
        private void UpdateBlockCollision()
        {
            // Jeżeli nastąpił kontakt spadającego klocka z jakimś
            // Innym ciałem, zmien flagę, że można puścić następny kloc.

            // TODO: Ogarnąć czy to jest dokladny sposob bo chyba nie.
            if (CurrentBlock != null)
            {
                if (CurrentBlock.myBody.ContactList != null)
                {
                    // Zmien flage.
                    blockOnHisWay = false;

                    //blocksOnPlatform.Add(CurrentBlock);
                    pos = CurrentBlock.myBody.Position;
                    rot = CurrentBlock.myBody.Rotation;
                    // CurrentBlock.myBody.LinearVelocity = Vector2.Zero;
                    CurrentBlock = null;
                    // CurrentBlock.myBody.LinearVelocity = Vector2.Zero;
                    // CurrentBlock.myBody.IgnoreGravity = false;
                    // Zapomnij tego klocka jako currenta.
                    //CurrentBlock = null;
                }
            }


        }


        #region FUNKCJE I POLA DO DEBUGINGU

        public float rot;
        public Vector2 pos;

        /// <summary>
        /// Zwraca pozycje ostatniego bloku
        /// </summary>
        /// <returns></returns>
        public string GetPosOfLastBlock()
        {
            return pos.ToString();
        }

        public string GetRotOFLastBlock()
        {
            return rot.ToString();
        }

        /*
         * WolrdCenter trzyma odleglosc od srodka swiata, na podstawie tego mozna by ogarniac kamere
         * jednostki od ok 0 do 6- leży na glebie
         * w miare jak kamera idzie w gore to zaczynamy od -0.1 -0.2 i dalej w minus - swiat zostaje
         *
         * 
         * TO co teraz jest pokazuje nam od 0 - 6 okolo czyli tak jakby pozycje wzgledem 
         * world center bedziemy sie przesuwali
         * spawn    środ      leży
         * |----------o---------|
         * |------------o-------|
         * |-------------o------|
         * .
         * .
         * .
         * .
         * |--------------------| o 
         * 
         * Jakoś policzyć tą kamerę i spawn
         * */
        public string GetSomething()
        {
            if (CurrentBlock != null)
                //return CurrentBlock.myBody.FixtureList[0].Body.Position.Y.ToString();

                return CurrentBlock.myBody.Position.Y.ToString();
            //return CurrentBlock.myBody.WorldCenter.Y.ToString();
            else
                return "null";
        }

        #endregion

    }


    #region Game Setting Struct

    /// <summary>
    /// Struktura trzymająca dane ustawien gry
    /// </summary>
    public struct GameSettings
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="friction">tarcie klocków</param>
        /// <param name="restitution">odbijalnosc klockos</param>
        /// <param name="_blockSize">Rozmair klocka</param>
        /// <param name="_blocksSpawnInterval">Odstęp między spawnowaniem następnych klocków</param>
        /// <param name="_gravity">Grawitacja swiata</param>
        public GameSettings(float friction, float restitution, float _blockSize, Vector2 _gravity,
                            Vector2 _spawnPoint)
        {
            blocksFriction = friction;
            blocksRestitution = restitution;
            blockSize = _blockSize;
            gravity = _gravity;
            spawnPoint = _spawnPoint;
        }



        public float blocksFriction;
        public float blocksRestitution;
        public float blockSize;
        public Vector2 gravity;
        public Vector2 spawnPoint;

    }

    #endregion
}
