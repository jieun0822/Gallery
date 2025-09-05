using System.Collections;
using UnityEngine;

[System.Serializable]
public class SoundData
{
    public AudioClip[] sound;
    public int[] soundIndex = new int[3];
}

public class SoundManager : MonoBehaviour
{
    [Header("����")]
    public GameManager gameManager;
    public AudioClip[] bgm;

    [Header("��Ʈ�� ��")]
    public AudioClip introClick;

    [Header("������ ��")]
    public AudioClip galleryClick;

    [Header("�عٶ�� ��")]
    public AudioClip[] flowerSound; // 0 : ����, 1 : ����, 2 : �õ���� 

    [Header("������ �� ��")]
    public AudioClip foundSound;

    [Header("�� ��")]
    public AudioClip[] scaleClips; // Inspector�� ��~�� 7�� ���� �ֱ�
    public AudioClip starSound;
    public AudioClip windSound;
    private int windIndex = -1;

    [Header("��� ��")]
    public AudioClip crowSound;
    private Coroutine crowCoroutine;
    private int crowIndex;

    [Header("�츮�� ������")]
    public AudioSource fanfareSource;

    [Header("AI ����")]
    public SoundData ai_sunflower;
    public SoundData ai_star;
    public SoundData ai_crow;
    public SoundData ai_room;
    public AudioClip cameraSound;

    [Header("���� ��")]
    public AudioClip stageBgm;
    public AudioClip curtainSound;

    [Header("����� �ҽ�")]
    public AudioSource bgmSource;
    public AudioSource[] bgmSources;
    public AudioSource[] effectSource;
    public AudioSource[] clickSource;

    private int noteIndex = 0; // ���� ����� ���� �ε���
    private GameEnums.eScene prevScene = GameEnums.eScene.None;

    private void Update()
    {
        // �ʱ�ȭ.
        if (prevScene != gameManager.currentScene)
        {
            if (prevScene == GameEnums.eScene.Star)
            {
                StopWindSound();
            }
            else if (prevScene == GameEnums.eScene.Crow)
            {
                StopCrowSound();
            }

            StopEffectSound();
            prevScene = gameManager.currentScene;
        }
    }

    public void PlayBGM(GameEnums.eScene scene)
    {
        int index = -1;
        switch (scene)
        {
            case GameEnums.eScene.Intro:
                index = 0;
                break;
            case GameEnums.eScene.SunFlower:
                index = 1;
                break;
            case GameEnums.eScene.Room:
                index = 2;
                break;
            case GameEnums.eScene.Star:
                index = 3;
                break;
            case GameEnums.eScene.Crow:
                index = 4;
                break;
            case GameEnums.eScene.EastArt:
                index = 5;
                break;
            case GameEnums.eScene.Stage:
                index = 6;
                break;
        }

        if (index >= 0 && index < bgm.Length)
        {
            if (bgmSource.clip != bgm[index])
            {
                bgmSource.clip = bgm[index];
                bgmSource.Play();
            }
        }
    }

    public void StopBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop(); 
            bgmSource.clip = null;
        }
    }

    public void PlayEffectSound()
    {
        int index = -1;
        for (int i = 0; i < effectSource.Length; i++)
        {
            if (effectSource[i].isPlaying) continue;

            index = i;
            break;
        }

        effectSource[index].clip = null;
    }

    public int FindSoundIndex(bool isClick)
    {
        if (isClick)
        {
            for (int i = 0; i < clickSource.Length; i++)
            {
                if (clickSource[i].isPlaying) continue;

                return i;
            }
        }
        else
        {
            for (int i = 0; i < effectSource.Length; i++)
            {
                if (effectSource[i].isPlaying) continue;

                return i;
            }
        }
       
        return -1;
    }

    public void StopEffectSound()
    {
        for (int i = 0; i < effectSource.Length; i++)
        {
            if (effectSource[i] != null && effectSource[i].isPlaying)
            {
                effectSource[i].Stop();
            }

            effectSource[i].clip = null;
        }

        StopFanfareSound();
    }

    // ��Ʈ�� ��.
    public void ClickSound(bool touchSound)
    {
        int index = FindSoundIndex(true);
        if (index == -1) return;

        clickSource[index].clip = (touchSound) ? introClick : galleryClick;
        clickSource[index].Play();
    }


    public void PlayEffectSound(GameEnums.eScene scene, int stuffIndex = -1)
    {
        int index = FindSoundIndex(false);
        if (index == -1) return;

        if (scene == GameEnums.eScene.SunFlower)
        {
            effectSource[index].clip = flowerSound[stuffIndex];
            effectSource[index].Play();
        }
        else if (scene == GameEnums.eScene.Room)
        {
            effectSource[index].clip = foundSound;
            effectSource[index].Play();
        }
        else if (scene == GameEnums.eScene.Star)
        {
            if (stuffIndex == 0)
            {
                effectSource[index].clip = starSound;
                effectSource[index].Play();
            }
            else if (stuffIndex == 1)
            {
                // �ٶ��Ҹ�.
                if (windIndex != -1 && effectSource[windIndex].isPlaying) return;
                windIndex = index;
                effectSource[index].clip = windSound;
                effectSource[index].Play();
            }
            else if (stuffIndex >= 2 && stuffIndex <= 8)
            {
                effectSource[index].clip = scaleClips[stuffIndex -2];
                effectSource[index].Play();
            }
        }
        else if (scene == GameEnums.eScene.Crow)
        {
            crowIndex = index;
            effectSource[index].clip = crowSound;

            crowCoroutine = StartCoroutine(CoPlayWithFadeOut(effectSource[index], 10, 1));
        }
    }

    // �� ��.
    public void StopWindSound()
    {
        if (windIndex == -1 || !effectSource[windIndex].isPlaying) return;

        effectSource[windIndex].Stop();
        effectSource[windIndex].clip = null;
        windIndex = -1;
    }

    public void PlayNextNote()
    {
        if (scaleClips == null || scaleClips.Length == 0) return;

        // ���� ���

        int index = FindSoundIndex(false);
        if (index == -1) return;

        effectSource[index].clip = scaleClips[noteIndex];
        effectSource[index].Play();

        // ���� �ε����� ����, 0~6 �ݺ�
        noteIndex = (noteIndex + 1) % scaleClips.Length;
    }

    // ��� ��
    public void StopCrowSound()
    {
        if (crowCoroutine == null) return;
        StopCoroutine(crowCoroutine);

        if (crowIndex != -1 && effectSource[crowIndex].isPlaying)
            effectSource[crowIndex].Stop();
    }

    private IEnumerator CoPlayWithFadeOut(AudioSource audio, float totalPlayTime, float fadeOutDuration)
    {
        // ������ �ʱⰪ���� ����
        audio.volume = 1f;
        audio.Play();

        // ���̵�ƿ� ������ ��ٸ���
        yield return new WaitForSeconds(totalPlayTime - fadeOutDuration);

        // ���̵�ƿ� ����
        float startVolume = audio.volume;
        float time = 0f;

        while (time < fadeOutDuration)
        {
            time += Time.deltaTime;
            audio.volume = Mathf.Lerp(startVolume, 0f, time / fadeOutDuration);
            yield return null;
        }

        // ������ 0�� �Ǿ����� ����
        audio.Stop();
        audio.clip = null;
        audio.volume = 1f; // ���� ����� ���� ����

        if (crowCoroutine != null) crowCoroutine = null;
    }

    // �츮�� ������.
    public void PlayFanfareSound()
    {
        if (!fanfareSource.isPlaying) fanfareSource.Play();
    }

    public void StopFanfareSound()
    {
        if(fanfareSource.isPlaying) fanfareSource.Stop();
    }

    // AI ����
    public void PlayAITeacherSound(GameEnums.eScene scene, int index)
    {
        int soundIndex = FindSoundIndex(true);
        if (soundIndex == -1) return;

        switch (scene)
        {
            case GameEnums.eScene.SunFlower: // �عٶ��
                ai_sunflower.soundIndex[soundIndex] = soundIndex;
                effectSource[soundIndex].clip = ai_sunflower.sound[index];
                break;
            case GameEnums.eScene.Room: // ������ ��
                ai_room.soundIndex[soundIndex] = soundIndex;
                effectSource[soundIndex].clip = ai_room.sound[index];
                break;
            case GameEnums.eScene.Star: // ���� ������ ��
                ai_star.soundIndex[soundIndex] = soundIndex;
                effectSource[soundIndex].clip = ai_star.sound[index];
                break;
            case GameEnums.eScene.Crow: // ��� ���� ���
                ai_crow.soundIndex[soundIndex] = soundIndex;
                effectSource[soundIndex].clip = ai_crow.sound[index];
                break;
        }
        effectSource[soundIndex].Play();
    }

    public void StopAITeacherSound()
    {
        var coroutine = gameManager.galleryManager.playSoundCoroutine;
        if(coroutine != null) StopCoroutine(coroutine);

        for (int i = 0; i < ai_sunflower.soundIndex.Length; i++)
        {
            var index = ai_sunflower.soundIndex[i];
            if (index == -1) continue;
            if(effectSource[index].isPlaying) effectSource[index].Stop();
            ai_sunflower.soundIndex[i] = -1;
        }

        for (int i = 0; i < ai_star.soundIndex.Length; i++)
        {
            var index = ai_star.soundIndex[i];
            if (index == -1) continue;
            if (effectSource[index].isPlaying) effectSource[index].Stop();
            ai_star.soundIndex[i] = -1;
        }

        for (int i = 0; i < ai_crow.soundIndex.Length; i++)
        {
            var index = ai_crow.soundIndex[i];
            if (index == -1) continue;
            if (effectSource[index].isPlaying) effectSource[index].Stop();
            ai_crow.soundIndex[i] = -1;
        }

        for (int i = 0; i < ai_room.soundIndex.Length; i++)
        {
            var index = ai_room.soundIndex[i];
            if (index == -1) continue;
            if (effectSource[index].isPlaying) effectSource[index].Stop();
            ai_room.soundIndex[i] = -1;
        }
    }

    public void PlaySound(AudioClip clip)
    {
        int index = FindSoundIndex(false);
        if (index == -1) return;

        effectSource[index].clip = clip;
        effectSource[index].Play();
    }
}
