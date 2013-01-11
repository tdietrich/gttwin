using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gttwin.MainC
{
    /// <summary>
    /// Klasa przedstawiajaca pojedynczy Poziom Gry
    /// </summary>
    class Level
    {
        /// <summary>
        /// Standardowy konstruktor, nie ustawia nic. Zalecane używanie Konstruktora przeładowanego, z danymi.
        /// </summary>
        public Level()
        {

        }

        /// <summary>
        /// Konstruktor levelu
        /// </summary>
        /// <param name="targetHeight">Wysokosc platformy - wysokosc > 80 powinna byc raczej</param>
        /// <param name="platformWidth">szerokosc platformy</param>
        /// <param name="locked">Zablokowany czy odblokowany dla usera</param>
        public Level(float targetHeight, float platformWidth, bool locked = true)
        {
            this.TargetHeight = targetHeight;
            this.PlatformWidth = platformWidth;
            this.isLocked = locked;

        }


        /// <summary>
        /// Informuje o tym czy level jest zablokowany czy odblokowany dla usera
        /// </summary>
        /// <returns></returns>
        public bool IsLocked()
        {
            return isLocked;
        }
        
        /// <summary>
        /// Wysokosc w jednostkach symulacji(?) po których wygrywa się grę.
        /// </summary>
        public float TargetHeight;


        /// <summary>
        /// Szerokosc platformy
        /// </summary>
        public float PlatformWidth;

        /// <summary>
        /// Zmienna mowiaca o tym czy level jest zablokowany dla playera
        /// </summary>
        private bool isLocked;

    }
}
