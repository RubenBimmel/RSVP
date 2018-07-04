using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePhone {
    public class Input : MonoBehaviour {

		public static bool isEnabled = true;
		private float holdTime = 1f;
		private float dragStartDistance = 5f;

        private List<Division> divs;
		private Division activeDiv;
		private Vector3 lastMousePosition;
		private bool dragging;
		private float timer;

		// Called on initialisation
		private void Awake() {
			PrepareForInput ();
		}

		// Called at the beginning of every new input
		private void PrepareForInput () {
			divs = new List<Division>();
			lastMousePosition = UnityEngine.Input.mousePosition;
			dragging = false;
			timer = 0f;
		}

        // Update is called once per frame
        private void Update() {
			if (enabled) {
				// On mouse down
				if (UnityEngine.Input.GetMouseButtonDown (0)) {
					PrepareForInput ();

					StartInput ();
				}

				// If mouse is above a division
				if (divs.Count > 0) {
					// On mouse hold
					if (UnityEngine.Input.GetMouseButton (0)) {
						UpdateInput ();
					}
				
					// On mouse up
					if (UnityEngine.Input.GetMouseButtonUp (0)) {
						EndInput ();
					}

					timer += Time.deltaTime;
				}
			}

            UpdateInteractables();
        }

        private void UpdateInteractables ()
        {
            if (GyroControl.eyeView)
            {
                //RaycastHit hit;
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
                RaycastHit[] hits = Physics.RaycastAll(ray);
                foreach (RaycastHit hit in hits) {
                    InteractableObject io = hit.transform.GetComponent<InteractableObject>();
                    if (io)
                    {
                        InteractableObject.activeObject = io;
                        return;
                    }
                }
            }
            InteractableObject.activeObject = null;
        }

		// Called on mouse down
		private void StartInput () {
			// Store all divisions from raycast in array
			RaycastHit2D[] hit = Physics2D.RaycastAll (transform.TransformPoint (UnityEngine.Input.mousePosition), Vector3.forward);

            // Store all divisions in a list
            divs = new List<Division>();
			for (int i = 0; i < hit.Length; i++) {
				Division newDiv = hit [i].transform.GetComponent<Division> ();
				if (newDiv) {
					divs.Add (newDiv);
				}
			}

			if (divs.Count > 0) {
				ActivateDivision (divs [0]);
			}
		}

		// Called while mouse is down
		private void UpdateInput () {
			Vector3 offset = UnityEngine.Input.mousePosition - lastMousePosition;
			
			// Check if the user is going to drag
			if (!dragging && offset.magnitude > dragStartDistance) {
				StartMouseDrag (offset);
			}

			// If user is dragging
			if (dragging) {
				UpdateMouseDrag (offset);
				lastMousePosition = UnityEngine.Input.mousePosition;
			} 

			// check if the user is holding down
			if (!dragging && timer >= holdTime) {
				MouseHold ();
			}
		}

		// Called on mouse up
		private void EndInput () {
			if (!dragging && timer < holdTime) {
				MouseClick ();
			}
			DeactivateDivision ();
		}

		// Activate a division and deactivate the old active division
		private void ActivateDivision (Division div) {
			if (div) {
				DeactivateDivision ();
				div.OnMouseSelect ();
				activeDiv = div;
			}
		}

		// deactivate a division
		private void DeactivateDivision () {
			if (activeDiv)
				activeDiv.OnMouseRelease ();
			activeDiv = null;
		}

		// Trigger OnClick event on the active division
		private void MouseClick() {
			if (activeDiv) {
				activeDiv.OnMouseClick ();
			}
		}

		// Trigger OnHold event on the active division
		private void MouseHold() {
			if (activeDiv) {
				activeDiv.OnMouseHold ();
			}
		}

		// Loop through all hits until a division is found that can drag
		private void StartMouseDrag(Vector3 velocity) {
			for (int i = 0; i < divs.Count; i++) {
				if (divs [i].CanDrag (velocity)) {
					dragging = true;
					ActivateDivision (divs [i]);
					return;
				}
			}
		}

		// Trigger OnDrag event on the active division
		private void UpdateMouseDrag(Vector3 velocity) {
			if (activeDiv) {
				activeDiv.OnMouseDrag (velocity);
			}
		}
    }
}
