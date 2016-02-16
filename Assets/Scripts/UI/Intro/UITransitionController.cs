using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Transition type.
/// </summary>
public enum TransitionType {
	None,
	FadeInOut
}

/// <summary>
/// Transition event arguments.
/// </summary>
public class TransitionEventArgs : System.EventArgs {
	// Nothing for now.
}

/// <summary>
/// Transition part event arguments.
/// </summary>
public class TransitionPartEventArgs : System.EventArgs {
	public bool transitedOut = false;
	public TransitionType transitionType;
}

public class UITransitionController : MonoBehaviour {

	/// <summary>
	/// The next scene.
	/// </summary>
	private string nextScene = "";

	[Tooltip("Which color the transition screen should be when full transparent.")]
	public Color transitFromColor = Color.clear;
	[Tooltip("Which color the transition screen should be when full visible.")]
	public Color transitToColor = Color.black;
	[Tooltip("How long the transition's part animation should take.")]
	public float transitionDuration = 2.0f;

	[Tooltip("Which kind of transition should happen.")]
	public TransitionType transitionType = TransitionType.FadeInOut;

	/// <summary>
	/// The transition canvas prefab.
	/// </summary>
	public GameObject transitionCanvasPrefab;
	/// <summary>
	/// The transition canvas behaviour script (from the transitionCanvasPrefab).
	/// </summary>
	private UITransitionCanvasBehaviour transitionCanvas;

	/// <summary>
	/// Transition completed event handler.
	/// </summary>
	public delegate void TransitionCompletedEventHandler (object source, TransitionEventArgs args);
	/// <summary>
	/// Occurs when transition completed.
	/// </summary>
	public event TransitionCompletedEventHandler TransitionCompleted;

	/// <summary>
	/// Trigger the transition completed event.
	/// </summary>
	/// <param name="source">Source.</param>
	/// <param name="args">Arguments.</param>
	protected virtual void OnTransitionCompleted (object source) {
		if (TransitionCompleted != null) {
			TransitionCompleted (source, new TransitionEventArgs ());
		}
	}

	void Awake () {
		// Instantiate the prefab in the scene and add it as gameObject's child
		GameObject canvasInstance = (GameObject)Instantiate (transitionCanvasPrefab, Vector3.zero, Quaternion.identity);
		canvasInstance.transform.SetParent (gameObject.transform);

		// Store the canvas behaviour script to later usage.
		transitionCanvas = canvasInstance.GetComponent<UITransitionCanvasBehaviour> ();
		transitionCanvas.TransitionPartCompleted += this.OnTransitionPartCompleted;
	}

	// Use this for initialization
	void Start () {
		// Begin the Intro with a fade out
		transitionCanvas.StartTransition (transitToColor, transitFromColor, transitionDuration, transitionType);
	}
	
	// Update is called once per frame
	void Update () {}

	/// <summary>
	/// The transition part completed event was triggered.
	/// </summary>
	/// <param name="source">Source.</param>
	/// <param name="args">Arguments.</param>
	public void OnTransitionPartCompleted (object source, TransitionPartEventArgs args) {
		OnTransitionCompleted (this);

		if (!args.transitedOut) {
			ChangeToNextScene ();
		} else {
			string curScene = SceneManager.GetActiveScene ().name;

			switch (curScene) {
			case "Intro":
				StartCoroutine (WaitBeforeTransitingIn (5f));
				break;
			}
		}
	}

	private IEnumerator WaitBeforeTransitingIn (float time) {
		yield return new WaitForSeconds (time);
		StartTransitionIn ();
	}

	private IEnumerator WaitBeforeTransitingOut (float time) {
		yield return new WaitForSeconds (time);
		StartTransitionOut ();
	}

	/// <summary>
	/// Changes to next scene.
	/// </summary>
	private void ChangeToNextScene () {
		// If something goes wrong, go back to the Main Menu
		if (nextScene == "") {
			nextScene = "MainMenu";
		}

		SceneManager.LoadScene (nextScene);
	}

	// Called when a new scene is loaded (doesn't is called when the game just started, first scene)
	void OnLevelWasLoaded (int level) {
		// Identify the loaded level
		string levelName = SceneManager.GetActiveScene ().name;

		// Do custom actions depending which scene was loaded
		switch (levelName) {
		case "MainMenu":
			break;
		default:
			break;
		}

		// Start transition out
		StartTransitionOut ();
	}

	/// <summary>
	/// Starts the transition in.
	/// </summary>
	public void StartTransitionIn () {
		transitionCanvas.StartTransition (transitFromColor, transitToColor, transitionDuration, transitionType);
	}

	/// <summary>
	/// Starts the transition out.
	/// </summary>
	public void StartTransitionOut () {
		transitionCanvas.StartTransition (transitToColor, transitFromColor, transitionDuration, transitionType);
	}

	/// <summary>
	/// Starts a transition.
	/// </summary>
	/// <param name="fromColor">From color.</param>
	/// <param name="toColor">To color.</param>
	/// <param name="duration">Duration.</param>
	/// <param name="type">Type.</param>
	private void StartTransition (Color fromColor, Color toColor, float duration, TransitionType type) {
		transitionCanvas.StartTransition (fromColor, toColor, duration, type);
	}
}
