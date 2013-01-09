using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Ruminate.GUI.Content;
using Ruminate.GUI.Framework;
using System.IO;
using Ruminate.GUI.Framework;

namespace gttwin.MainC
{

        /// <summary>
        /// Komponent głównego menu, dziedzczy po glownej klasie XNA rysowalnego komponentu,
        /// w tej chwili tylkpo tekst i mozliwosc przejscia do gry
        /// </summary>
        class RegisterComponent : DrawableGameComponent
        {
            public RegisterComponent(Game game)
                : base(game)
            {

                //var skin ;//= new Skin(game.GreyImageMap, game.GreyMap);
                //var text ;//= new Text(game.GreySpriteFont, Color.LightGray);
/*
               Gui _gui = new Gui(game, skin, text)
                {
                    Widgets = new Widget[] {
                    new ScrollBars {
                        Children = new Widget[] {
                            new Panel(10, 10, 1000, 1000) {
                                Children = new Widget[] {
                                    new ScrollBars {
                                        Children = new Widget[] {
                                            new Button(10, 10, "Test 1"),
                                            new Button(10, 50, "Test 2"),
                                            new Button(10, 90, "Test 3"),
                                            new Button(10, 130, "Test 4"),
                                            new Button(10, 170, "Test 5"),
                                            new Button(10, 210, "Test 6"),
                                            new Button(10, 250, "Test 7"),
                                            new Button(10, 290, "Test 8"),
                                            new Button(10, 330, "Test 9"),
                                            new Button(10, 370, "Test 10"),
                                            new Button(10, 410, "Test 11"),
                                            new Button(10, 450, "Test 12"),
                                            new Button(10, 490, "Test 13"),
                                            new Button(10, 530, "Test 14"),
                                            new Button(10, 570, "Test 15"),
                                            new Button(10, 610, "Test 16"),
                                            new Panel(100, 10, 200, 200) {
                                                Children = new Widget[] {
                                                    new Button(10, 10, "Test 1"),
                                                    new Button(10, 50, "Test 2"),
                                                    new Button(10, 90, "Test 3")      
                                                }
                                            },
                                            new Panel(100, 230, 400, 400) {
                                                Children = new Widget[] {
                                                    new ScrollBars {
                                                        Children = new Widget[] {
                                                            new Button(10, 10, "Test 1"),
                                                            new Button(10, 50, "Test 2"),
                                                            new Button(10, 90, "Test 3"),
                                                            new Button(10, 130, "Test 4"),
                                                            new Button(10, 170, "Test 5"),
                                                            new Button(10, 210, "Test 6"),
                                                            new Button(10, 250, "Test 7"),
                                                            new Button(10, 290, "Test 8"),
                                                            new Button(10, 330, "Test 9"),
                                                            new Button(10, 370, "Test 10"),
                                                            new Button(10, 410, "Test 11"),
                                                            new Button(10, 450, "Test 12"),
                                                            new Button(10, 490, "Test 13"),
                                                            new Button(10, 530, "Test 14"),
                                                            new Button(10, 570, "Test 15"),
                                                            new Button(10, 610, "Test 16"),
                                                            new Panel(100, 10, 600, 600) {
                                                                Children = new Widget[] { 
                                                                    new ScrollBars {
                                                                        Children = new Widget[] {
                                                                            new Button(10, 10, "Button"),
                                                                            new ToggleButton(10, 50, "Toggle Button"),
                                                                            new Panel(10, 90, 120, 120),
                                                                            new CheckBox(10, 215, "Check Box"),
                                                                            new RadioButton(10, 255, "GRP", "Radio 1"),
                                                                            new RadioButton(10, 285, "GRP", "Radio 2"),
                                                                            new RadioButton(10, 315, "GRP", "Radio 3"),
                                                                            new Label(10, 340, "Research"),
                                                                            new Label(10, 365, _beaker, "Research"),
                                                                            new Panel(140, 90, 220, 220) {
                                                                                Children = new Widget[] {
                                                                                    new TextBox(2, 600)
                                                                                }
                                                                            },
                                                                            new Panel(370, 70, 220, 220) {
                                                                                Children = new Widget[] {
                                                                                    new ScrollBars {
                                                                                        Children = new Widget[] {
                                                                                            new CheckBox(10, 10, "Button"),
                                                                                            new CheckBox(210, 10, "Button"),
                                                                                            new CheckBox(10, 210, "Button"),
                                                                                            new CheckBox(210, 210, "Button"),
                                                                                            new Panel(10, 230, 300, 300) {
                                                                                                Children = new Widget[] {
                                                                                                    new TextBox(2, 300)
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }           
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                }; */

            }


            # region methods

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
                MenuInputManager["Play"].Add(Keys.Enter);

               
                base.Initialize();
            }


            /// <summary>
            /// Ładowanie graficznych kontentów i ogolnie wszstkich
            /// </summary>
            protected override void LoadContent()
            {
                
                GreyImageMap = Game.Content.Load<Texture2D>("ImageMap");
                GreyMap = File.OpenText(Game.Content.RootDirectory + @"\Map.txt").ReadToEnd();
                GreySpriteFont = Game.Content.Load<SpriteFont>("font");


                var skin = new Skin(GreyImageMap, GreyMap);
                //var text = new Text(GreySpriteFont, Color.LightGray);

                TextRenderer text = new Ruminate.GUI.Framework.TextRenderer(GreySpriteFont, Color.White);
                PanelHeader = new Ruminate.GUI.Framework.Text(GreySpriteFont, Color.White);

                TextBox login = new TextBox(16, 16);
                TextBox haslo = new TextBox(8, 8);

                myGui = new Gui(this.Game, skin, text);
                myGui.AddWidget(login);

     


                spriteBatch = new SpriteBatch(this.GraphicsDevice);
                // ladowanie fontu z assetow
                //contentFont = Game.Content.Load<SpriteFont>("font");

                komunikat = "Zarejestruj swoje konto, aby grac";

                Vector2 wymiarKom = GreySpriteFont.MeasureString(komunikat);
                wspNaSrodek = new Vector2((GraphicsDevice.Viewport.TitleSafeArea.Width - wymiarKom.X) / 2, (GraphicsDevice.Viewport.TitleSafeArea.Height - wymiarKom.Y) / 2);
                
                base.LoadContent();
            }

            /// <summary>
            /// Rysowanie
            /// </summary>
            /// <param name="gameTime"></param>
            public override void Draw(GameTime gameTime)
            {

                GraphicsDevice.Clear(Color.Blue);
                


                spriteBatch.Begin();
                // Rysowanie tekstu
                spriteBatch.DrawString(GreySpriteFont, komunikat, wspNaSrodek, Color.White);
                // Zamykanie rysowania duszków w danej klatce
                PanelHeader.Render(spriteBatch, "Zarejestruj sie, aby grac", new Rectangle(50, 50, 150, 25));

                spriteBatch.End();

                myGui.Draw();


                base.Draw(gameTime);


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
                    //Game.Components.Add(new LevelChooserComponent(Game));

                    // Usuniecie komponentu MainMenu
                    //Game.Components.Remove((IGameComponent)this);
                }
                myGui.Update();


            }

            #endregion

            public Text PanelHeader;
            public SpriteFont GreySpriteFont;
            public Texture2D GreyImageMap;
            public string GreyMap;
            protected Gui myGui;
            private InputManager MenuInputManager;
            private Vector2 wspNaSrodek;
            private string komunikat;
            protected SpriteBatch spriteBatch;
            protected SpriteFont headerFont;
            protected SpriteFont contentFont;


        }
    }