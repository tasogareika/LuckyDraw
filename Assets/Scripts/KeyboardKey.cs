using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KeyboardKey : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    private Button thisButton;
    public string keyData, altKeyData;
    public bool isShift, isSpace, isBackSpace, isClear, isAuto, shiftPressed;

    private void Start()
    {
        if (!isShift && !isSpace && !isBackSpace & !isClear & !isAuto)
        {
            textDisplay = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        }

        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(delegate { keyPressed(); });
    }

    public void keyPressed()
    {
        if (!isSpecialKey())
        {
            //input character
            KeyboardHandler.singleton.textEntry(textDisplay.text.ToString());
        } else
        {
            if (isShift)
            {
                if (!shiftPressed)
                {
                    shiftPressed = true;
                } else
                {
                    shiftPressed = false;
                }

                KeyboardHandler.singleton.shiftPressed(shiftPressed);
            } else if (isBackSpace)
            {
                KeyboardHandler.singleton.backSpace();
            } else if (isSpace)
            {
                KeyboardHandler.singleton.letterSpace();
            } else if (isClear)
            {
                KeyboardHandler.singleton.clearInput();
            } else if (isAuto)
            {
                KeyboardHandler.singleton.autoEmailFill();
            }
        }
    }

    private bool isSpecialKey()
    {
        if (!isShift && !isSpace && !isBackSpace && !isClear && !isAuto)
        {
            return false;
        } else
        {
            return true;
        }
    }
}
