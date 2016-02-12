using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TransitionFade;

public class UITransitionController : MonoBehaviour {

	// The scene that should be loaded when the transition happen.
	[HideInInspector]
	public string nextScene = "";

	[Tooltip("How long the transition animation should take.")]
	public float transitionDuration = 1.0f;

	[Tooltip("The Canvas Transition's prefab.")]
	public GameObject PrefCanvTransition;
	private UICanvasTransition canvasTransition;

	void Awake () {
		// Instantiate the Transition's canvas
		GameObject canvasGameObject = (GameObject)Instantiate (PrefCanvTransition, Vector3.zero, new Quaternion ());
		canvasGameObject.transform.SetParent (gameObject.transform);
		canvasTransition = canvasGameObject.GetComponent<UICanvasTransition> ();

		canvasTransition.TransitionCompleted += this.OnTransitionCompleted;
	}

	// Use this for initialization
	void Start () {
		canvasTransition.StartTransition (transitionDuration, UICanvasTransition.TransitionType.FadeInOut);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Raised the transition completed event.
	/// </summary>
	/// <param name="source">Source.</param>
	/// <param name="args">Arguments.</param>
	public void OnTransitionCompleted (object source, FadeEventArgs args) {
		switch (args.eventFadeState) {
		case FadeState.FadedIn:
			ScreenTransitionInFinished ();
			break;
		case FadeState.FadedOut:
			break;
		case FadeState.NONE:
			ScreenTransitionInFinished ();
			break;
		default:
			ScreenTransitionInFinished ();
			break;
		}
	}

	public void ScreenTransitionInFinished () {
		// Check if it is the Intro scene.
		string changeToScene = nextScene == "" ? "MainMenu" : nextScene;

		SceneManager.LoadScene (changeToScene);
	}

	public void StartTransitionOut () {
		canvasTransition.StartTransition (transitionDuration, UICanvasTransition.TransitionType.FadeInOut);
	}

	void OnLevelWasLoaded (int level) {
		canvasTransition.StartTransition (transitionDuration, UICanvasTransition.TransitionType.FadeInOut);
	}

}
