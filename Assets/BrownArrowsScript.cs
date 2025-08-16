using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrownArrowsScript : MonoBehaviour {

    public KMAudio audio;
    public KMBombInfo bomb;
    public KMColorblindMode colorblind;

    public KMSelectable[] buttons;
    public Material[] colors;
    public GameObject numDisplay;
    public GameObject colorblindText;
    public GameObject[] displayObjs;

    private int heldItem;
    private int stage;
    private int curPosition;
    private int enemyPosition = -999;
    private int keyPosition = -1;
    private int chestPosition;
    private int[] rockPositions = { -1, -1, -1 };

    private bool lightsOn = false;
    private bool isAnimating = false;
    private bool willStrike = false;
    private bool treasureHunt = false;
    private bool colorblindActive = false;

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        moduleSolved = false;
        colorblindActive = colorblind.ColorblindModeActive;
        foreach (KMSelectable obj in buttons){
            KMSelectable pressed = obj;
            pressed.OnInteract += delegate () { PressButton(pressed); return false; };
        }
        GetComponent<KMBombModule>().OnActivate += OnActivate;
    }

    void Update()
    {
        if (moduleSolved != true && lightsOn == true)
        {
            if (curPosition == chestPosition && curPosition != enemyPosition)
                displayObjs[0].SetActive(true);
            else
                displayObjs[0].SetActive(false);
            if (curPosition == rockPositions[0] && curPosition != enemyPosition)
                displayObjs[1].SetActive(true);
            else
                displayObjs[1].SetActive(false);
            if (curPosition == rockPositions[1] && curPosition != enemyPosition)
                displayObjs[16].SetActive(true);
            else
                displayObjs[16].SetActive(false);
            if (curPosition == rockPositions[2] && curPosition != enemyPosition)
                displayObjs[17].SetActive(true);
            else
                displayObjs[17].SetActive(false);
            if (curPosition == enemyPosition)
                displayObjs[2].SetActive(true);
            else
                displayObjs[2].SetActive(false);
            if (curPosition == keyPosition)
                displayObjs[18].SetActive(true);
            else
                displayObjs[18].SetActive(false);
            bool onLeftEdge = GetCoordinate(curPosition).StartsWith("A");
            bool onRightEdge = GetCoordinate(curPosition).StartsWith("F");
            bool onTopEdge = GetCoordinate(curPosition).EndsWith("1");
            bool onBottomEdge = GetCoordinate(curPosition).EndsWith("6");
            if (enemyPosition == curPosition - 7 && !onTopEdge && !onLeftEdge)
                displayObjs[3].SetActive(true);
            else
                displayObjs[3].SetActive(false);
            if (enemyPosition == curPosition - 6 && !onTopEdge)
                displayObjs[4].SetActive(true);
            else
                displayObjs[4].SetActive(false);
            if (enemyPosition == curPosition - 5 && !onTopEdge && !onRightEdge)
                displayObjs[5].SetActive(true);
            else
                displayObjs[5].SetActive(false);
            if (enemyPosition == curPosition + 1 && !onRightEdge)
                displayObjs[6].SetActive(true);
            else
                displayObjs[6].SetActive(false);
            if (enemyPosition == curPosition + 7 && !onBottomEdge && !onRightEdge)
                displayObjs[7].SetActive(true);
            else
                displayObjs[7].SetActive(false);
            if (enemyPosition == curPosition + 6 && !onBottomEdge)
                displayObjs[8].SetActive(true);
            else
                displayObjs[8].SetActive(false);
            if (enemyPosition == curPosition + 5 && !onBottomEdge && !onLeftEdge)
                displayObjs[9].SetActive(true);
            else
                displayObjs[9].SetActive(false);
            if (enemyPosition == curPosition - 1 && !onLeftEdge)
                displayObjs[10].SetActive(true);
            else
                displayObjs[10].SetActive(false);
            if (onTopEdge)
                displayObjs[12].SetActive(true);
            else
                displayObjs[12].SetActive(false);
            if (onLeftEdge)
                displayObjs[13].SetActive(true);
            else
                displayObjs[13].SetActive(false);
            if (onRightEdge)
                displayObjs[14].SetActive(true);
            else
                displayObjs[14].SetActive(false);
            if (onBottomEdge)
                displayObjs[15].SetActive(true);
            else
                displayObjs[15].SetActive(false);
        }
    }

    void Start()
    {
        stage = 0;
        heldItem = -1;
        if (bomb.GetModuleNames().Contains("Treasure Hunt"))
            treasureHunt = true;
        chestPosition = UnityEngine.Random.Range(0, 36);
        List<int> used = new List<int>();
        used.Add(chestPosition);
        if (!treasureHunt)
        {
            for (int i = 0; i < 3; i++)
            {
                int choice = UnityEngine.Random.Range(0, 36);
                while (used.Contains(choice)) choice = UnityEngine.Random.Range(0, 36);
                rockPositions[i] = choice;
                used.Add(choice);
            }
        }
        else
        {
            int choice = UnityEngine.Random.Range(0, 36);
            while (used.Contains(choice)) choice = UnityEngine.Random.Range(0, 36);
            keyPosition = choice;
        }
        curPosition = UnityEngine.Random.Range(0, 36);
        if (!treasureHunt)
        {
            int choice2 = UnityEngine.Random.Range(0, 36);
            while (curPosition == choice2) choice2 = UnityEngine.Random.Range(0, 36);
            enemyPosition = choice2;
        }
        Debug.LogFormat("[Brown Arrows #{0}] The chest is located at {1}", moduleId, GetCoordinate(chestPosition));
        Debug.LogFormat("[Brown Arrows #{0}] You are starting at {1}", moduleId, GetCoordinate(curPosition));
        if (!treasureHunt)
        {
            Debug.LogFormat("[Brown Arrows #{0}] The rocks are located at {1}, {2} and {3}", moduleId, GetCoordinate(rockPositions[0]), GetCoordinate(rockPositions[1]), GetCoordinate(rockPositions[2]));
            Debug.LogFormat("[Brown Arrows #{0}] The creature is starting at {1}", moduleId, GetCoordinate(enemyPosition));
        }
        else
        {
            Debug.LogFormat("[Brown Arrows #{0}] A Treasure Hunt module is present", moduleId);
            Debug.LogFormat("[Brown Arrows #{0}] The key is located at {1}", moduleId, GetCoordinate(keyPosition));
        }
    }

    void OnActivate()
    {
        lightsOn = true;
        if (colorblindActive)
            colorblindText.SetActive(true);
    }

    void PressButton(KMSelectable pressed)
    {
        if (moduleSolved != true && lightsOn == true && willStrike != true)
        {
            pressed.AddInteractionPunch(0.25f);
            audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, pressed.transform);
            int index = Array.IndexOf(buttons, pressed);
            if (index == 4)
            {
                if (heldItem == -1)
                {
                    if (!treasureHunt)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (rockPositions[i] == curPosition)
                            {
                                heldItem = i;
                                Debug.LogFormat("[Brown Arrows #{0}] Picked up the rock at {1}", moduleId, GetCoordinate(rockPositions[heldItem]));
                                rockPositions[i] = -1;
                                break;
                            }
                            if (i == 2)
                            {
                                GetComponent<KMBombModule>().HandleStrike();
                                if (curPosition == chestPosition)
                                    Debug.LogFormat("[Brown Arrows #{0}] Attempted to hit the chest with nothing, strike", moduleId);
                                else
                                    Debug.LogFormat("[Brown Arrows #{0}] Attempted to pick up nothing, strike", moduleId);
                                Start();
                            }
                        }
                    }
                    else
                    {
                        if (keyPosition == curPosition)
                        {
                            heldItem = 3;
                            keyPosition = -1;
                            Debug.LogFormat("[Brown Arrows #{0}] Picked up the key", moduleId);
                        }
                        else
                        {
                            GetComponent<KMBombModule>().HandleStrike();
                            if (curPosition == chestPosition)
                                Debug.LogFormat("[Brown Arrows #{0}] Attempted to open the chest with nothing, strike", moduleId);
                            else
                                Debug.LogFormat("[Brown Arrows #{0}] Attempted to pick up nothing, strike", moduleId);
                            Start();
                        }
                    }
                }
                else if (curPosition == chestPosition)
                {
                    if (!treasureHunt)
                    {
                        audio.PlaySoundAtTransform("break" + UnityEngine.Random.Range(1, 3), transform);
                        Debug.LogFormat("[Brown Arrows #{0}] You hit the chest, it has been hit {1} time{2}", moduleId, stage + 1, (stage + 1) > 1 ? "s" : "");
                    }
                    else
                        audio.PlaySoundAtTransform("key", transform);
                    heldItem = -1;
                    stage++;
                    if (stage == 3 || (stage == 1 && treasureHunt))
                    {
                        moduleSolved = true;
                        for (int i = 0; i < displayObjs.Length; i++)
                            displayObjs[i].SetActive(false);
                        StartCoroutine(victory());
                        Debug.LogFormat("[Brown Arrows #{0}] You {1} the chest and got the treasure, module disarmed", moduleId, treasureHunt ? "opened" : "broke");
                    }
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (rockPositions[i] == curPosition)
                            break;
                        if (i == 2)
                        {
                            GetComponent<KMBombModule>().HandleStrike();
                            Debug.LogFormat("[Brown Arrows #{0}] Attempted to pick up nothing, strike", moduleId);
                            Start();
                        }
                    }
                }
                return;
            }
            if (index == 0)
            {
                if (!GetCoordinate(curPosition).StartsWith("A"))
                {
                    curPosition--;
                    Debug.LogFormat("<Brown Arrows #{0}> You moved left to {1}", moduleId, GetCoordinate(curPosition));
                }
                else
                    Debug.LogFormat("<Brown Arrows #{0}> You tried moving left but hit a wall", moduleId);
            }
            else if (index == 1)
            {
                if (!GetCoordinate(curPosition).StartsWith("F"))
                {
                    curPosition++;
                    Debug.LogFormat("<Brown Arrows #{0}> You moved right to {1}", moduleId, GetCoordinate(curPosition));
                }
                else
                    Debug.LogFormat("<Brown Arrows #{0}> You tried moving right but hit a wall", moduleId);
            }
            else if (index == 2)
            {
                if (!GetCoordinate(curPosition).EndsWith("1"))
                {
                    curPosition -= 6;
                    Debug.LogFormat("<Brown Arrows #{0}> You moved up to {1}", moduleId, GetCoordinate(curPosition));
                }
                else
                    Debug.LogFormat("<Brown Arrows #{0}> You tried moving up but hit a wall", moduleId);
            }
            else if (index == 3)
            {
                if (!GetCoordinate(curPosition).EndsWith("6"))
                {
                    curPosition += 6;
                    Debug.LogFormat("<Brown Arrows #{0}> You moved down to {1}", moduleId, GetCoordinate(curPosition));
                }
                else
                    Debug.LogFormat("<Brown Arrows #{0}> You tried moving down but hit a wall", moduleId);
            }
            if (curPosition == enemyPosition)
            {
                StartCoroutine(StrikeAnim());
                return;
            }
            if (!treasureHunt)
                EnemyMove(index);
        }
    }

    void EnemyMove(int index)
    {
        redo:
        int choice = UnityEngine.Random.Range(0, 4);
        if (choice == 0)
        {
            if (!GetCoordinate(enemyPosition).StartsWith("A"))
            {
                if (curPosition == enemyPosition - 1 && index == 1)
                    goto redo;
                enemyPosition--;
                Debug.LogFormat("<Brown Arrows #{0}> The creature moved left to {1}", moduleId, GetCoordinate(enemyPosition));
            }
            else
                Debug.LogFormat("<Brown Arrows #{0}> The creature tried moving left but hit a wall", moduleId);
        }
        else if (choice == 1)
        {
            if (!GetCoordinate(enemyPosition).StartsWith("F"))
            {
                if (curPosition == enemyPosition + 1 && index == 0)
                    goto redo;
                enemyPosition++;
                Debug.LogFormat("<Brown Arrows #{0}> The creature moved right to {1}", moduleId, GetCoordinate(enemyPosition));
            }
            else
                Debug.LogFormat("<Brown Arrows #{0}> The creature tried moving right but hit a wall", moduleId);
        }
        else if (choice == 2)
        {
            if (!GetCoordinate(enemyPosition).EndsWith("1"))
            {
                if (curPosition == enemyPosition - 6 && index == 3)
                    goto redo;
                enemyPosition -= 6;
                Debug.LogFormat("<Brown Arrows #{0}> The creature moved up to {1}", moduleId, GetCoordinate(enemyPosition));
            }
            else
                Debug.LogFormat("<Brown Arrows #{0}> The creature tried moving up but hit a wall", moduleId);
        }
        else if (choice == 3)
        {
            if (!GetCoordinate(enemyPosition).EndsWith("6"))
            {
                if (curPosition == enemyPosition + 6 && index == 2)
                    goto redo;
                enemyPosition += 6;
                Debug.LogFormat("<Brown Arrows #{0}> The creature moved down to {1}", moduleId, GetCoordinate(enemyPosition));
            }
            else
                Debug.LogFormat("<Brown Arrows #{0}> The creature tried moving down but hit a wall", moduleId);
        }
        if (curPosition == enemyPosition)
        {
            StartCoroutine(StrikeAnim());
            return;
        }
    }

    string GetCoordinate(int number)
    {
        string[] letters = { "A", "B", "C", "D", "E", "F" };
        return letters[number % 6] + ((number / 6) + 1);
    }

    private IEnumerator StrikeAnim()
    {
        willStrike = true;
        audio.PlaySoundAtTransform("creature" + UnityEngine.Random.Range(1, 4), transform);
        yield return new WaitForSeconds(1f);
        displayObjs[11].SetActive(true);
        audio.PlaySoundAtTransform("splat", transform);
        yield return new WaitForSeconds(1f);
        displayObjs[11].SetActive(false);
        GetComponent<KMBombModule>().HandleStrike();
        Debug.LogFormat("[Brown Arrows #{0}] The creature found you at {1}, strike", moduleId, GetCoordinate(curPosition));
        Start();
        willStrike = false;
    }

    private IEnumerator victory()
    {
        isAnimating = true;
        for (int i = 0; i < 100; i++)
        {
            int rand1 = UnityEngine.Random.Range(0, 10);
            if (i < 50)
            {
                numDisplay.GetComponent<TextMesh>().text = rand1 + "";
            }
            else
            {
                numDisplay.GetComponent<TextMesh>().text = "G" + rand1;
            }
            yield return new WaitForSeconds(0.025f);
        }
        numDisplay.GetComponent<TextMesh>().text = "GG";
        isAnimating = false;
        GetComponent<KMBombModule>().HandlePass();
    }

    //twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} up/down/left/right/screen [Presses the specified arrow button or the display screen] | Chain commands using spaces | Inputs can be simplified to one letter (Ex. right as r)";
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] parameters = command.Split(' ');
        var buttonsToPress = new List<KMSelectable>();
        foreach (string param in parameters)
        {
            if (param.EqualsIgnoreCase("up") || param.EqualsIgnoreCase("u"))
                buttonsToPress.Add(buttons[2]);
            else if (param.EqualsIgnoreCase("down") || param.EqualsIgnoreCase("d"))
                buttonsToPress.Add(buttons[3]);
            else if (param.EqualsIgnoreCase("left") || param.EqualsIgnoreCase("l"))
                buttonsToPress.Add(buttons[0]);
            else if (param.EqualsIgnoreCase("right") || param.EqualsIgnoreCase("r"))
                buttonsToPress.Add(buttons[1]);
            else if (param.EqualsIgnoreCase("screen") || param.EqualsIgnoreCase("s"))
                buttonsToPress.Add(buttons[4]);
            else
                yield break;
        }

        yield return null;
        foreach (KMSelectable btn in buttonsToPress)
        {
            btn.OnInteract();
            if (moduleSolved)
            {
                yield return "solve";
                break;
            }
            else if (willStrike)
            {
                yield return "strike";
                break;
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        while (!lightsOn) { yield return true; };
        if (willStrike)
        {
            StopAllCoroutines();
            moduleSolved = true;
            for (int i = 0; i < displayObjs.Length; i++)
                displayObjs[i].SetActive(false);
            StartCoroutine(victory());
        }
        else
        {
            if (treasureHunt)
            {
                while (!moduleSolved)
                {
                    if ((heldItem != -1 && curPosition == chestPosition) || curPosition == keyPosition)
                        buttons[4].OnInteract();
                    else if (heldItem != -1)
                        buttons[GetBestDirection(chestPosition)].OnInteract();
                    else
                        buttons[GetBestDirection(keyPosition)].OnInteract();
                    yield return new WaitForSeconds(.1f);
                }
            }
            else
            {
                while (!moduleSolved)
                {
                    if ((heldItem != -1 && curPosition == chestPosition) || (heldItem == -1 && (curPosition == rockPositions[0] || curPosition == rockPositions[1] || curPosition == rockPositions[2])))
                        buttons[4].OnInteract();
                    else if (heldItem != -1)
                        buttons[GetBestDirection(chestPosition)].OnInteract();
                    else if (rockPositions[0] != -1)
                        buttons[GetBestDirection(rockPositions[0])].OnInteract();
                    else if (rockPositions[1] != -1)
                        buttons[GetBestDirection(rockPositions[1])].OnInteract();
                    else
                        buttons[GetBestDirection(rockPositions[2])].OnInteract();
                    yield return new WaitForSeconds(.1f);
                }
            }
        }
        while (isAnimating) { yield return true; };
    }

    int GetBestDirection(int goal)
    {
        List<int> validDirs = new List<int>() { 0, 1, 2, 3 };
        if (displayObjs[3].activeSelf)
        {
            validDirs.Remove(0);
            validDirs.Remove(2);
        }
        else if (displayObjs[4].activeSelf)
            validDirs.Remove(2);
        else if (displayObjs[5].activeSelf)
        {
            validDirs.Remove(1);
            validDirs.Remove(2);
        }
        else if (displayObjs[6].activeSelf)
            validDirs.Remove(1);
        else if (displayObjs[7].activeSelf)
        {
            validDirs.Remove(1);
            validDirs.Remove(3);
        }
        else if (displayObjs[8].activeSelf)
            validDirs.Remove(3);
        else if (displayObjs[9].activeSelf)
        {
            validDirs.Remove(0);
            validDirs.Remove(3);
        }
        else if (displayObjs[10].activeSelf)
            validDirs.Remove(0);
        if (displayObjs[12].activeSelf)
            validDirs.Remove(2);
        if (displayObjs[13].activeSelf)
            validDirs.Remove(0);
        if (displayObjs[14].activeSelf)
            validDirs.Remove(1);
        if (displayObjs[15].activeSelf)
            validDirs.Remove(3);
        if (goal % 6 < curPosition % 6 && validDirs.Contains(0))
            return 0;
        else if (goal % 6 > curPosition % 6 && validDirs.Contains(1))
            return 1;
        else if (goal / 6 < curPosition / 6 && validDirs.Contains(2))
            return 2;
        else if (goal / 6 > curPosition / 6 && validDirs.Contains(3))
            return 3;
        if (validDirs.Count == 0)
        {
            if (displayObjs[12].activeSelf)
                validDirs.Add(2);
            if (displayObjs[13].activeSelf)
                validDirs.Add(0);
            if (displayObjs[14].activeSelf)
                validDirs.Add(1);
            if (displayObjs[15].activeSelf)
                validDirs.Add(3);
        }
        return validDirs.PickRandom();
    }
}