using OpenAi.Unity.V1;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInputfield : MonoBehaviour
{
    //public InputField Input;
    public TMP_InputField PlayerInput, DebugInput;
    [SerializeField]
    OpenAiCompleterV1 analyser;
    public Image LoadingIcon;

    public string LastString;

    [SerializeField]
    private string[] winStrings;

    public GameObject answerButton, continueButton, inputBox;
    private string optionSelected
    {
        get => m_optionSelected;
        set
        {
            m_optionSelected = value;
            ValidateAIOutput();
        }
    }
    private string m_optionSelected;

    [SerializeField]
    private bool isDebugging;

    private void Start()
    {
        DebugInput.gameObject.SetActive(isDebugging);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && PlayerInput.text != "")
        {
            DoApiCompletion();
        }
    }

    public void DoApiCompletion()
    {
        LastString = PlayerInput.text;
        if (CheckForWin())
        {
            return;
        }
        string text;

        if (isDebugging && DebugInput.text != "")
        {
            optionSelected = DebugInput.text;
            DebugInput.text = "";
            return;
        }
        else
        {
            text = BuildAIAnalyzeString(PlayerInput.text, StoryManager.Instance.CurrentPrompt.PromptOptions);
        }
        if (string.IsNullOrEmpty(text))
        {
            Debug.LogError("Example requires input in input field");
            return;
        }
        LoadingIcon.enabled = true;
        analyser.Complete(
            text,
            s => optionSelected = s,
            e => Debug.Log($"ERROR: StatusCode: {e.responseCode} - {e.error}")
        );
        PlayerInput.text = "";
        DisableAIUI();
    }

    public void Continue()
    {
        if (StoryManager.Instance.CurrentPrompt.NextPrompts[0] == null)
        {
            Debug.Log("No DirectPrompt set!");
            return;
        }
        StoryManager.Instance.CompileAnswer(0);
        //StoryManager.Instance.CurrentPrompt = StoryManager.Instance.CurrentPrompt.NextPrompts[0];
    }

    private void ValidateAIOutput()
    {
        LoadingIcon.enabled = false;
        switch (m_optionSelected)
        {
            case " a":
                DebugNNChoice(0);
                StoryManager.Instance.CompileAnswer(0);
                break;
            case " b":
                DebugNNChoice(1);
                StoryManager.Instance.CompileAnswer(1);
                break;
            case " c":
                DebugNNChoice(2);
                StoryManager.Instance.CompileAnswer(2);
                break;
            case " d":
                DebugNNChoice(3);
                StoryManager.Instance.CompileAnswer(3);
                break;
            case " e":
                DebugNNChoice(4);
                StoryManager.Instance.CompileAnswer(4);
                break;
            default:
                Debug.Log("Input doesnt make sense");
                break;
        }
    }

    private void DebugNNChoice(int _optionSelected)
    {
        if (StoryManager.Instance.CurrentPrompt.PromptOptions.Length > _optionSelected)
        {
            Debug.Log($"Option{m_optionSelected} was chosen ({StoryManager.Instance.CurrentPrompt.PromptOptions[_optionSelected]})");
        }
        else if (StoryManager.Instance.CurrentPrompt.PromptOptions.Length == _optionSelected)
        {
            Debug.Log($"Option{m_optionSelected} was chosen (None of the above)");
        }
        //switch (m_optionSelected)
        //{
        //    case " a":
        //        break;
        //    case " b":
        //        if (StoryManager.Instance.CurrentPrompt.NextPrompts.Length > 1)
        //            Debug.Log($"Option{m_optionSelected} was chosen ({StoryManager.Instance.CurrentPrompt.NextPrompts[0]})");
        //        break;
        //    case " c":
        //        if (StoryManager.Instance.CurrentPrompt.NextPrompts.Length > 2)
        //            Debug.Log($"Option{m_optionSelected} was chosen ({StoryManager.Instance.CurrentPrompt.NextPrompts[0]})");
        //        break;
        //    case " d":
        //        if (StoryManager.Instance.CurrentPrompt.NextPrompts.Length > 3)
        //            Debug.Log($"Option{m_optionSelected} was chosen ({StoryManager.Instance.CurrentPrompt.PromptOptions[3]})");
        //        break;
        //    default:
        //        break;
        //}
    }

    public void DisableAIUI()
    {
        inputBox.SetActive(false);
        answerButton.SetActive(false);
        continueButton.SetActive(false);
    }

    public void EnableContinueButton()
    {
        inputBox.SetActive(false);
        answerButton.SetActive(false);
        continueButton.SetActive(true);
    }

    public void DisableContinueButton()
    {
        inputBox.SetActive(true);
        answerButton.SetActive(true);
        continueButton.SetActive(false);
    }

    private string BuildAIAnalyzeString(string _playerInput, string[] _promptInput)
    {
        string result = "";
        for (int i = 0; i < _promptInput.Length + 1; i++)
        {
            switch (i)
            {
                case 0:
                    result += "A. ";
                    break;
                case 1:
                    result += "B. ";
                    break;
                case 2:
                    result += "C. ";
                    break;
                case 3:
                    result += "D. ";
                    break;
                case 4:
                    result += "E. ";
                    break;
                default:
                    break;
            }
            if (StoryManager.Instance.CurrentPrompt.IsLeadingQuestion 
                && i == StoryManager.Instance.LeadingQuestionIndex && StoryManager.Instance.ProceduralDialogueEnabled)
            {
                result += "Yes or ";
            }
            if (i >= _promptInput.Length)
            {
                result += "None of the Above \\n \\nAction: ";
                break;
            }
            result += _promptInput[i] + " \\n";
        }
        result += _playerInput + " ->";
        Debug.Log(result);
        return result;
        //A. Help the helpless Aisha(girl) \nB.Save yourself, run away \nC.Fight the zombies to buy time \nD.None of the above \n \nAction: Fight the zombies to buy time ->
    }

    private bool CheckForWin()
    {
        for (int i = 0; i < winStrings.Length; i++)
        {
            if (winStrings[i] == PlayerInput.text)
            {
                PlayerInput.text = "";
                StoryManager.Instance.WinGame();
                return true;
            }
        }
        return false;
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
