using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    // Add this to your player prefab and it will have the classic NoClip ability.
    public class NoClip : MonoBehaviour
    {
        [MMInformation("Toogle NoClip with <b>F4</b>",MoreMountains.Tools.MMInformationAttribute.InformationType.Info,false)]
        [MMReadOnly] public bool noClip;
     
     
        public virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.F4))
            {
                noClip = !noClip;
            }

            if(Input.GetKeyDown(KeyCode.F4) && !noClip)
            {
                Debug.Log("NOCLIP OFF");
                GetComponent<CorgiController>().CollisionsOn();
                GetComponent<CorgiController>().GravityActive(true);
                GetComponent<Health>().Invulnerable = false;
            }
            else if (Input.GetKeyDown(KeyCode.F4) && noClip)
            {
                Debug.Log("NOCLIP ON");
                GetComponent<CorgiController>().CollisionsOff();
                GetComponent<CorgiController>().GravityActive(false);
                GetComponent<Health>().Invulnerable = true;
            }
        }
    }
}