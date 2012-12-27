using System;
using System.Net;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;

namespace gtt.MainC
{

    /// <summary>
    /// Klasa przedstawiająca linię - wysokosc do osiągnięcia - w którym zaliczony jest tak jakby level
    /// Na razie brak widocznej linii na ekranie
    /// 
    /// autor: Tomasz Dietrich
    /// </summary>
    public class LevelLine
    {
        /// <summary>
        /// Wysokosc danej linii. Należy ją zmieniać w miare jak idziemy do góry, i wywolywac rysowanie.
        /// Co kazdy draw gry. Wtedy linia się bedzie rysowac w takiej wysokosci jak ta.
        /// </summary>
        public int height;

        /// <summary>
        /// Wierzchołki
        /// </summary>
        public VertexPositionColor[] vertices { private set; get; }

        /// <summary>
        /// Tekst informujacy o wysokosci
        /// </summary>
        //private TextBlock heightText;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public LevelLine(int _height, GraphicsDevice graphic)
        {
            if (_height <= 100)
                throw new Exception("Ustawianie wysokosci ponizej 100 jest bez sensu, ");

            // Przypisania, tworzenie obiektow
            height = _height;
           // heightText = new TextBlock();
            // Wypelnienie linii textem
           // heightText.Text = height.ToString() + "m";

            var realHeight = graphic.Viewport.Height - _height;

            vertices = new VertexPositionColor[2];
            vertices[0].Position = new Vector3(0, realHeight, 0);
            vertices[0].Color = Color.White;
            vertices[1].Position = new Vector3(graphic.Viewport.Width, realHeight, 0);
            vertices[1].Color = Color.White;
        }

    }
}
