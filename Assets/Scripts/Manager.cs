using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class Manager : MonoBehaviour
{

    public static Manager m = null;
    private static readonly object padlock = new object();

    public string[] keyActions;
    public string[] keyTrigger;
    public string[] standardKeyTrigger;
    public Dictionary<string, string> keyActionTriggers = new Dictionary<string, string>();

    public string version;

    void Awake()
    {
        lock (padlock)
        { 
            if (m == null)
            {
                m = this;

                const int keyActionCount = 43;
                keyActions = new string[keyActionCount]
                {
                    "Left",        // 1
                    "Right",       // 2
                    "Jump",          // 3
                    "AskNo",           // 4
                    "EditMode",        // 5
                    "Shop1",           // 6
                    "Shop2",           // 7
                    "Buy",             // 8
                    "ExitShop",        // 9
                    "SwipeRight",      // 10
                    "SwipeLeft",       // 11
                    "CancelBuilding",  // 12 (Removed)
                    "RotateRight",     // 13
                    "RotateLeft",      // 14
                    "Transform",       // 15
                    "SetTransform1",   // 16
                    "SetTransform2",   // 17
                    "SetTransform3",   // 18
                    "SetTransform4",   // 19
                    "SetTransform5",   // 20
                    "Undo",            // 21
                    "Redo",            // 22
                    "RepairMode",      // 23
                    "Market",          // 24
                    "Mission",         // 25
                    "QuickTimeEvents", // 26
                    "FactoryHalls",    // 27
                    "FactoryHall1",    // 28
                    "FactoryHall2",    // 29
                    "FactoryHall3",    // 30
                    "FactoryHall4",    // 31
                    "FactoryHall5",    // 32
                    "FactoryHall6",    // 33
                    "FactoryHall7",    // 34
                    "FactoryHall8",    // 35
                    "FactoryHall9",    // 36
                    "FactoryHall10",   // 37
                    "SkipMusic",       // 38
                    "FactorySound",    // 39
                    "EffectSound",     // 40
                    "MusicSound",      // 41
                    "ResetOres",       // 42 (Removed)
                    "RestartGame",     // 43 (Removed)
                };

                standardKeyTrigger = new string[keyActionCount]
                {
                    "ArrowLeft",//1
                    "ArrowRight",//2
                    "W",//3
                    "N",//4
                    "Tab",//5
                    "Q",//6
                    "W",//7
                    "Enter",//8
                    "Backspace",//9
                    "D",//10
                    "A",//11
                    "",//12
                    "ClickRight",//13
                    "",//14
                    "E",//15
                    "F1",//16
                    "F2",//17
                    "F3",//18
                    "F4",//19
                    "F5",//20
                    "A",//21
                    "D",//22
                    "R",//23
                    "M",//24
                    "I",//25
                    "Z",//26
                    "F",//27
                    "1",//28
                    "2",//29
                    "3",//30
                    "4",//31
                    "5",//32
                    "6",//33
                    "7",//34
                    "8",//35
                    "9",//36
                    "0",//37
                    "",//38
                    "",//39
                    "",//40
                    "",//41
                    "",//42
                    "",//43
                };
                keyTrigger = new string[keyActionCount];
                standardKeyTrigger.CopyTo(keyTrigger, 0);
                //for (int i = 0; i < keyActionTrigger.Length; i++)
                //{
                //    if (PlayerPrefs.GetString(version + "_" + "Hotkey" + keyActions[i] + "_Trigger") != "")
                //    {
                //        if (PlayerPrefs.GetString(version + "_" + "Hotkey" + keyActions[i] + "_Trigger") != "--")
                //        {
                //            keyActionTrigger[i] = PlayerPrefs.GetString(version + "_" + "Hotkey" + keyActions[i] + "_Trigger");
                //        }
                //        else
                //        {
                //            keyActionTrigger[i] = "";
                //        }
                //    }
                //}
                for (int i = 0; i < keyActions.Length; i++)
                {
                    keyActionTriggers.Add(keyActions[i], keyTrigger[i]);
                }
            }
            else if (m != this) Destroy(gameObject);
        }
    }


    public void Exit()
    {
        print("Exit");
        Application.Quit();
    }
}
