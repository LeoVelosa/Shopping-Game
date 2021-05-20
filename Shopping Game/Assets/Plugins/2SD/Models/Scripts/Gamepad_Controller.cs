using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TSD.Assets
{
	public class Gamepad_Controller : MonoBehaviour
	{

		[Header("Button states")]
		public GamepadProperties gamepadProperties;
		
		[Header("Button Setup")]
		public GamepadButton btn_windows;
		public GamepadButton btn_menu;
		
		public GamepadButton btn_x;
		public GamepadButton btn_a;
		public GamepadButton btn_b;
		public GamepadButton btn_y;
		
		public GamepadButton btn_wifi;
		public GamepadButton btn_xb;
		
		public GamepadButton bumper_left;
		public GamepadButton trigger_left;
		public GamepadButton bumper_right;
		public GamepadButton trigger_right;

		public GamepadAxisDual stick_left;
		public GamepadAxisDual stick_right;
		public GamepadAxisDual dpad;

		[Tooltip("Animates the buttons over time")]
		public bool runDemo;
		
		private List<IButton> _buttons;
		
		private void Start()
		{
			_buttons = new List<IButton> {btn_menu, trigger_left, trigger_right, bumper_left, bumper_right, btn_windows, btn_x, btn_a, btn_b, btn_y, btn_wifi, btn_xb, stick_left, stick_right, dpad};
			foreach (var VARIABLE in _buttons)
			{
				VARIABLE.Init();
			}
		}

		private void Update()
		{
			foreach (var t in _buttons)
			{
				t.Update();
			}

			if (runDemo)
			{
				var time = Time.time * 8f;
				var index = (int)time % _buttons.Count;

				if (_buttons[index] is GamepadButton btn)
					btn.t = Mathf.PingPong(time, 1f);

				if (_buttons[index] is GamepadAxisDual axis)
				{
					var t = Mathf.PingPong(time, 1f);
					axis.t1 = Mathf.InverseLerp(0, 0.5f,  Mathf.Clamp(t, 0, 0.5f));
				}
				
				gamepadProperties.btn_middle_emission = btn_xb.t > .9;
				gamepadProperties.Update();
				return;
			}
			
			btn_menu.t = gamepadProperties.btn_menu;
			btn_windows.t = gamepadProperties.btn_windows;
			
			btn_x.t = gamepadProperties.btn_x;
			btn_a.t = gamepadProperties.btn_a;
			btn_b.t = gamepadProperties.btn_b;
			btn_y.t = gamepadProperties.btn_y;

			btn_wifi.t = gamepadProperties.btn_wifi;
			btn_xb.t = gamepadProperties.btn_xb;

			dpad.t1 = gamepadProperties.dpadHorizontal;
			dpad.t2 = gamepadProperties.dpadVertical;
			
			stick_left.t1 = gamepadProperties.stick_leftHorizontal;
			stick_left.t2 = gamepadProperties.stick_leftVertical;
			
			stick_right.t1 = gamepadProperties.stick_rightHorizontal;
			stick_right.t2 = gamepadProperties.stick_rightVertical;
			
			gamepadProperties.Update();
		}
	}

	[System.Serializable]
	public class GamepadProperties
	{
		[Header("Buttons")] 
		private bool last_btn_middle_emission;
		public bool btn_middle_emission;
		[Range(0f, 1f)] public float btn_windows;
		[Range(0f, 1f)] public float btn_menu;
		
		[Range(0f, 1f)] public float btn_x;
		[Range(0f, 1f)] public float btn_a;
		[Range(0f, 1f)] public float btn_b;
		[Range(0f, 1f)] public float btn_y;

		[Range(0f, 1f)] public float btn_wifi;
		[Range(0f, 1f)] public float btn_xb;
		
		[Header("Axis")]
		[Range(0f, 1f)] public float stick_leftHorizontal = .5f;
		[Range(0f, 1f)] public float stick_leftVertical = .5f;
		[Range(0f, 1f)] public float stick_rightHorizontal = .5f;
		[Range(0f, 1f)] public float stick_rightVertical = .5f;
		[Range(0f, 1f)] public float dpadHorizontal = .5f;
		[Range(0f, 1f)] public float dpadVertical = .5f;

		public Material controller_material;

		public void Update()
		{
			if (btn_middle_emission != last_btn_middle_emission)
			{
				if(btn_middle_emission)
					controller_material.EnableKeyword("_EMISSION");
				else
					controller_material.DisableKeyword("_EMISSION");
				last_btn_middle_emission = btn_middle_emission;
			}
		}
	}

	[System.Serializable]
	public class GamepadButton : IButton
	{
		public bool rotation = true;
		public bool position = true;
		
		[HideInInspector]
		public float t;
		//[HideInInspector]
		public Vector3 rotation_min;
		public Vector3 rotation_max;
		
		[HideInInspector]
		public Vector3 pos_min;
		public Vector3 pos_max;

		public Transform bone;

		public void Init()
		{
			pos_min = bone.transform.localPosition;
		}
		
		public void Update()
		{
			if(rotation)
				bone.localRotation = Quaternion.Euler(Vector3.Lerp(rotation_min, rotation_max, t));
			if(position)
				bone.localPosition = Vector3.Lerp(pos_min, pos_max, t);
		}
	}
	
	[System.Serializable]
	public class GamepadAxisDual :IButton
	{
		[HideInInspector]
		public float t1;
		//[HideInInspector]
		public Vector3 rotation_min;
		public Vector3 rotation_max;
		
		[HideInInspector]
		public float t2;
		//[HideInInspector]
		public Vector3 rotation_min2;
		public Vector3 rotation_max2;
		
		public Transform bone;
		public void Update()
		{
			bone.localRotation = Quaternion.Euler(Vector3.Lerp(rotation_min, rotation_max, t1)) * Quaternion.Euler(Vector3.Lerp(rotation_min2, rotation_max2, t2));
		}

		public void Init()
		{
			//throw new NotImplementedException();
		}
	}

	public interface IButton
	{
		void Update();
		void Init();
	}
}