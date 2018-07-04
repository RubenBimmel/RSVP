using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GamePhone {
	public class SwipeButton : Panel {

		public UnityEvent OnSwipe;
		public float maxDistance;
		private bool triggered;

		// Called on initialisation
		protected override void Awake() {
			if (OnSwipe == null) {
				OnSwipe = new UnityEvent ();
			}
			triggered = false;

			base.Awake ();
		}

		// Called every frame
		protected override void Update () {
			// Execute panel movement
			base.Update ();

			// If the panel moves beyond the treshold, trigger the event
			if (Vector3.Distance (startPosition, transform.localPosition) >= maxDistance) {
				if (!triggered) {
					OnSwipe.Invoke ();
					triggered = true;
				}
			} else {
				triggered = false;
			}
		}
	}
}
