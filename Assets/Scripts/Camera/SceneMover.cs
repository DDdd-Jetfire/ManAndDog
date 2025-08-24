    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SceneMover : MonoBehaviour
    {
        public int toScene = 0;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision != null)
            {
                if (collision.GetComponent<HumanController>() != null)
                {
                    ChainManager.instance.RedoAlertAndTo(toScene);
                }
            }
        }
    }
