using Spine;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PosData
{
    public string name;
    public Transform pos;
    public GaugeController gauge;
}

// �عٶ�� ��.
[System.Serializable]
public class FlowerPosData : PosData
{
    public GameEnums.eFlower flowerType;
    public SkeletonAnimation Item;
    public SkeletonAnimation touchAni;
    public SkeletonAnimation glitterAni;
}

// �� ��.
[System.Serializable]
public class RoomPosData : PosData
{
    public bool isFound;
    public bool isFindable;
    
    public List<string> basicAni;
    public List<string> findAni;
    public List<string> touchAni;

    public SkeletonAnimation Item;
    public SkeletonAnimation bottomItem;
    public Image checkItem;
}

// �� ��.
[System.Serializable]
public class StarPosData : PosData
{
    public SkeletonAnimation Item;
    public string touchName;
}

public class CheckObjectInUIImage : MonoBehaviour
{
    [Header("����")]
    public GameManager manager;
    public SpineUIController spine;
    private Dictionary<Transform, List<GaugeController>> lastHoveredGauge = new();
    private List<PosData> datas;
    public GameEnums.eScene prevScene = GameEnums.eScene.None;
    public bool isTouchable = true;

    [Header("�عٶ�� ��")]
    public List<FlowerPosData> flowerPosDatas;
    public List<Transform> handImgs;          // ���� ǥ���ϴ� UI �̹���

    private float glitterTime = 3f; //��¦��.
    private float glitterTimer = 0f;

    [Header("�� ��")]
    public List<RoomPosData> stuffPosDatas;
    private int targetCount = 0; // ã�ƾ� �� �� ����

    [Header("�� ��")]
    public List<StarPosData> starPosDatas;

    private void Start()
    {
        manager.check = this;
    }

    private void InitSpine(SkeletonAnimation spine)
    {
        spine.AnimationState.ClearTrack(0);
        spine.Skeleton.SetToSetupPose();
        spine.Update(0);
    }

    public void InitFlowerScene()
    {
        foreach (var item in flowerPosDatas)
        {
            item.gauge.isTouchable = true;
        }

        //������ ����.
        for (int i = 0; i < spine.potGroup.Length; i++)
        {
            InitSpine(spine.potGroup[i]);
            spine.potGroup[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < flowerPosDatas.Count; i++)
        {
            // ����
            flowerPosDatas[i].Item.gameObject.SetActive(false);
            var skeleton = flowerPosDatas[i].Item;
            skeleton.AnimationState.ClearTrack(0);
       
            var name = flowerPosDatas[i].name;
            name = name.EndsWith("_02") ? name.Substring(0, name.Length - 3) : name;
            skeleton.AnimationState.SetAnimation(0, name + "_none", false);
            skeleton.Update(0);

            // ����Ʈ
            flowerPosDatas[i].touchAni.gameObject.SetActive(false);
            InitSpine(flowerPosDatas[i].touchAni);
            flowerPosDatas[i].glitterAni.gameObject.SetActive(false);
            InitSpine(flowerPosDatas[i].glitterAni);

            // ������
            flowerPosDatas[i].gauge.gameObject.SetActive(false);
            flowerPosDatas[i].gauge.Init();
        }

        //�ڷ�ƾ ����.
        foreach (var c in spine.flowerCoroutines)
        {
            StopCoroutine(c);
        }
        spine.flowerCoroutines.Clear();

        spine.InitFlowerScene();

        prevScene = GameEnums.eScene.SunFlower;
    }

    public void InitRoomScene()
    {
        targetCount = 0;
        spine.InitRoomScene();

        foreach (var item in stuffPosDatas)
        {
            item.isFound = false;
            item.gauge.isTouchable = true;
            item.gauge.gameObject.SetActive(true);
            spine.foundCount = 0;
            if (item.isFindable) targetCount++;

            if (item.name == "bed_shoes")
            {
                item.gauge.isTouchable = true;
                item.Item.gameObject.SetActive(true);
            }

            InitSpine(item.Item);

            if (item.bottomItem == null) continue;
            InitSpine(item.bottomItem);
            item.bottomItem.gameObject.SetActive(false);
            item.checkItem.gameObject.SetActive(false);
        }
    }

    public void InitStarScene()
    {
        foreach (var item in starPosDatas)
        {
            item.gauge.isTouchable = true;
            item.gauge.gameObject.SetActive(true);

            var skeleton = item.Item;
            skeleton.AnimationState.SetAnimation(0, "basic", false);
            skeleton.Update(0);
        }

        //�ڷ�ƾ ����.
        spine.InitStarScene();
    }

    void Update()
    {
        switch (manager.currentScene)
        {
            case GameEnums.eScene.SunFlower:
                if (prevScene != GameEnums.eScene.SunFlower) InitFlowerScene();
                datas = new List<PosData>(flowerPosDatas);// ��ĳ����.
                break;
            case GameEnums.eScene.Room:
                if (prevScene != GameEnums.eScene.Room) InitRoomScene();
                datas = new List<PosData>(stuffPosDatas);

                if (targetCount == spine.foundCount)
                {
                    spine.foundCount = 0;
                    //manager.ShowGuideWin(true);
                    //�� ã����.
                }
                break;
            case GameEnums.eScene.Star:
                if (prevScene != GameEnums.eScene.Star) InitStarScene();
                datas = new List<PosData>(starPosDatas);
                break;
            case GameEnums.eScene.Crow:
                return;  
        }

        if (prevScene != manager.currentScene)
            prevScene = manager.currentScene;

        if (prevScene == GameEnums.eScene.None) return;
        if (!isTouchable) return;

        Dictionary<Transform, List<GaugeController>> hoveredThisFrame = new();

        int count = 0;

        foreach (Transform hand in handImgs)
        {
            // ���� �� �ν��� �ʿ� ���� ��
            if (prevScene != GameEnums.eScene.SunFlower &&
                prevScene != GameEnums.eScene.Star &&
                count == 20) break;

            count++;
            if (!hand.gameObject.activeSelf) continue;
            RectTransform handRect = hand as RectTransform;
            if (handRect == null) continue;

            foreach (var value in datas)
            {
                RectTransform ImageRect = value.pos as RectTransform;

                if (ImageRect == null) continue;
                if (IsOverlappingOnScreen(handRect, ImageRect, Camera.main))
                {
                    GaugeController gc = null;
                    RoomPosData roomData = null;

                    switch (manager.currentScene)
                    {
                        case GameEnums.eScene.SunFlower:
                            FlowerPosData flowerData = value as FlowerPosData; // �ٿ�ĳ����.
                            gc = flowerData.gauge;
                            break;
                        case GameEnums.eScene.Room:
                            roomData = value as RoomPosData; // �ٿ�ĳ����.
                            gc = roomData.gauge;
                            break;
                        case GameEnums.eScene.Star:
                            StarPosData starData = value as StarPosData; // �ٿ�ĳ����.
                            gc = starData.gauge;
                            break;
                    }

                    if (gc == null || gc.IsComplete()) continue;

                    // �̹� ������ ȣ�� ó��
                    if (gc != null && gc.isTouchable)
                        gc.SetIsFilling(true);
                    
                    if (gc.triggered && manager.currentScene == GameEnums.eScene.Room)
                    {
                        spine.TurnOnEffect(roomData);
                    }

                    if (!hoveredThisFrame.ContainsKey(hand))
                        hoveredThisFrame[hand] = new List<GaugeController>();

                    if (!hoveredThisFrame[hand].Contains(gc))
                        hoveredThisFrame[hand].Add(gc);
                }
            }
        }

        // ���� �����ӿ��� ��������, �̹� �����ӿ��� �� ��ģ �� ����
        foreach (var kvp in lastHoveredGauge)
        {
            Transform hand = kvp.Key;
            List<GaugeController> prevGauges = kvp.Value;

            foreach (var gc in prevGauges)
            {
                if (!hoveredThisFrame.TryGetValue(hand, out var currentList) || !currentList.Contains(gc))
                {
                    gc.SetIsFilling(false);
                }
            }
        }

        // ���� ����
        lastHoveredGauge = hoveredThisFrame;

        //�عٶ���.
        if (manager.currentScene == GameEnums.eScene.SunFlower)
        {
            glitterTimer += Time.deltaTime;
            if (glitterTimer >= glitterTime)
            {
                glitterTimer = 0f;
                TurnOnGlitter();
            }
        }
    }

    bool IsOverlappingOnScreen(RectTransform a, RectTransform b, Camera cam)
    {
        Rect aRect = GetScreenRect(a, cam);
        Rect bRect = GetScreenRect(b, cam);

        return aRect.Overlaps(bRect);
    }

    Rect GetScreenRect(RectTransform rt, Camera cam)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners); // ����, �»�, ���, ���� ��

        Vector2 bottomLeft = RectTransformUtility.WorldToScreenPoint(cam, corners[0]);
        Vector2 topRight = RectTransformUtility.WorldToScreenPoint(cam, corners[2]);

        return new Rect(bottomLeft, topRight - bottomLeft);
    }

    // �عٶ���.
    void TurnOnGlitter()
    {
        //���� �ȵȰ� ���� ��¦�� ����.
        List<GaugeController> noHovered = new List<GaugeController>();
        foreach (var data in flowerPosDatas)
        {
            bool isContain = false;

            foreach (var kvp in lastHoveredGauge)
            {
                var list = kvp.Value;
                if (list.Contains(data.gauge))
                {
                    isContain = true;
                    break;
                }
            }
            if (!isContain) noHovered.Add(data.gauge);
        }

        Debug.Log("count : " + noHovered.Count);

        if (noHovered.Count == 0) return;

        int randomIndex = UnityEngine.Random.Range(0, noHovered.Count); // 0 �̻� Count �̸�

        Debug.Log("random : " + randomIndex);
        GaugeController selectedGauge = noHovered[randomIndex];
        spine.TurnOnGiltter(selectedGauge.gaugeName);
    }

    public void DisableGauge(GameEnums.eScene scene)
    {
        switch (scene)
        {
            case GameEnums.eScene.SunFlower:
                for (int i = 0; i < flowerPosDatas.Count; i++)
                {
                    var gauge = flowerPosDatas[i].gauge;
                    gauge.gameObject.SetActive(false);
                }
                break;
            case GameEnums.eScene.Room:
                for (int i = 0; i < stuffPosDatas.Count; i++)
                {
                    var gauge = stuffPosDatas[i].gauge;
                    gauge.gameObject.SetActive(false);
                }
                break;
            case GameEnums.eScene.Star:
                for (int i = 0; i < starPosDatas.Count; i++)
                {
                    var gauge = starPosDatas[i].gauge;
                    gauge.gameObject.SetActive(false);
                }
                break;
        }
    }
}
