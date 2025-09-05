using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;

public class CustomDropdownManager : MonoBehaviour
{
    public GalleryManager galleryManager;
    public SliderScrollSync scrollSync;
    public List<DropdownItem> items;
    private int currentIndex = 0;
    public DropdownItem selectedItem;
    public DropdownItem prevItem = null;
    public Sprite selectedSprite;

    private string parentPath;

    void Start()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].dropdownManager = this;
        }

        SelectItemByIndex(0);
        CreateFolder();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveSelection(1);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveSelection(-1);
        }
        //else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        //{
        //    if (selectedItem != null)
        //        selectedItem.OnClick();
        //}
    }

    public void SelectItem(DropdownItem item)
    {
        bool isChanged = false;
        if (selectedItem != null)
        {
            selectedItem.SetSelected(false);

            if (selectedItem != item)
            {
                // �ٲ���� �� ���ͷ���.
                isChanged = true;
            }
        }

        selectedItem = item;
        currentIndex = items.IndexOf(item);
        selectedItem.SetSelected(true);

        if (isChanged)
        {
            galleryManager.wallMoving.ResetWall(1);
            galleryManager.classManager.SetPainting();
            galleryManager.UpdateWallUI();
        }
    }

    private void MoveSelection(int direction)
    {
        int nextIndex = currentIndex + direction;

        if (nextIndex < 0) nextIndex = items.Count - 1;
        if (nextIndex >= items.Count) nextIndex = 0;

        SelectItemByIndex(nextIndex);
    }

    public void SelectItemByIndex(int index)
    {
        if (index < 0 || index >= items.Count) return;
        SelectItem(items[index]);
    }

    public void CreateFolder()
    {
        CreateParentFolder();

        for (int i = 0; i < items.Count; i++)
        {
            var txt = items[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            if (txt != null)
            {
                CreateFolder(txt.text);
            }
        }
    }

    public void CreateFolder(string folderName)
    {
        if (string.IsNullOrWhiteSpace(folderName)) return;
       
        string folderPath = Path.Combine(parentPath, folderName);

        // ��ø ���� ����
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log("���� ������: " + folderPath);
        }
        else
        {
            Debug.Log("�̹� ������: " + folderPath);
        }
    }

    public void CreateParentFolder()
    {
        // ������Ʈ ��Ʈ ��� (Assets ����)
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string newFolderName = "�츮�� ������";

        // ��ü ���
        parentPath = Path.Combine(projectRoot, newFolderName);

        if (!Directory.Exists(parentPath))
        {
            Directory.CreateDirectory(parentPath);
            Debug.Log("������ ���: " + parentPath);
        }
        else
        {
            Debug.Log("�̹� �����ϴ� ����: " + parentPath);
        }
    }

    public string GetCurrentPath()
    {
        var folderName = selectedItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        return Path.Combine(parentPath, folderName.text);
    }

}
