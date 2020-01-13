using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KeyboardHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject qwertyKeyboard, numpad;
    private string emailFront, endString, frontString;
    public static KeyboardHandler singleton;
    public bool isOnScreen, touchingKeys, middleCaret;
    [HideInInspector] public List<GameObject> keyboardKeys;
    public List<string> emailDomains;
    [HideInInspector] public TMP_InputField currInput;
    [HideInInspector] public int cursorPos;
    private int emailNo;

    private void Awake()
    {
        singleton = this;
    }

    private void Start()
    {
        KeyboardKey[] keys = transform.GetComponentsInChildren<KeyboardKey>();
        foreach (var k in keys)
        {
            keyboardKeys.Add(k.gameObject);
        }
        isOnScreen = false;
        touchingKeys = false;
        middleCaret = false;
        numpad.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        numpad.SetActive(false);
        gameObject.SetActive(false);
    }

    public void shiftPressed(bool on)
    {
        emailNo = 0;
        foreach (var k in keyboardKeys)
        {
            KeyboardKey key = k.GetComponent<KeyboardKey>();
            if (!key.isBackSpace && !key.isShift && !key.isSpace && !key.isClear && !key.isAuto)
            {
                if (on)
                {
                    key.textDisplay.text = key.altKeyData.ToString();
                }
                else
                {
                    key.textDisplay.text = key.keyData.ToString();
                }
            }
        }
    }

    public void OnPointerEnter (PointerEventData pointer)
    {
        touchingKeys = true;
    }

    public void OnPointerExit (PointerEventData pointer)
    {
        touchingKeys = false;
    }

    public void hideKeyboard()
    {
        isOnScreen = false;
        gameObject.SetActive(false);
    }

    public void showKeyboard()
    {
        emailNo = -1;
        isOnScreen = true;
        gameObject.SetActive(true);
    }

    public void textEntry(string s)
    {
        if (currInput != null)
        {
            emailNo = -1;
            if (currInput.text.Length != 0)
            {
                endString = currInput.text.Substring(cursorPos);
                frontString = currInput.text.Substring(0, cursorPos);
            }

            if (currInput.name != "MobileInput")
            {
                if (middleCaret)
                {
                    if (currInput.characterLimit > currInput.text.Length)
                    {
                        currInput.text = frontString + s + endString;
                        cursorPos++;
                    }
                } else
                {
                    if (currInput.characterLimit > currInput.text.Length)
                    {
                        currInput.text += s;
                        cursorPos = currInput.text.Length;
                    }
                }
            } else
            {
                int i;
                if (int.TryParse(s, out i))
                {
                    if (middleCaret)
                    {
                        currInput.text = frontString + s + endString;
                        cursorPos++;
                    }
                    else
                    {
                        currInput.text += s;
                        cursorPos = currInput.text.Length;
                    }
                }
            }
        }
    }

    public void backSpace()
    {
        if (currInput != null)
        {
            emailNo = -1;
            string currText = currInput.text.ToString();
            endString = currText.Substring(cursorPos);
            frontString = currText.Substring(0, cursorPos);

            if (currText.Length > 1)
            {
                if (cursorPos != 0)
                {
                    frontString = frontString.Remove(frontString.Length - 1);
                    currText = frontString + endString;
                    cursorPos--;
                }
            } else
            {
                currText = string.Empty;
                middleCaret = false;
                cursorPos = 0;
            }

            if (!currText.Contains("g"))
            {
                currText = currText + "g";
                cursorPos = 0;
            }

            currInput.text = currText;

            if (currText.Length < 2)
            {
                middleCaret = true;
                cursorPos = 0;
            }
        }
    }

    public void letterSpace()
    {
        if (currInput != null)
        {
            emailNo = -1;
            endString = currInput.text.Substring(cursorPos);
            frontString = currInput.text.Substring(0, cursorPos);
            if (middleCaret)
            {
                currInput.text = frontString + " " + endString;
                cursorPos++;
            }
            else
            {
                currInput.text += " ";
                cursorPos = currInput.text.Length;
            }
        }
    }

    public void clearInput()
    {
        if (currInput != null)
        {
            emailNo = -1;
            currInput.text = "g";
            middleCaret = true;
            cursorPos = 0;
        }
    }

    public void autoEmailFill()
    {
        if (currInput != null)
        {
            if (currInput.name == "EmailInput")
            {
                if (emailNo < 0)
                {
                    if (currInput.text.Contains("@"))
                    {
                        int i = currInput.text.IndexOf('@');
                        emailFront = currInput.text.Substring(0, i);
                    }
                    else
                    {
                        emailFront = currInput.text;
                    }
                }

                emailNo++;
                if (emailNo > emailDomains.Count - 1)
                {
                    emailNo = 0;
                }

                currInput.text = emailFront + emailDomains[emailNo];
                cursorPos = currInput.text.Length;
            }
        }
    }

    public void numberToggle(bool show)
    {
        if (show)
        {
            foreach (var k in keyboardKeys)
            {
                var key = k.GetComponent<KeyboardKey>();
                if (!key.isBackSpace && !key.isShift && !key.isSpace && !key.isClear && !key.isAuto)
                {
                    int i;
                    if (!int.TryParse(key.keyData, out i))
                    {
                        k.GetComponent<Button>().interactable = false;
                    }
                    numpad.SetActive(true);
                    qwertyKeyboard.SetActive(false);
                }
            }
        }
        else
        {
            foreach (var k in keyboardKeys)
            {
                k.GetComponent<Button>().interactable = true;
            }
            numpad.SetActive(false);
            qwertyKeyboard.SetActive(true);
        }
    }
}
