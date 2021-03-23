using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KHSKController
{
    class KHSKExBpkController
    {
        private Transform kerbalSpine;


        //private Part exBpkPart;

        private Transform exBpkPack;
        private Transform exBpkUnpack;
        private Renderer exBpkPackRenderer;
        private Renderer exBpkUnpackRenderer;

        private Vector3 mainBodyPos;
        private Vector3 mainBodyRot;

        public int expandInventorySlots;
        public float expandInventoryMass;
        public float expandInventoryVolume;

        private Transform exBpk;
        public KHSKExBpkController(Transform spine, KerbalEVA parentKerbal)
        {
            Part aviPartPrefab = KHSKUtility.GetAviPartFromPartLoader("KSInfBackpack");
            if (aviPartPrefab == null)
            {
                throw new Exception("find aviPart fail");
            }

            KHSKExBackpack exBpkModule = aviPartPrefab.FindModuleImplementing<KHSKExBackpack>();

            kerbalSpine = spine;

            mainBodyPos = exBpkModule.mainBodyOffset0;
            mainBodyRot = exBpkModule.mainBodyRotate0;
            //expandInventorySlots = exBpkModule.slots;
            //expandInventoryMass = exBpkModule.mass;
            //expandInventoryVolume = exBpkModule.volume;

            exBpk = KHSKUtility.InitalizeGameObject("kerbalExtendBox", spine, mainBodyPos, mainBodyRot ,parentKerbal.transform, aviPartPrefab);
            if (exBpk == null)
            {
                throw new Exception("InitalGameObject fail");
            }
            else
            {
                Debug.Log("[KHSK DEBUG]exBackpack=" + exBpk.ToString());
            }

            exBpkPack = KHSKUtility.BoneFindByPath("PackBox", exBpk);
            exBpkUnpack = KHSKUtility.BoneFindByPath("UnpackBox", exBpk);

            exBpkUnpackRenderer = exBpkUnpack.GetComponent<MeshRenderer>();
            exBpkPackRenderer = exBpkPack.GetComponent<MeshRenderer>();

            exBpkUnpackRenderer.enabled = false;


        }

        public void ExBpkUpdate()
        {
            KHSKUtility.UpdateGameObjectTransformByAnchor(exBpkPack, kerbalSpine, mainBodyPos, mainBodyRot);
            KHSKUtility.UpdateGameObjectTransformByAnchor(exBpkUnpack, kerbalSpine, mainBodyPos, mainBodyRot);
        }

        public void DestroyPart()
        {
            UnityEngine.Object.Destroy(exBpk.gameObject);
        }

        //-----------------------------Feartures------------------------------
        /// <summary>
        /// change Inflate backpack Statue(false For close/true for open)
        /// </summary>
        /// <param name="switcher"></param>
        public void ToggleExBpk(bool switcher)
        {
            if (switcher)
            {
                exBpkPackRenderer.enabled = false;
                exBpkUnpackRenderer.enabled = true;
            }
            else
            {
                exBpkPackRenderer.enabled = true;
                exBpkUnpackRenderer.enabled = false;
            }
        }
    }
}
