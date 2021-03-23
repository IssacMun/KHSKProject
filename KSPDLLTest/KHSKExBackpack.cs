using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KHSKController
{
    public class KHSKExBackpack : PartModule
    {
        [KSPField]
        public Vector3 mainBodyOffset0 = new Vector3(0.0f,0.0f,0.0f);
        [KSPField]
        public Vector3 mainBodyRotate0 = new Vector3(0.0f, 0.0f, 0.0f);
        [KSPField]
        public Vector3 mainBodyOffset1 = new Vector3(0.0f, 0.0f, 0.0f);
        [KSPField]
        public Vector3 mainBodyRotate1 = new Vector3(0.0f, 0.0f, 0.0f);
        [KSPField]
        public Vector3 mainBodyOffset2 = new Vector3(0.0f, 0.0f, 0.0f);
        [KSPField]
        public Vector3 mainBodyRotate2 = new Vector3(0.0f, 0.0f, 0.0f);
        [KSPField]
        public Vector3 mainBodyOffset3 = new Vector3(0.0f, 0.0f, 0.0f);
        [KSPField]
        public Vector3 mainBodyRotate3 = new Vector3(0.0f, 0.0f, 0.0f);

        private Transform UnpackBox;
        //private Renderer UnpackBoxRenderer;

        public override void OnAwake()
        {
            UnpackBox = KerbalKHSK.BoneFindByPath("model/KSInfBackpack/AnchorMain/UnpackBox", part.transform);
            /*
            UnpackBoxRenderer = UnpackBox.GetComponent<Renderer>();
            UnpackBoxRenderer.enabled = false;*/
            UnpackBox.gameObject.SetActive(false);
        }

        /*public override void OnStart(StartState state)
        {
            UnpackBoxRenderer.enabled = false;
        }

        public override void OnPartCreatedFomInventory(ModuleInventoryPart moduleInventoryPart)
        {
            UnpackBoxRenderer.enabled = false;
        }*/
    }
}
