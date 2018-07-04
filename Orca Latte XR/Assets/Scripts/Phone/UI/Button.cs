using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GamePhone {
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class Button : Division {

        public Sprite normalSprite;
        public Sprite activeSprite;

        public UnityEvent OnClick;
        public UnityEvent OnHold;

        private SpriteRenderer spriteRenderer;
        private bool triggered;

		// Called on initialisation
        protected virtual void Awake() {
			if (OnClick == null) {
				OnClick = new UnityEvent ();
			}
			if (OnHold == null) {
				OnHold = new UnityEvent ();
			}
            spriteRenderer = GetComponent<SpriteRenderer>();
            Reset();
        }

		// Resets the button size so that it is the same as the sprite
        public void Reset() {
            if (normalSprite) {
                spriteRenderer.sprite = normalSprite;
				bounds.size = normalSprite.bounds.size;
				bounds.center = normalSprite.bounds.center;
            }
			GetComponent<BoxCollider2D> ().size = bounds.size;
        }

		// Gets called when the user touches this division
        public override void OnMouseSelect () {
			if (activeSprite) {
				spriteRenderer.sprite = activeSprite;
			}
			base.OnMouseSelect();
        }

		// Gets called when the user resleases this division
		public override void OnMouseRelease () {
			if (normalSprite) {
				spriteRenderer.sprite = normalSprite;
			}
			triggered = false;
			base.OnMouseRelease ();
		}

		// Gets called when the user clicks on this division
		public override void OnMouseClick ()
		{
			OnClick.Invoke ();
			base.OnMouseClick ();
		}

		// Gets called every frame (after the treshold) that the user holds this division
		public override void OnMouseHold ()
		{
			if (!triggered) {
				OnHold.Invoke ();
				triggered = true;
			}
			base.OnMouseHold ();
		}

        // Called every time the division bounds change
        protected override void OnResize () {
			Reset();
		}
    }
}
