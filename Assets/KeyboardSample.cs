using UnityEngine;
using Valve.VR;

public class KeyboardSample : MonoBehaviour
{
	public UnityEngine.UI.InputField textEntry;
	public bool minimalMode;
	private static bool keyboardShowing;
	private string text = "";
	private static KeyboardSample activeKeyboard = null;
	private SteamVR_Events.Action keyboardCharInputAction;
	private SteamVR_Events.Action keyboardClosedAction;
	private UIClicker uiClicker;

	// Use this for initialization
	private void Start ()
	{
		uiClicker = GetComponent<UIClicker>();
		if (uiClicker != null)
		{
			uiClicker.Clicked += KeyboardDemo_Clicked;
		}
	}

	private void OnDestroy()
	{
		if (uiClicker != null)
		{
			uiClicker.Clicked -= KeyboardDemo_Clicked;
		}
	}

	private void OnEnable()
	{
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
		keyboardCharInputAction = SteamVR_Events.SystemAction(EVREventType.VREvent_KeyboardCharInput, OnKeyboard);
		keyboardCharInputAction.enabled = true;
		keyboardClosedAction = SteamVR_Events.SystemAction(EVREventType.VREvent_KeyboardClosed, OnKeyboardClosed);
		keyboardClosedAction.enabled = true;
	}

	private void OnDisable()
	{
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
		keyboardCharInputAction.enabled = false;
		keyboardCharInputAction = null;
		keyboardClosedAction.enabled = false;
		keyboardClosedAction = null;
	}

	private void OnKeyboard(VREvent_t ev)
	{
		if (activeKeyboard != this)
			return;
		VREvent_Keyboard_t keyboard = ev.data.keyboard;
		byte[] inputBytes = new byte[] { keyboard.cNewInput0, keyboard.cNewInput1, keyboard.cNewInput2, keyboard.cNewInput3, keyboard.cNewInput4, keyboard.cNewInput5, keyboard.cNewInput6, keyboard.cNewInput7 };
		int len = 0;
		for (; inputBytes[len] != 0 && len < 7; len++) ;
		string input = System.Text.Encoding.UTF8.GetString(inputBytes, 0, len);

		if (minimalMode)
		{
			if (input == "\b")
			{
				if (text.Length > 0)
				{
					text = text.Substring(0, text.Length - 1);
				}
			}
			else if (input == "\x1b")
			{
				// Close the keyboard
				var vr = SteamVR.instance;
				vr.overlay.HideKeyboard();
				keyboardShowing = false;
			}
			else
			{
				text += input;
			}
			textEntry.text = text;
		}
		else
		{
			System.Text.StringBuilder textBuilder = new System.Text.StringBuilder(1024);
			uint size = SteamVR.instance.overlay.GetKeyboardText(textBuilder, 1024);
			text = textBuilder.ToString();
            textEntry.text = text;
		}
	}

	private void OnKeyboardClosed(VREvent_t ev)
	{
		if (activeKeyboard != this)
			return;
		keyboardShowing = false;
		activeKeyboard = null;
    }

	private void KeyboardDemo_Clicked()
	{
		if(!keyboardShowing)
		{
			keyboardShowing = true;
			activeKeyboard = this;
			SteamVR.instance.overlay.ShowKeyboard((int)EGamepadTextInputMode.k_EGamepadTextInputModeNormal, (int)EGamepadTextInputLineMode.k_EGamepadTextInputLineModeSingleLine, "Description", 256, text, minimalMode, 0);
		}
	}
}
