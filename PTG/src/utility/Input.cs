using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PTG.utility
{
	public static class Input
	{
		#region KeyboardInput

		private static KeyboardState currentKeyboardState, previousKeyboardState;

		public static bool IsKeyHold(Keys key)
		{
			return currentKeyboardState.IsKeyDown(key);
		}

		public static bool IsKeyPressed(Keys key)
		{
			return (currentKeyboardState.IsKeyDown(key) && !previousKeyboardState.IsKeyDown(key));
		}

		public static bool IsKeyReleased(Keys key)
		{
			return (!currentKeyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyDown(key));
		}

		public static Keys GetPressedKey()
		{
			List<Keys> currentKeys = currentKeyboardState.GetPressedKeys().ToList();
			if (currentKeys.Count == 0)
			{
				return Keys.None;
			}

			Keys currentKey = currentKeys.Last();
			if (IsKeyPressed(currentKey))
			{
				return currentKey;
			}

			return Keys.None;
		}

		#endregion

		#region MouseInput

		private static MouseState currentMouseState, previousMouseState;
		private static DateTime lastLeftClickTime = DateTime.Now;
		private static DateTime lastRightClickTime = DateTime.Now;
		private static int leftPressCount;
		private static int rightPressCount;
		private const int doubleClickDelay = 1000;  // Milliseconds

		public static Point MousePosition()
		{
			return currentMouseState.Position;
		}

		public static Point PreviousMousePosition()
		{
			return previousMouseState.Position;
		}

		public static bool MouseMotion()
		{
			return currentMouseState.Position != previousMouseState.Position;
		}

		public static bool IsLeftHold()
		{
			return currentMouseState.LeftButton == ButtonState.Pressed;
		}

		public static bool IsLeftPressed()
		{
			return (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released);
		}

		public static bool IsLeftDoubleClicked()
		{
			return leftPressCount == 2;
		}

		public static bool IsLeftReleased()
		{
			return (currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed);
		}

		public static bool IsRightHold()
		{
			return currentMouseState.RightButton == ButtonState.Pressed;
		}

		public static bool IsRightPressed()
		{
			return (currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Released);
		}

		public static bool IsRightDoubleClicked()
		{
			return rightPressCount == 2;
		}

		public static bool IsRightReleased()
		{
			return (currentMouseState.RightButton == ButtonState.Released && previousMouseState.RightButton == ButtonState.Pressed);
		}

		public static bool IsMiddleHold()
		{
			return currentMouseState.MiddleButton == ButtonState.Pressed;
		}

		public static bool IsMiddlePressed()
		{
			return (currentMouseState.MiddleButton == ButtonState.Pressed && previousMouseState.MiddleButton == ButtonState.Released);
		}

		public static bool IsMiddleReleased()
		{
			return (currentMouseState.MiddleButton == ButtonState.Released && previousMouseState.MiddleButton == ButtonState.Pressed);
		}

		public static int ScrollValue()
		{
			return currentMouseState.ScrollWheelValue - previousMouseState.ScrollWheelValue;
		}

		#endregion

		// Updates the last- and current states
		public static void Update()
		{
			// Update keyboard states
			previousKeyboardState = currentKeyboardState;
			currentKeyboardState = Keyboard.GetState();

			// Update mouse states
			previousMouseState = currentMouseState;
			currentMouseState = Mouse.GetState();

			// Update double clicks
			// Left
			if ((DateTime.Now - lastLeftClickTime).TotalMilliseconds > doubleClickDelay || leftPressCount == 2)
			{
				leftPressCount = 0;
			}

			if (IsLeftPressed())
			{
				lastLeftClickTime = DateTime.Now;
				leftPressCount++;
			}

			// Right
			if ((DateTime.Now - lastRightClickTime).TotalMilliseconds > doubleClickDelay || rightPressCount == 2)
			{
				rightPressCount = 0;
			}

			if (IsRightPressed())
			{
				lastRightClickTime = DateTime.Now;
				rightPressCount++;
			}
		}
	}
}
