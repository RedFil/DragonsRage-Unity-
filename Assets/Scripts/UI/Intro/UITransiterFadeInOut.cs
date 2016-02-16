using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITransiterFadeInOut : UITransiter {

	/// <summary>
	/// The fade image.
	/// </summary>
	private Image fadeImage;

	private float startTime;

	void Awake () {
		// Get and hold the object's Image to easierly later usage
		fadeImage = GetComponent<Image> ();

		// Check if it exists
		if (fadeImage == null) {
			Debug.LogError ("No Image instance was found in the object " + gameObject.name);
			Debug.Break ();
		}
	}

	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (isTransiting) {
			// Calculate the fade stage based on the starting time and the current time
			float fadeStage = (Time.time - startTime) / duration;

			// Change the image's color according to the fade stage
			FadeLerp (fadeStage);

			// Check if the fade finished
			if (fadeStage >= 1.0f) {
				FinishFade ();
			}
		}
	}

	/// <summary>
	/// Starts the transition.
	/// </summary>
	/// <param name="startColor">Start color.</param>
	/// <param name="endColor">End color.</param>
	/// <param name="duration">Duration.</param>
	public override void StartTransition (Color startColor, Color endColor, float duration) {
		this.startColor = startColor;
		this.endColor = endColor;
		this.duration = duration;

		this.startTime = Time.time;

		this.isTransiting = true;
	}

	/// <summary>
	/// Enables the gameObject Image.
	/// </summary>
	public override void EnableImage ()
	{
		fadeImage.enabled = true;
	}

	/// <summary>
	/// Disables the gameObject Image.
	/// </summary>
	public override void DisableImage ()
	{
		fadeImage.enabled = false;
	}

	/// <summary>
	/// Fades the screen image in a linear movement.
	/// </summary>
	/// <param name="fadeStage">Fade stage (between 0.0f and 1.0f).</param>
	private void FadeLerp (float fadeStage) {
		fadeImage.color = Color.Lerp (startColor, endColor, fadeStage);
	}

	/// <summary>
	/// Finishs the fade.
	/// </summary>
	private void FinishFade () {
		isTransiting = false;

		OnTransiterActionCompleted (this, new TransitionPartEventArgs () {
			transitionType = TransitionType.FadeInOut
		});
	}
}
