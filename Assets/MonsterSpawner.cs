using UnityEngine;
using TMPro;

public class MonsterSpawner : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject monsterPrefab; // The monster prefab to spawn
    public Transform[] spawnPoints; // Optional spawn points array
    public float textHeightOffset = 2f; // How high above monster to place text

    [Header("Level Settings")]
    public int currentLevel = 1;

    void Start()
    {
        SpawnMonstersForLevel(currentLevel);
    }

    void SpawnMonstersForLevel(int level)
    {
        switch (level)
        {
            case 1:
                SpawnMonster("Friendly", new string[] { "Hello Friend" }, new string[] { "Hullo", "Hallo" });
                break;

            case 2:
                SpawnMonster("Friendly", new string[] { "Follow me" }, new string[] { "Follo mee", "Fo low mee" });
                SpawnMonster("Dangerous", null, new string[] { "Follo mee", "Fo low mee" });
                break;

            case 3:
                SpawnMonster("Friendly", new string[] { "Trustworthy" }, new string[] { "Trustwurthy", "Trustw0rthy", "Truztworthy" });
                for (int i = 0; i < 3; i++)
                    SpawnMonster("Dangerous", null, new string[] { "Trustwurthy", "Trustw0rthy", "Truztworthy" });
                break;

            default:
                Debug.LogWarning("Level not configured. No monsters spawned.");
                break;
        }
    }

    /// <summary>
    /// Spawns a monster with a text prompt above them
    /// </summary>
    /// <param name="tag">Friendly or Dangerous</param>
    /// <param name="friendlyPrompts">Optional array of prompts if this is a friendly monster</param>
    /// <param name="dangerousPrompts">Array of prompts for dangerous monsters</param>
    void SpawnMonster(string tag, string[] friendlyPrompts, string[] dangerousPrompts)
    {
        Vector3 spawnPosition;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            spawnPosition = spawnPoints[randomIndex].position;
        }
        else
        {
            spawnPosition = transform.position;
        }

        GameObject monster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
        monster.tag = tag;

        // Decide what prompt to assign
        string prompt = "";
        if (tag == "Friendly" && friendlyPrompts != null && friendlyPrompts.Length > 0)
        {
            int randIndex = Random.Range(0, friendlyPrompts.Length);
            prompt = friendlyPrompts[randIndex];
        }
        else if (tag == "Dangerous" && dangerousPrompts != null && dangerousPrompts.Length > 0)
        {
            int randIndex = Random.Range(0, dangerousPrompts.Length);
            prompt = dangerousPrompts[randIndex];
        }

        // Attach Text above monster
        CreatePromptText(monster.transform, prompt);
    }

    void CreatePromptText(Transform monsterTransform, string prompt)
    {
        // Create a new TextMeshPro object
        GameObject textObj = new GameObject("MonsterPrompt");
        textObj.transform.SetParent(monsterTransform);
        textObj.transform.localPosition = Vector3.up * textHeightOffset;
        textObj.transform.localRotation = Quaternion.identity;

        textObj.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        textObj.transform.localScale = Vector3.one;

        TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
        tmp.text = prompt;
        tmp.fontSize = 3;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        // Optional: make text always face camera
        textObj.AddComponent<FaceCamera>();
    }
}

/// <summary>
/// Simple helper to make text always face the camera
/// </summary>
public class FaceCamera : MonoBehaviour
{
    void Update()
    {
        if (Camera.main != null)
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                 Camera.main.transform.rotation * Vector3.up);

        }
    }
}
