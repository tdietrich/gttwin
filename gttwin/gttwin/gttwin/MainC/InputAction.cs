using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace gttwin.MainC
{
    /**
     * Source: http://engineeringdotnet.blogspot.com/2010/05/input-management-in-xna.html
     * 
     * */
    public class InputAction
    {
        String name;
        List<Keys> keyList = new List<Keys>();
        List<Buttons> buttonList = new List<Buttons>();
        InputManager parent = null;
        bool currentStatus = false;
        bool previousStatus = false;

        public bool IsDown { get { return currentStatus; } }
        public bool IsTapped { get { return (currentStatus) && (!previousStatus); } }
        public String Name { get { return name; } }

        public InputAction(InputManager p, string n)
        {
            parent = p;
            name = n;
        }

        public void Add(Keys key)
        {
            if (!keyList.Contains(key))
                keyList.Add(key);
        }

        public void Add(Buttons button)
        {
            if (!buttonList.Contains(button))
                buttonList.Add(button);
        }

        internal void Update(KeyboardState kbState, GamePadState gpState)
        {
            previousStatus = currentStatus;
            currentStatus = false;
            foreach (Keys k in keyList)
                if (kbState.IsKeyDown(k))
                    currentStatus = true;
            foreach (Buttons b in buttonList)
                if (gpState.IsButtonDown(b))
                    currentStatus = true;
        }
    }
}
