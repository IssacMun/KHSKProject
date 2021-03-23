using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KHSKController
{
    public class KHSKExoSkeleton : PartModule
    {
        #region KSPField.
        /// ConstructionLimit Power
        [KSPField]
        public float liftMuilty = 15f;

        [KSPField]
        public string bonePath = string.Empty;

        [KSPField]
        public Vector3 mainBodyOffset = new Vector3(0.0f, 0.0f, 0.0f);
        [KSPField]
        public Vector3 mainBodyRotate = new Vector3(0.0f, 0.0f, 0.0f);
        [KSPField]
        public Vector3 rightArmOffset = new Vector3(0.0f, 0.0f, 0.0f);
        [KSPField]
        public Vector3 rightArmRotate = new Vector3(0.0f, 0.0f, 0.0f);
        [KSPField]
        public Vector3 leftArmOffset = new Vector3(0.0f, 0.0f, 0.0f);
        [KSPField]
        public Vector3 leftArmRotate = new Vector3(0.0f, 0.0f, 0.0f);

        #endregion
    }
}
