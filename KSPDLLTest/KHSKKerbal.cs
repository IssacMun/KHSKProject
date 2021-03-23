using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KHSKController
{
    public class KHSKKerbal : PartModule
    {
        [KSPField(isPersistant = true, guiActive = false)]
        public bool isExBpkExpand = false;



        public KerbalEVA currentKerbal;
        private ModuleInventoryPart kerbalInventory;

        private ConfigNode cargoPartsSaveNode;
        //----------------Kerbal Bone---------------------------------------
        private Transform kerbalSpine;
        private Transform kerbalLeftArm;
        private Transform kerbalRightArm;

        //-----------------exSkeleton----------------------------
        public bool hasExoSkeleton = false;
        KHSKExoController exoSkeletonController = null;
        //-----------------exBpk-------------------------------
        private bool hasExBpk = false;
        KHSKExBpkController exBpkController = null;
        UIPartActionInventory kerbalInventoryUI;
        KHSKKerbalExCargo extendInventory;

        private StartState nowState;


        #region KSPEVENT
        /// <summary>
        /// KSPEVENT
        /// </summary>

        //ExoSkeleton
        public void EquipExoSkeleton()
        {
            exoSkeletonController = new KHSKExoController(kerbalSpine, kerbalRightArm, kerbalLeftArm, currentKerbal);
            exoSkeletonController.ExoSkUpdate();
        }

        public void UnEquipExoSkeleton()
        {
            exoSkeletonController.DestroyPart();
            exoSkeletonController = null;
            exoSkeletonController.ExoSkFunction(false);
        }

        //ExBackpack
        [KSPEvent(name="ExpandExBpk",guiName ="Expand Inflate Backpack", guiActive = false , guiActiveUnfocused = false)]
        public void ExpandExBpk()
        {
            extendInventory = (KHSKKerbalExCargo)part.AddModule("KHSKKerbalExCargo",true);
            extendInventory.OnStart(part.GetModuleStartState());
            DropExBpk();

            exBpkController.ToggleExBpk(true);

            Events["ExpandExBpk"].guiActive = false;
            Events["PackExBpk"].guiActive = true;
            Events["DropExBpk"].guiActive = true;

            isExBpkExpand = true;

        }
        [KSPEvent(name = "PackExBpk", guiName = "Pack Inflate Backpack", guiActive = false, guiActiveUnfocused = false)]
        public void PackExBpk()
        {
            if (extendInventory.InventoryIsEmpty)
            {
                part.RemoveModule(extendInventory);

                exBpkController.ToggleExBpk(false);

                Events["ExpandExBpk"].guiActive = true;
                Events["PackExBpk"].guiActive = false;
                Events["DropExBpk"].guiActive = false;

                isExBpkExpand = false;
            }
            else
            {
                ScreenMessages.PostScreenMessage("Cargo need to be cleaned before pack");
            }
        }
        [KSPEvent(name = "DropExBpk", guiName = "Drop Inflate Backpack Item(Slot2-6)", guiActive = false, guiActiveUnfocused = false)]
        public void DropExBpk()
        {
            for(int i=0; i<extendInventory.InventorySlots;i++)
            {
                extendInventory.ClearPartAtSlot(i);
            }
        }

        #endregion



        //--------------------------ExoSkeletonUpdate-----------------------
        private void ExoSkeletonUpdate()
        {

            hasExoSkeleton = kerbalInventory.ContainsPart("exoSkeleton");

            if (hasExoSkeleton)
            {
                if (exoSkeletonController != null)
                {
                    exoSkeletonController.ExoSkUpdate();

                    if (this.vessel == FlightGlobals.ActiveVessel)
                    {
                        if (currentKerbal.InConstructionMode)
                        {
                            exoSkeletonController.ExoSkFunction(true);
                        }
                        else
                        {
                            exoSkeletonController.ExoSkFunction(false);
                        }
                    }
                }
                else
                {
                    EquipExoSkeleton();
                }
            }
            else
            {
                if (exoSkeletonController != null)
                    UnEquipExoSkeleton();
            }
        }

        //--------------------------ExBpkUpdate-----------------------
        private void ExBpkUpdate()
        {
            hasExBpk = kerbalInventory.ContainsPart("KSInfBackpack");

            if (hasExBpk)
            {
                if (exBpkController == null)
                {
                    exBpkController = new KHSKExBpkController(kerbalSpine, currentKerbal);

                    Events["ExpandExBpk"].guiActive = true;
                    exBpkController.ExBpkUpdate();
                }
                else
                {
                    exBpkController.ExBpkUpdate();
                }
            }
            else
            {
                if (Events["PackExBpk"].guiActive == false)
                { 
                    if (exBpkController != null)
                    {
                        exBpkController.DestroyPart();
                        exBpkController = null;
                        Events["ExpandExBpk"].guiActive = false;
                        Events["PackExBpk"].guiActive = false;
                        Events["DropExBpk"].guiActive = false;
                    }
                }
            }
        }


        //----------------------------------Life Cycle---------------------------------------
        public override void OnSave(ConfigNode node)
        {
            if(isExBpkExpand)
            {
                extendInventory.OnSave(node.AddNode("EXCARGO"));
            }
        }

        public override void OnLoad(ConfigNode node)
        {
            if (isExBpkExpand)
            {
                cargoPartsSaveNode = node.GetNode("EXCARGO");
                if (cargoPartsSaveNode == null)
                    throw new Exception("ExCargoParts Node Not Found!");
            }
        }


        public override void OnStart(StartState state)
        {

            //Debug.Log("[KHSK DEBUG] "+ Game.ToString());

            nowState = state;

            currentKerbal = this.vessel.GetComponent<KerbalEVA>();
            kerbalInventory = part.FindModuleImplementing<ModuleInventoryPart>();

            kerbalInventoryUI = kerbalInventory.constructorModeInventory;

            Transform BackpackSilm = KHSKUtility.BoneFindByPath("model/EVAStorageSlim/BackpackSlim/BackpackSlim", currentKerbal.transform);
            Debug.Log("[KHSK DEBUG]BackpackSilm Render = " + BackpackSilm.GetComponent<Renderer>().ToString());


            Transform kerbalSpA01 = KHSKUtility.BoneFindByPath("globalMove01/joints01/bn_spA01", currentKerbal.transform);
            kerbalSpine = KHSKUtility.BoneFindByPath("bn_spB01/bn_spc01/bn_spD01", kerbalSpA01);
            kerbalLeftArm = KHSKUtility.BoneFindByPath("bn_spB01/bn_spc01/bn_spD01/bn_l_shld01/be_l_shldEnd01/bn_l_arm01 1/bn_l_elbow_a01/bn_l_elbow_b01", kerbalSpA01);
            kerbalRightArm = KHSKUtility.BoneFindByPath("bn_spB01/bn_spc01/bn_spD01/bn_r_shld01/be_r_shldEnd01/bn_r_arm01 1/bn_r_elbow_a01/bn_r_elbow_b01", kerbalSpA01);

            if (isExBpkExpand)
            {
                extendInventory = (KHSKKerbalExCargo)part.AddModule("KHSKKerbalExCargo", true);
                extendInventory.OnStart(state);
                DropExBpk();

                exBpkController = new KHSKExBpkController(kerbalSpine, currentKerbal);

                exBpkController.ExBpkUpdate();
                exBpkController.ToggleExBpk(true);

                Events["ExpandExBpk"].guiActive = false;
                Events["PackExBpk"].guiActive = true;
                Events["DropExBpk"].guiActive = true;
            }
        }

        public override void OnStartFinished(StartState state)
        {
            if (isExBpkExpand)
            {
                //Load Parts in inventory
                ConfigNode[] cargoNodeArray = cargoPartsSaveNode.GetNode("STOREDPARTS").GetNodes("STOREDPART");
                int num = cargoNodeArray.Length;

                StoredPart[] tempStoredPart = new StoredPart[num];
                for (int i = 0; i < num; i++)
                {
                    tempStoredPart[i] = new StoredPart("Name", i);
                    tempStoredPart[i].Load(cargoNodeArray[i]);

                    extendInventory.StoreCargoPartAtSlot(tempStoredPart[i].snapshot, tempStoredPart[i].slotIndex);
                    if (tempStoredPart[i].CanStack)
                    {
                        extendInventory.UpdateStackAmountAtSlot(tempStoredPart[i].slotIndex, tempStoredPart[i].quantity, tempStoredPart[i].variantName);
                    }
                }
            }
        }

        public void Update()
        {
            ExoSkeletonUpdate();
            ExBpkUpdate();
        }
    }
}
