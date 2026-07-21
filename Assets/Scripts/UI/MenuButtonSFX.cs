using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[DefaultExecutionOrder(-100)]
public sealed class MenuButtonSFX : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private UISoundType clickSound = UISoundType.Confirm;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (button == null)
            button = GetComponent<Button>();

        button.onClick.RemoveListener(PlayClick);
        button.onClick.AddListener(PlayClick);
    }

    private void OnDisable()
    {
        if (button != null)
            button.onClick.RemoveListener(PlayClick);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button != null && button.IsInteractable())
            UISoundManager.Instance?.PlayHover();
    }

    private void PlayClick()
    {
        UISoundManager.Instance?.Play(clickSound);
    }
}
