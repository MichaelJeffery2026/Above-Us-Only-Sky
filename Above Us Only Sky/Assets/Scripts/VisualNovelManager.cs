using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VisualNovelManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public RawImage characterImage1;
    public RawImage characterImage2;
    public RawImage background;
    public Dictionary<string, Texture2D> characterTextures;
    public Dictionary<string, Texture2D> backgroundTextures;
    public AudioSource typingAudioSource;
    public AudioClip typingSound;
    public TextAsset dialogueFile; // Public variable for text file input
    private Queue<(string characterName, string dialogue)> dialogueQueue = new Queue<(string, string)>();
    private bool isTyping = false;
    public float typingSpeed = 0.05f;
    public float punctuationPause = 0.3f;
    private string lastSpeaker = "";
    private string previousSpeaker = "";

    private void Start()
    {
        characterTextures = new Dictionary<string, Texture2D>
        {
            { "NightOwl", LoadTexture("Assets/Sprites/Character/NightOwl.png") },
            { "Huginn//Echo", LoadTexture("Assets/Sprites/Character/Huginn.png") },
            { "Castellan-5", LoadTexture("Assets/Sprites/Character/Castellan.png") },
            { "C3RB-0X", LoadTexture("Assets/Sprites/Character/C3RB-0X.png") },
            { "Ezekiel Z3K Cross", LoadTexture("Assets/Sprites/Character/Z3K.png") }
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
        texture.filterMode = FilterMode.Point; // Ensures crisp pixels
        texture.wrapMode = TextureWrapMode.Clamp; // Prevents edge bleeding
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

            UpdateSpeakers(characterName);

            foreach (char letter in currentDialogue)
            {
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
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        }
    }

    private void UpdateSpeakers(string characterName)
    {
        if (characterName != lastSpeaker)
        {
            previousSpeaker = lastSpeaker;
            lastSpeaker = characterName;
        }
        UpdateCharacterImages();
    }

    private void UpdateCharacterImages()
    {
        characterImage1.enabled = !string.IsNullOrEmpty(previousSpeaker) && characterTextures.ContainsKey(previousSpeaker);
        characterImage2.enabled = !string.IsNullOrEmpty(lastSpeaker) && characterTextures.ContainsKey(lastSpeaker);

        if (characterImage1.enabled)
            characterImage1.texture = characterTextures[previousSpeaker];
        if (characterImage2.enabled)
            characterImage2.texture = characterTextures[lastSpeaker];
    }

    private void UpdateBackground(string bgName)
    {
        if (backgroundTextures.ContainsKey(bgName))
        {
            background.texture = backgroundTextures[bgName];
        }
    }
}
