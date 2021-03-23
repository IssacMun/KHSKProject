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
        /// Variables
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

        
        /*public void LoopBonePathShowMore(Transform rootBone)
        {
            foreach (Transform a in rootBone)
            {
                Debug.LogFormat("[KHS DEBUG] [0]{0}={1}", a.name, rootBone.name);
                foreach (Transform b in a)
                {
                    Debug.LogFormat("[KHS DEBUG] [1]{0}={2}/{1}", b.name, a.name, rootBone.name);
                    foreach (Transform c in b)
                    {
                        Debug.LogFormat("[KHS DEBUG] [2]{0}={3}/{2}/{1}", c.name, b.name, a.name, rootBone.name);
                        foreach (Transform d in c)
                        {
                            Debug.LogFormat("[KHS DEBUG] [3]{0}={4}/{3}/{2}/{1}", d.name, c.name, b.name, a.name, rootBone.name);
                            foreach (Transform e in d)
                            {
                                Debug.LogFormat("[KHS DEBUG] [4]{0}={5}/{4}/{3}/{2}/{1}", e.name, d.name, c.name, b.name, a.name, rootBone.name);
                                foreach (Transform f in e)
                                {
                                    Debug.LogFormat("[KHS DEBUG] [5]{0}={6}/{5}/{4}/{3}/{2}/{1}", f.name, e.name, d.name, c.name, b.name, a.name, rootBone.name);
                                    foreach (Transform g in f)
                                    {
                                        Debug.LogFormat("[KHS DEBUG] [6]{0}={7}/{6}/{5}/{4}/{3}/{2}/{1}", g.name, f.name, e.name, d.name, c.name, b.name, a.name, rootBone.name);
                                        foreach (Transform h in g)
                                        {
                                            Debug.LogFormat("[KHS DEBUG] [7]{0}={8}/{7}/{6}/{5}/{4}/{3}/{2}/{1}", h.name, g.name, f.name, e.name, d.name, c.name, b.name, a.name, rootBone.name);
                                            foreach (Transform i in h)
                                            {
                                                Debug.LogFormat("[KHS DEBUG] [8]{0}={9}/{8}/{7}/{6}/{5}/{4}/{3}/{2}/{1}", i.name, h.name, g.name, f.name, e.name, d.name, c.name, b.name, a.name, rootBone.name);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        */

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

        /*public static void UpdateGameObjectTransformByAnchor(Transform currentObject, Transform anchorTransform,
                                                Vector3 toolPosVector, Vector3 toolRotateVector)
        {
            if (currentObject != null && anchorTransform != null)
            {

                currentObject.position = anchorTransform.TransformPoint(toolPosVector);
                currentObject.rotation = anchorTransform.rotation * Quaternion.Euler(toolRotateVector);
            }
            else
            {
                if (currentObject == null)
                    Debug.LogWarning("[KHSK] [UpdatePart] 'currentObject' is not exsist!");
                if (anchorTransform == null)
                    Debug.LogWarning("[KHSK] [UpdatePart] 'anchorObject' is not exsist!");
            }

        }*/
    }
}
