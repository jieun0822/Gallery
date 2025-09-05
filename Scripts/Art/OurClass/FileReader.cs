using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileReader : MonoBehaviour
{
    public GalleryManager galleryManager;
    public CustomDropdownManager dropdownManager;
    
    public Material targetMaterial;
    public GameObject obj;
    public Sprite[] sprites;

    // ��ũ����
    private Texture2D tex;
    private List<string> folderPaths = new List<string>();

    private void Start()
    {
        CreateParentFolder();
    }

    [ContextMenu("Load Image To Material")]
    public void ReadAllFiles()
    {
        DestroySprites();

        string folderPath = dropdownManager.GetCurrentPath();
        if (!Directory.Exists(folderPath))
        {
            // ������ �������� �ʽ��ϴ�
            return;
        }

        string[] filePaths = Directory.GetFiles(folderPath, "*.png");
        if (filePaths.Length == 0)
        {
            // PNG ������ �����ϴ�
            return;
        }

        int fileCount = filePaths.Length;
        if (fileCount == 100) return;

        // sprites �迭�� null�̰ų� ũ�Ⱑ �����ϸ� ���� �ʱ�ȭ
        if (sprites == null || sprites.Length < fileCount)
        {
            sprites = new Sprite[fileCount];
        }

        for (int i = 0; i < fileCount; i++)
        {
            string filePath = filePaths[i];
            byte[] fileData = File.ReadAllBytes(filePath);

            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(fileData))
            {
                Sprite sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );

                sprites[fileCount - 1 - i] = sprite;
            }
        }
    }

    public void DestroySprites()
    {
        if (sprites == null) return;

        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i] = null;
        }

        sprites = null;
    }

    public void DestroyFile()
    {
        string folderPath = dropdownManager.GetCurrentPath();
        if (!Directory.Exists(folderPath))
        {
            // ������ �������� �ʽ��ϴ�
            return;
        }

        string[] filePaths = Directory.GetFiles(folderPath, "*.png");
        if (filePaths.Length == 0)
        {
            // ������ PNG ������ �����ϴ�
            return;
        }

        foreach (string filePath in filePaths)
        {
            try
            {
                File.Delete(filePath);
                // ���� ���� ����
            }
            catch (IOException ioEx)
            {
                Debug.LogError("���� ���� �� ���� �߻�: " + filePath + "\n" + ioEx.Message);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("����ġ ���� ����: " + filePath + "\n" + ex.Message);
            }
        }
    }

    // ������ ��ũ�� ��.
    public void CreateParentFolder()
    {
        // ������Ʈ ��Ʈ ��� (Assets ����)
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string newFolderName = "��ũ����";

        // ��ü ���
        string folderPath = Path.Combine(projectRoot, newFolderName);

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // ����
        GameEnums.eScene[] scenes = { GameEnums.eScene.SunFlower, GameEnums.eScene.Room, GameEnums.eScene.Star, GameEnums.eScene.Crow };

        string childFolderName = null;
        for (int i = 0;  i < scenes.Length; i++)
        {
            switch (scenes[i])
            {
                case GameEnums.eScene.SunFlower:
                    childFolderName = "�عٶ��";
                    break;
                case GameEnums.eScene.Room:
                    childFolderName = "������ ��";
                    break;
                case GameEnums.eScene.Star:
                    childFolderName = "���� ������ ��";
                    break;
                case GameEnums.eScene.Crow:
                    childFolderName = "��� ���� �й�";
                    break;

            }

            // ���� ����
            var childFolderPath = Path.Combine(folderPath, childFolderName);
            if (!Directory.Exists(childFolderPath))
            {
                Directory.CreateDirectory(childFolderPath);
            }

            folderPaths.Add(childFolderPath);
        }
    }

    public void Capture()
    {
        StartCoroutine(CaptureArea());
    }

    private IEnumerator CaptureArea()
    {
        var uiManager = galleryManager.galleryUIManager;
        uiManager.SetCanvasActive(false);

        var soundManager = galleryManager.gameManager.soundManager;
        yield return new WaitForEndOfFrame();
        soundManager.PlaySound(soundManager.cameraSound);
        uiManager.screenFlash.PlayFlash();

        tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        tex.Apply();

        byte[] bytes = tex.EncodeToPNG();
        string fileName = $"ĸó_{System.DateTime.Now:yyyyMMdd_HHmmss}.png"; ;

        var curscene = galleryManager.gameManager.currentScene;
        int index = -1;
        switch (curscene)
        {
            case GameEnums.eScene.SunFlower: index = 0; break;
            case GameEnums.eScene.Room: index = 1; break;
            case GameEnums.eScene.Star: index = 2; break;
            case GameEnums.eScene.Crow: index = 3; break;
        }
        string folderPath = folderPaths[index];
        string fullPath = Path.Combine(folderPath, fileName);
        File.WriteAllBytes(fullPath, bytes);

        Destroy(tex);
        tex = null;
        uiManager.SetCanvasActive(true);
    }
}
