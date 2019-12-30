using TMPro;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BackendHandler : MonoBehaviour
{
    public static BackendHandler singleton;
    private string currInput, chosenCode, filePath;
    public GameObject numberDisplay, enterKey;
    public List<GameObject> gamePages;
    public Animation vaultAnim;
    private List<string> vaultCodes;
    public GameObject winImage, loseImage;
    public TextMeshProUGUI resultDisplayEng, resultDisplayCh;
    public Texture2D winTicket, loseTicket;

    private void Awake()
    {
        singleton = this;
        currInput = string.Empty;
    }

    private void Start()
    {
        //setup vault code
        filePath = Application.dataPath + "/vaultCodes.txt";
        vaultCodes = new List<string>();
        StreamReader reader = new StreamReader(filePath);
        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();
            vaultCodes.Add(line);
        }
        chosenCode = vaultCodes[Random.Range(0, vaultCodes.Count)];
        Debug.Log(chosenCode);

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
            confirmInput();
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

        if (currInput.Length < 5)
        {
            if (num < 10)
            {
                string newNum = num.ToString();
                currInput = currInput + newNum;
            }
        }

        foreach (Transform d in numberDisplay.transform)
        {
            d.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "_";
            enterKey.GetComponent<Button>().interactable = false;
        }

        if (currInput.Length > 0)
        {
            for (int n = 0; n < currInput.Length; n++)
            {
                int no = int.Parse(currInput[n].ToString());
                numberDisplay.transform.GetChild(n).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = no.ToString();
            }

            if (currInput.Length == 5)
            {
                enterKey.GetComponent<Button>().interactable = true;
            }
        }
    }

    public void confirmInput()
    {
        vaultAnim.Play("VaultOpenAnim");
        StartCoroutine(goToResult(vaultAnim.GetClip("VaultOpenAnim").length));
    }

    public void resetVault()
    {
        SceneManager.LoadScene(0);
    }

    public void printTicket()
    {
        if (winImage.activeInHierarchy)
        {
            PrinterPlugin.print(winTicket, true, PrinterPlugin.PrintScaleMode.PAGE_WIDTH);
        } else if (loseImage.activeInHierarchy)
        {
            PrinterPlugin.print(loseTicket, true, PrinterPlugin.PrintScaleMode.PAGE_WIDTH);
        }
    }

    private void displayResult(bool win)
    {
        winImage.SetActive(false);
        loseImage.SetActive(false);

        if (win)
        {
            winImage.SetActive(true);
            resultDisplayEng.text = "Please collect your printout and proceed in store for verification and collection.";
            resultDisplayCh.text = "谢谢你参加，你得到了安慰奖!\n请收集打印卡，到柜台领取你的奖品！";
        } else
        {
            loseImage.SetActive(true);
            resultDisplayEng.text = "Nice Try, you win a consolation prize! Please collect your printout and proceed in store for assistance.";
            resultDisplayCh.text = "恭喜你！你得到了大奖！\n请收集打印卡，到柜台确认和领取你的奖品！";
        }

        gamePages[2].SetActive(false);
        gamePages[3].SetActive(true);
        int child = gamePages[3].transform.childCount;
        gamePages[3].transform.GetChild(child - 1).GetComponent<ObjectFadeInAndOut>().toggleFade();
    }

    private IEnumerator goToResult (float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (currInput == chosenCode)
        {
            displayResult(true);
        }
        else
        {
            displayResult(false);
        }
    }
}
