using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePhone {
    [CreateAssetMenu(fileName = "NewContact", menuName = "GamePhone/Contact")]
    public class Contact : ScriptableObject
    {
        public enum State {
            Offline,
            Notified,
            Online
        }

        public Sprite image;
        public float writingSpeed = 1f;
        public float readingSpeed = 1f;
        public float responseTime = 5f;
        public float activeTime = 120f;
        [HideInInspector]
        public float readDelay;
        public bool finishedReading { get { return readDelay < 0; } }
        [HideInInspector]
        public State state;
        [HideInInspector]
        public float offlineTimer = 0f;

        public void AddMessageToRead (Message message) {
            if (message.contact != this) {
                readDelay = Mathf.Max(.4f, readDelay) + message.text.Length / (10f * readingSpeed);
            }
        }

        public void UpdateReading()
        {
            if (readDelay > 0)
            {
                readDelay -= Time.deltaTime;
                if (readDelay < 0)
                {
                    NarrativeHandler.instance.ResetMessageScreen(true, false, false, false);
                }
            }
        }
    }
}
