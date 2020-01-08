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
    public GameObject numberDisplay, enterKey, peopleBtn;
    public List<GameObject> gamePages;
    public Animation vaultAnim;
    public AudioClip clickSFX;
    public AudioSource BGMPlayer, SFXPlayer;
    private List<string> vaultCodes;
    public GameObject winImage, loseImage;
    public TextMeshProUGUI resultDisplayEng, resultDisplayCh, peopleDisplay;
    public Texture2D winTicket, loseTicket, ticket1, ticket2, tncTicket;
    private int clickNo, peopleNo, giftNo, discountNo, winNo;
    private float clickTime, clickDelay, idleTimer, idleTimerMax;

    private void Awake()
    {
        singleton = this;
        currInput = string.Empty;
    }

    private void Start()
    {
        //set portrait res
        #if UNITY_STANDALONE
        //Screen.SetResolution(720, 1280, false);
        Screen.SetResolution(1080, 1920, true);
        #endif
        SetupVault();
    }

    private void SetupVault()
    {
        clickNo = 0;
        clickDelay = 0.5f;

        idleTimerMax = 15f;
        idleTimer = idleTimerMax;

        togglePeopleBtn(false);

        //setup vault code
        filePath = Application.dataPath + "/vaultCodes.txt";

        vaultCodes = new List<string>();

        StreamReader reader = new StreamReader(filePath);
        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();
            vaultCodes.Add(line);
        }

        #region STATS INFO
        if (PlayerPrefs.HasKey("peopleData"))
        {
            peopleNo = PlayerPrefs.GetInt("peopleData");
        } else
        {
            peopleNo = 0;
        }

        if (PlayerPrefs.HasKey("winData"))
        {
            winNo = PlayerPrefs.GetInt("winData");
        } else
        {
            winNo = 0;
        }

        if (PlayerPrefs.HasKey("discountData"))
        {
            discountNo = PlayerPrefs.GetInt("discountData");
        } else
        {
            discountNo = 0;
        }

        if (PlayerPrefs.HasKey("giftData"))
        {
            giftNo = PlayerPrefs.GetInt("giftData");
        } else
        {
            giftNo = 0;
        }

        if (PlayerPrefs.HasKey("volume"))
        {
            toggleSound();
        } else
        {
            PlayerPrefs.SetInt("volume", 1);
            toggleSound();
        }
        #endregion

        if (PlayerPrefs.HasKey("prevCode"))
        {
            string oldCode = PlayerPrefs.GetString("prevCode");
            if (oldCode.Contains(","))
            {
                vaultCodes.Clear();
            }
            else
            {
                for (int i = 0; i < vaultCodes.Count; i++)
                {
                    if (vaultCodes[i] == oldCode)
                    {
                        vaultCodes.RemoveAt(i);
                    }
                }
            }
        } else
        {
            vaultCodes.RemoveAt(1);
        }

        if (vaultCodes.Count > 0)
        {
            chosenCode = vaultCodes[Random.Range(0, vaultCodes.Count)];
        }

        Debug.Log(chosenCode);

        foreach (var p in gamePages)
        {
            p.SetActive(false);
        }
        gamePages[0].SetActive(true);
    }

    public void goToReady()
    {
        playClickSFX();
        gamePages[0].SetActive(false);
        gamePages[1].SetActive(true);
    }

    public void startGame()
    {
        playClickSFX();
        gamePages[1].SetActive(false);
        gamePages[2].SetActive(true);
        enterKey.GetComponent<Button>().interactable = false;
    }

    public void inputNumber(int num)
    {
        playClickSFX();

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
        
        for (int i = 1; i < numberDisplay.transform.childCount; i++)
        {
            numberDisplay.transform.GetChild(i).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "_";
            enterKey.GetComponent<Button>().interactable = false;
        }

        if (currInput.Length > 0)
        {
            for (int n = 0; n < currInput.Length; n++)
            {
                int no = int.Parse(currInput[n].ToString());
                numberDisplay.transform.GetChild(n + 1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = no.ToString();
            }

            if (currInput.Length == 5)
            {
                enterKey.GetComponent<Button>().interactable = true;
            }
        }
    }

    private void playClickSFX()
    {
        SFXPlayer.clip = clickSFX;
        SFXPlayer.Play();
    }

    public void confirmInput()
    {
        BGMPlayer.Pause();
        if (currInput == chosenCode)
        {
            vaultAnim.Play("VaultOpenAnim");
            StartCoroutine(goToResult(vaultAnim.GetClip("VaultOpenAnim").length));
        } else
        {
            vaultAnim.Play("VaultErrorAnim");
            StartCoroutine(goToResult(vaultAnim.GetClip("VaultErrorAnim").length));
        }
    }

    public void resetVault()
    {
        savePlayerPrefs();

        #region FILE OVERWRITE (UNUSED)
        //ref: https://stackoverflow.com/questions/33646428/c-sharp-overwriting-file-with-streamwriter-created-from-filestream
        //overwriting data file
        /*using (FileStream fs = new FileStream(filePath2, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
        {
            StreamReader sr = new StreamReader(fs);
            using (StreamWriter sw = new StreamWriter(fs))
            {
                fs.SetLength(0);

                foreach (string c in vaultData)
                {
                    sw.WriteLine(c);
                }
                sw.Close();
            }
            sr.Close();
            fs.Close();
        }*/
        #endregion

        SceneManager.LoadScene(0);
    }

    public void printTicket()
    {
        if (winImage.activeInHierarchy)
        {
            PrinterPlugin.print(winTicket, false, PrinterPlugin.PrintScaleMode.PAGE_WIDTH);
        } else if (loseImage.activeInHierarchy)
        {
            PrinterPlugin.print(loseTicket, false, PrinterPlugin.PrintScaleMode.PAGE_WIDTH);
        }
    }

    public void doubleClick()
    {
        clickNo++;
        if (clickNo == 1)
        {
            clickTime = Time.time;
        }

        if (clickNo > 1 && Time.time - clickTime < clickDelay)
        {
            clickNo = 0;
            clickTime = 0;
            togglePeopleBtn(true);
        }
        else if (clickNo > 2 || Time.time - clickTime > 1)
        {
            clickNo = 0;
        }
    }

    public void doubleClickSound()
    {
        clickNo++;
        if (clickNo == 1)
        {
            clickTime = Time.time;
        }

        if (clickNo > 1 && Time.time - clickTime < clickDelay)
        {
            clickNo = 0;
            clickTime = 0;
            int v = PlayerPrefs.GetInt("volume");
            switch (v)
            {
                case 0:
                    PlayerPrefs.SetInt("volume", 1);
                    break;

                case 1:
                    PlayerPrefs.SetInt("volume", 0);
                    break;
            }
            toggleSound();
        }
        else if (clickNo > 2 || Time.time - clickTime > 1)
        {
            clickNo = 0;
        }
    }

    private void togglePeopleBtn(bool show)
    {
        if (!show)
        {
            peopleBtn.GetComponent<Button>().interactable = true;
            peopleDisplay.color = Color.clear;
        } else
        {
            peopleBtn.GetComponent<Button>().interactable = false;
            peopleDisplay.text = "Win: " + winNo.ToString() + "\n$8 Off: " + discountNo.ToString() + "\nFree Gift: " + giftNo.ToString() + "\nTotal: " + peopleNo.ToString();
            peopleDisplay.color = Color.white;
            StartCoroutine(showPeople(2f));
        }
    }

    private void toggleSound()
    {
        int vol = PlayerPrefs.GetInt("volume");
        SFXPlayer.volume = vol;
        BGMPlayer.volume = vol;
    }

    private void savePlayerPrefs()
    {
        PlayerPrefs.SetInt("peopleData", peopleNo);
        PlayerPrefs.SetInt("winData", winNo);
        PlayerPrefs.SetInt("discountData", discountNo);
        PlayerPrefs.SetInt("giftData", giftNo);
        PlayerPrefs.Save();
    }

    private void Update()
    {
        //reset people counter
        if (Input.GetKeyDown(KeyCode.R))
        {
            peopleNo = 0;
            discountNo = 0;
            giftNo = 0;
            winNo = 0;
            PlayerPrefs.DeleteKey("prevCode");
            savePlayerPrefs();
        }

        //timeout during vault code input
        if (gamePages[2].activeInHierarchy)
        {
            if (Input.GetMouseButtonDown(0))
            {
                idleTimer = idleTimerMax;
            }

            if (!vaultAnim.isPlaying)
            {
                if (idleTimer > 0)
                {
                    idleTimer -= Time.deltaTime;
                } else
                {
                    idleTimer = idleTimerMax;
                    resetVault();
                }
            }
        }
    }

    private void displayResult(bool win)
    {
        peopleNo++;
        winImage.SetActive(false);
        loseImage.SetActive(false);

        if (win)
        {
            winNo++;
            if (PlayerPrefs.HasKey("prevCode")) //add onto prev code
            {
                string code = PlayerPrefs.GetString("prevCode");
                code = code + "," + chosenCode;
                PlayerPrefs.SetString("prevCode", code);
            }
            else
            {
                PlayerPrefs.SetString("prevCode", chosenCode);
            }
            winImage.SetActive(true);
            resultDisplayEng.text = "CONGRATULATIONS! You made the right guess and won a 10g gold bar!\n<size=30>*To be redeemed at this terminal</size>";
            resultDisplayCh.text = "恭喜，您猜对了，赢得了999.9纯金金条(10克)\n<size=30>*仅限在此Terminal兑换</size>";
        } else
        {
            loseImage.SetActive(true);

            int r = Random.Range(0, 10);
            if (r <= 4)
            {
                discountNo++;
                loseTicket = ticket1;
                resultDisplayEng.text = "THANK YOU FOR PARTICIPATING. PLEASE PRESENT THIS CARD AT CASHIER TO REDEEM INSTANT $8 OFF.";
                resultDisplayCh.text = "感谢您的参与. 请在柜台出示此卡以享受$ 8折扣！";
            } else
            {
                giftNo++;
                loseTicket = ticket2;
                resultDisplayEng.text = "THANK YOU FOR PARTICIPATING. PLEASE PRESENT THIS CARD AT CASHIER TO REDEEM A FREE GIFT.";
                resultDisplayCh.text = "感谢您的参与. 请在柜台出示此卡以换取精美礼品！";
            }
        }

        gamePages[2].SetActive(false);
        gamePages[3].SetActive(true);

        int child = gamePages[3].transform.childCount;
        gamePages[3].transform.GetChild(child - 1).GetComponent<ObjectFadeInAndOut>().toggleFade();
        StartCoroutine(togglePrint(gamePages[3].transform.GetChild(child - 1).GetComponent<ObjectFadeInAndOut>().timerMax + 1f));
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

    private IEnumerator showPeople (float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        togglePeopleBtn(false);
    }

    private IEnumerator togglePrint(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        printTicket();
        StartCoroutine(resetDelay(20f));
    }

    private IEnumerator resetDelay (float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        resetVault();
    }
}