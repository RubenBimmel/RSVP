using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePhone {
    public class Indicator : Symbol {

        public Sprite[] sprites;
        public int value { get; private set; }
        public int maxValue { get { return sprites.Length - 1; } }
        private SpriteRenderer spriteRenderer;

        private void Awake() {
            spriteRenderer = transform.GetComponent<SpriteRenderer>();
            SetValue(maxValue);
        }

        public void SetValue(int _value) {
            value = Mathf.Clamp(_value, 0, sprites.Length - 1);
            if (sprites.Length > 0) {
                spriteRenderer.sprite = sprites[value];
            }
        }
    }
}
