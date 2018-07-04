using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Story {
    public string name;
    public Passage[] passages;
    public int currentPassage;
    public int startPassage;
    public bool finished;
    public StoryBeat storyBeat;

    public void ResetStory () {
        currentPassage = startPassage;
    }

    public void MakeDecision (int id) {
        currentPassage = id;
        //CheckIfFinished();
    }

    /*public void CheckIfFinished ()
    {
        if (GetDecisions().Length == 0)
        {
            finished = true;
            if (storyBeat)
            {
                NarrativeHandler.instance.OnFinishedStory(storyBeat);
            }
        }
    }*/

    public string[] GetMessages () {
        return passages[currentPassage - 1].GetMessages();
    }

    public Decision[] GetDecisions() {
        return passages[currentPassage - 1].decisions;
    }

    public static implicit operator bool(Story story)
    {
        try { return story.passages.Length > 0; }
        catch { return false; }
    }
}