using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace gttwin.MainC
{
    /// <summary>
    /// Interfejs Playera
    /// 
    /// autor: Tomasz Dietrich
    /// </summary>
    interface IPlayer
    {

        void AddPoints(int numof);

        void AddWin();

        void UnlockNextLevel(int currentLevelNum);

        
    }
}
