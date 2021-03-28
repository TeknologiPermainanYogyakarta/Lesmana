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

    private Vector3 originalPos;

    private void Awake()
    {
        originalPos = panel.rectTransform.anchoredPosition3D;
        nextButton.onClick.AddListener(() =>
        {
            StoryManager.Instance.NextDialogue();
        });
    }

    public void StartDialogue()
    {
        panel.rectTransform.anchoredPosition3D = Vector3.down * 100;
        LeanTween.moveY(panel.gameObject, originalPos.y, 1).setEaseOutQuint();
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
        LeanTween.moveY(panel.gameObject, -100, 0.5f).setEaseInQuint().setOnComplete(() =>
        {
            panel.gameObject.SetActive(false);
        });
    }
}