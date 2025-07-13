using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonGroup : MonoBehaviour
{
    public bool customSprite = false;

    public List<Button> buttons;
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color defaultColor = Color.white;

    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite defaultSprite;

    private Button selectedButton;

    private void OnEnable()
    {
        OnButtonClicked(buttons[0]);
    }

    private void Start()
    {
        foreach (var button in buttons)
        {
            button.onClick.AddListener(() => OnButtonClicked(button));
        }

        OnButtonClicked(buttons[0]);
    }

    private void OnButtonClicked(Button clickedButton)
    {
        if (selectedButton != null)
        {
            foreach (var btn in buttons)
            {
                if (btn != clickedButton)
                {
                    SetButtonSelected(btn, false);
                }
            }
            // Reset previous button appearance
        }

        selectedButton = clickedButton;
        SetButtonSelected(selectedButton, true);
    }

    private void SetButtonSelected(Button button, bool isSelected)
    {



        if (customSprite)
        {
            button.image.sprite = isSelected ? selectedSprite : defaultSprite;
        }
        else
        {
            var colors = button.colors;
            colors.normalColor = isSelected ? selectedColor : defaultColor;
            button.colors = colors;
        }
        // Example: change button color or interactability

    }


}
