using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePhone {
    public class UIContacts : MonoBehaviour {

        public Chat chat;
        public TMPro.TextMeshPro name;
        public TMPro.TextMeshPro message;
        public SpriteRenderer image;
        public GameObject unreadMessageCounter;
        public GameObject background;

        public SpriteRenderer status;
        public Sprite onlineStatus;
        public Sprite offlineStatus;

        public Message lastMessage;

        // Use this for initialization
        public void SetChat (Chat c)
        {
            chat = c;

            name.text = c.name;
            image.sprite = c.image;

            if (!chat.IsPersonalChat())
            {
                status.gameObject.SetActive(false);
            }

            SetUnreadMessageCounter();
        }

        // Update is called once per frame
        void Update() {

            string text;
            if (chat.HasIncommingMessage())
            {
                if (chat.IsPersonalChat())
                {
                    text = "typing...";
                } else
                {
                    text = chat.queue.pendingMessage.contact.name + " is typing...";
                }
            } else
            {
                if (lastMessage != null)
                {
                    if (lastMessage.contact == NarrativeHandler.instance.You)
                    {
                        text = "You: " + lastMessage.message;
                    }
                    else
                    {
                        if (chat.IsPersonalChat())
                        {
                            text = lastMessage.message;
                        }
                        else
                        {
                            text = lastMessage.contact.name + ": " + lastMessage.message;
                        }
                    }
                } else
                {
                    text = "";
                }
            }

            if (chat.HasIncommingMessage() || chat.unreadMessages > 0)
            {
                text = "<#2ECC71>" + text + "</color>";
            }

            message.text = text;

            if (chat.IsPersonalChat())
            {
                SetStatus(chat);
            }

            SetUnreadMessageCounter();
            SetBackground();
        }

        // Set contact online status
        public void SetStatus(Chat activeChat)
        {
            if (activeChat.IsPersonalChat())
            {
                status.gameObject.SetActive(true);
                if (activeChat.contacts[0].state == Contact.State.Online)
                {
                    status.sprite = onlineStatus;
                }
                else
                {
                    status.sprite = offlineStatus;
                }
            }
            else
            {
                status.gameObject.SetActive(false);
            }
        }

        // Set unread message counter
        void SetUnreadMessageCounter()
        {
            if (chat.unreadMessages == 0)
            {
                unreadMessageCounter.SetActive(false);
            }
            else
            {
                unreadMessageCounter.SetActive(true);
                unreadMessageCounter.GetComponentInChildren<TMPro.TextMeshPro>().text = chat.unreadMessages.ToString();
            }
        }

        void SetBackground ()
        {
            if (!chat.story)
            {
                background.SetActive(false);
                return;
            }

            if (chat.queue.pendingMessage)
            {
                if (chat.queue.pendingMessage.contact == NarrativeHandler.instance.You)
                {
                    background.SetActive(true);
                }
                else
                {
                    background.SetActive(false);
                }
                return;
            }

            if (chat.queue.isEmpty && chat.story.finished == false)
            {
                background.SetActive(true);
                return;
            }

            background.SetActive(false);
        }
    }
}
