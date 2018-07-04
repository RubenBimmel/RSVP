using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePhone {

    public class MessageQueue {

        public bool isEmpty { get { return (pendingMessage == null && queue.Count == 0); } }

        public Message pendingMessage;
        private List<Message> queue;

        public void Update() {
            if (pendingMessage)
            {
                if (pendingMessage.contact != NarrativeHandler.instance.You) 
                {
                    if (NarrativeHandler.instance.debugMode || (pendingMessage.contact.state != Contact.State.Notified && GameManager.time > pendingMessage.time)) {
                        SendNextInQueue();
                    }
                }
            }
            else if (queue.Count > 0) 
            {
                pendingMessage = queue[0];
                pendingMessage.time = GameManager.time + Mathf.Max(0f, pendingMessage.contact.readDelay) + pendingMessage.delay;
                queue.RemoveAt(0);

                NarrativeHandler.instance.ResetMessageScreen(false, false, false, true);
            }
        }

        public void AddToQueue (Message m)
        {
            queue.Add(m);
        }

        public void SendNextInQueue ()
        {
            // pendingMessage needs to be null so that the game can check if this was the last message
            Message m = pendingMessage;
            pendingMessage = null;
            NarrativeHandler.instance.Send(m);
        }

        public void ResetQueue ()
        {
            queue = new List<Message>();
            pendingMessage = null;
        }
    }
}