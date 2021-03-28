using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialgoueData", menuName = "ScriptableObjects/New DialgoueData", order = 1)]
[System.Serializable]
public class DialogueMasterData : ScriptableObject
{
    public List<LevelStoryData> StoryList = new List<LevelStoryData>();
}

[System.Serializable]

public class LevelStoryData
{
    public string StoryName;
    public List<ChapterData> Chapter = new List<ChapterData>();
}

[System.Serializable]

public class ChapterData
{
    public string chapterName;
    public List<ChatData> Chat = new List<ChatData>();
}

[System.Serializable]
public class ChatData
{
    public string chat;
    public string actorName;
    public bool isOnRightSide;
}