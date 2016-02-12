using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace TransitionFade {

	/// <summary>
	/// Enum to say in which direction a image should be fading (IN, OUT or NONE).
	/// </summary>
	public enum FadeDirection {
		Out = -1,
		None,
		In
	}

	/// <summary>
	/// Enum to say the current state of the fade animation.
	/// </summary>
	public enum FadeState {
		FadingOut = -1,
		FadedOut,
		FadingIn,
		FadedIn,
		NONE // For no transition animation
	}

	/// <summary>
	/// Fade event arguments.
	/// </summary>
	public class FadeEventArgs : System.EventArgs {
		public FadeState eventFadeState { get; set; }
	}

	/// <summary>
	/// Class used in objects that have fade animation from one color to clear color (invisible).
	/// </summary>
	public class UIFadeInOut : MonoBehaviour {

		[Tooltip("Which color the screen should be when full visible.")]
		public Color fadeColor = Color.black;

		[Tooltip("How long the fade animation will take.")]
		public float fadeDuration = 1.0f;

		// Hold the Image to save processment, but costing memory
		private Image fadeImage;

		// Get/set the Image's enabled variable
		public bool isEnabled {
			get { return fadeImage.enabled; }
			private set { fadeImage.enabled = value; }
		}

		// Essential variables to the fade animation
		private float fadeStartTime = 0;
		private FadeDirection fadeDirection = 0;
		public FadeState fadeState { get; private set; }

		/// <summary>
		/// Screen fade completed event handler.
		/// </summary>
		public delegate void FadeCompletedEventHandler (object source, FadeEventArgs args);
		/// <summary>
		/// Occurs when a screen fade transition is completed.
		/// </summary>
		public event FadeCompletedEventHandler FadeCompleted;

		void Awake () {
			fadeImage = GetComponent<Image> ();

			// Check if there is an Image component
			if (fadeImage == null) {
				Debug.LogError ("No Image instance was found in the object '" + gameObject.name + "'");
				Debug.Break ();
			}

			fadeState = FadeState.FadedIn;
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void FixedUpdate () {
			if (fadeDirection != FadeDirection.None) {
				FadeTick ();
			}
		}

		/// <summary>
		/// Send the command to the UI to start to fade in.
		/// </summary>
		/// <param name="duration">Animation's duration.</param>
		public void StartFadeIn (float duration) {
			fadeDuration = duration;

			// Only allow the start fade command if totally faded out (invisible to the player)
			if (fadeState == FadeState.FadedOut) {
				StartFade (FadeDirection.In);
			}
		}

		/// <summary>
		/// Send the command to the UI to start to fade out.
		/// </summary>
		/// <param name="duration">Animation's duration.</param>
		public void StartFadeOut (float duration) {
			fadeDuration = duration;

			// Only allow the start fade command if totally faded in (totally visible to the player, alpha = 100%)
			if (fadeState == FadeState.FadedIn) {
				StartFade (FadeDirection.Out);
			}
		}

		/// <summary>
		/// Send the command to this object to start to fade in or out.
		/// </summary>
		/// <param name="direction">In which <c>Direction</c> the fade animation should go.</param>
		private void StartFade (FadeDirection direction) {
			fadeDirection = direction;
			fadeStartTime = Time.time;
		}

		/// <summary>
		/// Whether the UI is in the middle of a fading animation or not.
		/// </summary>
		/// <returns><c>true</c>, if the fade animation is running, <c>false</c> otherwise.</returns>
		public bool isFading () {
			if (fadeState == FadeState.FadingIn || fadeState == FadeState.FadingOut) {
				return true;
			}

			return false;
		}

		/// <summary>
		/// Called every tick when the object should be animated (fading in or out).
		/// </summary>
		private void FadeTick () {
			if (fadeDirection == FadeDirection.In) {
				FadeLerp (Color.clear, fadeColor, (Time.time - fadeStartTime) / fadeDuration);
			} else if (fadeDirection == FadeDirection.Out) {
				FadeLerp (fadeColor, Color.clear, (Time.time - fadeStartTime) / fadeDuration);
			}
		}

		/// <summary>
		/// Called to in fact change the object's color.
		/// </summary>
		/// <param name="fadeFrom">The color this object is fading from.</param>
		/// <param name="fadeTo">The color this object should fade to.</param>
		/// <param name="fadeStage">In which stage the fade should go (0 to 1.0f).</param>
		private void FadeLerp (Color fadeFrom, Color fadeTo, float fadeStage) {
			fadeImage.color = Color.Lerp (fadeFrom, fadeTo, fadeStage);

			if (fadeStage != 0) {
				CheckFadeState ();
			}
		}

		/// <summary>
		/// Checks and change the <c>fadeState</c> according to the fade animation progress.
		/// </summary>
		public void CheckFadeState () {
			if (fadeImage.color == Color.clear) {
				fadeDirection = 0;
				fadeState = FadeState.FadedOut;

				OnFadeCompleted ();
			} else if (fadeImage.color == fadeColor) {
				fadeDirection = 0;
				fadeState = FadeState.FadedIn;

				OnFadeCompleted ();
			} else {
				if (fadeDirection == FadeDirection.In) {
					fadeState = FadeState.FadingIn;
				} else {
					fadeState = FadeState.FadingOut;
				}
			}
		}

		/// <summary>
		/// Instantaneously change the screen to the FadeColor (without the fade animation).
		/// </summary>
		public void InstanteFadeIn () {
			// Make it start faded in (for the Intro)
			fadeImage.color = fadeColor;
			fadeState = FadeState.FadedIn;
		}

		/// <summary>
		/// Instantaneously change the screen to the Color.clean (full transparent).
		/// </summary>
		public void InstanteFadeOut () {
			fadeImage.color = Color.clear;
			fadeState = FadeState.FadedOut;
		}

		/// <summary>
		/// Raises the fade completed event.
		/// </summary>
		protected virtual void OnFadeCompleted () {
			if (FadeCompleted != null) {
				FadeCompleted (this, new FadeEventArgs { eventFadeState = fadeState });
			}
		}
	}

}