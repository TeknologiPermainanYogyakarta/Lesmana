using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class StoryManager : Singleton<StoryManager>
{
    [SerializeField]
    private DialogueController ui;

    [SerializeField]
    private DialogueMasterData dialogueData;

    private bool _paused = false;
    private Coroutine _storyCoroutine;

    [ContextMenu("Start Story")]

    public void test()
    {
        StartStory(0);
    }

    [ContextMenu("Next")]
    public void NextDialogue()
    {
        _paused = false;
    }

    public void StartStory(int _chapter)
    {
        if (_storyCoroutine != null)
        {
            Debug.LogError("Story still running!");
            return;
        }

        ChapterData selectedChapter = dialogueData.StoryList[0].Chapter[_chapter];

        ui.StartDialogue();
        _storyCoroutine = StartCoroutine(StartStory(selectedChapter, OnStoryDone));
    }

    private IEnumerator StartStory(ChapterData chapterData, UnityAction onStoryDone)
    {
        for (int i = 0; i < chapterData.Chat.Count; i++)
        {
            _paused = true;
            string _text = chapterData.Chat[i].chat;
            string _actor = chapterData.Chat[i].actorName;

            yield return ui.SetText(_text, _actor);

            yield return new WaitUntil(() => !_paused);
        }

        yield return null;
        onStoryDone?.Invoke();
    }

    private void OnStoryDone()
    {
        StopCoroutine(_storyCoroutine);
        _storyCoroutine = null;

        ui.CloseDialogue();
    }
}