using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VisualNovelManager : MonoBehaviour
{
    [Tooltip("The UI Text element to display dialogue.")]
    public TextMeshProUGUI dialogueText;

    [Tooltip("The UI RawImage element for the background.")]
    public RawImage background;

    [Tooltip("List of character UI elements, each corresponding to a character name.")]
    public List<CharacterUI> characterUIs;

    [Tooltip("Dictionary mapping character names to their textures.")]
    public Dictionary<string, Texture2D> characterTextures;

    [Tooltip("Dictionary mapping background names to their textures.")]
    public Dictionary<string, Texture2D> backgroundTextures;

    [Tooltip("Audio source for playing typing sound.")]
    public AudioSource typingAudioSource;

    [Tooltip("Audio clip to play for typing sound effect.")]
    public AudioClip typingSound;

    [Tooltip("Text asset containing the dialogue script.")]
    public TextAsset dialogueFile;

    private Queue<(string characterName, string dialogue)> dialogueQueue = new Queue<(string, string)>();
    private bool isTyping = false;
    private bool skipTyping = false;

    [Tooltip("Speed at which characters appear during typing.")]
    public float typingSpeed = 0.05f;

    [Tooltip("Pause duration after punctuation like periods and ellipses.")]
    public float punctuationPause = 0.3f;

    [Tooltip("Color for dimmed characters when they are not speaking.")]
    public Color dimmedColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    [Tooltip("Color for the active speaking character.")]
    public Color normalColor = Color.white;

    [Tooltip("Height offset applied to non-speaking characters.")]
    public float loweredHeightOffset = -50f;

    private void Start()
    {
        characterTextures = new Dictionary<string, Texture2D>
        {
            { "NightOwl", LoadTexture("Assets/Sprites/Character/NightOwl.png") },
            { "Huginn//Echo", LoadTexture("Assets/Sprites/Character/Huginn.png") },
            { "Castellan-5", LoadTexture("Assets/Sprites/Character/Castellan.png") },
            { "C3RB-0X", LoadTexture("Assets/Sprites/Character/C3RB-0X.png") },
            { "Ezekiel Z3K Cross", LoadTexture("Assets/Sprites/Character/Z3K.png") },
            { "Z3K", LoadTexture("Assets/Sprites/Character/Z3K.png") }
        };

        backgroundTextures = new Dictionary<string, Texture2D>
        {
            
        };

        if (dialogueFile != null)
        {
            DisplayDialogue(dialogueFile.text);
        }
    }

    private Texture2D LoadTexture(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"Texture file not found: {path}");
            return null;
        }

        byte[] fileData = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        texture.LoadImage(fileData);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        return texture;
    }

    public void DisplayDialogue(string fullText)
    {
        string[] lines = fullText.Split('\n');
        foreach (string line in lines)
        {
            if (line.StartsWith("[show ") && line.EndsWith("]"))
            {
                string bgName = line.Substring(6, line.Length - 7);
                UpdateBackground(bgName);
                continue;
            }

            Match match = Regex.Match(line, "^(.*?): (.*)$");
            if (match.Success)
            {
                string characterName = match.Groups[1].Value;
                string dialogue = match.Groups[2].Value;
                dialogueQueue.Enqueue((characterName, characterName + ": " + dialogue));
            }
        }
        if (!isTyping)
        {
            StartCoroutine(TypeDialogue());
        }
    }

    private IEnumerator TypeDialogue()
    {
        while (dialogueQueue.Count > 0)
        {
            isTyping = true;
            dialogueText.text = "";
            var (characterName, currentDialogue) = dialogueQueue.Dequeue();

            UpdateCharacterUI(characterName);

            foreach (char letter in currentDialogue)
            {
                if (skipTyping)
                {
                    dialogueText.text = currentDialogue;
                    break;
                }
                dialogueText.text += letter;
                if (typingAudioSource && typingSound)
                {
                    typingAudioSource.PlayOneShot(typingSound);
                }
                if (letter == '.' || letter == '…')
                {
                    yield return new WaitForSeconds(punctuationPause);
                }
                else
                {
                    yield return new WaitForSeconds(typingSpeed);
                }
            }

            isTyping = false;
            skipTyping = false;
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        }
    }

    private void UpdateCharacterUI(string activeCharacter)
    {
        foreach (var characterUI in characterUIs)
        {
            if (characterUI.name == activeCharacter || (characterUI.name == "Z3K" && activeCharacter == "Ezekiel Z3K Cross"))
            {
                characterUI.image.color = normalColor;
                characterUI.image.rectTransform.anchoredPosition = new Vector2(characterUI.image.rectTransform.anchoredPosition.x, 0);
            }
            else
            {
                characterUI.image.color = dimmedColor;
                characterUI.image.rectTransform.anchoredPosition = new Vector2(characterUI.image.rectTransform.anchoredPosition.x, loweredHeightOffset);
            }
        }
    }

    private void UpdateBackground(string bgName)
    {
        if (backgroundTextures.ContainsKey(bgName))
        {
            background.texture = backgroundTextures[bgName];
        }
    }

    private void Update()
    {
        if (isTyping && Input.GetMouseButtonDown(0))
        {
            skipTyping = true;
        }
    }
}

[System.Serializable]
public class CharacterUI
{
    public string name;
    public RawImage image;
}
