using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePhone {
    public class SendBoxHandler : MonoBehaviour {

        public MessageQueue queue;

        public SpriteRenderer sendButton;
        public TMPro.TextMeshPro textBox;

        public Sprite activeSendButton;
        public Sprite inactiveSendButton;   

        private Message message;
        private string typedMessage;
        private int maxChars = 32;
        private float nextLetterTime;

        // Update the send box with the currently pending message
        public void Reset (Chat activeChat) {
            queue = activeChat.queue;
            if (queue.pendingMessage && queue.pendingMessage.contact == NarrativeHandler.instance.You) {
                if (message != queue.pendingMessage) {
                    message = queue.pendingMessage;
                    typedMessage = "";
                    textBox.text = "";
                }
            }
        }

        // Update the send box text and button
        public void Update() {
            if (message) {
                if (NarrativeHandler.instance.debugMode)
                {
                    textBox.text = message.text;
                    typedMessage = message.text;
                    sendButton.sprite = activeSendButton;
                    return;
                }

                if (typedMessage.Length < message.text.Length) {
                    if (Time.time > nextLetterTime) {
                        typedMessage += message.text[typedMessage.Length];
                        if (typedMessage.Length > maxChars) {
                            textBox.text = "..." + typedMessage.Substring(typedMessage.Length - maxChars);
                        } else {
                            textBox.text = typedMessage;
                        }

                        nextLetterTime = Time.time + .1f / queue.pendingMessage.contact.writingSpeed;
                        if (message.text[typedMessage.Length-1] == ',') {
                            nextLetterTime += .3f;
                        }
                    }
                }
                else {
                    sendButton.sprite = activeSendButton;
                }
            }
        }

        // Send message and clear send box
        public void SendMessage() {
            if (message) {
                if (typedMessage == message.text) {
                    if (queue.pendingMessage.contact == NarrativeHandler.instance.You) {
                        queue.SendNextInQueue();
                        sendButton.sprite = inactiveSendButton;
                        textBox.text = "";
                        typedMessage = "";
                        message = null;
                    }
                }
            }
        }
    }
}
