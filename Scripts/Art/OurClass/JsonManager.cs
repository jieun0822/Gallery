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
    /// JSON 파일 경로 반환
    /// </summary>
    public string GetJsonPath()
    {
        return Path.Combine(GetFolderPath(), jsonFileName);
    }

    /// <summary>
    /// 저장 (리스트에 추가 후 JSON 갱신)
    /// </summary>
    public void SaveCapture( string name, string filePath)
    {
        string jsonPath = GetJsonPath();

        CaptureDataList allData = new CaptureDataList();

        // 기존 JSON 불러오기
        if (File.Exists(jsonPath))
        {
            string jsonOld = File.ReadAllText(jsonPath);
            allData = JsonUtility.FromJson<CaptureDataList>(jsonOld);
            if (allData == null) allData = new CaptureDataList();
        }

        // 같은 filePath 있는지 검사
        var existing = allData.items.Find(item => item.filePath == filePath);

        if (existing != null)
        {
            // 이미 있으면 이름만 갱신
            existing.personName = name;
            Debug.Log($"기존 데이터 수정됨: {filePath}");
        }
        else
        {
            // 새 항목 추가
            allData.items.Add(new CaptureData()
            {
                personName = name,
                filePath = filePath
            });
        }

        // 다시 저장
        string jsonNew = JsonUtility.ToJson(allData, true);
        File.WriteAllText(jsonPath, jsonNew);
        jsonDatas = LoadCaptures();

        Debug.Log($"JSON 저장 완료 → {jsonPath}");
    }

    /// <summary>
    /// JSON 불러오기 (전체 리스트 반환)
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

        Debug.LogWarning("JSON 파일 없음 → 새 리스트 반환");
        return new List<CaptureData>();
    }

    public void ClearAllCaptures()
    {
        string jsonPath = GetJsonPath();

        // 리스트 비우기
        jsonDatas.Clear();

        // 비어있는 CaptureDataList 생성
        CaptureDataList emptyData = new CaptureDataList();

        // JSON으로 저장
        string jsonNew = JsonUtility.ToJson(emptyData, true);
        File.WriteAllText(jsonPath, jsonNew);
    }
}
