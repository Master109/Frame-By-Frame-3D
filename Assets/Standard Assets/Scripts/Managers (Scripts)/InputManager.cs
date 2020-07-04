using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Rewired;
using System;
using Unity.XR.Oculus.Input;
using UnityEngine.InputSystem;

namespace PlunderMouse
{
	public class InputManager : MonoBehaviour
	{
		public static OculusHMD hmd;
		public static OculusTouchController leftTouchController;
		public static OculusTouchController rightTouchController;
		// public static Player inputter;
		public InputDevice inputDevice;
		
		public virtual IEnumerator Start ()
		{
			hmd = InputSystem.GetDevice<OculusHMD>();
			leftTouchController = (OculusTouchController) InputSystem.GetDevice("OculusTouchControllerLeft");
			rightTouchController = (OculusTouchController) InputSystem.GetDevice("OculusTouchControllerRight");
			// inputter = ReInput.players.GetPlayer("Player");
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			hmd = InputSystem.GetDevice<OculusHMD>();
			leftTouchController = (OculusTouchController) InputSystem.GetDevice("OculusTouchControllerLeft");
			rightTouchController = (OculusTouchController) InputSystem.GetDevice("OculusTouchControllerRight");
		}
		
		public virtual void ResetInput ()
		{
			Input.ResetInputAxes();
		}

		public static float GetAxis (InputControl<float> positiveButton, InputControl<float> negativeButton)
		{
			return positiveButton.ReadValue() - negativeButton.ReadValue();
		}

		public static Vector2 GetAxis2D (InputControl<float> positiveXButton, InputControl<float> negativeXButton, InputControl<float> positiveYButton, InputControl<float> negativeYButton)
		{
			Vector2 output = new Vector2();
			output.x = positiveXButton.ReadValue() - negativeXButton.ReadValue();
			output.y = positiveYButton.ReadValue() - negativeYButton.ReadValue();
			output = Vector2.ClampMagnitude(output, 1);
			return output;
		}
		
		// [Serializable]
		// public class Hotkey
		// {
		// 	public ButtonEntry[] requiredButtons;
			
		// 	public virtual bool IsPressed ()
		// 	{
		// 		bool output = true;
		// 		foreach (ButtonEntry requiredButton in requiredButtons)
		// 		{
		// 			if (!requiredButton.IsPressed ())
		// 			{
		// 				output = false;
		// 				break;
		// 			}
		// 		}
		// 		return output;
		// 	}
			
		// 	[Serializable]
		// 	public class ButtonEntry
		// 	{
		// 		public string keyString;
		// 		public KeyCode key;
		// 		public VRButtonGroup vrButtonGroup;
		// 		public HotkeyState pressState;
				
		// 		public virtual bool IsPressed ()
		// 		{
		// 			bool output = false;
		// 			switch (pressState)
		// 			{
		// 				case HotkeyState.Down:
		// 					output = vrButtonGroup.GetDown() || (!string.IsNullOrEmpty(keyString) && Input.GetKeyDown(keyString)) || Input.GetKeyDown(key);
		// 					break;
		// 				case HotkeyState.Held:
		// 					output = vrButtonGroup.Get() || (!string.IsNullOrEmpty(keyString) && Input.GetKey(keyString)) || Input.GetKey(key);
		// 					break;
		// 				case HotkeyState.Up:
		// 					output = vrButtonGroup.GetUp() || (!string.IsNullOrEmpty(keyString) && Input.GetKeyUp(keyString)) || Input.GetKeyUp(key);
		// 					break;
		// 			}
		// 			return output;
		// 		}
		// 	}
		// }
		
		// [Serializable]
		// public class VRButtonGroup
		// {
		// 	public OVRInput.Button[] buttons;
			
		// 	public bool GetDown ()
		// 	{
		// 		bool output = false;
		// 		foreach (OVRInput.Button button in buttons)
		// 			output |= OVRInput.GetDown(button);
		// 		return output;
		// 	}
			
		// 	public bool Get ()
		// 	{
		// 		bool output = false;
		// 		foreach (OVRInput.Button button in buttons)
		// 			output |= OVRInput.Get(button);
		// 		return output;
		// 	}
			
		// 	public bool GetUp ()
		// 	{
		// 		bool output = false;
		// 		foreach (OVRInput.Button button in buttons)
		// 			output |= OVRInput.GetUp(button);
		// 		return output;
		// 	}
		// }
		
		public enum HotkeyState
		{
			Down,
			Held,
			Up,
		}
		
		public enum InputDevice
		{
			OculusGo,
			OculusRift,
			KeyboardAndMouse,
		}
	}
}