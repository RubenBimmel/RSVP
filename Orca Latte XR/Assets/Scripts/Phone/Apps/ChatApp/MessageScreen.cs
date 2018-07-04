using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePhone {
    public class MessageScreen : MonoBehaviour {

        public Grid messageGrid;
        public Grid decisionGrid;
        public TMPro.TextMeshPro messageScreenContactName;
        public SpriteRenderer avatar;
        public Division UpcommingMessage;
        public SendBoxHandler sendBox;
        public SpriteRenderer status;
        public Sprite onlineStatus;
        public Sprite offlineStatus;

        // Clear message screen and load new messages
        public void ResetMessages(Chat activeChat) {
            if (enabled)
            {
                StopAllCoroutines();
                StartCoroutine(WipeAndRedrawMessages(activeChat));
            }
        }

        // Clear message screen and load new messages
        public IEnumerator WipeAndRedrawMessages(Chat activeChat) {
            // Clear grid
            List<GameObject> oldMessages = new List<GameObject>();
            foreach (Transform child in messageGrid.transform) {
                if (child != messageGrid.transform) {
                    oldMessages.Add(child.gameObject);
                }
            }
            messageGrid.ClearGrid();

            // Set contact information
            messageScreenContactName.text = activeChat.name;
            avatar.sprite = activeChat.image;
            SetStatus(activeChat);

            // Add new messages
            string path = Application.persistentDataPath + "/Resources/ChatApp/Messages/" + activeChat.name;
            if (System.IO.Directory.Exists(path)) {
                Message[] messages = SOSaver.LoadAll<Message>(path);
                List<UIMessage> messageItems = new List<UIMessage>();

                foreach (Message m in messages) {
                    if (m.chat && m.chat == activeChat) {
                        UIMessage newMessage = null;

                        // Choose prefab based on sender
                        if (m.contact == NarrativeHandler.instance.You) {
                            newMessage = Instantiate(Resources.Load<UIMessage>("Apps/ChatApp/Prefabs/MessageByYou"), messageGrid.transform);
                        }
                        else {
                            newMessage = Instantiate(Resources.Load<UIMessage>("Apps/ChatApp/Prefabs/MessageByContact"), messageGrid.transform);
                        }

                        if (newMessage) {
                            newMessage.SetText(m.text);

                            messageGrid.cells.Add(newMessage);
                            messageItems.Add(newMessage);
                        }
                    }
                }

                yield return null;

                // Remove old messages
                foreach (GameObject g in oldMessages) {
                    Destroy(g);
                }

                // Scale new messages and push them forward
                foreach (UIMessage m in messageItems) {
                    Vector3 position = m.transform.localPosition;
                    position.z = 0;
                    m.transform.localPosition = position;
                    m.ScaleToFitText();
                    
                }

                // Check for incoming messages
                if (activeChat.HasIncommingMessage()) {
                    Division upcomming = Instantiate(UpcommingMessage, messageGrid.transform);
                    messageGrid.cells.Add(upcomming);
                }

                messageGrid.ResetGrid();
            }

            sendBox.Reset(activeChat);
        }

        // Set contact online status
        public void SetStatus(Chat activeChat) {
            if (activeChat.IsPersonalChat()) {
                status.gameObject.SetActive(true);
                if (activeChat.contacts[0].state == Contact.State.Online) {
                    status.sprite = onlineStatus;
                }
                else {
                    status.sprite = offlineStatus;
                }
            }
            else {
                status.gameObject.SetActive(false);
            }
        }

        // Clear the decisions and add new decisions
        public void ResetDecisions(Chat activeChat) {
            // Clear grid
            foreach (Transform child in decisionGrid.transform) {
                if (child != decisionGrid.transform) {
                    Destroy(child.gameObject);
                }
            }
            decisionGrid.ClearGrid();

            if (activeChat.queue.isEmpty) {
                // Add new decisions
                string path = Application.persistentDataPath + "/Resources/ChatApp/Options/" + activeChat.name;
                if (System.IO.Directory.Exists(path)) {
                    MessageOption[] options = SOSaver.LoadAll<MessageOption>(path);
                    foreach (MessageOption d in options) {
                        if (d.chat == activeChat) {
                            UIMessageOption newDecision = Instantiate(Resources.Load<UIMessageOption>("Apps/ChatApp/Prefabs/DialogueOption"), decisionGrid.transform);

                            newDecision.Initialise(d);

                            if (newDecision) {
                                decisionGrid.cells.Add(newDecision);
                            }
                        }
                    }
                    decisionGrid.ResetGrid();
                }
            }
        }

        public void ClearDecisions() {
            // Clear grid
            foreach (Transform child in decisionGrid.transform) {
                if (child != decisionGrid.transform) {
                    Destroy(child.gameObject);
                }
            }
            decisionGrid.ClearGrid();
        }
    }
}