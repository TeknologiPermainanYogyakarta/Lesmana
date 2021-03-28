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

    public void SetText(string _text, string _actor)
    {
        chat.text = _text;
        actor.text = _actor;
    }

    public void CloseDialogue()
    {
        panel.gameObject.SetActive(false);
    }
}