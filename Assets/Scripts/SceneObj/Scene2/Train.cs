using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Train : NetworkBehaviour
{
    HumanController hc;
    public AniBase ani;
    public bool hitFlag = false;

    public float maxHitTime = 3f;
    private float currentHitTime = 0;


    private bool isRunning = false;
    Rg r;

    [SyncVar(hook = nameof(OnRedChange))]private bool redflag = false;

    private void OnRedChange(bool old, bool  newFlag)
    {
        Debug.Log($"Set Red Flag To {newFlag}");
        if (newFlag)
        {

            ani.PlayOneShoot("show");

            if (isServer)
            {
                SetRedFlag(false);
            }
        }
    }

    private void Awake()
    {
        ani = gameObject.GetComponent<AniBase>();
        r = FindFirstObjectByType<Rg>();
        currentHitTime = maxHitTime;

        // ����Э�̣���ʼ�л����̵�
        StartCoroutine(SwitchTrafficLights());
    }

    private void Update()
    {
        ani.TransAction("idle");

        if (isRunning && hitFlag ==false)
        {
            if (hc != null)
            {
                hc.CmdUpdateTrainHit(true);
                hitFlag = true;
            }
        }
        //if (redflag)
        //{
        //    redflag = false;
        //}
    }

    public void TrainHit()
    {
        isRunning = true;
    }

    // Э�̣�ÿ3���л�һ�κ��̵�
    IEnumerator SwitchTrafficLights()
    {
        while (true)
        {
            // �л������
            Debug.Log("red start");

            if (isServer)
            {

                SetRedFlag(true);
            }
            if (r != null) r.SetRed();

            yield return new WaitForSeconds(3f); // �ȴ� 3 ��

            Debug.Log("green start");
            if (r != null) r.SetGreen();
            // �л����̵�
            yield return new WaitForSeconds(3f); // �ȴ� 3 ��
        }
    }

    [Server]
    private void SetRedFlag(bool flag)
    {

        Debug.Log($"server set red flag");
        redflag = flag;
    }

    public void HitEnd()
    {
        isRunning = false;
        if (hitFlag == true)
        {

            hitFlag = false;
            hc.CmdUpdateTrainHit(false);
            ChainManager.instance.ResetCurrentScene();
            hc = null;
        }
        //currentHitTime = maxHitTime;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (hitFlag == false)
        {

            if (collision != null)
            {
                hc = collision.GetComponent<HumanController>();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (hitFlag == false)
        {

            if (collision != null)
            {
                HumanController hcr = collision.GetComponent<HumanController>();
                if (hcr != null)
                {
                    hc = null;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }
}
