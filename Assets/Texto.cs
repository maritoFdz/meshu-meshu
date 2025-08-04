using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class EnhancedText : MonoBehaviour
{
    public string Name;
    private string content = "Hola <shake><color=#0000FF>Erito</color></shake> <b>FUNCIONA</b>";
    public TMP_Text outputText;
    public TMP_TextInfo textInfo;
    private readonly float velocity = 0.05f;
    private readonly List<TextEffect> Animations = new();
    private int visibleCount = 0;  // Controla cuántos caracteres están visibles

    void Start()
    {
        outputText = GetComponent<TMP_Text>();
        outputText.richText = true;
        outputText.text = content;
        GetTextAnimations();
        outputText.ForceMeshUpdate();
        textInfo = outputText.textInfo;
        HideChars();
        StartCoroutine(Display());
    }

    void Update()
    {
        TMP_TextInfo textInfo = outputText.textInfo;
        Vector3[] vertices;

        for (int i = 0; i < Animations.Count; i++)
        {
            TextEffect effect = Animations[i];
            if (effect.effect != TextEffectType.Shake) continue;

            for (int c = effect.start; c <= effect.end; c++)
            {
                if (c >= visibleCount) break;
                if (!textInfo.characterInfo[c].isVisible) continue;

                int materialIndex = textInfo.characterInfo[c].materialReferenceIndex;
                int vertexIndex = textInfo.characterInfo[c].vertexIndex;
                vertices = textInfo.meshInfo[materialIndex].vertices;
                Vector3 offset = new(
                    Mathf.Sin(Time.time * 20f + c) * 0.1f,
                    Mathf.Cos(Time.time * 25f + c) * 0.1f,
                    0f
                );
                for (int j = 0; j < 4; j++)
                    vertices[vertexIndex + j] += offset;
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            outputText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }


    public void HideChars()
    {
        // pone invisibles todos los caracteres
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            int mesh = textInfo.characterInfo[i].materialReferenceIndex;
            Color32[] vertexColors = textInfo.meshInfo[mesh].colors32;
            for (int j = 0; j < 4; j++)
            {
                Color32 c = vertexColors[vertexIndex + j];
                c.a = 0;
                vertexColors[vertexIndex + j] = c;
            }
        }
        outputText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    public void GetTextAnimations()
    {
        StringBuilder cleanText = new();
        GetShakes(cleanText);
        outputText.text = cleanText.ToString();
        outputText.ForceMeshUpdate();
        textInfo = outputText.textInfo;
    }

    public void GetShakes(StringBuilder cleanText)
    {
        string text = outputText.text;
        string shakeTag = "<shake>";
        string endTag = "</shake>";
        bool isActive = false;
        int effectStart = -1; // esto es por si tienes algun tag abierto </efecto> sin abrirlo antes
        int visibleIndex = 0; // para contar solo caracteres visibles reales, no tags

        for (int i = 0; i < text.Length; i++)
        {
            // abre tag shake
            if (i + shakeTag.Length <= text.Length && text.Substring(i, shakeTag.Length) == shakeTag)
            {
                isActive = true;
                effectStart = visibleIndex;
                i += shakeTag.Length - 1;
                continue;
            }

            // cierra tag shake
            if (i + endTag.Length <= text.Length && text.Substring(i, endTag.Length) == endTag && isActive)
            {
                isActive = false;
                Animations.Add(new TextEffect
                {
                    start = effectStart,
                    end = visibleIndex - 1,
                    effect = TextEffectType.Shake
                });
                effectStart = -1;
                i += endTag.Length - 1;
                continue;
            }

            // si detecta otro tag lo copia entero para no perder formato
            if (text[i] == '<')
            {
                int close = text.IndexOf('>', i);
                if (close != -1)
                {
                    cleanText.Append(text.Substring(i, close - i + 1));
                    i = close;
                    continue;
                }
            }

            cleanText.Append(text[i]);
            visibleIndex++;
        }
    }

    public IEnumerator Display()
    {
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;
            visibleCount = i + 1; // Actualiza cuántos se ven
            int mesh = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            Color32[] vertexColors = textInfo.meshInfo[mesh].colors32;

            for (int j = 0; j < 4; j++)
            {
                Color32 c = vertexColors[vertexIndex + j];
                c.a = 255;
                vertexColors[vertexIndex + j] = c;
            }

            outputText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            yield return new WaitForSeconds(velocity);
        }
    }
}

public enum TextEffectType
{
    Shake
}

public struct TextEffect
{
    public int start;
    public int end;
    public TextEffectType effect;
}
