using System;
using System.Net;
namespace gtt.MainC
{
    /// <summary>
    /// Klasa trzymająca opcje
    /// 
    /// autor: Tomasz Dietrich
    /// </summary>
    public static class OptionsHandler
    {
        /// <summary>
        /// Sprężystość Bloków
        /// </summary>
        public static float blocksBounciness = 0.01f;

        /// <summary>
        /// Tarcie Bloków
        /// </summary>
        public static float blocksFriction = 3.0f;

        /// <summary>
        /// Ustaw Defaultowe Wartośći tarcia i "sprężystości"
        /// </summary>
        /// <param name="friction">Tarcie do ustawienia</param>
        /// <param name="bounc">Sprężystość</param>
        public static void SetDefault(float friction, float bounc)
        {
            blocksBounciness = bounc;
            blocksFriction = friction;
        }
    }
}
