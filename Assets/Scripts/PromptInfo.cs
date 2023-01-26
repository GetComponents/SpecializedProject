using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Prompt ", menuName = "ScriptableObjects/PromptInfo", order = 1)]
public class PromptInfo : ScriptableObject
{
    public string SpeakerName;
    [Space]
    public string Prompt;
    public string[] PromptOptions;
    public PromptInfo[] NextPrompts;
    [Space]
    public bool IsProcedural;
    public bool IsLeadingQuestion;
    public bool KnowsName;
    [Space]
    public bool IsDescriptiveText;
    //public PromptInfo DirectPrompt;
}
