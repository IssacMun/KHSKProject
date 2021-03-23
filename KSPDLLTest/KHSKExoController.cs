using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KHSKController
{
    public class KHSKExoController
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

        //-----------------ExoSkeleton Variables-----------------------------
        private Transform exoSkeleton;
        private Transform exSkMainBody;
        private Transform exSkLeftArm;
        private Transform exSkRightArm;
        private Transform exSkLeftArm1;
        private Transform exSkLeftArm2;
        private Transform exSkRightArm1;
        private Transform exSkRightArm2;

        //--------------Propoties----------------------
        private const float defualtConstructionWeightLimit = 588.399f;
        private const float defaultPerKerbalConstructionWeightLimit = 588.399f;
        public float nowPerKerbalConstructionWeightLimit;

        Func<Vessel, bool> TestVesselFindKerbals = delegate (Vessel ves) { return ves.isEVA; };
        List<Vessel> kerbalsLoaded = new List<Vessel>();
        private float totalPerKerbalLiftWeight;
        private KHSKKerbal otherKHSKKerbalModule;

        public KHSKExoController(Transform spine, Transform rightArm, Transform leftArm, KerbalEVA parentKerbal)
        {
            Part aviPartExoSk = KHSKUtility.GetAviPartFromPartLoader("exoSkeleton");

            KHSKExoSkeleton exoSkModule = aviPartExoSk.FindModuleImplementing<KHSKExoSkeleton>();

            kerbalSpine = spine;
            kerbalRightArm = rightArm;
            kerbalLeftArm = leftArm;

            mainBodyPos = exoSkModule.mainBodyOffset;
            mainBodyRot = exoSkModule.mainBodyRotate;
            rightArmPos = exoSkModule.rightArmOffset;
            rightArmRot = exoSkModule.rightArmRotate;
            leftArmPos = exoSkModule.leftArmOffset;
            leftArmRot = exoSkModule.leftArmRotate;
            liftMuilty = exoSkModule.liftMuilty;
            nowPerKerbalConstructionWeightLimit = defaultPerKerbalConstructionWeightLimit * liftMuilty;

            exoSkeleton = KHSKUtility.InitalizeGameObject("exoSkeleton", spine, mainBodyPos, mainBodyRot, parentKerbal.transform ,aviPartExoSk);

            exSkMainBody = KHSKUtility.BoneFindByPath("KSPack", exoSkeleton);
            exSkLeftArm = KHSKUtility.BoneFindByPath("KSLeftRing", exoSkeleton);
            exSkRightArm = KHSKUtility.BoneFindByPath("KSRightRing", exoSkeleton);
            exSkRightArm1 = KHSKUtility.BoneFindByPath("KSRightArm1", exSkMainBody);
            exSkRightArm2 = KHSKUtility.BoneFindByPath("KSRightArm2", exSkRightArm);
            exSkLeftArm1 = KHSKUtility.BoneFindByPath("KSLeftArm1", exSkMainBody);
            exSkLeftArm2 = KHSKUtility.BoneFindByPath("KSLeftArm2", exSkLeftArm);
        }


        private void UpdateBump(Transform bumpColumn , Transform bumpStick)
        {
            if (bumpColumn != null && bumpStick != null)
            {
                bumpColumn.LookAt(bumpStick.position);
                bumpStick.LookAt(bumpColumn.position);
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
            KHSKUtility.UpdateGameObjectTransformByAnchor(exSkMainBody, kerbalSpine, mainBodyPos, mainBodyRot);
            KHSKUtility.UpdateGameObjectTransformByAnchor(exSkRightArm, kerbalRightArm, rightArmPos, rightArmRot);
            KHSKUtility.UpdateGameObjectTransformByAnchor(exSkLeftArm, kerbalLeftArm, leftArmPos, leftArmRot);
            UpdateBump(exSkRightArm1, exSkRightArm2);
            UpdateBump(exSkLeftArm1, exSkLeftArm2);
        }

        public void DestroyPart()
        {
            UnityEngine.Object.Destroy(exoSkeleton.gameObject);
        }

        //---------------------------Feartures------------------------------
        public void ExoSkFunction(bool needLiftMuilty)
        {
            /*totalPerKerbalLiftWeight = 0;
            kerbalsLoaded = FlightGlobals.FindNearestVesselWhere(currentKerbal.transform.position, TestVesselFindKerbals);
            foreach (Vessel ker in kerbalsLoaded)
            {
                bool v = Vector3.Distance(ker.transform.position, currentKerbal.transform.position) < GameSettings.EVA_CONSTRUCTION_COMBINE_RANGE;
                otherKHSKKerbalModule = ker.FindPartModuleImplementing<KHSKKerbal>();
                if (otherKHSKKerbalModule.hasExoSkeleton)
                {
                    totalPerKerbalLiftWeight += nowPerKerbalConstructionWeightLimit;
                }
                else
                {
                    totalPerKerbalLiftWeight += defaultPerKerbalConstructionWeightLimit;
                }
            }*/

            if (needLiftMuilty)
            {
                PhysicsGlobals.ConstructionWeightLimit = defualtConstructionWeightLimit * liftMuilty;
            }
            else
            {
                PhysicsGlobals.ConstructionWeightLimit = defualtConstructionWeightLimit;
            }
        }
    }
}
