using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueController : MonoBehaviour
{
    [SerializeField]
    private Image panel = null;

    [SerializeField]
    private TextMeshProUGUI chat = null;

    [SerializeField]
    private TextMeshProUGUI actor = null;

    [SerializeField]
    private Button nextButton = null;

    private void Start()
    {
        nextButton.onClick.AddListener(() =>
        {
            StoryManager.Instance.NextDialogue();
        });
    }

    public void StartDialogue()
    {
        panel.gameObject.SetActive(true);
    }

    public IEnumerator SetText(string _text, string _actor)
    {
        string showText = "";
        actor.text = _actor;

        foreach (var item in _text.ToCharArray())
        {
            showText += item;
            chat.text = showText;

            yield return null;
        }

        yield return null;
    }

    public void CloseDialogue()
    {
        panel.gameObject.SetActive(false);
    }
}