using System.Collections;
using UnityEngine;

public class Rg : MonoBehaviour
{
    public Sprite redLightSprite;   // ��Ƶ� Sprite
    public Sprite greenLightSprite; // �̵Ƶ� Sprite
    private SpriteRenderer spriteRenderer; // ������ʾ���̵Ƶ� SpriteRenderer

    public bool isRed = true;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // ��ȡ SpriteRenderer ���
        //if (spriteRenderer != null)
        //{
        //    // ����Э�̣���ʼ�л����̵�
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


    // Э�̣�ÿ3���л�һ�κ��̵�
    //IEnumerator SwitchTrafficLights()
    //{
    //    while (true)
    //    {
    //        // �л������
    //        isRed = true;
    //        spriteRenderer.sprite = redLightSprite;
    //        yield return new WaitForSeconds(3f); // �ȴ� 3 ��
    //
    //        // �л����̵�
    //        isRed = false;
    //        spriteRenderer.sprite = greenLightSprite;
    //        yield return new WaitForSeconds(3f); // �ȴ� 3 ��
    //    }
    //}
}
