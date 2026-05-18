using System.Collections;
using TMPro;
using UnityEngine;

public static class RoleAnnouncement
{
    public static void Show(string roleText, string subtitle)
    {
        var canvas = GameObject.Find("GameUI")?.GetComponent<Canvas>();
        if (canvas == null) return;

        var go = new GameObject("RoleAnnouncement", typeof(RectTransform));
        go.transform.SetParent(canvas.transform, false);

        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0, 50);
        rect.sizeDelta = new Vector2(600, 120);

        var text = go.AddComponent<TextMeshProUGUI>();
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = 48;
        text.fontStyle = FontStyles.Bold;
        text.color = Color.white;
        text.text = roleText;

        var subGo = new GameObject("RoleSubtitle", typeof(RectTransform));
        subGo.transform.SetParent(go.transform, false);

        var subRect = subGo.GetComponent<RectTransform>();
        subRect.anchorMin = new Vector2(0.5f, 0f);
        subRect.anchorMax = new Vector2(0.5f, 0f);
        subRect.anchoredPosition = new Vector2(0, -50);
        subRect.sizeDelta = new Vector2(600, 50);

        var subText = subGo.AddComponent<TextMeshProUGUI>();
        subText.alignment = TextAlignmentOptions.Center;
        subText.fontSize = 24;
        subText.fontStyle = FontStyles.Normal;
        subText.color = new Color(0.8f, 0.8f, 0.8f, 1f);
        subText.text = subtitle;

        go.transform.localScale = Vector3.zero;

        CoroutineRunner.Instance.StartCoroutine(Animate(go, text, subText));
    }

    static IEnumerator Animate(GameObject root, TextMeshProUGUI main, TextMeshProUGUI sub)
    {
        float t = 0;
        while (t < 0.4f)
        {
            t += Time.deltaTime;
            float s = Mathf.SmoothStep(0, 1, t / 0.4f);
            root.transform.localScale = Vector3.one * s;
            yield return null;
        }

        root.transform.localScale = Vector3.one;

        yield return new WaitForSeconds(2f);

        float fade = 0;
        while (fade < 0.8f)
        {
            fade += Time.deltaTime;
            float a = Mathf.Lerp(1, 0, fade / 0.8f);
            main.color = new Color(main.color.r, main.color.g, main.color.b, a);
            sub.color = new Color(sub.color.r, sub.color.g, sub.color.b, a);
            yield return null;
        }

        Object.Destroy(root);
    }
}

public class CoroutineRunner : MonoBehaviour
{
    static CoroutineRunner instance;
    public static CoroutineRunner Instance
    {
        get
        {
            if (instance == null)
            {
                var go = new GameObject("CoroutineRunner");
                Object.DontDestroyOnLoad(go);
                instance = go.AddComponent<CoroutineRunner>();
            }
            return instance;
        }
    }
}
