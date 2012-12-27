using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace gtt.MainC
{
    /// <summary>
    /// Interfejs Playera
    /// 
    /// autor: Tomasz Dietrich
    /// </summary>
    interface IPlayer
    {

        void AddPoints(int numof);

        void AddTimePlayed(int numof);

        void AddWin();

        
    }
}
