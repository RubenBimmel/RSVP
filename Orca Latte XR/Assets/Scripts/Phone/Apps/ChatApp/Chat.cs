using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePhone {
    [CreateAssetMenu(fileName = "NewContact", menuName = "GamePhone/Chat")]
    public class Chat : ScriptableObject {
        [HideInInspector]
        public Story story;
        public Sprite image;
        public Contact[] contacts;
        [HideInInspector]
        public MessageQueue queue;
        [HideInInspector]
        public int unreadMessages;
        public TextAsset preconversation;

        public bool IsPersonalChat () {
            return contacts.Length <= 2;
        }

        public bool HasIncommingMessage () {
            if (!queue.pendingMessage) {
                return false;
            }
            if (queue.pendingMessage.contact.state != Contact.State.Online) {
                return false;
            }
            return queue.pendingMessage.contact != NarrativeHandler.instance.You && queue.pendingMessage.contact.finishedReading;
        }

        public Message LastMessage ()
        {
            string path = Application.persistentDataPath + "/Resources/ChatApp/Messages/" + name;
            if (System.IO.Directory.Exists(path))
            {
                Message[] messages = SOSaver.LoadAll<Message>(path);
                if (messages.Length > 0)
                {
                    return messages[messages.Length - 1];
                }
            }
            return null;
        }

        public void CheckIfFinished()
        {
            if (story)
            {
                if (story.GetDecisions().Length == 0 && queue.isEmpty)
                {
                    story.finished = true;
                    if (story.storyBeat)
                    {
                        NarrativeHandler.instance.OnFinishedStory(story.storyBeat);
                    }
                }
            }
        }

        public void LoadPreConversation () {
            if (preconversation) {
                string conversation = System.Text.Encoding.Default.GetString(preconversation.bytes);
                string[] messages = conversation.Split('\n');
                foreach (string m in messages) {
                    NarrativeHandler.instance.SendMessage(this, m, true);
                }
            }
        }

        public void SetStory (TextAsset newStory)
        {
            string json = System.Text.Encoding.Default.GetString(newStory.bytes);
            story = Twine.ImportStory(json);
            NarrativeHandler.instance.UpdatePassage(this);
        }
    }
}
