using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class CaptureData
{
    public string personName;
    public string filePath;
}

[System.Serializable]
public class CaptureDataList
{
    public List<CaptureData> items = new List<CaptureData>();
}

public class JsonManager : MonoBehaviour
{
    private string jsonFileName = "nameList.json";
    public List<CaptureData> jsonDatas;

    private void Start()
    {
        jsonDatas = LoadCaptures();
    }

    public string GetFolderPath()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        string folderPath = Path.Combine(root, "Json");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        return folderPath;
    }

    /// <summary>
    /// JSON ���� ��� ��ȯ
    /// </summary>
    public string GetJsonPath()
    {
        return Path.Combine(GetFolderPath(), jsonFileName);
    }

    /// <summary>
    /// ���� (����Ʈ�� �߰� �� JSON ����)
    /// </summary>
    public void SaveCapture( string name, string filePath)
    {
        string jsonPath = GetJsonPath();

        CaptureDataList allData = new CaptureDataList();

        // ���� JSON �ҷ�����
        if (File.Exists(jsonPath))
        {
            string jsonOld = File.ReadAllText(jsonPath);
            allData = JsonUtility.FromJson<CaptureDataList>(jsonOld);
            if (allData == null) allData = new CaptureDataList();
        }

        // ���� filePath �ִ��� �˻�
        var existing = allData.items.Find(item => item.filePath == filePath);

        if (existing != null)
        {
            // �̹� ������ �̸��� ����
            existing.personName = name;
            Debug.Log($"���� ������ ������: {filePath}");
        }
        else
        {
            // �� �׸� �߰�
            allData.items.Add(new CaptureData()
            {
                personName = name,
                filePath = filePath
            });
        }

        // �ٽ� ����
        string jsonNew = JsonUtility.ToJson(allData, true);
        File.WriteAllText(jsonPath, jsonNew);
        jsonDatas = LoadCaptures();

        Debug.Log($"JSON ���� �Ϸ� �� {jsonPath}");
    }

    /// <summary>
    /// JSON �ҷ����� (��ü ����Ʈ ��ȯ)
    /// </summary>
    public List<CaptureData> LoadCaptures()
    {
        string jsonPath = GetJsonPath();

        if (File.Exists(jsonPath))
        {
            string json = File.ReadAllText(jsonPath);
            CaptureDataList dataList = JsonUtility.FromJson<CaptureDataList>(json);
            if (dataList != null)
            {
                return dataList.items;
            }
        }

        Debug.LogWarning("JSON ���� ���� �� �� ����Ʈ ��ȯ");
        return new List<CaptureData>();
    }

    public void ClearAllCaptures()
    {
        string jsonPath = GetJsonPath();

        // ����Ʈ ����
        jsonDatas.Clear();

        // ����ִ� CaptureDataList ����
        CaptureDataList emptyData = new CaptureDataList();

        // JSON���� ����
        string jsonNew = JsonUtility.ToJson(emptyData, true);
        File.WriteAllText(jsonPath, jsonNew);
    }
}
