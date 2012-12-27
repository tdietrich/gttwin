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
        public static float blocksBounciness = 0.01f;
        public static float blocksFriction = 3.0f;

        public static void SetDefault(float friction, float bounc)
        {
            blocksBounciness = bounc;
            blocksFriction = friction;
        }
    }
}
