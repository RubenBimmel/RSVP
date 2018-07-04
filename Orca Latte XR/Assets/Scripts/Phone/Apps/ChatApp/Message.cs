using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePhone {
	[CreateAssetMenu(fileName = "NewMessage", menuName = "GamePhone/Message")]
	public class Message : ScriptableObject {
        public Chat chat;
        public Contact contact;

        public string message;
		public string text;
		public float delay;

		[HideInInspector]
		public float time;
	}
}
