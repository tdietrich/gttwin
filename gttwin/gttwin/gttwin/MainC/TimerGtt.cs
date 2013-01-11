using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace gttwin.MainC
{
    /// <summary>
    /// Klasa przedstawiająca timer. działający w trybach <see cref="TimerGttModes"/>.
    /// 
    /// 
    /// Autor: Tomasz Dietrich
    /// </summary>
    class TimerGtt
    {
        /// <summary>
        /// Konstruktor Timera. Korzysta z elemetnów TimeSpan. Działa w 2ch opcjach patrz - <see cref="TimerGttModes"/>
        /// </summary>
        /// <param name="mode"> Tryb pracy timera</param>
        /// <param name="time"> Czas, w zaleznosci od trybu oznacza czas początkowy odliczania w dół, lub czas do którego należy doliczać, jeżeli = -1, timer liczy w nieskonczonosc</param>
        /// <param name="infinity"> Jezeli timer ma liczyc w nieskononosc set to true, dotyczy tylko modu JUST_COUNT</param>
        public TimerGtt(TimerGttModes mode, TimeSpan time, bool infinity=false)
        {
            this.mode = mode;
            this.time = time;
            countEnded = false;
            infinityMode = infinity;
            currentTime = new TimeSpan();

            
            if (mode == TimerGttModes.COUNTDOWN)
                currentTime = time;
            
        }

        # region Methods

        public void Update(GameTime gameTime)
        {
            switch (mode)
            {
                case TimerGttModes.COUNTDOWN:

                    if (!countEnded)
                    {
                        // Jeżeli zmniejszenie nastepne doprowadzi do ponizej zera
                        if ((currentTime - gameTime.ElapsedGameTime) < Zero)
                        {
                            currentTime = Zero;
                            countEnded = true;
                        }
                        else
                            currentTime -= gameTime.ElapsedGameTime;
                    }
                    break;

                case TimerGttModes.JUST_COUNT:

                    // Po prostu licz w nieskonczonosc
                    if (infinityMode)
                        currentTime += gameTime.ElapsedGameTime;
                    else
                    {
                        if (!countEnded)
                        {
                            // Jeżeli zmniejszenie nastepne doprowadzi do ponizej zera
                            if ((currentTime + gameTime.ElapsedGameTime) > time)
                            {
                                currentTime = time;
                                countEnded = true;
                            }
                            else
                                currentTime += gameTime.ElapsedGameTime;

                        }


                    }

                    break;
                

            }
        }


        /// <summary>
        /// Zwraca Czas ktory teraz "jest" na timerze
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return currentTime.ToString();
        }

        #endregion

        # region Fields
        private bool infinityMode;
        public bool countEnded;
        private TimeSpan Zero = new TimeSpan(0,0,0,0);
        private TimeSpan currentTime;
        private TimeSpan time;
        private TimerGttModes mode;
        # endregion Fields
    }

    /// <summary>
    /// Opcje działania Timera
    /// </summary>
    enum TimerGttModes
    {
        COUNTDOWN = 1,
        JUST_COUNT = 2,
    }
}
