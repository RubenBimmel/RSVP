using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePhone {
	public class LockScreen : Division {

		private Transform swipeButton;
		private Vector3 startPosition;

		// Called on initialisation
		private void Awake () {
			swipeButton = GetComponentInChildren<SwipeButton> ().transform;
			startPosition = transform.localPosition;
		}

		// Called on unlock
		public void StartAnimation () {
			StartCoroutine (UnlockAnimation());
		}

		// Updates the unlock animation
		private IEnumerator UnlockAnimation () {
			// Move the screen further up
			Vector3 pos = swipeButton.localPosition;
			while (pos.y < 1920) {
				pos += Vector3.up * 5000 * Time.deltaTime;
				transform.localPosition = pos - swipeButton.localPosition;
				yield return null;
			}

			// Reset all positions
			transform.localPosition = startPosition;
			swipeButton.GetComponent<SwipeButton> ().Reset ();
			gameObject.SetActive(false);
		}
	}
}