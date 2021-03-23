using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KHSKController
{

    public class KerbalKHSK : PartModule
    {
        /// <summary>
        /// Variables as
        /// </summary>
        public KerbalEVA currentKerbal;
        public ModuleInventoryPart kerbalInventory;

        public Renderer kerbalBackpackSlimRenderer;
        public Renderer kerbalBackpackRenderer;
        public Renderer kerbalBackpackSlimSTRenderer;
        public Renderer kerbalBackpackRendererFlag;

        public static Transform BoneFindByPath(string bonePath, Transform rootBoneTransform)
        {
            Transform bone = rootBoneTransform.Find(bonePath);
            if (bone == null)
            {
                string[] boneGroup = bonePath.Split(new char[1] { '/' });

                Debug.LogWarning("[KHSK] [FindBone] Caution!There is no bone named " + boneGroup[boneGroup.Length-1]);
            }
            return bone;
        }

        /// <summary>
        /// use PartLoader to get part
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected Part GetAviPartFromPartLoader(string name)
        {
            AvailablePart availablePart = PartLoader.getPartInfoByName(name);
            if (availablePart != null)
            {
                return availablePart.partPrefab;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Initialize Part after GetAviPartFromPartLoader
        /// </summary>
        /// <param name="partName"></param>
        /// <param name="anchorObject"></param>
        /// <param name="parentObject"></param>
        /// <param name="skeletonPart"></param>
        /// <returns></returns>
        protected Transform InitalizeGameObject(string partName, Transform anchorTransform, Vector3 anchorPosOffset, Vector3 anchorRotOffset, Transform parentTransform, Part skeletonPart)
        {
            if (skeletonPart != null)
            {
                Vector3 positionOffset = anchorTransform.TransformPoint(anchorPosOffset);
                Quaternion rotation = anchorTransform.rotation * Quaternion.Euler(anchorRotOffset);

                GameObject newObject = UnityEngine.Object.Instantiate(skeletonPart.FindModelTransform(partName).gameObject, positionOffset, rotation, parentTransform);

                return newObject.transform;
            }
            else
            {
                return null;
            }
        }

        protected TrackRigObject SetTrackRigObject(Transform anchorTrans, Transform target, Transform childTrans,
                                                Vector3 childPos, Vector3 childRot)
        {
            TrackRigObject tro;
            tro = anchorTrans.gameObject.AddComponent(typeof(TrackRigObject)) as TrackRigObject;
            tro.keepInitialOffset = false;
            tro.target = target;
            tro.trackingMode = TrackRigObject.TrackMode.LateUpdate;

            childTrans.localPosition = childPos;
            childTrans.localEulerAngles = childRot;

            return tro;
        }

        protected TrackRigObject SetTrackRigObject(Transform anchorTrans, Transform target)
        {
            TrackRigObject tro;
            tro = anchorTrans.gameObject.AddComponent(typeof(TrackRigObject)) as TrackRigObject;
            tro.keepInitialOffset = false;
            tro.target = target;
            tro.trackingMode = TrackRigObject.TrackMode.LateUpdate;

            return tro;
        }

        protected void ToggleRenderer(bool switcher, params Renderer[] renderers)
        {
            foreach (Renderer rd in renderers)
            {
                if (rd != null)
                    rd.enabled = switcher;
                else
                    Debug.LogWarningFormat("[KHSK DEBUG] {0} not found!", rd.name.ToString());
            }
        }


    }
}
