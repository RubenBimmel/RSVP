using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GamePhone {
    public class TimeDisplay : MonoBehaviour {

        private TextMeshPro text;

        // Use this for initialization
        void Start() {
            text = transform.GetComponentInChildren<TextMeshPro>();
            if (!text) {
                Debug.LogWarning("Time display is missing a text object.");
            }
        }

        // Update is called once per frame
        void Update() {
            if (text) {
				text.text = GameManager.GetTimeAsString (Mathf.FloorToInt(GameManager.time));
            }
        }
    }
}
