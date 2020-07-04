using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlunderMouse;
using Extensions;
using UnityEngine.InputSystem;
using Unity.XR.Oculus.Input;

public class OVRCameraRig : MonoBehaviour, IUpdatable
{
	public new Camera camera;
	public Transform trackingSpace;
	public Transform centerEyeAnchor;
	public Transform leftHandAnchor;
	public Transform rightHandAnchor;
	public Transform trs;
	public Transform handsParent;
	static Transform currentHand;
	public static Transform CurrentHand
	{
		get
		{
			return currentHand;
		}
		set
		{
			currentHand = value;
		}
	}
	Vector3 positionOffset;
	Quaternion rota;
	Vector3 previousTrackingSpaceForward;
	public float lookRate;
	bool wasPreviouslySettingOrienation;
	
	public virtual void Start ()
	{
		CurrentHand = rightHandAnchor;
		positionOffset = trs.localPosition;
		trs.SetParent(null);
		SetOrientation ();
		GameManager.updatables = GameManager.updatables.Add(this);
	}

	public virtual void DoUpdate ()
	{
		if (PlayerObject.CurrentActive != null)
			trs.position = PlayerObject.CurrentActive.trs.position + (rota * positionOffset);
		Vector2 rotaInput = Mouse.current.delta.ToVec2().FlipY() * lookRate * Time.deltaTime;
		if (rotaInput != Vector2.zero)
		{
			trackingSpace.RotateAround(trackingSpace.position, trackingSpace.right, rotaInput.y);
			trackingSpace.RotateAround(trackingSpace.position, Vector3.up, rotaInput.x);
		}
		if (GameManager.GetSingleton<InputManager>().inputDevice == InputManager.InputDevice.OculusRift)
			UpdateAnchors ();
		if ((GameManager.GetSingleton<InputManager>().inputDevice == InputManager.InputDevice.KeyboardAndMouse && Keyboard.current.spaceKey.isPressed) || (GameManager.GetSingleton<InputManager>().inputDevice == InputManager.InputDevice.OculusRift && (InputManager.leftTouchController.gripPressed.isPressed || InputManager.rightTouchController.gripPressed.isPressed)))
		{
			if (!wasPreviouslySettingOrienation)
			{
				wasPreviouslySettingOrienation = true;
				SetOrientation ();
			}
		}
		else
			wasPreviouslySettingOrienation = false;
	}

	public virtual void UpdateAnchors ()
	{
		centerEyeAnchor.localPosition = InputManager.hmd.centerEyePosition.ToVec3();
		centerEyeAnchor.localRotation = InputManager.hmd.centerEyeRotation.ToQuat();
		leftHandAnchor.localRotation = InputManager.leftTouchController.deviceRotation.ToQuat();
		rightHandAnchor.localRotation = InputManager.rightTouchController.deviceRotation.ToQuat();
		leftHandAnchor.localPosition = InputManager.leftTouchController.devicePosition.ToVec3();
		rightHandAnchor.localPosition = InputManager.rightTouchController.devicePosition.ToVec3();
	}
	
	public virtual void SetOrientation ()
	{
		rota = Quaternion.LookRotation(centerEyeAnchor.forward.GetXZ().SetY(centerEyeAnchor.forward.y), Vector3.up);
		trackingSpace.forward = centerEyeAnchor.forward.GetXZ();
	}

	public virtual void OnDestroy ()
	{
		GameManager.updatables = GameManager.updatables.Remove(this);
	}
}
