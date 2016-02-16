using UnityEngine;
using System.Collections;

public abstract class UITransiter : MonoBehaviour {

	/// <summary>
	/// The fade start color.
	/// </summary>
	protected Color startColor { get; set; }
	/// <summary>
	/// The fade end color.
	/// </summary>
	protected Color endColor { get; set; }
	/// <summary>
	/// The fade duration.
	/// </summary>
	protected float duration { get; set; }

	/// <summary>
	/// Gets a value indicating whether this <see cref="UITransiter"/> is transiting.
	/// </summary>
	/// <value><c>true</c> if is transiting; otherwise, <c>false</c>.</value>
	public bool isTransiting { get; protected set; }

	/// <summary>
	/// Fade completed event handler.
	/// </summary>
	public delegate void TransiterActionCompletedEventHandler (object source, TransitionPartEventArgs args);
	/// <summary>
	/// Occurs when fade completed.
	/// </summary>
	public event TransiterActionCompletedEventHandler TransiterActionCompleted;

	/// <summary>
	/// Raises the fade completed event.
	/// </summary>
	/// <param name="source">Source.</param>
	/// <param name="args">Arguments.</param>
	protected virtual void OnTransiterActionCompleted (object source, TransitionPartEventArgs args) {
		if (TransiterActionCompleted != null) {
			TransiterActionCompleted (gameObject, args);
		}
	}

	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update () {}

	public abstract void StartTransition (Color startColor, Color endColor, float duration);

	public abstract void EnableImage ();
	public abstract void DisableImage ();
}
