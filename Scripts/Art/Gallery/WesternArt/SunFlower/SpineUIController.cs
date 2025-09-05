using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Rendering;

public class SpineUIController : MonoBehaviour
{
    [Header("공용 씬")]
    public GameManager gameManager;
    public CheckObjectInUIImage check;

    [Header("해바라기 씬")]
    public SkeletonAnimation[] potGroup;
    public List<Coroutine> flowerCoroutines = new List<Coroutine>();
    public Coroutine coInitCoroutine = null;

    [Header("룸 씬")]
    public int foundCount = 0; // 현재까지 찾은 개수
    public List<Coroutine> roomCoroutines = new List<Coroutine>();

    [Header("별 씬")]
    public List<Coroutine> starCoroutines = new List<Coroutine>();
    public List<Coroutine> treeCoroutines = new List<Coroutine>();
    public List<SkeletonAnimation> treeList = new List<SkeletonAnimation>();
    public SkeletonAnimation dark;
    public List<string> darkAni = new List<string>();
    public bool isTreeMoving = false;
    private int darkIndex = 0;
    private bool isBrightening = false;

    // 해바라기씬.
    public void InitFlowerScene()
    {
        coInitCoroutine = StartCoroutine(coInitFlowerScene());
    }

    private IEnumerator coInitFlowerScene()
    {
        check.isTouchable = false;

        for (int i = 0; i < potGroup.Length; i++)
        {
            potGroup[i].gameObject.SetActive(true);
            potGroup[i].AnimationState.SetAnimation(0, "pot_grow_up", false);
        }

        TrackEntry track = potGroup[0].AnimationState.GetCurrent(0);
        float delay = track.Animation.Duration;
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < check.flowerPosDatas.Count; i++)
        {
            var data = check.flowerPosDatas[i];
            var skeleton = data.Item;
            skeleton.gameObject.SetActive(true);

            var name = data.name;
            name = name.EndsWith("_02") ? name.Substring(0, name.Length - 3) : name;
            skeleton.AnimationState.SetAnimation(0, name + "_grow_up", false);
        }

        yield return new WaitForSeconds(2f);

        // 게이지
        for (int i = 0; i < check.flowerPosDatas.Count; i++)
        {
            check.flowerPosDatas[i].gauge.gameObject.SetActive(true);
        }
        check.isTouchable =true;

        coInitCoroutine = null;
    }

    public void InitRoomScene()
    {
        foreach (var c in roomCoroutines)
        {
            StopCoroutine(c);
        }
        roomCoroutines.Clear();
    }

    public void InitStarScene()
    {
        isTreeMoving = false ;

        foreach (var c in starCoroutines)
        {
            StopCoroutine(c);
        }
        starCoroutines.Clear();

        StopTree();

        // 배경 어둡게
        darkIndex = 0;
        dark.AnimationState.SetAnimation(0, darkAni[darkIndex], false);
    }

    public void PlayJumpAnimation(string str)
    {
        TurnOnEffect(str);
        if (gameManager.currentScene == GameEnums.eScene.Star)
        {
            int index = 0;
            switch (str)
            {
                case "star_09": index = 2; break;
                case "star_10": index = 3; break;
                case "star_11": index = 4; break;
                case "star_08": index = 5; break;
                case "star_05": index = 6; break;
                case "star_07": index = 7; break;
                case "moon": index = 8; break;
            }
            gameManager.soundManager.PlayEffectSound(gameManager.currentScene, index);
        }
    }

    private void TurnOnEffect(string str)
    {
        if (gameManager.currentScene == GameEnums.eScene.SunFlower)
        {
            if (!Enum.TryParse<GameEnums.eFlowerType>(
            str.EndsWith("_02") ? str.Substring(0, str.Length - 3) : str,
            out var result)) return;

            List<FlowerPosData> data = check.flowerPosDatas;
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].name.Equals(str))
                {
                    // 사운드 재생.
                    int flowerIndex = -1;
                    string effName = null;
                    switch (data[i].flowerType)
                    {
                        case GameEnums.eFlower.seed:
                            flowerIndex = 0;
                            effName = "effcet_seed";
                            break;
                        case GameEnums.eFlower.flower:
                            flowerIndex = 1;
                            effName = "effect_petals";
                            break;
                        case GameEnums.eFlower.withered_flower:
                            flowerIndex = 1;
                            effName = "effect_withered petals";
                            break;
                    }
                    gameManager.soundManager.PlayEffectSound(gameManager.currentScene, flowerIndex);

                    // 떨어지는 효과.
                    GameObject obj = data[i].touchAni.gameObject;
                    obj.gameObject.SetActive(true);
                    var effSkeleton = obj.GetComponent<SkeletonAnimation>();

                    // 현재 트랙에서 실행 중인 애니메이션 확인
                    var current = effSkeleton.AnimationState.GetCurrent(0);
                    if (current == null || current.IsComplete == true)
                    {
                        // 새 애니메이션 실행
                        effSkeleton.AnimationState.SetAnimation(0, effName, false);
                    }

                    // 꽃 터치.
                    int index = check.flowerPosDatas.FindIndex(f => f.name == str);
                    var flowerData = check.flowerPosDatas[index];

                    var skeleton = flowerData.Item;

                    string touchName = flowerData.name.EndsWith("_02") ? 
                        flowerData.name.Substring(0, str.Length - 3) : flowerData.name;
                    touchName = touchName + "_touch";
                    skeleton.AnimationState.SetAnimation(0, touchName, false);

                    // 플레이되는 동안 터치 막기.
                    TrackEntry track = skeleton.AnimationState.GetCurrent(0);
                    float delay = (track != null) ? track.Animation.Duration : 0f;

                    Coroutine c = null;
                    c = StartCoroutine(coStopTouch(str, delay, () => flowerCoroutines.Remove(c)));
                    flowerCoroutines.Add(c);
                    
                    // 반짝이는 효과.
                    GameObject gliterObj = data[i].glitterAni.gameObject;
                    if (gliterObj.activeSelf)
                        gliterObj.SetActive(false);

                    break;
                }
            }
        }
        else if (gameManager.currentScene == GameEnums.eScene.Star)
        {
            if (!Enum.TryParse<GameEnums.eStarType>(str, out GameEnums.eStarType result)) return;

            // 화면 밝게.
            if (!isBrightening && darkIndex < 10)
            {
                darkIndex++;
                StartCoroutine(coBrighteningBG());
            }

            // 별 터치.
            int index = check.starPosDatas.FindIndex(f => f.name == str);
            SkeletonAnimation skeleton = check.starPosDatas[index].Item;
            skeleton.AnimationState.SetAnimation(0, check.starPosDatas[index].touchName, false);

            TrackEntry track = skeleton.AnimationState.GetCurrent(0);
            float delay = (track != null) ? track.Animation.Duration : 0f;

            Coroutine c = null;
            c = StartCoroutine(coStopTouch(str, delay, () => starCoroutines.Remove(c)));
            starCoroutines.Add(c);
        }
    }

    //터치 막기.
    private IEnumerator coStopTouch(string str, float delay, Action onComplete)
    {
        int index = -1;
        GaugeController controller = null;

        switch (gameManager.currentScene)
        {
            case GameEnums.eScene.SunFlower:
                index = check.flowerPosDatas.FindIndex(f => f.name == str);
                controller = check.flowerPosDatas[index].gauge;
                break;
            case GameEnums.eScene.Star:
                index = check.starPosDatas.FindIndex(f => f.name == str);
                controller = check.starPosDatas[index].gauge;
                break;
        }

        controller.isTouchable = false;
        
        yield return new WaitForSeconds(delay);

        controller.isTouchable = true;
        onComplete?.Invoke();
    }
    
    //랜덤 반짝임.
    public void TurnOnGiltter(string str)
    {
        if (!Enum.TryParse<GameEnums.eFlowerType>(
        str.EndsWith("_02") ? str.Substring(0, str.Length - 3) : str,
        out var result)) return;

        List<FlowerPosData> data = check.flowerPosDatas;
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].name.Equals(str))
            {
                data[i].glitterAni.gameObject.SetActive(true);

                SkeletonAnimation skeleton = data[i].glitterAni;
                skeleton.AnimationState.SetAnimation(0, "effect_glitter", false);
                skeleton.Update(0);
                break;
            }
        }
    }

    // 룸씬.
    public void TurnOnEffect(RoomPosData data)
    {
        if (!Enum.TryParse<GameEnums.eRooomType>(data.name, out GameEnums.eRooomType result)) return;

        string foundAni = null;
        string basicAni = null;
        string touchAni = null;

        basicAni = data.basicAni[0];
        touchAni = data.touchAni[0];
        if (data.isFindable) foundAni = data.findAni[0];

        TrackEntry track = null;
        if (!data.isFound && data.isFindable)
        {
            gameManager.soundManager.PlayEffectSound(gameManager.currentScene);
            track = data.Item.AnimationState.SetAnimation(0, foundAni, false);
        }
        else
        {
            track = data.Item.AnimationState.SetAnimation(0, touchAni, false);
        }

        Coroutine c = null;
        c = StartCoroutine(coStopTouch(data, track.Animation.Duration, () => roomCoroutines.Remove(c)));
        roomCoroutines.Add(c);

        TurnOnNextAni(track, data.Item, basicAni);
    }

    private void TurnOnNextAni(TrackEntry track, SkeletonAnimation spine, string aniName)
    {
        Spine.AnimationState.TrackEntryDelegate onComplete = null;
        onComplete = (trackEntry) =>
        {
            if (trackEntry == track)
            {
                // 이벤트 핸들러 제거
                spine.AnimationState.Complete -= onComplete;

                // 다음 애니메이션 실행
                spine.AnimationState.SetAnimation(0, aniName, false);
            }
        };

        // 이벤트 등록
        spine.AnimationState.Complete += onComplete;
    }

    private IEnumerator coStopTouch(RoomPosData data, float delay, Action onComplete)
    {
        data.gauge.isTouchable = false;
        yield return new WaitForSeconds(delay);

        if (!data.isFound && data.isFindable)
        {
            data.isFound = true;

            if (data.name == "bed_shoes")
            {
                data.Item.gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(0.3f);

            // 아래아이템.
            data.bottomItem.gameObject.SetActive(true);
            var track = data.bottomItem.AnimationState.SetAnimation(0, "find_effect", false);
            float animationLength = track.Animation.Duration;

            yield return new WaitForSeconds(animationLength);
            data.checkItem.gameObject.SetActive(true);

            foundCount++;
        }

        if (data.name == "bed_shoes") data.gauge.isTouchable = false;
        else data.gauge.isTouchable = true;

        onComplete?.Invoke();
    }

    //별이 빛나는 밤에 씬.
    public void MoveTree()
    {
        if (isTreeMoving) return;

        isTreeMoving = true;
        TrackEntry track = null;
        for (int i = 0; i < treeList.Count; i++)
        {
            treeList[i].AnimationState.SetAnimation(0, "touch", false);
            track = treeList[i].AnimationState.GetCurrent(0);
        }
        float delay = track != null ? track.Animation.Duration : 0f;

        Coroutine c = null;
        c = StartCoroutine(coFinishTreeAnimation(delay, () => treeCoroutines.Remove(c)));
        treeCoroutines.Add(c);
    }

    private IEnumerator coFinishTreeAnimation(float delay, Action action)
    { 
        yield return new WaitForSeconds(delay);
        isTreeMoving = false;
    }

    public void StopTree()
    {
        for (int i = 0; i < treeList.Count; i++)
        {
            treeList[i].AnimationState.ClearTrack(0);
            treeList[i].Skeleton.SetToSetupPose();
            treeList[i].Update(0);
        }

        foreach (var c in treeCoroutines)
        {
            StopCoroutine(c);
        }
        treeCoroutines.Clear();
    }

    private IEnumerator coBrighteningBG()
    {
        var track = dark.AnimationState.SetAnimation(0, darkAni[darkIndex], false);
        float animationLength = track.Animation.Duration;
        TurnOnNextAni(track, dark, darkAni[++darkIndex]);

        yield return new WaitForSeconds(animationLength);

        isBrightening = false;
    }  
}
