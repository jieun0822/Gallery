using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GaugeController : MonoBehaviour
{
    [Header("°ø¿ë")]
    public GameManager manager;
    public Image gaugeBar;
    private float fillSpeed = 6f;
    public UnityEvent onGaugeComplete;
    private float currentFill = 0f;
    private bool isFilling = false;
    public bool IsFilling { get => isFilling; }
    public bool triggered = false;

    private float cooldownTime = 0f;
    private float cooldownTimer = 0f;
    [Header("ÇØ¹Ù¶ó±â ¾À")]
    public GameEnums.eFlowerType sunFlowerType;
    public string gaugeName;
    public SkeletonGraphic skeletonGraphic;

    [Header("·ë ¾À")]
    public bool isTouchable = true;

    private void Start()
    {
        SetIsFilling(false);
    }

    public void Init()
    {
        currentFill = 0f;
        gaugeBar.fillAmount = 0f;
        triggered = false;
    }

    void Update()
    {
        if (!isTouchable && triggered)
        {
            currentFill = 0f;
            gaugeBar.fillAmount = 0f;
            triggered = false;
            return;
        }
        else if (!isTouchable) return;

        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (isFilling && !triggered)
        {
            currentFill += Time.deltaTime * fillSpeed;
            gaugeBar.fillAmount = Mathf.Clamp01(currentFill);

            if (currentFill >= 1f)
            {
                triggered = true;
                currentFill = 1f;  // ²Ë Âù »óÅÂ À¯Áö
                onGaugeComplete?.Invoke();

                // °ÔÀÌÁö ²Ë Ã¡À¸¸é ÀÚµ¿À¸·Î isFilling ²ô±â
                SetIsFilling(false);
                cooldownTimer = cooldownTime;  // Äð´Ù¿î ½ÃÀÛ
            }
        }
        else if (!isFilling)
        {
            currentFill = 0f;
            gaugeBar.fillAmount = 0f;
            triggered = false;
        }
    }

    public void SetIsFilling(bool filling)
    {
        if (cooldownTimer > 0f && filling == true) return; // Äð´Ù¿î ÁßÀÌ¸é ¹«½Ã

        if (isFilling != filling)
        {
            isFilling = filling;
        }
    }

    public bool IsComplete()
    {
        return triggered && !isFilling && cooldownTimer > 0f;
    }

    float GetAnimationDuration(string animationName)
    {
        var animation = skeletonGraphic.Skeleton.Data.FindAnimation(animationName);
        if (animation != null)
            return animation.Duration;
        else return 0f;
    }
}
