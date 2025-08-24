using System.Collections;
using UnityEngine;

public class Rg : MonoBehaviour
{
    public Sprite redLightSprite;   // 红灯的 Sprite
    public Sprite greenLightSprite; // 绿灯的 Sprite
    private SpriteRenderer spriteRenderer; // 用于显示红绿灯的 SpriteRenderer

    public bool isRed = true;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // 获取 SpriteRenderer 组件
        //if (spriteRenderer != null)
        //{
        //    // 启动协程，开始切换红绿灯
        //    StartCoroutine(SwitchTrafficLights());
        //}
        //else
        //{
        //    Debug.LogError("SpriteRenderer not found on the GameObject.");
        //}
    }

    public void SetRed()
    {
        spriteRenderer.sprite = redLightSprite;

    }
    public void SetGreen()
    {
        spriteRenderer.sprite = greenLightSprite;

    }


    // 协程：每3秒切换一次红绿灯
    //IEnumerator SwitchTrafficLights()
    //{
    //    while (true)
    //    {
    //        // 切换到红灯
    //        isRed = true;
    //        spriteRenderer.sprite = redLightSprite;
    //        yield return new WaitForSeconds(3f); // 等待 3 秒
    //
    //        // 切换到绿灯
    //        isRed = false;
    //        spriteRenderer.sprite = greenLightSprite;
    //        yield return new WaitForSeconds(3f); // 等待 3 秒
    //    }
    //}
}
