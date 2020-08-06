using UnityEngine;
using Valve.VR;
using Valve.VR.Extras;

public class UIClicker : MonoBehaviour
{
	public delegate void ClickHandler();

	public event ClickHandler Clicked;

	// Update is called once per frame
	void Update ()
	{
		foreach(SteamVR_LaserPointer pointer in FindObjectsOfType<SteamVR_LaserPointer>())
		{
			Ray r = new Ray(pointer.transform.position, pointer.transform.forward);
			Plane p = new Plane(transform.forward, transform.position);
			float enter;
			if (p.Raycast(r, out enter))
			{
				Vector3 target = pointer.transform.position + enter * pointer.transform.forward;
				Vector3 localTarget = ((RectTransform)transform).InverseTransformPoint(target);
				bool hit = ((RectTransform)transform).rect.Contains(new Vector2(localTarget.x, localTarget.y));


				SteamVR_Input_Sources inputSource = pointer.GetComponent<SteamVR_Behaviour_Pose>().inputSource;
				bool trigger = pointer.interactWithUI.GetLastStateDown(inputSource);
				if (hit && Clicked != null && trigger)
				{
					Clicked();
				}
			}
		}
	}
}
