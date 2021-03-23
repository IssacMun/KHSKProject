using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KHSKController
{
    class KerbalKHSKExSk : KerbalKHSK
    {
        //----------------Kerbal Bone----------------------
        private Transform kerbalSpine;
        private Transform kerbalRightArm;
        private Transform kerbalLeftArm;

        private Vector3 mainBodyPos;
        private Vector3 mainBodyRot;
        private Vector3 rightArmPos;
        private Vector3 rightArmRot;
        private Vector3 leftArmPos;
        private Vector3 leftArmRot;
        private float liftMuilty;

        private float bumpStartDis;
        private float bumpScaleFactor;

        //-----------------ExoSkeleton Variables-----------------------------
        private Transform exoSkeleton = null;

        private Transform anchorMain;
        private Transform anchorRight;
        private Transform anchorLeft;
        private TrackRigObject anchorMainTrackRig;
        private TrackRigObject anchorRightTrackRig;
        private TrackRigObject anchorLeftTrackRig;

        private Transform exSkMainBody;
        private Transform exSkLeftArm;
        private Transform exSkRightArm;
        private Transform exSkLeftArm1;
        private Transform exSkLeftArmMid;
        private Transform exSkLeftArm2;
        private Transform exSkRightArm1;
        private Transform exSkRightArmMid;
        private Transform exSkRightArm2;

        //--------------Propoties----------------------
        private const float defualtConstructionWeightLimit = 588.399f;
        private const float defaultPerKerbalConstructionWeightLimit = defualtConstructionWeightLimit;
        public float nowPerKerbalConstructionWeightLimit;

        Func<Vessel, bool> TestVesselFindKerbals = delegate (Vessel ves) { return ves.isEVA; };
        List<Vessel> kerbalsLoaded = new List<Vessel>();
        private float totalPerKerbalLiftWeight;
        private KerbalKHSKExSk otherKHSKKerbalModule;
        private uint kerbalCounter = 0;

        //-----------------exSkeleton----------------------------
        public bool hasExoSkeleton = false;
        private string exoPartName = "KSExoSkeleton";

        //--------------------Function-------------------------
        private void CreateModel()
        {
            Part aviPartExoSk = GetAviPartFromPartLoader(exoPartName);
            if (aviPartExoSk == null)
                throw new Exception("find exoSkeleton aviPart fail");

            KHSKExoSkeleton exoSkModule = aviPartExoSk.FindModuleImplementing<KHSKExoSkeleton>();

            mainBodyPos = exoSkModule.mainBodyOffset;
            mainBodyRot = exoSkModule.mainBodyRotate;
            rightArmPos = exoSkModule.rightArmOffset;
            rightArmRot = exoSkModule.rightArmRotate;
            leftArmPos = exoSkModule.leftArmOffset;
            leftArmRot = exoSkModule.leftArmRotate;
            liftMuilty = exoSkModule.liftMuilty;
            bumpStartDis = 0.016f;
            bumpScaleFactor = 30;

            nowPerKerbalConstructionWeightLimit = defaultPerKerbalConstructionWeightLimit * liftMuilty;

            exoSkeleton = InitalizeGameObject(exoPartName, kerbalSpine, mainBodyPos, mainBodyRot, currentKerbal.transform, aviPartExoSk);
            if (exoSkeleton == null)
                throw new Exception("ExoSkeleton InitalGameObject fail");

            anchorMain = BoneFindByPath("AnchorMain", exoSkeleton);
            exSkMainBody = BoneFindByPath("KSPack", anchorMain);
            exSkRightArm1 = BoneFindByPath("KSRightArm1", exSkMainBody);
            exSkLeftArm1 = BoneFindByPath("KSLeftArm1", exSkMainBody);
            exSkRightArmMid = BoneFindByPath("KSRightArmMid", exSkMainBody);
            exSkLeftArmMid = BoneFindByPath("KSLeftArmMid", exSkMainBody);

            anchorLeft = BoneFindByPath("AnchorLeft", exoSkeleton);
            exSkLeftArm = BoneFindByPath("KSLeftRing", anchorLeft);
            exSkLeftArm2 = BoneFindByPath("KSLeftArm2", exSkLeftArm);

            anchorRight = BoneFindByPath("AnchorRight", exoSkeleton);
            exSkRightArm = BoneFindByPath("KSRightRing", anchorRight);
            exSkRightArm2 = BoneFindByPath("KSRightArm2", exSkRightArm);

            anchorMainTrackRig = SetTrackRigObject(anchorMain, kerbalSpine, exSkMainBody, mainBodyPos, mainBodyRot);
            anchorRightTrackRig = SetTrackRigObject(anchorRight, kerbalRightArm, exSkRightArm, rightArmPos, rightArmRot);
            anchorLeftTrackRig = SetTrackRigObject(anchorLeft, kerbalLeftArm, exSkLeftArm, leftArmPos, leftArmRot);

            Collider exoSkCollider = BoneFindByPath("Collider", exSkMainBody).GetComponent<Collider>();
            exoSkCollider.enabled = false;
        }

        private void UpdateBump(Transform bumpColumn, Transform bumpStick, Transform bumpMid)
        {
            if (bumpColumn != null && bumpStick != null)
            {
                bumpMid.position = (bumpColumn.position + bumpStick.position) / 2;

                bumpColumn.LookAt(bumpStick.position);
                bumpMid.LookAt(bumpStick.position);

                float bumpDistance = (bumpColumn.position - bumpStick.position).sqrMagnitude;
                float bumpMidScale;
                if (bumpDistance > bumpStartDis) bumpMidScale = (bumpDistance - bumpStartDis) * bumpScaleFactor + 1;
                else bumpMidScale = 1;

                bumpMid.localScale = new Vector3(1, 1, bumpMidScale);

                bumpStick.LookAt(bumpColumn.position);

                //ScreenMessages.PostScreenMessage("bumpScaleFactor"+ bumpScaleFactor+"/"+ "bumpStartDis"+ bumpStartDis+"/" +"BumpDistance=" +bumpDistance+"/"+"BumpMidScale"+bumpMidScale);
                
            }
            else
            {
                if (bumpColumn == null)
                    Debug.LogWarning("[KHSK] [exoControllerUpdateBump] bumpColumn not exist");
                if (bumpStick == null)
                    Debug.LogWarning("[KHSK] [exoControllerUpdateBump] bumpStick not exist");
            }
        }

        public void ExoSkUpdate()
        {
            UpdateBump(exSkRightArm1, exSkRightArm2, exSkRightArmMid);
            UpdateBump(exSkLeftArm1, exSkLeftArm2, exSkLeftArmMid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="switcher"></param>
        private void ChangeWeightLimit(bool switcher)
        {
            if (switcher)
            {
                kerbalCounter = 0;
                totalPerKerbalLiftWeight = 0;
                kerbalsLoaded = FlightGlobals.FindNearestVesselWhere(currentKerbal.transform.position, TestVesselFindKerbals);
                foreach (Vessel ves in kerbalsLoaded)
                {
                    bool v = Vector3.Distance(part.transform.position, ves.transform.position) < GameSettings.EVA_CONSTRUCTION_RANGE;
                    if (v && ves != this.vessel)
                    {
                        otherKHSKKerbalModule = ves.FindPartModuleImplementing<KerbalKHSKExSk>();
                        totalPerKerbalLiftWeight += otherKHSKKerbalModule.nowPerKerbalConstructionWeightLimit;
                        kerbalCounter++;
                    }
                }

                ScreenMessages.PostScreenMessage("Assets Kerbal Num= " + kerbalCounter);

                if (kerbalCounter != 0)
                    PhysicsGlobals.ConstructionWeightLimitPerKerbalCombine = totalPerKerbalLiftWeight / kerbalCounter;
                else
                    PhysicsGlobals.ConstructionWeightLimitPerKerbalCombine = defaultPerKerbalConstructionWeightLimit;

                PhysicsGlobals.ConstructionWeightLimit = defualtConstructionWeightLimit * liftMuilty;
            }
            else
            {
                PhysicsGlobals.ConstructionWeightLimit = defualtConstructionWeightLimit;
            }
        }


        ///---------------------Life Cycle---------------------------

        public override void OnStart(StartState state)
        {
            currentKerbal = part.FindModuleImplementing<KerbalEVA>();
            kerbalInventory = part.FindModuleImplementing<ModuleInventoryPart>();

            Transform kerbalSpA01 = BoneFindByPath("globalMove01/joints01/bn_spA01", part.transform);
            kerbalSpine = BoneFindByPath("bn_spB01/bn_spc01/bn_spD01/bn_jetpack01", kerbalSpA01);
            kerbalLeftArm = BoneFindByPath("bn_spB01/bn_spc01/bn_spD01/bn_l_shld01/be_l_shldEnd01/bn_l_arm01 1/bn_l_elbow_a01/bn_l_elbow_b01", kerbalSpA01);
            kerbalRightArm = BoneFindByPath("bn_spB01/bn_spc01/bn_spD01/bn_r_shld01/be_r_shldEnd01/bn_r_arm01 1/bn_r_elbow_a01/bn_r_elbow_b01", kerbalSpA01);

            Transform kerbalBackpackMain = BoneFindByPath("model/EVAStorage/Backpack", part.transform);
            kerbalBackpackRenderer = BoneFindByPath("Storage", kerbalBackpackMain).GetComponent<Renderer>();
            kerbalBackpackRendererFlag = BoneFindByPath("EVAStorage_flagDecals", kerbalBackpackMain).GetComponent<Renderer>();

            kerbalBackpackSlimRenderer = BoneFindByPath("model/EVABackpack_standalone/kerbalCargoContainerPack/base", part.transform).GetComponent<Renderer>();
        }

        public void Update()
        {
            hasExoSkeleton = kerbalInventory.ContainsPart(exoPartName);

            if(hasExoSkeleton)
            {
                ///-------------Disable Backpack Renderer---------------
                ToggleRenderer(false, kerbalBackpackSlimRenderer, kerbalBackpackRenderer, kerbalBackpackRendererFlag);

                if (exoSkeleton == null)
                {
                    ///Equip
                    CreateModel();
                    ExoSkUpdate();
                }
                else
                {
                    if (this.vessel == FlightGlobals.ActiveVessel)
                    {
                        if (currentKerbal.InConstructionMode) ChangeWeightLimit(true);
                        else ChangeWeightLimit(false);
                    }
                }
            }
            else
            {
                if(exoSkeleton != null)
                {
                    ///UnEquip
                    Destroy(exoSkeleton.gameObject);
                    exoSkeleton = null;
                    ToggleRenderer(true, kerbalBackpackSlimRenderer, kerbalBackpackRenderer, kerbalBackpackRendererFlag);
                    ChangeWeightLimit(false);
                }
            }
        }

        public void LateUpdate()
        {
            if(hasExoSkeleton)
            {
                ExoSkUpdate();
            }
        }


    }
}
