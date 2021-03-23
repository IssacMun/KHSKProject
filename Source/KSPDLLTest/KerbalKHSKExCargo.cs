using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KHSKController
{
    class KerbalKHSKExCargo : KerbalKHSK
    {
        [KSPField(isPersistant = true, guiActive = false)]
        public bool isExBpkExpand = false;

        ///------------Kerbal Bone---------------
        private Transform kerbalSpine;

        ///-------------Backpack Object-----------------
        private Transform exBpk = null;
        private Transform exBpkAnchorMain;
        private Transform exBpkPack;
        private Transform exBpkUnpack;
        private Renderer exBpkPackRenderer;
        private Renderer exBpkUnpackRenderer;
        private TrackRigObject exBpkTrackRig;

        private Vector3[] mainBodyPos = new Vector3[4];
        private Vector3[] mainBodyRot = new Vector3[4];

        ///--------------Propotise---------------
        private bool hasExBpk;
        KHSKBodyCargo extendInventory;
        private int packState;
        private ConfigNode cargoPartsSaveNode;
        private string infBpkPartName = "KSInfBackpack";


        #region KSPEVENT
        /// <summary>
        /// KSPEVENT
        /// </summary>
        //ExBackpack
        [KSPEvent(name = "ExpandExBpk", guiName = "Expand inflatable backpack", guiActive = false, guiActiveUnfocused = false)]
        public void ExpandExBpk()
        {
            extendInventory = (KHSKBodyCargo)part.AddModule("KHSKBodyCargo", true);
            extendInventory.OnStart(part.GetModuleStartState());
            /*extendInventory.InventorySlots = 6;
            extendInventory.massLimit = 0.2f;
            extendInventory.packedVolumeLimit = 120;
            part.PartActionWindow.UpdateWindow();*/
            DropExBpk();

            ToggleExBpk(true);

            Events["ExpandExBpk"].guiActive = false;
            Events["PackExBpk"].guiActive = true;
            Events["DropExBpk"].guiActive = true;

            isExBpkExpand = true;

        }
        [KSPEvent(name = "PackExBpk", guiName = "Pack inflatable backpack", guiActive = false, guiActiveUnfocused = false)]
        public void PackExBpk()
        {
            if (extendInventory.InventoryIsEmpty)
            {
                part.RemoveModule(extendInventory);

                ToggleExBpk(false);

                Events["ExpandExBpk"].guiActive = true;
                Events["PackExBpk"].guiActive = false;
                Events["DropExBpk"].guiActive = false;

                isExBpkExpand = false;
            }
            else
            {
                ScreenMessages.PostScreenMessage("Inflatable backpack is not empty");
            }
        }
        [KSPEvent(name = "DropExBpk", guiName = "Drop inflatable backpack items", guiActive = false, guiActiveUnfocused = false)]
        public void DropExBpk()
        {
            for (int i = 0; i < extendInventory.InventorySlots; i++)
            {
                extendInventory.ClearPartAtSlot(i);
            }
        }

        #endregion

        ///-----------------Function--------------------///
        private void CreateModel()
        {
            Part aviPartPrefab = GetAviPartFromPartLoader(infBpkPartName);
            if (aviPartPrefab == null)
                throw new Exception("find ExCargo aviPart fail");

            KHSKExBackpack exBpkModule = aviPartPrefab.FindModuleImplementing<KHSKExBackpack>();

            mainBodyPos[0] = exBpkModule.mainBodyOffset0;
            mainBodyRot[0] = exBpkModule.mainBodyRotate0;
            mainBodyPos[1] = exBpkModule.mainBodyOffset1;
            mainBodyRot[1] = exBpkModule.mainBodyRotate1;
            mainBodyPos[2] = exBpkModule.mainBodyOffset2;
            mainBodyRot[2] = exBpkModule.mainBodyRotate2;
            mainBodyPos[3] = exBpkModule.mainBodyOffset3;
            mainBodyRot[3] = exBpkModule.mainBodyRotate3;


            exBpk = InitalizeGameObject(infBpkPartName, kerbalSpine, mainBodyPos[packState], mainBodyRot[packState], currentKerbal.transform, aviPartPrefab);
            if (exBpk == null)
                throw new Exception("ExCargo InitalGameObject fail");

            exBpkAnchorMain = BoneFindByPath("AnchorMain", exBpk);
            exBpkPack = BoneFindByPath("PackBox", exBpkAnchorMain);
            exBpkUnpack = BoneFindByPath("UnpackBox", exBpkAnchorMain);
            exBpkUnpack.gameObject.SetActive(true);

            exBpkTrackRig = SetTrackRigObject(exBpkAnchorMain, kerbalSpine);

            exBpkUnpackRenderer = exBpkUnpack.GetComponent<MeshRenderer>();
            exBpkPackRenderer = exBpkPack.GetComponent<MeshRenderer>();

            Collider exBpkPackCol = BoneFindByPath("PackCollider", exBpkAnchorMain).GetComponent<Collider>();
            exBpkPackCol.enabled = false;

            exBpkUnpackRenderer.enabled = false;

        }

        public void ExBpkUpdate()
        {
            ///Backpack position
            if (hasExBpk)
            {
                if (kerbalInventory.InventoryIsFull)
                {
                    if (currentKerbal.HasParachute)
                        packState = 1;
                    else
                        packState = 2;
                }
                else
                {
                    packState = 0;
                }
            }
            else
            {
                if (kerbalInventory.InventoryIsFull)
                    packState = 2;
                else if (kerbalInventory.InventoryIsEmpty)
                    packState = 0;
                else
                    packState = 3;
            }

            exBpkPack.localPosition = mainBodyPos[packState];
            exBpkPack.localEulerAngles = mainBodyRot[packState];
            if (packState == 1) packState = 2;
            exBpkUnpack.localPosition = mainBodyPos[packState];
            exBpkUnpack.localEulerAngles = mainBodyRot[packState];
        }



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

        ///-----------------Life Cycle-----------------------///
        public override void OnSave(ConfigNode node)
        {
            if (isExBpkExpand)
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
            currentKerbal = part.FindModuleImplementing<KerbalEVA>();
            kerbalInventory = part.FindModuleImplementing<ModuleInventoryPart>();

            kerbalSpine = BoneFindByPath("globalMove01/joints01/bn_spA01/bn_spB01/bn_spc01/bn_spD01/bn_jetpack01", part.transform);

            kerbalBackpackSlimRenderer = BoneFindByPath("model/EVABackpack/kerbalCargoContainerPack/base", part.transform).GetComponent<Renderer>();
            kerbalBackpackSlimSTRenderer = BoneFindByPath("model/EVABackpack_standalone/kerbalCargoContainerPack/base", part.transform).GetComponent<Renderer>();

            packState = 0;

            ///Recreate Saved Cargo
            if (isExBpkExpand)
            {
                CreateModel();
                ExBpkUpdate();

                ExpandExBpk();
            }

        }

        public override void OnStartFinished(StartState state)
        {
            if (isExBpkExpand)
            {
                ///Load Parts in inventory
                StoredPart tempStoredPart = new StoredPart("Name",0);
                ConfigNode[] cargoNodeArray = cargoPartsSaveNode.GetNode("STOREDPARTS").GetNodes("STOREDPART");
                foreach(ConfigNode cn in cargoNodeArray)
                {
                    tempStoredPart.Load(cn);
                    extendInventory.StoreCargoPartAtSlot(tempStoredPart.snapshot, tempStoredPart.slotIndex);
                    if(tempStoredPart.CanStack)
                    {
                        extendInventory.UpdateStackAmountAtSlot(tempStoredPart.slotIndex, tempStoredPart.quantity, tempStoredPart.variantName);
                    }
                }
            }
        }

        public void Update()
        {
            hasExBpk = kerbalInventory.ContainsPart(infBpkPartName);

            if(hasExBpk)
            {
                ///Backpack Display Logic
                ToggleRenderer(false, kerbalBackpackSlimSTRenderer, kerbalBackpackSlimRenderer);

                if (exBpk == null)
                {
                    CreateModel();
                    ExBpkUpdate();

                    Events["ExpandExBpk"].guiActive = true;                   
                }
                else
                {
                    ExBpkUpdate();
                }
            }
            else
            {
                if (isExBpkExpand==false)
                {
                    if(exBpk != null)
                    {
                        Destroy(exBpk.gameObject);
                        exBpk = null;
                        isExBpkExpand = false;

                        ToggleRenderer(true, kerbalBackpackSlimSTRenderer, kerbalBackpackSlimRenderer);

                        Events["ExpandExBpk"].guiActive = false;
                        Events["PackExBpk"].guiActive = false;
                        Events["DropExBpk"].guiActive = false;
                    }
                }
                else
                {
                    ExBpkUpdate();
                }
            }

        }

    }
}
