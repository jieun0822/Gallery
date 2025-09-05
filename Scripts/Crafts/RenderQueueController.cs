using UnityEngine;

public class RenderQueueController : MonoBehaviour
{
    public int renderQueue;

    private void Start()
    {
        SetRenderQueue(3001);
    }

    private void Update()
    {
        SetRenderQueue(renderQueue);
    }

    private void SetRenderQueue(int queue)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null) return;

        foreach (Material mat in renderer.materials)
        {
            if (mat != null)
                mat.renderQueue = queue;
        }
    }
}
