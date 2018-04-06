using UnityEngine;

namespace PlanetarySurfaceStructures
{
    class ModuleContractTest : PartModule
    {

        [KSPField]
        public string testString = "NO";

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            Debug.Log("[KPSB] Contract Added: " + testString);
        }
    }
}
