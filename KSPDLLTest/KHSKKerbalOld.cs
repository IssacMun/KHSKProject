using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KHSKController
{
    public class KHSKKerbalOld : PartModule
    {

        // ------------------Public State-----------------------
        // ConstructionLimit Power
        [KSPField]
        public double liftMuilty = 5.0;
        [KSPField]
        public string bonePath = string.Empty;
        [KSPField]
        public string boneParentPath = string.Empty;
        [KSPField]
        public string bonePathPrintList = string.Empty;

        [KSPField]
        public Vector3 mainBodyOffset = new Vector3(0.0f, 0.0f, 0.0f);
        [KSPField]
        public Vector3 mainBodyRotate = new Vector3(0.0f, 0.0f, 0.0f);
        [KSPField]
        public Vector3 rightArmOffset = new Vector3(0.0f, 0.0f, 0.0f);
        [KSPField]
        public Vector3 rightArmRotate = new Vector3(0.0f, 0.0f, 0.0f);
        [KSPField]
        public Vector3 rightCalfOffset = new Vector3(0.0f, 0.0f, 0.0f);
        [KSPField]
        public Vector3 rightCalfRotate = new Vector3(0.0f, 0.0f, 0.0f);
        [KSPField]
        public Vector3 leftArmOffset = new Vector3(0.0f, 0.0f, 0.0f);
        [KSPField]
        public Vector3 leftArmRotate = new Vector3(0.0f, 0.0f, 0.0f);
        [KSPField]
        public Vector3 leftCalfOffset = new Vector3(0.0f, 0.0f, 0.0f);
        [KSPField]
        public Vector3 leftCalfRotate = new Vector3(0.0f, 0.0f, 0.0f);



        // ------------------Private State-----------------------

        private KerbalEVA currentKerbal;
        private GameObject mainTargetBone;
        private GameObject mainParentBone;


        private ModuleInventoryPart kerbalInventory;
        private bool hasExSkeleton = false;
        private double defualtConstructionWeightLimit = PhysicsGlobals.ConstructionWeightLimit;
        private GameObject exSkeletonObject;

        //-----------------Hand Calf variables-----------------------------
        private Transform exSkMainBody;
        private Transform exSkLeftArm;
        private Transform exSkRightArm;
        private Transform exSkLeftArm1;
        private Transform exSkLeftArm2;
        private Transform exSkRightArm1;
        private Transform exSkRightArm2;


        private Transform kerbalSpine;
        private Transform kerbalLeftArm;
        private Transform kerbalRightArm;


        private void GetBoneList()
        {
            //KHSKUtility.LoopBoneFindByPath(bonePathPrintList, transform);
            //KHSKUtility.LoopBonePathShowMore(KHSKUtility.LoopBoneFindByPath(bonePathPrintList, transform));
            KHSKUtility.LoopBonePathShowMore(transform);
        }

        private GameObject InitalizeGameObject(string partName, GameObject anchorObject, GameObject parentObject ,out Part skeletonPart)
        {
            AvailablePart availablePart = PartLoader.getPartInfoByName(partName);
            if (availablePart != null)
            {
                Vector3 positionOffset = anchorObject.transform.position;
                Quaternion rotation = anchorObject.transform.rotation;


                skeletonPart = availablePart.partPrefab;


                GameObject newObject = Instantiate(skeletonPart.FindModelTransform("exSkeleton").gameObject);

                return newObject;
            }
            else
            {
                Debug.LogWarning("[KHS DEBUG] [CreateBone] There is no part called:" + partName);
                skeletonPart = null;
                return null;
            }
        }


        private void UpdateGameObjectTransformByParent(Transform currentObject, Transform anchorTransform,
                                                        Vector3 toolPosVector, Vector3 toolRotateVector)
        {
            if (currentObject != null)
            {

                currentObject.position = anchorTransform.TransformPoint(toolPosVector);
                currentObject.rotation = anchorTransform.rotation * Quaternion.Euler(toolRotateVector);
            }
            else
            {
                Debug.LogWarningFormat("[KHS DEBUG] [UpdateBone] {0} is not exsist!", currentObject.name.ToString());
            }
        }

        private void UpdateSkeletonBump(Transform selfBump , Transform targetBump)
        {
            selfBump.LookAt(targetBump.position);
        }


        // ------------------Life Cycle-----------------------


        public override void OnStart(StartState state)
        {
            currentKerbal = this.vessel.GetComponent<KerbalEVA>();
            kerbalInventory = this.vessel.GetComponent<ModuleInventoryPart>();

            mainTargetBone = KHSKUtility.BoneFindByPath(bonePath, transform).gameObject;
            //mainParentBone = KHSKUtility.LoopBoneFindByPath(boneParentPath, transform).gameObject;
            mainParentBone = transform.gameObject;

            Part skPart;
            exSkeletonObject = InitalizeGameObject("exSkeleton", mainParentBone, mainParentBone ,out skPart);

            exSkMainBody = KHSKUtility.BoneFindByPath("KSPack", exSkeletonObject.transform);
            exSkLeftArm = KHSKUtility.BoneFindByPath("KSLeftRing", exSkeletonObject.transform);
            exSkRightArm = KHSKUtility.BoneFindByPath("KSRightRing", exSkeletonObject.transform);
            exSkRightArm1 = KHSKUtility.BoneFindByPath("KSRightArm1", exSkMainBody);
            exSkRightArm2 = KHSKUtility.BoneFindByPath("KSRightArm2", exSkRightArm);
            exSkLeftArm1 = KHSKUtility.BoneFindByPath("KSLeftArm1", exSkMainBody);
            exSkLeftArm2 = KHSKUtility.BoneFindByPath("KSLeftArm2", exSkLeftArm);

            //-----------kerbalSkeletonObjects---------
            Transform kerbalSpA01 = KHSKUtility.BoneFindByPath("globalMove01/joints01/bn_spA01", transform);
            kerbalSpine = KHSKUtility.BoneFindByPath("bn_spB01/bn_spc01/bn_spD01", kerbalSpA01);
            kerbalLeftArm = KHSKUtility.BoneFindByPath("bn_spB01/bn_spc01/bn_spD01/bn_l_shld01/be_l_shldEnd01/bn_l_arm01 1/bn_l_elbow_a01/bn_l_elbow_b01", kerbalSpA01);
            kerbalRightArm = KHSKUtility.BoneFindByPath("bn_spB01/bn_spc01/bn_spD01/bn_r_shld01/be_r_shldEnd01/bn_r_arm01 1/bn_r_elbow_a01/bn_r_elbow_b01", kerbalSpA01);


            //-------------DEBUG---------------

            Debug.Log("\n[KHS DEBUG] kerbalInventory =" + kerbalInventory);
            Debug.Log("[KHS DEBUG] LiftLimit =" + PhysicsGlobals.ConstructionWeightLimit.ToString());
            //Debug.Log("[KHS DEBUG] exSkeleton Name =" + exSkeletonObject.name);
            //Debug.Log("[KHS DEBUG] exskeletonPosition=" + exSkeletonObject.transform.position.ToString());
            //Debug.Log("[KHS DEBUG] KerbalEVAPosition=" + vessel.transform.position.ToString());

            if (mainTargetBone != null)
                Debug.Log("[KHS DEBUG] targetBonePosition=" + mainTargetBone.transform.position.ToString());
            else
                Debug.LogWarning("[KHS DEBUG] There is no targetBone in Kerbal");

            //GetBoneList();

            foreach(Part part in this.vessel.parts)
            {
                if (part != null)
                    Debug.Log("[KHSK DEBUG] kerbalPart = " + part.ToString());
                else
                    Debug.Log("[KHSK DEBUG] kerbalPart = null");
            }


        }



        public void Update()
        {
            //Screen Debug Display
            ScreenMessages.PostScreenMessage(vessel.name +
                                            " WalkSpeed=" + currentKerbal.walkSpeed.ToString() +
                                            " RunSpeed=" + currentKerbal.runSpeed.ToString() +
                                            " LiftStrangth=" + PhysicsGlobals.ConstructionWeightLimit.ToString().Substring(0, 6) +
                                            " hasExSkeleton=" + hasExSkeleton +
                                            " kerbalPosition=" + vessel.transform.position.ToString() +
                                            " targetBonePosition=" + mainTargetBone.transform.position.ToString(),
                                            .1f, ScreenMessageStyle.UPPER_LEFT);

            /*ScreenMessages.PostScreenMessage(exSkeletonObject.name + " " + exSkeletonObject.transform.position.ToString(),
                                .1f, ScreenMessageStyle.UPPER_CENTER);*/



            UpdateGameObjectTransformByParent(exSkMainBody, kerbalSpine, mainBodyOffset, mainBodyRotate);
            UpdateGameObjectTransformByParent(exSkRightArm, kerbalRightArm, rightArmOffset, rightArmRotate);
            UpdateGameObjectTransformByParent(exSkLeftArm, kerbalLeftArm, leftArmOffset, leftArmRotate);
            UpdateSkeletonBump(exSkLeftArm1, exSkLeftArm2);
            UpdateSkeletonBump(exSkLeftArm2, exSkLeftArm1);
            UpdateSkeletonBump(exSkRightArm1, exSkRightArm2);
            UpdateSkeletonBump(exSkRightArm2, exSkRightArm1);




            hasExSkeleton = kerbalInventory.ContainsPart("exSkeleton");
            if (hasExSkeleton)
            {
                /*backpackSTRender = currentKerbal.StorageTransform.GetComponent<MeshRenderer>();
                //backpackSTRender.enabled = false;
                Debug.Log("[DEBUG KHSK] " + backpackSTRender.ToString());*/


                if (currentKerbal.InConstructionMode)
                    PhysicsGlobals.ConstructionWeightLimit = defualtConstructionWeightLimit * liftMuilty;
                else
                    PhysicsGlobals.ConstructionWeightLimit = defualtConstructionWeightLimit;
            }


        }

        public void OnDestory()
        {
            Debug.Log("[KHS DEBUG] Destroy triggered");
            Destroy(exSkeletonObject);
        }

    }
}
