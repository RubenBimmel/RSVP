using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePhone {
	[RequireComponent(typeof(BoxCollider2D))]
    public class Division : MonoBehaviour {
		public Bounds bounds;
		public bool encapsulate;

        private Division parent;
        private BoxCollider2D boxCollider;

		// Use this for initialization
		protected virtual void Start() {
            boxCollider = GetComponent<BoxCollider2D>();
            UpdateCollider();

            if (encapsulate) {
				foreach (Division div in GetComponentsInChildren<Division> ()) {
                    CheckEncapsulation(div);
				}
			}
		}

		// Check if div is still encapsulated by parent
		public void CheckEncapsulation (Division div) {
            if (!encapsulate) {
                return;
            }

            if (div.transform.parent != transform) {
                return;
            }

            bounds.Encapsulate(div.transform.localPosition + div.bounds.min);
            bounds.Encapsulate(div.transform.localPosition + div.bounds.max);
            OnResize();

            if (!parent) {
                parent = transform.parent.GetComponentInParent<Division>();
            }
            if (parent) {
                parent.CheckEncapsulation(this);
            }
        }

		// Used to safely resize a division
		public void Resize (Bounds newBounds) {
			bounds = newBounds;
            OnResize();
        }

        // Used to safely resize a division
        public void Resize(float width, float height) {
            bounds.size = new Vector3(width, height, 0);
            OnResize();
        }

        // Gets called when the user touches this division
        public virtual void OnMouseSelect () {
			//Debug.Log (name + " Select");
		}

		// Gets called when the user resleases this division
		public virtual void OnMouseRelease () {
			//Debug.Log (name + " Release");
		}

		// Gets called when the user clicks on this division
        public virtual void OnMouseClick () {
			//Debug.Log (name + " Click");
        }

		// Gets called every frame (after the treshold) that the user holds this division
		public virtual void OnMouseHold () {
			//Debug.Log (name + " Hold");
		}

		// Gets called every frame the division is dragged
		public virtual void OnMouseDrag (Vector3 velocity) {
			//Debug.Log (name + " Dragging with velocity " + velocity);
		}

		// Returns if this division is allowed to drag
		public virtual bool CanDrag (Vector3 velocity) {
			return false;
		}

        // Called every time the division bounds change
        protected virtual void OnResize() {
            UpdateCollider();
            CheckEncapsulation(this);
        }

        // Returns width of the parent division, or width of the screen if it does not have a parent division
        protected Bounds parentBounds {
			get {
				if (transform.parent) {
					if (!parent) {
						parent = transform.parent.GetComponentInParent<Division>();
					}
					if (parent) {
						return parent.bounds;
					}
				}
				return new Bounds(Vector3.zero, UnityEngine.Screen.safeArea.size);
			}
		}

        // Set the collider bounds equal to the div bounds
        private void UpdateCollider() {
            if (boxCollider) {
                boxCollider.offset = bounds.center;
                boxCollider.size = bounds.size;
            }
        }

        // Draw the outline of this division inside the editor
        private void OnDrawGizmos() {
			Gizmos.color = Color.white;
			Gizmos.DrawLine(transform.TransformPoint(bounds.min), transform.TransformPoint(bounds.min + Vector3.up * bounds.size.y));
			Gizmos.DrawLine(transform.TransformPoint(bounds.min), transform.TransformPoint(bounds.min + Vector3.right * bounds.size.x));
			Gizmos.DrawLine(transform.TransformPoint(bounds.max), transform.TransformPoint(bounds.min + Vector3.up * bounds.size.y));
			Gizmos.DrawLine(transform.TransformPoint(bounds.max), transform.TransformPoint(bounds.min + Vector3.right * bounds.size.x));
		}

		// Draw the outline of this division inside the editor when selected
		private void OnDrawGizmosSelected() {
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(transform.TransformPoint(bounds.min), transform.TransformPoint(bounds.min + Vector3.up * bounds.size.y));
			Gizmos.DrawLine(transform.TransformPoint(bounds.min), transform.TransformPoint(bounds.min + Vector3.right * bounds.size.x));
			Gizmos.DrawLine(transform.TransformPoint(bounds.max), transform.TransformPoint(bounds.min + Vector3.up * bounds.size.y));
			Gizmos.DrawLine(transform.TransformPoint(bounds.max), transform.TransformPoint(bounds.min + Vector3.right * bounds.size.x));
		}
    }
}
