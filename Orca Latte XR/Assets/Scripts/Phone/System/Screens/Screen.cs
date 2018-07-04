using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePhone {
    public class Screen : MonoBehaviour {
        public Sprite preview;

		// Gets called on opening of this screen
		public virtual void Open () {
			gameObject.SetActive(true);
		}

		// Gets called when this screen goes to the background
		public virtual void Pause () {
			gameObject.SetActive (false);
		}

		// Gets called on closing this screen
		public virtual void Close() {
			Destroy (gameObject);
		}

		// Catch a go back action
		public virtual bool GoBack () {
			return false;
		}
    }
}
