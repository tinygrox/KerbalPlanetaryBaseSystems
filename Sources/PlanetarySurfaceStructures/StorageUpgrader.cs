using System;
using SaveUpgradePipeline;
using UnityEngine;

namespace PlanetarySurfaceStructures
{
    [UpgradeModule(LoadContext.SFS | LoadContext.Craft, sfsNodeUrl = "GAME/FLIGHTSTATE/VESSEL/PART", craftNodeUrl = "PART")]
    public class StorageUpgrader : UpgradeScript
    {
        //visible name of the upgrader
        public override string Name
        {
            get
            {
                return "Upgrader for KPBS 2.0.0";
            }
        }

        //description for the upgrade process 
        public override string Description
        {
            get
            {
                return "Replaces all the storage parts by their switchable counterpart";
            }
        }

        //check the maximal version
        protected override bool CheckMaxVersion(Version v)
        {
            return true;// v <= TargetVersion;
        }

        //get the earliest version that can be upgraded
        public override Version EarliestCompatibleVersion
        {
            get
            {
                return new Version(1, 0, 4);
            }
        }

        //get the target version of the save file
        public override Version TargetVersion
        {
            get
            {
                return new Version(1, 3, 1);
            }
        }

        //test the save file for upgrades
        public override TestResult OnTest(ConfigNode node, LoadContext loadContext, ref string nodeName)
        {
            nodeName = NodeUtil.GetPartNodeName(node, loadContext);

            return checkPart(node, loadContext);
        }

        //upgrade the save file
        public override void OnUpgrade(ConfigNode node, LoadContext loadContext)
        {
            upgradePart(node, loadContext);
        }

        //Check of the parts has to be upgraded
        private TestResult checkPart(ConfigNode part, LoadContext loadContext)
        {
            if (loadContext == LoadContext.Craft)
            {
                //check the parts
                string[] partName = part.GetValue("part").Split('_');

                switch (partName[0])
                {
                    case "KKAOSS.Xenon.Tank":
                        return TestResult.Upgradeable;
                }

                //check the links
                string[] links = part.GetValues("link");
                for (int i = 0; i < links.Length; i++)
                {
                    string linkedPart = links[i].Split('_')[0];
                    switch (linkedPart)
                    {
                        case "KKAOSS.Xenon.Tank":
                            return TestResult.Upgradeable;
                    }
                }

                //check the attach nodes
                string[] attNs = part.GetValues("attN");
                for (int i = 0; i < attNs.Length; i++)
                {
                    string attachedPart = attNs[i].Split(',')[1].Split('_')[0];
                    switch (attachedPart)
                    {
                        case "KKAOSS.Xenon.Tank":
                            return TestResult.Upgradeable;
                    }
                }
            }
            else if (loadContext == LoadContext.SFS)
            {
                string partName = part.GetValue("name");

                switch (partName)
                {
                    case "KKAOSS.Xenon.Tank":
                        return TestResult.Upgradeable;
                }

            }

            return TestResult.Pass;
        }


        //Upgrade the part
        private void upgradePart(ConfigNode part, LoadContext loadContext)
        {
            //update the files of the craft
            if (loadContext == LoadContext.Craft)
            {
                //replace the part name
                string[] partName = part.GetValue("part").Split('_');
                switch (partName[0])
                {
                    case "KKAOSS.Xenon.Tank":
                        part.SetValue("part", "KKAOSS.RCS.Tank_" + partName[1]);
                        break;
                }

                //replace the links
                string[] links = part.GetValues("link");
                part.RemoveValues("link");
                for (int i = 0; i < links.Length; i++)
                {
                    string[] linkedPart = links[i].Split('_');
                    string newLinkedPart;
                    switch (linkedPart[0])
                    {
                        case "KKAOSS.Xenon.Tank":
                            newLinkedPart = "KKAOSS.RCS.Tank";
                            break;
                        default:
                            newLinkedPart = linkedPart[0];
                            break;
                    }
                    part.AddValue("link", newLinkedPart + "_" + linkedPart[1]);
                }

                //replace the attach nodes
                string[] attNs = part.GetValues("attN");
                part.RemoveValues("attN");
                for (int i = 0; i < attNs.Length; i++)
                {
                    string[] attN = attNs[i].Split(',');
                    string[] attachedPart = attN[1].Split('_');
                    string newAttachedPart;
                    switch (attachedPart[0])
                    {
                        case "KKAOSS.Xenon.Tank":
                            newAttachedPart = "KKAOSS.RCS.Tank";
                            break;
                        default:
                            newAttachedPart = attachedPart[0];
                            break;
                    }
                    part.AddValue("attN", attN[0]+","+ newAttachedPart + "_" + attachedPart[1]);
                }
            }
            else if (loadContext == LoadContext.SFS)
            {
                string partName = part.GetValue("name");
                switch (partName)
                {
                    case "KKAOSS.Xenon.Tank":
                        part.SetValue("name", "KKAOSS.RCS.Tank");
                        break;
                }
            }

        }
    }
}