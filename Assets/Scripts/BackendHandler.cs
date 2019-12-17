using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackendHandler : MonoBehaviour
{
    public static BackendHandler singleton;
    private string currInput;
    public GameObject numberDisplay, enterKey;
    public List<GameObject> gamePages;
    public List<Sprite> numberSprites;

    private void Awake()
    {
        singleton = this;
        currInput = string.Empty;
    }

    private void Start()
    {
        foreach (var p in gamePages)
        {
            p.SetActive(false);
        }

        gamePages[0].SetActive(true);
    }

    public void goToReady()
    {
        gamePages[0].SetActive(false);
        gamePages[1].SetActive(true);
    }

    public void startGame()
    {
        gamePages[1].SetActive(false);
        gamePages[2].SetActive(true);
        enterKey.GetComponent<Button>().interactable = false;
    }

    public void inputNumber(int num)
    {
        if (num == 11)
        {
            currInput = string.Empty;
        }

        if (num == 12 && currInput.Length > 0)
        {
            if (currInput.Length > 1)
            {
                currInput = currInput.Substring(0, currInput.Length - 1);
            } else if (currInput.Length == 1)
            {
                currInput = string.Empty;
            }
        }

        if (currInput.Length < 4)
        {
            if (num < 10)
            {
                string newNum = num.ToString();
                currInput = currInput + newNum;
            }
        }

        foreach (Transform d in numberDisplay.transform)
        {
            d.GetComponent<Image>().sprite = null;
            enterKey.GetComponent<Button>().interactable = false;
        }

        if (currInput.Length > 0)
        {
            for (int n = 0; n < currInput.Length; n++)
            {
                int no = int.Parse(currInput[n].ToString());
                numberDisplay.transform.GetChild(n).GetComponent<Image>().sprite = numberSprites[no];
            }

            if (currInput.Length == 4)
            {
                enterKey.GetComponent<Button>().interactable = true;
            }
        }
    }

    public void confirmInput()
    {
        Debug.Log(currInput);
    }
}
