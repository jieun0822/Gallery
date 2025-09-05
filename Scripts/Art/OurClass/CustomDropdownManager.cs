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
                // 바뀌었을 때 인터렉션.
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

        // 중첩 폴더 생성
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log("폴더 생성됨: " + folderPath);
        }
        else
        {
            Debug.Log("이미 존재함: " + folderPath);
        }
    }

    public void CreateParentFolder()
    {
        // 프로젝트 루트 경로 (Assets 상위)
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string newFolderName = "우리반 갤러리";

        // 전체 경로
        parentPath = Path.Combine(projectRoot, newFolderName);

        if (!Directory.Exists(parentPath))
        {
            Directory.CreateDirectory(parentPath);
            Debug.Log("생성된 경로: " + parentPath);
        }
        else
        {
            Debug.Log("이미 존재하는 폴더: " + parentPath);
        }
    }

    public string GetCurrentPath()
    {
        var folderName = selectedItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        return Path.Combine(parentPath, folderName.text);
    }

}
