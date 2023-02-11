using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using OpenAi.Unity.V1;

public class StoryManager : MonoBehaviour
{
    [SerializeField]
    PlayerInputfield playerInput;

    public static StoryManager Instance;

    public bool ProceduralDialogueEnabled;

    private bool isRepeatedPrompt;

    public PromptInfo StartPrompt;

    public PromptInfo CurrentPrompt;

    [SerializeField]
    NewAiCompleterV1 rephraser;

    [SerializeField]
    public TextMeshProUGUI textBox;
    //TODO PRIVATE
    public bool WaitingForResponse;

    public int LeadingQuestionIndex;

    private string AiText
    {
        get => aiText;
        set
        {
            aiText = value;
            PostAIText();
        }
    }

    private string aiText;

    [SerializeField]
    private PromptInfo winPrompt;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        CurrentPrompt = StartPrompt;
        textBox.text = CurrentPrompt.Prompt;
        if (CurrentPrompt.IsDescriptiveText)
        {
            playerInput.EnableContinueButton();
        }
    }

    public void StartWaitingForResponse()
    {
        playerInput.LoadingIcon.enabled = true;
        playerInput.DisableAIUI();
        WaitingForResponse = true;
        StartCoroutine(WaitForResponse());
    }

    public void StopWaitingForResponse()
    {
        playerInput.inputBox.SetActive(true);
        if (CurrentPrompt.IsDescriptiveText)
        {
            playerInput.continueButton.SetActive(true);
        }
        else
        {
            playerInput.answerButton.SetActive(true);
        }
        playerInput.LoadingIcon.enabled = false;
        WaitingForResponse = false;
    }

    private IEnumerator WaitForResponse()
    {
        yield return new WaitForSeconds(5);
        if (WaitingForResponse)
        {
            StopWaitingForResponse();
            PostDefaultText();
        }

    }

    public void CompileAnswer(int _playerAction)
    {
        if (_playerAction > CurrentPrompt.NextPrompts.Length)
        {
            Debug.LogWarning($"NextPrompt: {_playerAction} in {CurrentPrompt.name} not set!");
            return;
        }

        if (CurrentPrompt.NextPrompts.Length > _playerAction && CurrentPrompt == CurrentPrompt.NextPrompts[_playerAction])
        {
            isRepeatedPrompt = true;
        }
        else
        {
            isRepeatedPrompt = false;
            CurrentPrompt = CurrentPrompt.NextPrompts[_playerAction];
        }
        SceneryManager.Instance.CheckForSceneryChange(CurrentPrompt.SpeakerName);
        if (CurrentPrompt.IsDescriptiveText)
        {
            playerInput.EnableContinueButton();
        }
        else
        {
            playerInput.DisableContinueButton();
        }

        if (!ProceduralDialogueEnabled || !CurrentPrompt.IsProcedural)
        {
            PostDefaultText();
            return;
        }


        string proceduralText = BuildAIDialogueRephrase(CurrentPrompt);
        StartWaitingForResponse();
        rephraser.Complete(
        proceduralText,
        s => AiText = s,
        e => Debug.Log($"ERROR: StatusCode: {e.responseCode} - {e.error}"));
    }


    private string BuildAIDialogueRephrase(PromptInfo _prompt)
    {
        string result = "";
        //Character name
        result += _prompt.SpeakerName;
        //"name"
        if (_prompt.KnowsName)
        {
            result += " \\n name";
        }
        //respond to / rephrase
        if (isRepeatedPrompt)
        {
            result += " \\n respond to: " + playerInput.LastString;
        }
        else
        {
            result += " \\n rephrase: " + _prompt.Prompt;
        }
        //IsLeadingQuestion
        if (_prompt.IsLeadingQuestion)
        {
            LeadingQuestionIndex = Random.Range(0, _prompt.PromptOptions.Length);
            //LeadingQuestionIndex = _prompt.PromptOptions.Length - 1;
            result += " \\n " + _prompt.PromptOptions[LeadingQuestionIndex];
        }
        result += " ->";
        Debug.Log("Dialogue: " + result);
        return result;
    }

    private void PostDefaultText()
    {
        List<char> text = new List<char>();
        if (!CurrentPrompt.IsDescriptiveText)
        {
            text.Add('"');
        }
        for (int i = 0; i < CurrentPrompt.Prompt.Length; i++)
        {
            text.Add(CurrentPrompt.Prompt[i]);
        }
        if (!CurrentPrompt.IsDescriptiveText)
        {
            text.Add('"');
        }
        textBox.text = new string(text.ToArray());
    }

    private void PostAIText()
    {
        if (!WaitingForResponse)
        {
            return;
        }
        List<char> text = new List<char>();
        text.Add('"');
        for (int i = 0; i < aiText.Length; i++)
        {
            if (aiText[i] == '\\')
            {
                break;
            }
            text.Add(aiText[i]);
        }
        text.Add('"');
        textBox.text = new string(text.ToArray());
        StopWaitingForResponse();
    }

    public void WinGame()
    {
        CurrentPrompt = winPrompt;
        if (CurrentPrompt.IsDescriptiveText)
        {
            playerInput.EnableContinueButton();
        }
        else
        {
            playerInput.DisableContinueButton();
        }

        if (!ProceduralDialogueEnabled || !CurrentPrompt.IsProcedural)
        {
            PostDefaultText();
            return;
        }


        string proceduralText = BuildAIDialogueRephrase(CurrentPrompt);
        StartWaitingForResponse();
        rephraser.Complete(
        proceduralText,
        s => AiText = s,
        e => Debug.Log($"ERROR: StatusCode: {e.responseCode} - {e.error}"));
    }
}
