using System;
using System.Net;

namespace gttwin.MainC
{

    /// <summary>
    /// Klasa przedstawiająca gracza
    /// 
    /// autor: Tomasz Dietrich
    /// </summary>
    public class Player : IPlayer
    {

        public Player(string login, string password,int[] _unlockedLevels)
        {
            this.login = login;
            this.pass = password;
            this.UnlockedLevels = new int[_unlockedLevels.Length];
            _unlockedLevels.CopyTo(this.UnlockedLevels, 0);
        }

        public void AddPoints(int numof)
        {
            throw new NotImplementedException();
        }

        public void AddTimePlayed(int numof)
        {
            throw new NotImplementedException();
        }

        public void AddWin()
        {
            throw new NotImplementedException();
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
        public int[] UnlockedLevels;

    }

}
