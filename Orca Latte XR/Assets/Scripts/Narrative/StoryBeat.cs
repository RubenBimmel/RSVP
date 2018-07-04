using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GamePhone;

[CreateAssetMenu(fileName = "NewStoryBeat", menuName = "GamePhone/StoryBeat")]
public class StoryBeat : ScriptableObject {
    public Chat chat;
    public TextAsset story;
    public UnityEvent OnFinishedStory;

    public void Activate ()
    {
        chat.SetStory(story);
        chat.story.storyBeat = this;
        //chat.story.CheckIfFinished();
    }
}
