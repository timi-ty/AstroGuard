using UnityEngine;
using System.Collections;
using System;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class AnimatedText : MonoBehaviour
{
    #region Components
    private TextMeshProUGUI targetTextMesh;
    #endregion

    #region Data
    private string holdingtext;
    private string visibleText { get => targetTextMesh.text; set => targetTextMesh.text = value; }
    #endregion

    #region Coroutines
    private Coroutine inflateCoroutine;
    private Coroutine deflateCoroutine;
    #endregion

    #region Settings
    public float inflateLettersPerSecond = 100;
    public float deflateLettersPerSecond = 100;
    #endregion

    private void Start()
    {
        targetTextMesh = GetComponent<TextMeshProUGUI>();

        DeflateText();
    }

    public void InflateText(Action onTextInflated = null)
    {
        StopAll();

        inflateCoroutine = StartCoroutine(InflateTextCoroutine(onTextInflated));
    }

    public void DeflateText(Action onTextDeflated = null)
    {
        StopAll();

        deflateCoroutine = StartCoroutine(DeflateTextCoroutine(onTextDeflated));
    }

    public void SetTextImmediately(string text)
    {
        holdingtext = visibleText = text;
    }

    public void ClearVisibleText()
    {
        visibleText = "";
    }

    private IEnumerator InflateTextCoroutine(Action onTextInflated = null)
    {
        yield return null; //always wait one frame to makesure Start() is called before running;

        if (holdingtext == null) yield break;

        deflateCoroutine = StartCoroutine(DeflateTextCoroutine());

        yield return new WaitWhile(() => visibleText.Length > 0);

        if(deflateCoroutine != null) StopCoroutine(deflateCoroutine);

        char[] textBuffer = holdingtext.ToCharArray();
        Array.Reverse(textBuffer);

        Stack textQueue = new Stack(textBuffer);

        while (textQueue.Count > 0)
        {
            visibleText += textQueue.Pop();
            yield return new WaitForSecondsRealtime(1 / Mathf.Abs(inflateLettersPerSecond));
        }

        onTextInflated?.Invoke();
    }

    private IEnumerator DeflateTextCoroutine(Action onTextDeflated = null)
    {
        yield return null; //always wait one frame to makesure Start() is called before running;

        while (visibleText.Length > 0)
        {
            visibleText = visibleText.Substring(0, visibleText.Length - 1);
            yield return new WaitForSecondsRealtime(1 / Mathf.Abs(deflateLettersPerSecond));
        }

        onTextDeflated?.Invoke();
    }

    public void SetNextText(string text)
    {
        holdingtext = text;
    }

    private void StopAll()
    {
        if (inflateCoroutine != null) StopCoroutine(inflateCoroutine);
        if (deflateCoroutine != null) StopCoroutine(deflateCoroutine);
    }
}