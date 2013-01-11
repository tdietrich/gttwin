using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using gtt;
using gtt.MainC;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using Timer = System.Timers.Timer;

/*
 * TODO:
 *  Kamera&spawnPoint                           [GAMEPLAY]      - [PENDING] - [HIGH]
 *  Statystyki                                  [STATS]         - [PENDING] - [HIGH]
 *  Polaczenie z siecia                         [NET_CONNETION] - [PENDING] - [HIGH]
 *      Done:
 *          - Klasa Wrapper dla funkcji sieciowych - NetControler.cs
 *  
 *  
 *  
 * Eh... Znowu zrobiło się fchuj flag. Trzeba było na początku zrobić maszynę stanów jakąś prostą
 * Machającą stanami... Ale jest jak jest.
 * 
 * 
 * */
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
        /// <param name="game">Standardowy arg, XNa-Game</param>
        /// <param name="levelNum">Numer Poziomu który mamy 'wczytać'/grać</param>
        /// <param name="thisLevelInfo">Informacje o Poziomie</param>
        public GameCC(Game game, Level thisLevelInfo, uint levelNum)
            :base(game)
        {
            platformWidth = thisLevelInfo.PlatformWidth;
            this.TargetHeight = thisLevelInfo.TargetHeight;
            this.levelImPlayingNumber = levelNum;
        }

        # region Fields


        /// <summary>
        /// Świat fizyczny
        /// </summary>
        public World world;

        /// <summary>
        /// Hashmapa/słownik textur
        /// </summary>
        public Dictionary<BLOCKTYPES, Texture2D> ShapesTextures;

        
        /// <summary>
        /// Lista bloków leżących na platformie
        /// </summary>
        protected List<float> yOfBlocksOnPlatform;

        /// <summary>
        /// Trzyma informacyjnie numer levelu w ktory gramy
        /// </summary>
        private uint levelImPlayingNumber;

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

        private SpriteBatch spriteBatch;
        private SpriteBatch hudBatch;
        private SpriteBatch endingBatch;
        /// <summary>
        /// Struktura trzymające settingsy gry
        /// </summary>
        public static GameSettings Settings;

        /// <summary>
        /// Zmienna mówiąca czy blok wylosowany 'leci', czy został postawiony.
        /// </summary>
        protected bool blockOnHisWay;

        /// <summary>
        /// Informacja jaki ksztalt bedzie nastepny
        /// </summary>
        public int nextShape { get; set; }

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

        /// <summary>
        /// Timer zliczający czas gry
        /// </summary>
        public Timer timer { private set; get; }


        /// <summary>
        /// Czas grania w level
        /// </summary>
        private string timeElapsed;

        /// <summary>
        /// Szybkosc bloku
        /// </summary>
        static public float BlockSpeed = 0.6f;

        /// <summary>
        /// Osiągnięto wysokosc linii
        /// </summary>
        private bool lineReached;

        /// <summary>
        /// Flaga informujaca czy koniec gry - czy spadlo cos na podloge.
        /// </summary>
        private bool failFlag;

        /// <summary>
        /// Flaga informujaca ze nastapil koniec gry poprzez osiagniecie wysokosci
        /// </summary>
        private bool winFlag;

        /// <summary>
        /// Flaga informujaca ze w tryb pauzy weszlismy
        /// </summary>
        private bool pauseFlag;

        /// <summary>
        /// Flaga = true jeśli, wieza osiagnela wymaganą wysokość i zaczelo sie odliczanie sprawdzające czy klocki nie pospadały
        /// inaczej false.
        /// </summary>
        private bool countDownStarted;

        /// <summary>
        /// Timer zliczajacy czas ustabilizowania sie wiezy - po ktorym jezeli nie spadnie zaden klocek to wygrana
        /// </summary>
        private TimerGtt CountDownTimer;

        /// <summary>
        /// Ciało które osiągneło wysokość porządaną
        /// </summary>
        private Body WinningBody;

        /// <summary>
        /// Pozycja w momencie złapania linii przez cialo wygrywajace
        /// </summary>
        private Vector2 WinningBodyPos;

        /// <summary>
        /// Flaga informuajca nas ze przegrana z powodu niestabilnosci wiezy
        /// </summary>
        private bool notStableFlag;
        #endregion

        # region Level Specific vars
        /// <summary>
        /// Wysyokosc do osiagniecia w levelu w jednostkach symulacj
        /// </summary>
        private float TargetHeight;
        protected int floorHeight;
        protected int platformHeight;
        protected float platformWidth;
        private bool afterWinProcedureDone;



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
            GameInputManager.AddAction("RepeatLevel");
            GameInputManager.AddAction("Pause");
            GameInputManager.AddAction("CCW");
            GameInputManager.AddAction("CW");
            GameInputManager.AddAction("Left");
            GameInputManager.AddAction("Right");


            GameInputManager["RepeatLevel"].Add(Keys.Enter);
            GameInputManager["BackToMenu"].Add(Keys.Escape);
            GameInputManager["Pause"].Add(Keys.P);
            GameInputManager["CCW"].Add(Keys.Up);
            GameInputManager["CW"].Add(Keys.Down);
            GameInputManager["Left"].Add(Keys.Left);
            GameInputManager["Right"].Add(Keys.Right);

            // Wysokosci podlogi i platformy
            floorHeight = 15;
            platformHeight = 40;
            highestBodyPosition = 1000;

            // Ustawienie flag zwyciestwa lub przegranej
            failFlag                = false;
            lineReached             = false;
            winFlag                 = false;
            countDownStarted        = false;
            notStableFlag           = false;
            afterWinProcedureDone   = false;
            pauseFlag               = false;
            // Utworzenie timera ktory bedzie zliczał w doł w przypadku osiagniecia wysokosci,
            // Czas odliczania to 3 - arg. sec TimeSpan'a
            CountDownTimer = new TimerGtt(TimerGttModes.COUNTDOWN, new TimeSpan(0, 0, 0, 3, 0));

            // Ustawienie defaultowych danych, w calej grze dane są z tej zmiennej brane
            Settings = new GameSettings(OptionsHandler.blocksFriction, OptionsHandler.blocksBounciness, 0.12f, new Vector2(0.0f, 1.0f),
                                        new Vector2(this.GraphicsDevice.Viewport.Width / 2, 40));

            // Standardowy "Efekt" do rysowania prymitywów w XNA
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter
               (0, GraphicsDevice.Viewport.Width,     // left, right
                GraphicsDevice.Viewport.Height, 0,    // bottom, top
                0, 1);                                         // near, far plane


            // Stworzenie batchy do rysowania
            spriteBatch = new SpriteBatch(GraphicsDevice);
            hudBatch = new SpriteBatch(GraphicsDevice);
            endingBatch = new SpriteBatch(GraphicsDevice);


            // Stworzenie kontenera na linie levelu
            LevelLines = new List<LevelLine>();

            // Dodanie linii levelu.
            LevelLines.Add(new LevelLine(TargetHeight, GraphicsDevice));


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
            Vector2 posi = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height - floorHeight/2);
            _floor.Position = ConvertUnits.ToSimUnits(posi);
            _floor.BodyType = BodyType.Static;
            _floor.IsStatic = true;
            _floor.Restitution = 0.1f;
            _floor.Friction = 2.5f;
            _floor.OnCollision += new OnCollisionEventHandler(CheckFloorCollision);
            // Tworzenie platformy
            _platform = BodyFactory.CreateRectangle(world,
                                                ConvertUnits.ToSimUnits(platformWidth),
                                                ConvertUnits.ToSimUnits(platformHeight),
                                                10f);
            //_platform.Position = ConvertUnits.ToSimUnits(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height - 100);
            Vector2 posi2 = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height - platformHeight/2 - floorHeight);
            _platform.Position = ConvertUnits.ToSimUnits(posi2);
            _platform.BodyType = BodyType.Static;
            _platform.IsStatic = true;
            _platform.Restitution = 0.1f;
            _platform.Friction = 5.0f;

            // Żaden blok nie spada
            blockOnHisWay = false;


            // lista bloków leżących na platformie
            yOfBlocksOnPlatform = new List<float>();

            timer = new Timer(1000);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timerCallback);
            timer.Start();
        }


        /// <summary>
        /// Odpalane w momencie gdy cokolwiek spadnie na podloge - wywoluje sekwencje zdarzeń, zakonczenia levelu
        /// poprzez przegraną zmieniając flagę 
        /// <code>
        ///     failFlag = true;
        /// </code>
        /// </summary>
        /// <param name="fixtureA"></param>
        /// <param name="fixtureB"></param>
        /// <param name="contact"></param>
        /// <returns></returns>
        bool CheckFloorCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            failFlag = true;
            return true;
        }

        /// <summary>
        /// Callback timera liczacego czas gry
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timerCallback(object sender, System.Timers.ElapsedEventArgs e)
        {
            timeElapsed = e.SignalTime.Second.ToString();
        }


        /// <summary>
        /// Load content
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();


            // ładowanie textur kształtów do słownika
            ShapesTextures = new Dictionary<BLOCKTYPES, Texture2D>();

            ShapesTextures.Add(BLOCKTYPES.I_SHAPE, Game.Content.Load<Texture2D>("shapes/ishape"));
            ShapesTextures.Add(BLOCKTYPES.J_SHAPE, Game.Content.Load<Texture2D>("shapes/jshape"));
            ShapesTextures.Add(BLOCKTYPES.L_SHAPE, Game.Content.Load<Texture2D>("shapes/lshape"));
            ShapesTextures.Add(BLOCKTYPES.O_SHAPE, Game.Content.Load<Texture2D>("shapes/oshape"));
            ShapesTextures.Add(BLOCKTYPES.S_SHAPE, Game.Content.Load<Texture2D>("shapes/sshape"));
            ShapesTextures.Add(BLOCKTYPES.T_SHAPE, Game.Content.Load<Texture2D>("shapes/tshape"));
            ShapesTextures.Add(BLOCKTYPES.Z_SHAPE, Game.Content.Load<Texture2D>("shapes/zshape"));


            // TODO: use this.content to load your game content here
            // Tymczasowe textury, syf i mogiła, na razie nie używane, ale nie wywalać.
           //tex = contentManager.Load<Texture2D>("ApplicationIcon");
            tex2 = Game.Content.Load<Texture2D>("asdawdas");
            tex3 = Game.Content.Load<Texture2D>("floor");

            // ladowanie fontu z assetow
            hudFont = Game.Content.Load<SpriteFont>("font");
            //OurBlock = new Block(BLOCKTYPES.Z_SHAPE, ref world, tex);
           _floorS = new Sprite(tex3);
           _platformS = new Sprite(tex2);
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
                                       //Color.White, rectangles.Rotation,
                                       //rectangleSprite.Origin + offset, 1f, SpriteEffects.None, 0f);

            // Rysuj Bloki
            //DrawBlocks();

            // Rysuj linie poziomów
            DrawLevelLines();

            /*
             * RYSOWANIE
             */
           
            var projection = Matrix.CreateOrthographicOffCenter(0f,
                ConvertUnits.ToSimUnits(GraphicsDevice.Viewport.Width),
                ConvertUnits.ToSimUnits(GraphicsDevice.Viewport.Height), 0f, 0f,
                1f);

            Vector3 trans = new Vector3(0, 0 , 0);

            // Wartosc 1000 jest poczatkowa zaporowa, dla 1 wejscia w warunek.  Gra jej nigdy nie osiagnie dlatego jest bezpieczna.
            if (highestBodyPosition > 1000)
            {
                // Tutaj bedzie przesuwanie ekranu do góry - vector3
                trans = new Vector3(0, highestBodyPosition * 0.05f, 0);
            }


            var view = Matrix.CreateTranslation(trans) * projection;


            debugView.RenderDebugData(ref view);

            
            // Draws user hud
            DrawHud();


            spriteBatch.End();

        }


        /// <summary>
        /// Przygotowana funkcja do rysowania interfejsu uzytkownika
        /// </summary>
        private void DrawHud()
        {
            if (pauseFlag)
            {
                hudBatch.Begin();
                hudBatch.DrawString(hudFont, "PAUZA\nWcisnij p aby powrocic do gry", new Vector2(150, 180), Color.Green);
                hudBatch.Draw(ShapesTextures[(BLOCKTYPES)1], new Rectangle(190, 50, 350, 400), Color.Black);
                hudBatch.End();
            }
            else if (!winFlag && !failFlag && !lineReached && !countDownStarted)
            {
                BLOCKTYPES komunikat = (BLOCKTYPES)nextShape;
                string pos = "";
                double position = 0.0;

                
                if (CurrentBlock != null)
                {
                    pos = ConvertUnits.ToDisplayUnits(CurrentBlock.myBody.Position).ToString();
                   // position = GraphicsDevice.Viewport.Height - platformHeight - floorHeight - 10 - ConvertUnits.ToDisplayUnits(CurrentBlock.myBody.Position).Y;
                    position = CurrentBlock.myBody.Position.Y;

                }
                
                hudBatch.Begin();
                // Rysowanie tekstu
                hudBatch.DrawString(hudFont, "Next Shape: " , new Vector2(510, 25), Color.Black);
                if(nextShape != 0)
                    hudBatch.Draw(ShapesTextures[komunikat], new Rectangle(720, 20, (int)(ShapesTextures[komunikat].Width*0.8), (int)(ShapesTextures[komunikat].Height*0.8)),Color.White);
                hudBatch.DrawString(hudFont, "Level: " + levelImPlayingNumber.ToString(), new Vector2(320, 10), Color.Black);
                hudBatch.DrawString(hudFont, "LineAt: " + LevelLines[0].height.ToString(), new Vector2(10, 40), Color.Black);
                hudBatch.DrawString(hudFont, "LineAtSim: " + LevelLines[0].heightForDisplay.ToString(), new Vector2(10, 70), Color.Black);
                hudBatch.DrawString(hudFont, "PWidth: " + platformWidth.ToString(), new Vector2(10, 100), Color.Black);
                //hudBatch.DrawString(hudFont, "pos: " + position.ToString(), new Vector2(10, 120), Color.Black);
                hudBatch.DrawString(hudFont, "highestY: " + highestBodyPosition.ToString(), new Vector2(10, 150), Color.Black);
                // Zamykanie rysowania duszków w danej klatce
                hudBatch.End();
            }
               
            else if (failFlag || winFlag)
            {
                if (failFlag)
                    DrawEndLevelInfo(false);
                if (winFlag)
                    DrawEndLevelInfo(true);

            }
            else if (countDownStarted)
            {
                hudBatch.Begin();
               // Math.Abs(WinningBody.Position.X - WinningBodyPos.X) > 0.15f || Math.Abs(WinningBody.Position.Y - WinningBodyPos.Y) > 0.15f
                //hudBatch.DrawString(hudFont, "zmienne" + WinningBody.Position.ToString(), new Vector2(150, 100), Color.Black);
                //hudBatch.DrawString(hudFont, "stale" + WinningBodyPos.ToString(), new Vector2(150, 100), Color.Black);
                hudBatch.DrawString(hudFont, "Wysokosc osiagnieta\n Obliczanie Stabilnosci...\n" + CountDownTimer.ToString(), new Vector2(150, 180), Color.Green);
                hudBatch.End();
                
            }


        }

        /// <summary>
        /// Rysuje linie pokazujące wysokosci wiezy do osiągnięcia
        /// </summary>
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

            // PAUZA WŁĄCZONA - FREEZE THE GAME, nie odswiezamy silnika fizycznego
            if (pauseFlag)
            {

                // zresetuj czas zeby nie bylo zadnych bugow silnika fizycznego
                this.Game.ResetElapsedTime();
            }
            else
            {
                // Klocek nie spadl na ziemie, gra się toczy
                if (!failFlag && !winFlag && !countDownStarted)
                {
                    // Jeżeli poprzedni blok został usytuowany
                    if (!blockOnHisWay)
                    {
                        // Losowanie randomowego typu bloku
                        var random = new Random();

                        int type = 0;

                        // Losowanie kształtu teraz i następnego
                        if (nextShape == 0)
                        {
                            var firstType = random.Next(1, 7);
                            var next = random.Next(1, 7);

                            nextShape = next;
                            type = firstType;
                        }
                        else
                        {
                            type = nextShape;
                            var next = random.Next(1, 7);

                            nextShape = next;
                        }


                        // Losowanie randomowego obrotu klocka, od 0 do 360
                        float rot = (float)random.Next(0, 360);



                        // Stworzenie bloku = jednoznacze z dodaniem go do świata
                        CurrentBlock = new Block(GraphicsDevice, ref world, ShapesTextures[(BLOCKTYPES)type], (BLOCKTYPES)type, rot);

                        // Dodanie handlera odpalanego przy kolizji
                        CurrentBlock.myBody.OnCollision += new OnCollisionEventHandler(myBody_OnCollision);
                        CurrentBlock.myBody.IgnoreGravity = true;
                        CurrentBlock.myBody.LinearVelocity = new Vector2(0f, GameCC.BlockSpeed);

                        blockOnHisWay = true;


                    }


                    // Aktualizacja fizyki

                    world.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f, (1f / 3f)));



                    // Sprawdzanie kolizji spadającego klocka
                    UpdateLevelData();

                    // Zmiana pozycji kamery ze względu na wysokość wieży
                    // UpdateHighestBodyPos();



                }

                // Jeżeli osiągnięto wysokosc, zacznij odliczanie
                else if (countDownStarted)
                {
                    // Update timera
                    CountDownTimer.Update(gameTime);

                    // Jeżeli update osiagnal 0, to znacyz ze wygrana
                    if (CountDownTimer.countEnded)
                    {
                        if (!failFlag)
                            winFlag = true;
                        countDownStarted = false;
                    }
                    // Jeżeli nie osiagnal zera, to aktualizuj fizyke, jeżeli klocek jakiś spadnie, to kolizja z podloga wywola flage fail
                    else
                    {
                        world.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f, (1f / 3f)));

                        // Jeżeli zmienilo cialo swoje polozenie gruboa
                        if (Math.Abs(WinningBody.Position.X - WinningBodyPos.X) > 0.15f || Math.Abs(WinningBody.Position.Y - WinningBodyPos.Y) > 0.15f)
                        {
                            failFlag = true;
                            notStableFlag = true;
                            countDownStarted = false;
                        }
                        // OSTATECZNA WYGRANA - AKCJE ZWIAZANE Z AKTUALIZACJA GRY PO WYGRANEj
                        else
                        {
                            AfterWinProcedure();
                        }
                    }

                }



            }

            // Update input niezaleznie od tego jaki stan
            UpdateInput();
          
        }


        /// <summary>
        /// Procedura wywoływana 1 raz po skonczeniu danego levelu
        /// </summary>
        private void AfterWinProcedure()
        {
            if (!afterWinProcedureDone)
            {
                Game1.player.UnlockNextLevel((int)levelImPlayingNumber);
                afterWinProcedureDone = true;
            }
        }
        
        /// <summary>
        /// Funkcja wypisujaca informacje o koncu gry
        /// </summary>
        /// <param name="ifWin">bool czy gra zakonczona poprzez wygrana, czy przegrana
        /// true - wygrana
        /// false - przegrana</param>
        private void DrawEndLevelInfo(bool ifWin)
        {
            endingBatch.Begin();

            switch (ifWin)
            {
                case true:
                    endingBatch.DrawString(hudFont, "Gratulacje! Osiagnales wysokosc!", new Vector2(150,150), Color.Green);
                    endingBatch.DrawString(hudFont, "Przeszedles poziom " + levelImPlayingNumber.ToString(), new Vector2(150, 180), Color.White);
                    endingBatch.DrawString(hudFont, "Przejdz do ekranu wyboru poziomu - Enter\nPowrot do menu - ESC", new Vector2(150, 200), Color.White);
                    break;


                case false:
                    if (notStableFlag)
                    {
                        endingBatch.DrawString(hudFont, "Przegrana! Wieza niestabilna", new Vector2(150, 150), Color.Red);
                    }
                    else
                    {
                        endingBatch.DrawString(hudFont, "Przegrana! Klocek na ziemi", new Vector2(150, 150), Color.Red);
                    }
                    endingBatch.DrawString(hudFont, "Gra ponowna - Enter\nPowrot do menu - ESC", new Vector2(150, 180), Color.White);
                    break;
            }
            endingBatch.End();

        }

        /// <summary>
        /// Event handler przy kolizji klocka
        /// </summary>
        /// <param name="fixtureA"></param>
        /// <param name="fixtureB"></param>
        /// <param name="contact"></param>
        /// <returns></returns>
        bool myBody_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            // Blok doleciał do jakiegos innego - oznacz flagę że doleciał, oraz usuń ten event handler, aby nie
            // Powtarzało się w nieskonczoność.
            for (float x = -_platform.Position.X; x <= _platform.Position.X * 2; x = x + 0.05f)
            {
                List<Fixture> a = world.TestPointAll(new Vector2(x, (float)TargetHeight));
                if (a.Count != 0)
                {
                    if (a[0].Body.Equals(fixtureA.Body))
                    {
                        //lineReached = true;
                        countDownStarted = true;
                        WinningBody = a[0].Body;
                        WinningBodyPos = new Vector2(WinningBody.Position.X, WinningBody.Position.Y);
                    }
                }
            }

            blockOnHisWay = false;
            
            // Dodanei klocka jako lezacego na platformie
            yOfBlocksOnPlatform.Add(CurrentBlock.myBody.Position.Y);

            // Update pozycji njwyzszego klocka
            UpdateHighestBodyPos();

            fixtureA.Body.IgnoreGravity = false;
            fixtureA.Body.LinearVelocity = Vector2.Zero;
            fixtureA.Body.OnCollision -= myBody_OnCollision;
            return true;
        }

        /// <summary>
        /// Funkcja aktualizująca Input
        /// </summary>
        private void UpdateInput()
        {
            GameInputManager.Update();

            var przes = new Vector2(1,0);

            // Flaga pauzy, tylko stad mozemy wyjsc z gry 
            if (pauseFlag)
            {
                if (GameInputManager["BackToMenu"].IsTapped)
                {
                    Game.Components.Add(new MainMenuComponent(Game));
                    Game.Components.Remove((IGameComponent)this);
                }
            }

            // Gra się toczy
            if (!failFlag && !winFlag)
            {
                // Nie mozna zapauzowac jak juz odliczamy
                if (!countDownStarted)
                {
                    if (GameInputManager["Pause"].IsTapped)
                    {
                        pauseFlag = !pauseFlag;
                    }
                }

                if (GameInputManager["CCW"].IsDown)
                {
                    CurrentBlock.myBody.Rotation -= 0.04f;

                }

                if (GameInputManager["CW"].IsDown)
                {
                    CurrentBlock.myBody.Rotation += 0.04f;

                }

                if (GameInputManager["Left"].IsDown)
                {

                    CurrentBlock.myBody.Position -= 0.01f * przes;
                }

                if (GameInputManager["Right"].IsDown)
                {
                    CurrentBlock.myBody.Position += 0.01f * przes;
                }
            }
            // Przegrana
            else if(failFlag)
            {
                if (GameInputManager["RepeatLevel"].IsTapped)
                {
                    ResetLevelProcedure();
                }

                if (GameInputManager["BackToMenu"].IsTapped)
                {
                    Game.Components.Add(new MainMenuComponent(Game));

                    Game.Components.Remove((IGameComponent)this);

                }

            }
             // Wygrana
            else if (winFlag)
            {
                if (GameInputManager["RepeatLevel"].IsTapped)
                {
                    Game.Components.Add(new LevelChooserComponent(Game));

                    Game.Components.Remove((IGameComponent)this);
                }

                if (GameInputManager["BackToMenu"].IsTapped)
                {
                    Game.Components.Add(new MainMenuComponent(Game));

                    Game.Components.Remove((IGameComponent)this);

                }

            }
            //Tu dodać elsa

        }


        /// <summary>
        /// Procedura resetujaca swiat fizyczny i wszystkie danye, oraz zaczynajaca gre od nowa
        /// </summary>
        private void ResetLevelProcedure()
        {
            // reset flag zwyciestwa
            failFlag = false;
            winFlag = false;
            afterWinProcedureDone = false;
            highestBodyPosition = 1000;
            // wyzerowanie obecnie spadajacego bloku - referencji, ona i tak bedzie null, po ponizszej isntrukcji
            // ale dla pewnosci
            CurrentBlock = null;

            // zresetuj czas zeby nie bylo zadnych bugow silnika fizycznego
            this.Game.ResetElapsedTime();
            
            world.Clear();

            // Tworzenie podlogi od nowa bo cos sie chrzanilo
            _floor = BodyFactory.CreateRectangle(world,
                                                ConvertUnits.ToSimUnits(GraphicsDevice.Viewport.Width),
                                                ConvertUnits.ToSimUnits(floorHeight),
                                                10f);
            //_floor.Position = ConvertUnits.ToSimUnits(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height - 50);
            Vector2 posi = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height - floorHeight / 2);
            _floor.Position = ConvertUnits.ToSimUnits(posi);
            _floor.BodyType = BodyType.Static;
            _floor.IsStatic = true;
            _floor.Restitution = 0.1f;
            _floor.Friction = 2.5f;
            _floor.OnCollision += new OnCollisionEventHandler(CheckFloorCollision);
            // Tworzenie platformy
            _platform = BodyFactory.CreateRectangle(world,
                                                ConvertUnits.ToSimUnits(platformWidth),
                                                ConvertUnits.ToSimUnits(platformHeight),
                                                10f);
            //_platform.Position = ConvertUnits.ToSimUnits(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height - 100);
            Vector2 posi2 = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height - platformHeight / 2 - floorHeight);
            _platform.Position = ConvertUnits.ToSimUnits(posi2);
            _platform.BodyType = BodyType.Static;
            _platform.IsStatic = true;
            _platform.Restitution = 0.1f;
            _platform.Friction = 5.0f;


            // usuniecie z listy
            //blocksOnPlatform.RemoveRange(0, blocksOnPlatform.Count);

            // Zmienna umozliwiajaca pszczenie nastepnego bloku
            blockOnHisWay = false;

            
            timer.Start();
        }

        /// <summary>
        /// Ustawienie pozycji najwyzszego klocka
        /// </summary>
        private void UpdateHighestBodyPos()
        {
                // Jeżeli obecny klocek
                foreach(float b in yOfBlocksOnPlatform)
                {
                    if (b < highestBodyPosition)
                    {
                        highestBodyPosition = b;
                    }

                }

        }

        /// <summary>
        /// Sprawdzanie wieży - czy osiągneła linię levelu, czy sie przewrocila
        /// </summary>
        private void UpdateLevelData()
        {
            // Pozycja srodka ciala
            Vector2 blockPositionInDisplay = new Vector2();

            if (CurrentBlock != null)
                blockPositionInDisplay = ConvertUnits.ToDisplayUnits(CurrentBlock.myBody.Position);



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

        #region pola Hudu
        private SpriteFont hudFont;





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
