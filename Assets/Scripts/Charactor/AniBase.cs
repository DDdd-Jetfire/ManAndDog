using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LEAaniInfo
{
    public string aniName;
    public AnimationClip aniClip;
    public int priority;
}

public class AniBase : MonoBehaviour
{
    [SerializeField]
    protected Animator ani;

    protected AnimatorStateInfo animatorInfo;
    //currentStateName

    public bool inPlayOneShoot = false;

    public List<LEAaniInfo> aniInfos = new List<LEAaniInfo>();

    private Dictionary<string, string> aniNameIndex = new Dictionary<string, string>();
    private Dictionary<string, int> aniPriority = new Dictionary<string, int>();
    public string currentAniName = "null";

    void Start()
    {
        for (int i = 0; i < aniInfos.Count; i++)
        {
            aniNameIndex.Add(aniInfos[i].aniName, aniInfos[i].aniClip.name);
            aniPriority.Add(aniInfos[i].aniName, aniInfos[i].priority);
            //Debug.Log($"{aniInfos[i].aniName}  as  {aniInfos[i].aniClip.name}");
            //Debug.Log($"{aniInfos[i].aniName}  priority:  {aniInfos[i].aniClip.name}");
        }
    }

    void Update()
    {
        if (inPlayOneShoot)
        {
            animatorInfo = ani.GetCurrentAnimatorStateInfo(0);
            //Debug.Log(animatorInfo.normalizedTime);
            if (animatorInfo.normalizedTime >= 0.96f)
            {
                OnStateEnd();
            }
            //if (animatorInfo.normalizedTime % 1 >= 0.96f) OnStateEnd();
        }
    }


    public void TransAction(string actionName)
    {
        if (inPlayOneShoot) return;

        if (aniNameIndex.ContainsKey(actionName))
        {
            //!!注意：如果这里提示找不到，说明animator中的层不同，默认是Base Layer，Base Layer.aniName这样的
            ani.Play(aniNameIndex[actionName]);
            currentAniName = actionName;
        }
    }
    public void TransAction(string actionName, float progress = -1)
    {
        if (inPlayOneShoot) return;

        //专门给拖动动画用的
        if (aniNameIndex.ContainsKey(actionName))
        {
            if (progress == -1)
            {
                //!!注意：如果这里提示找不到，说明animator中的层不同，默认是Base Layer，Base Layer.aniName这样的
                ani.Play(aniNameIndex[actionName]);
            }
            else
            {
                progress = Mathf.Clamp01(progress);
                ani.Play(aniNameIndex[actionName], 0, progress);
            }
            currentAniName = actionName;
        }
        ani.speed = 0;
    }

    public void SetActionPos(float progress)
    {
        progress = Mathf.Clamp01(progress);
        ani.Play(aniNameIndex[currentAniName], 0, progress);
    }


    public void PlayOneShoot(string actionName)
    {
        if (inPlayOneShoot)
        {
            if (aniPriority.ContainsKey(actionName))
            {
                if (aniPriority[actionName] > aniPriority[currentAniName])
                {
                    ani.Play(aniNameIndex[actionName]);
                    currentAniName = actionName;
                }
            }
            return;
        }
        //Debug.Log("playOneTime");
        if (aniNameIndex.ContainsKey(actionName))
        {
            ani.Play(aniNameIndex[actionName]);
            currentAniName = actionName;
        }

        inPlayOneShoot = true;
    }

    public void SetAniSpeed(float targetSpeed)
    {
        ani.speed = targetSpeed;
    }

    public void CancelPlayOneShoot()
    {
        inPlayOneShoot = false;
    }

    protected void OnStateEnd()
    {
        //Debug.Log("end");
                //Debug.Log("break");

        inPlayOneShoot = false;
    }
}