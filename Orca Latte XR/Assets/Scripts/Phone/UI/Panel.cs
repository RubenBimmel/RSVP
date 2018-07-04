using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePhone {
    public class Panel : Division {

        public bool AllowScrollHorizontal;
        public bool AllowScrollVertical;
		public int margin = 100;
        public bool autoScrollMax;
		public bool spring;
		public float springStrength;

		protected Vector3 mousePosition;
		protected Vector3 velocity;
		protected bool dragging;
		protected float clampSpeed = 20;
		protected float springVelocity;

		protected Vector3 startPosition;

        // Called on initialisation
		protected virtual void Awake () {
			startPosition = transform.localPosition;
            AutoScroll();
        }

		// Called every frame
        protected virtual void Update() {
            if (!dragging) {
                velocity *= .8f;
                Release();
            }
        }

		// Gets called when the user touches this division
		public override void OnMouseSelect ()
		{
			dragging = true;
			base.OnMouseSelect ();
		}

		// Gets called when the user resleases this division
		public override void OnMouseRelease ()
		{
			dragging = false;
			base.OnMouseRelease ();
		}

		// Returns if this division is allowed to drag
		public override bool CanDrag (Vector3 velocity)
		{
			if (Mathf.Abs (velocity.x) > Mathf.Abs (velocity.y) && AllowScrollHorizontal) {
				return true;
			}
			if (Mathf.Abs (velocity.x) < Mathf.Abs (velocity.y) && AllowScrollVertical) {
				return true;
			}
			return base.CanDrag (velocity);
		}

		// Gets called every frame the division is dragged
		public override void OnMouseDrag (Vector3 velocity)
		{
			this.velocity = velocity;
			Vector3 position = transform.localPosition;

			// Update horizontal and veritcal position and clamp it to parent div size + margin
			if (AllowScrollHorizontal && bounds.size.x > parentBounds.size.x) {
				position.x += velocity.x;
				position.x = Mathf.Clamp(position.x, parentBounds.max.x - bounds.max.x - margin, parentBounds.min.x - bounds.min.x + margin);
			}
			if (AllowScrollVertical && bounds.size.y > parentBounds.size.y) {
				position.y += velocity.y;
				position.y = Mathf.Clamp(position.y, parentBounds.max.y - bounds.max.y - margin, parentBounds.min.y - bounds.min.y + margin);
			}

			transform.localPosition = position;
			mousePosition = UnityEngine.Input.mousePosition;
			base.OnMouseDrag (velocity);
		}

		// Gets called when the user is no longer dragging the panel
		protected virtual void Release() {
			if (spring) {
				ReleaseSpring ();
			} else {
				ReleaseFree ();
			}
        }

		// When there is no spring active the panel keeps moving until its velocity is zero
		protected virtual void ReleaseFree () {
			Vector3 position = transform.localPosition;

			if (AllowScrollHorizontal) {
				// Clamp the horizontal position so that the panel fully overlaps the parent division
				if (position.x < parentBounds.max.x - bounds.max.x) {
					position.x = Mathf.MoveTowards (position.x, parentBounds.max.x - bounds.max.x, margin * clampSpeed * Time.deltaTime);
					velocity.x = 0;
				} 
				else if (position.x > parentBounds.min.x - bounds.min.x) {
					position.x = Mathf.MoveTowards (position.x, parentBounds.min.x - bounds.min.x, margin * clampSpeed * Time.deltaTime);
					velocity.x = 0;
				}

				// Apply horizontal velocity
				position.x += velocity.x;
			}

			if (AllowScrollVertical) {
				// Clamp the vertical position so that the panel fully overlaps the parent division
				if (position.y < parentBounds.max.y - bounds.max.y) {
					position.y = Mathf.MoveTowards(position.y, parentBounds.max.y - bounds.max.y, margin * clampSpeed * Time.deltaTime);
					velocity.y = 0;
				}
				else if (position.y > parentBounds.min.y - bounds.min.y) {
					position.y = Mathf.MoveTowards(position.y, parentBounds.min.y - bounds.min.y, margin * clampSpeed * Time.deltaTime);
					velocity.y = 0;
				}

				// Apply vertical velocity
				position.y += velocity.y;
			}

			transform.localPosition = position;
		}

		// If the spring is active the panel will go back to its starting position;
		protected virtual void ReleaseSpring () {
			Vector3 position = transform.localPosition;

            springVelocity += springStrength * Time.deltaTime;
            if (velocity != Vector3.zero) {
                velocity = Vector3.MoveTowards(velocity, Vector3.zero, springVelocity);

                if (AllowScrollHorizontal) {
                    position.x += velocity.x;
                }

                if (AllowScrollVertical) {
                    position.y += velocity.y;
                }

                transform.localPosition = position;
            }
            else {
                if (position != startPosition) {
                    transform.localPosition = Vector3.MoveTowards(position, startPosition, springVelocity);
                }
                else {
                    springVelocity = 0f;
                }
            }
		}

		// Called to reset the panels position
		public void Reset () {
			transform.localPosition = startPosition;
			velocity = Vector3.zero;
            AutoScroll();
        }

        // Called every time the division bounds change
        protected override void OnResize() {
            base.OnResize();
            AutoScroll();
        }

        // Auto update the panels position
        private void AutoScroll() {
            if (autoScrollMax) {
                Vector3 position = transform.localPosition;
                if (AllowScrollHorizontal) {
                    position.x = parentBounds.min.x - bounds.min.x;
                }
                if (AllowScrollVertical) {
                    position.y = parentBounds.min.y - bounds.min.y;
                }
                transform.localPosition = position;
            }
        }
    }
}
