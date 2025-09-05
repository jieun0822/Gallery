using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftsSoundManager : MonoBehaviour
{
    public CraftsManager manager;

    public AudioClip bgm;
    public AudioClip clickSound;
    public AudioClip[] guideSounds;

    [Header("오디오 소스")]
    public AudioSource bgmSource;
    public AudioSource[] effectSource;
    public AudioSource guideSource;

    private Coroutine guideCoroutine = null;
    private bool isPlaying = false;

    private void Start()
    {
        if (manager == null)
            manager = FindAnyObjectByType<CraftsManager>();
    }

    public void Init()
    {
        isPlaying = false;

        if (guideCoroutine != null)
        {
            StopCoroutine(guideCoroutine);
            EndGuideSound();
        }
    }

    public void PlayBGM()
    {
        if (bgmSource.isPlaying) return;

        bgmSource.clip = bgm;
        bgmSource.volume = 0f;
        bgmSource.Play();

        StartCoroutine(FadeInBGM(1f, 2f));
    }

    private IEnumerator FadeInBGM(float targetVolume, float duration)
    {
        float startVolume = bgmSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            yield return null;
        }

        bgmSource.volume = targetVolume; // 마지막 볼륨 보정
    }

    public void ClickSound()
    {
        int index = FindSoundIndex();
        if (index == -1) return;

        effectSource[index].clip = clickSound;
        effectSource[index].Play();
    }

    public int FindSoundIndex()
    {
        for (int i = 0; i < effectSource.Length; i++)
        {
            if (effectSource[i].isPlaying) continue;
            return i;
        }
        return -1;
    }

    public void ClickSoundBtn()
    {
        if (!isPlaying)
        {
            PlayGuideSound(manager.currentIndex);
        }
        else
        {
            MuteGuideSound();
        }
    }

    public void PlayGuideSound()
    {
        PlayGuideSound(manager.currentIndex);
    }

    public void PlayGuideSound(int index)
    {
        // ui.
        var uiManager = manager.uiManager;
        uiManager.ActiveSoundAni(true);

        if (guideSource.clip == null || guideSource.clip != guideSounds[index])
            guideSource.clip = guideSounds[index];

        guideSource.Play();

        isPlaying = true;

        if (guideCoroutine != null)
            StopCoroutine(guideCoroutine);
        guideCoroutine = StartCoroutine(WaitAndStop());
    }

    private IEnumerator WaitAndStop()
    {
        // 재생 중인 동안 대기
        while (guideSource.isPlaying)
        {
            yield return null;
        }

        // 재생이 끝나면 정지
        EndGuideSound();
    }

    public void EndGuideSound()
    {
        // ui.
        var uiManager = manager.uiManager;
        uiManager.EndSoundImg();

        guideSource.Stop();
        guideSource.clip = null;

        if (guideCoroutine != null)
        {
            StopCoroutine(guideCoroutine);
            guideCoroutine = null;
        }

        isPlaying = false;
    }

    public void MuteGuideSound()
    {
        // ui.
        var uiManager = manager.uiManager;
        uiManager.MuteSoundImg();

        guideSource.Stop();
    }
}