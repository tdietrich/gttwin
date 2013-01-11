using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;

namespace gttwin.MainC
{

    /// <summary>
    /// Klasa przedstawiająca gracza
    /// 
    /// autor: Tomasz Dietrich
    /// </summary>
    public class Player : IPlayer
    {
        /// <summary>
        /// Konstruktor Playera
        /// </summary>
        /// <param name="login">Login playera</param>
        /// <param name="password"> hasło </param>
        /// <param name="_unlockedLevels">Lista odblokowanych Poziomów</param>
        public Player(string login, string password, List<int> _unlockedLevels)
        {
            this.login = login;
            this.pass = password;
            this.UnlockedLevels = new List<int>();
            this.UnlockedLevels = _unlockedLevels;
        }

        /// <summary>
        /// Funkcja Dodaje punkty dla Playera
        /// </summary>
        /// <param name="numof">Ile Punktów dodać</param>
        public void AddPoints(int numof)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Funkcja Dodaje 1 wygraną do playera.
        /// </summary>
        public void AddWin()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Odblokuj następny poziom.
        /// </summary>
        /// <param name="currentLevelNum">Jaki jest numer teraz wygranego/granego poziomu</param>
        public void UnlockNextLevel(int currentLevelNum)
        {
            // Jeżeli nie zostal unlockowany level nastepny juz, to odblokuj
            // Kiedy koncza sie levele ? moze user ma miec info o wszystkich a nie tylko o odblokowanyc  ?
            if (!IsLevelUnlocked(currentLevelNum + 1))
            {
                UnlockedLevels.Add(currentLevelNum + 1);
            }

        }
        
        /// <summary>
        /// Metoda do spradzania czy jest level w tabloicy
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public bool IsLevelUnlocked(int level)
        {
            foreach (int x in UnlockedLevels)
            {
                if (x.Equals(level))
                    return true;
            }
            return false;

        }

        public string login;
        public string pass;
        public List<int> UnlockedLevels;

    }

}
