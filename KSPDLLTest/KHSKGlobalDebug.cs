using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KHSKController
{
    [KSPAddon(KSPAddon.Startup.Flight , false)]
    public class KHSKGlobalDebug : MonoBehaviour
    {
        public void Update()
        {
            ScreenMessages.PostScreenMessage("[GlobalDEBUG]LiftStrangth=" + PhysicsGlobals.ConstructionWeightLimit +
                                             "\n[GlobalDEBUG]PerLiftStangth="+PhysicsGlobals.ConstructionWeightLimitPerKerbalCombine,
                                             .1f,ScreenMessageStyle.UPPER_LEFT);
        }
    }
}
