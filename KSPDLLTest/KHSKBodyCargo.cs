using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KHSKController
{
    class KHSKBodyCargo : ModuleInventoryPart
    {
        public override void OnAwake()
        {
            InventorySlots = 6;
            packedVolumeLimit = 120;
            massLimit = 0.2f;
        }

    }
}
