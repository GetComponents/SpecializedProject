using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneryManager : MonoBehaviour
{
    public static SceneryManager Instance;
    [SerializeField]
    Sprite KidImage, NurseImage, MrFredsonImage, TeacherImage, BlackImage, WhiteImage;
    [SerializeField]
    Image personSpeakingImage, backgroundImage;
    private Sprite currentSprite;
    [SerializeField]
    float fadeSpeed;
    [SerializeField]
    Color baseColor;

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

    public void CheckForSceneryChange(string _speaker)
    {
        switch (_speaker)
        {
            case "Kid":
                FadeInPerson(KidImage);
                break;
            case "Nurse":
                FadeInPerson(NurseImage);
                break;
            case "Teacher":
                FadeInPerson(MrFredsonImage);
                break;
            case "Homeroom Teacher":
                FadeInPerson(TeacherImage);
                break;
            case "/":
                FadeInPerson(null);
                break;
            case "Blackout":
                StartCoroutine(FadeBackground(false));
                break;
            case "White":
                StartCoroutine(FadeBackground(true));
                break;
            default:
                break;
        }
    }

    private void FadeInPerson(Sprite _sprite)
    {
        if (currentSprite != _sprite)
        {
            currentSprite = _sprite;
            personSpeakingImage.sprite = currentSprite;
        }
    }

    private IEnumerator FadeBackground(bool _fadeIn)
    {
        Color alpha = baseColor;
        if (_fadeIn)
        {
            alpha.a = 0;
            while (alpha.a < 1)
            {
                yield return new WaitForEndOfFrame();
                alpha.a += Time.deltaTime * fadeSpeed;
                backgroundImage.color = alpha;
                personSpeakingImage.color = alpha;
            }
        }
        else
        {
            alpha.a = 1;
            while (alpha.a > 0)
            {
                yield return new WaitForEndOfFrame();
                alpha.a -= Time.deltaTime * fadeSpeed;
                backgroundImage.color = alpha;
                personSpeakingImage.color = alpha;
            }
        }

    }
}
