using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Transition state.
/// </summary>
public enum TransitionState {
	TransitingIn,
	TransitingOut,
	TransitedIn,
	TransitedOut
}

public class UITransitionCanvasBehaviour : MonoBehaviour {

	/// <summary>
	/// Gets the current state of the transition.
	/// </summary>
	/// <value>The state of the transit.</value>
	public TransitionState transitState { get; private set; }

	/// <summary>
	/// The transiters.
	/// </summary>
	private Dictionary<TransitionType, UITransiter> transiters = new Dictionary<TransitionType, UITransiter> ();

	/// <summary>
	/// Transition part completed event handler.
	/// </summary>
	public delegate void TransitionPartCompletedEventHandler (object source, TransitionPartEventArgs args);
	/// <summary>
	/// Occurs when transition part completed.
	/// </summary>
	public event TransitionPartCompletedEventHandler TransitionPartCompleted;

	/// <summary>
	/// Raises the transition part completed event.
	/// </summary>
	/// <param name="source">Source.</param>
	/// <param name="args">Arguments.</param>
	protected virtual void OnTransitionPartCompleted (object source, TransitionPartEventArgs args) {
		if (TransitionPartCompleted != null) {
			TransitionPartCompleted (source, new TransitionPartEventArgs () {
				transitedOut = (transitState == TransitionState.TransitedOut)
			});
		}
	}

	void Awake () {
		transitState = TransitionState.TransitedIn;

		UITransiterFadeInOut fadeInOut = GetComponentInChildren<UITransiterFadeInOut> ();
		transiters.Add (TransitionType.FadeInOut, fadeInOut);

		try {
			transiters[TransitionType.FadeInOut].TransiterActionCompleted += this.OnTransiterActionCompleted;
		} catch {
			Debug.LogError ("No UIFadeInOut was found in the children of the object " + gameObject.name);
			Debug.Break ();
		}

		transiters.Add (TransitionType.None, null);
	}

	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update () {}

	public void StartTransition (Color fromColor, Color toColor, float duration, TransitionType type) {
		if (transitState == TransitionState.TransitedIn || transitState == TransitionState.TransitedOut) {
			// Change the state to transiting in/out (according to the previous state)
			transitState = transitState == TransitionState.TransitedIn ? TransitionState.TransitingOut : TransitionState.TransitingIn;

			// Enable the transiter's Image
			transiters[type].EnableImage ();

			// Call the transition animation according to the given TransitionType
			transiters[type].StartTransition (fromColor, toColor, duration);
		}
	}

	/// <summary>
	/// The transiter action completed event was triggered.
	/// </summary>
	/// <param name="source">Source.</param>
	/// <param name="args">Arguments.</param>
	public void OnTransiterActionCompleted (object source, TransitionPartEventArgs args) {
		if (transitState == TransitionState.TransitingOut) {
//			((UITransiter)source).DisableImage ();
			transitState = TransitionState.TransitedOut;
			args.transitedOut = true;
		} else {
			transitState = TransitionState.TransitedIn;
		}

		if (TransitionPartCompleted != null) {
			TransitionPartCompleted (source, args);
		}
	}
}
