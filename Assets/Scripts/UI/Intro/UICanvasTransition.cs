using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

using TransitionFade;

public class UICanvasTransition : MonoBehaviour {

	/// <summary>
	/// Enum to say which kind of transition the screen should do.
	/// </summary>
	public enum TransitionType {
		None,
		FadeInOut,
		Random
	}

	[Tooltip("Which type of transition animation the screen should do.")]
	public TransitionType transType = TransitionType.FadeInOut;

	/// <summary>
	/// Transition completed event handler.
	/// </summary>
	public delegate void TransitionCompletedEventHandler (object source, FadeEventArgs args);
	/// <summary>
	/// Occurs when a screen transition is completed.
	/// </summary>
	public event TransitionCompletedEventHandler TransitionCompleted;

	// Holder of the FadeInOut script
	private UIFadeInOut fadeTrans;

	void Awake () {
		fadeTrans = GetComponentInChildren<UIFadeInOut> ();

		try {
			fadeTrans.FadeCompleted += this.OnFadeCompleted;
		} catch {
			Debug.LogError ("No UIFadeInOut was found in the children of the object " + gameObject.name);
			Debug.Break ();
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Starts a transition.
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="transitionDuration">How long the transition animation will take.</param>
	public void StartTransition (float transitionDuration, TransitionType type = TransitionType.None) {
		// Check if it is a recursive call
		TransitionType transitionTypeChecker = type == TransitionType.None ? transType : type;

		// Call the appropriated methods to the given transition animation
		switch (transitionTypeChecker) {
		case TransitionType.None:
			OnTrasitionCompleted (fadeTrans, new FadeEventArgs () { eventFadeState = FadeState.NONE });
			break;
		case TransitionType.FadeInOut:
			if (fadeTrans.fadeState == FadeState.FadedOut) {
				fadeTrans.StartFadeIn (transitionDuration);
			} else {
				fadeTrans.StartFadeOut (transitionDuration);
			}

			break;
		case TransitionType.Random:
			System.Array fadeTypes = System.Enum.GetValues (typeof(TransitionType));

			// Random a different transition type and start recursive method
			TransitionType transRecursive = (TransitionType)fadeTypes.GetValue (Random.Range (0, fadeTypes.Length));
			StartTransition (transitionDuration, transRecursive);

			break;
		}
	}

	/// <summary>
	/// Raised the fade completed event.
	/// </summary>
	/// <param name="source">Source.</param>
	/// <param name="args">Arguments.</param>
	public void OnFadeCompleted (object source, FadeEventArgs args) {
		OnTrasitionCompleted (source, args);
	}

	/// <summary>
	/// Raises the trasition completed event.
	/// </summary>
	/// <param name="source">Source.</param>
	/// <param name="args">Arguments.</param>
	protected virtual void OnTrasitionCompleted (object source, FadeEventArgs args) {
		if (TransitionCompleted != null) {
			TransitionCompleted (source, args);
		}
	}
}
