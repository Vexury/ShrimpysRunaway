using UnityEngine;

public class Flipbook : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Sprite[] flipbookSprites;
    [SerializeField] float interval = 0.2f;

    float timer;
    int index;

    MaterialPropertyBlock block;
    static readonly int BaseMapID = Shader.PropertyToID("_BaseMap");

    void Awake() => block = new MaterialPropertyBlock();

    void Update()
    {
        if (flipbookSprites.Length == 0) return;

        timer += Time.deltaTime;
        if (timer > interval)
        {
            index = (index + 1) % flipbookSprites.Length;
            meshRenderer.GetPropertyBlock(block);
            block.SetTexture(BaseMapID, flipbookSprites[index].texture);
            meshRenderer.SetPropertyBlock(block);
            timer = 0f;
        }
    }
}
