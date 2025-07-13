using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class LocalizationData
{
    public Dictionary<string, string> entries;
}

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    [SerializeField] private string localizationFileName = "localization.json";
    private Dictionary<string, string> localizedTexts = new Dictionary<string, string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLocalization();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadLocalization()
    {
        string path = Path.Combine(Application.streamingAssetsPath, localizationFileName);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            LocalizationData data = JsonUtility.FromJson<LocalizationData>(json);
            if (data != null && data.entries != null)
            {
                localizedTexts = data.entries;
            }
            else
            {
                Debug.LogError("Localization JSON format invalid.");
            }
        }
        else
        {
            Debug.LogError("Localization file not found: " + path);
        }
    }

    public string GetLocalizedValue(string key)
    {
        if (localizedTexts.TryGetValue(key, out string value))
        {
            return value;
        }

        return key;
    }
}
