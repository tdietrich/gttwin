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
    public class InputManager
    {
        List<InputAction> actions = new List<InputAction>();

        public void AddAction(String ActionName)
        {
            actions.Add(new InputAction(this, ActionName));
        }

        public InputAction this[String ActionName]
        {
            get
            {
                return actions.Find((InputAction a) => { return a.Name == ActionName; });
            }
        }

        public void Update()
        {
            
            KeyboardState kbState = Keyboard.GetState(Microsoft.Xna.Framework.PlayerIndex.One);
            GamePadState gpState = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);

            foreach (InputAction a in actions)
                a.Update(kbState, gpState);

            
        }

    }
}
