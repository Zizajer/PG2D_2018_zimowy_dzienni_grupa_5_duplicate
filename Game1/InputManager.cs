using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    public class InputManager
    {
        public const int MaxInputs = 4;

        public readonly KeyboardState[] CurrentKeyboardStates;
        public readonly KeyboardState[] LastKeyboardStates;

        public InputManager()
        {
            CurrentKeyboardStates = new KeyboardState[MaxInputs];
            LastKeyboardStates = new KeyboardState[MaxInputs];
            CurrentMouseState = new MouseState();
            LastMouseState = new MouseState();
        }

        public MouseState CurrentMouseState
        {
            get;
            private set;
        }

        public MouseState LastMouseState
        {
            get;
            private set;
        }

        /// <summary>
        ///    Reads the latest state of the keyboard.
        /// </summary>
        public void Update()
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                LastKeyboardStates[i] = CurrentKeyboardStates[i];
                CurrentKeyboardStates[i] = Keyboard.GetState();
            }

            LastMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
        }

        public bool IsNewLeftMouseClick(out MouseState mouseState)
        {
            mouseState = CurrentMouseState;
            return (CurrentMouseState.LeftButton == ButtonState.Released && LastMouseState.LeftButton == ButtonState.Pressed);
        }

        public bool IsNewRightMouseClick(out MouseState mouseState)
        {
            mouseState = CurrentMouseState;
            return (CurrentMouseState.RightButton == ButtonState.Released && LastMouseState.RightButton == ButtonState.Pressed);
        }

        public bool IsNewThirdMouseClick(out MouseState mouseState)
        {
            mouseState = CurrentMouseState;
            return (CurrentMouseState.MiddleButton == ButtonState.Pressed && LastMouseState.MiddleButton == ButtonState.Released);
        }

        public bool IsNewMouseScrollUp(out MouseState mouseState)
        {
            mouseState = CurrentMouseState;
            return (CurrentMouseState.ScrollWheelValue > LastMouseState.ScrollWheelValue);
        }

        public bool IsNewMouseScrollDown(out MouseState mouseState)
        {
            mouseState = CurrentMouseState;
            return (CurrentMouseState.ScrollWheelValue < LastMouseState.ScrollWheelValue);
        }

        /// <summary>
        ///    Helper for checking if a key was newly pressed during this update. The
        ///    controllingPlayer parameter specifies which player to read input for.
        ///    If this is null, it will accept input from any player. When a keypress
        ///    is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsNewKeyPress(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                var i = (int)playerIndex;

                return (CurrentKeyboardStates[i].IsKeyDown(key) && LastKeyboardStates[i].IsKeyUp(key));
            }
            else
            {
                // Accept input from any player.
                return (IsNewKeyPress(key, PlayerIndex.One, out playerIndex) || IsNewKeyPress(key, PlayerIndex.Two, out playerIndex)
                         || IsNewKeyPress(key, PlayerIndex.Three, out playerIndex) || IsNewKeyPress(key, PlayerIndex.Four, out playerIndex));
            }
        }

        /// <summary>
        ///    Helper for checking if a button was newly pressed during this update.
        ///    The controllingPlayer parameter specifies which player to read input for.
        ///    If this is null, it will accept input from any player. When a button press
        ///    is detected, the output playerIndex reports which player pressed it.
        /// </summary>

        public bool IsKeyPressed(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                var i = (int)playerIndex;

                return (CurrentKeyboardStates[i].IsKeyDown(key));
            }
            else
            {
                // Accept input from any player.
                return (IsKeyPressed(key, PlayerIndex.One, out playerIndex) || IsKeyPressed(key, PlayerIndex.Two, out playerIndex)
                         || IsKeyPressed(key, PlayerIndex.Three, out playerIndex) || IsKeyPressed(key, PlayerIndex.Four, out playerIndex));
            }
        }


        public bool IsExitGame(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex);
        }

        public bool IsLeft(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.A, controllingPlayer, out playerIndex);
        }

        public bool IsRight(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.D, controllingPlayer, out playerIndex);
        }

        public bool IsUp(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.W, controllingPlayer, out playerIndex);
        }
        public bool IsDown(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.S, controllingPlayer, out playerIndex);
        }

        public bool IsSpace(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyPress(Keys.Space, controllingPlayer, out playerIndex);
        }
        public bool IsScrollLeft(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsKeyPressed(Keys.Left, controllingPlayer, out playerIndex);
        }
        public bool IsScrollRight(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsKeyPressed(Keys.Right, controllingPlayer, out playerIndex);
        }
        public bool IsScrollUp(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsKeyPressed(Keys.Up, controllingPlayer, out playerIndex);
        }
        public bool IsScrollDown(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsKeyPressed(Keys.Down, controllingPlayer, out playerIndex);
        }
        public bool IsZoomOut(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyPress(Keys.OemPeriod, controllingPlayer, out playerIndex);
        }

        public bool IsZoomIn(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyPress(Keys.OemComma, controllingPlayer, out playerIndex);
        }

        public bool IsShift(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyPress(Keys.LeftShift, controllingPlayer, out playerIndex);
        }
    }
}
