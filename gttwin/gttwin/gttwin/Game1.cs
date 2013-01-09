using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using gttwin.MainC;
using System.Runtime.Serialization;
using System.IO;
using System.Text;

namespace gttwin
{
    /// <summary>
    /// This is the main type for your game
    /// 
    /// TODO:
    ///     timer w grze, 
    ///     hash w txt pliku sprawdzaj¹cy,
    ///     sprawdzanie czy doszlismy juz do wysokosci dla levelu granicznej - odpalenie timera wtedy, przecezkanie czy nie opadnie
    ///     po³¹czenie z baz¹ w momenciejak lokalnie dane wciagniemy, laczenie z baz¹ - czy s¹ prawdziwe
    ///     register Component
    /// 
    /// 
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 650;
            graphics.PreferredBackBufferWidth = 800;

            graphics.ApplyChanges();

            Content.RootDirectory = "Content";

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            buffer = new byte[256];
            string login;
            string haslo;
            int[] unlocked;

            // Sprawdzenie czy istnieja dane playera
            if (File.Exists(playerDataFileName))
            {

                StreamReader sr = new StreamReader(playerDataFileName, Encoding.UTF8);
                dataFromFile = sr.ReadToEnd();
               


                string[] dataSplited = dataFromFile.Split(';');
                login = dataSplited[0];
                haslo = dataSplited[1];
                unlocked = new int[dataSplited.Length - 2];
                int count = 0;

                for (int x = 2; x < dataSplited.Length; x++)
                {
                    unlocked[count++] = int.Parse(dataSplited[x]);

                }


                player = new Player(login, haslo,unlocked);

                // Dodanie komponentu g³ownego menu jako pierszego ktory widzi player
                // Dodanei go PO initialize wywala buga, nie jest inicjalizowany komponent wtedy!!!
                Components.Add(new MainMenuComponent(this));

            }
            // dane nie istniej¹
            else
            {
                Components.Add(new RegisterComponent(this));

            }


            // TODO: Add your initialization logic here

            base.Initialize();


            //OurGame.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);


 
            //OurGame.LoadOurContent();
            
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

           

            base.Update(gameTime);
            // TODO: Add your update logic here
            //OurGame.Update(gameTime);
        }


        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameCC OurGame;
        public static Player player;
        public static string playerDataFileName = "data.txt";
        private byte[] buffer;
        private string dataFromFile;


    }
}
