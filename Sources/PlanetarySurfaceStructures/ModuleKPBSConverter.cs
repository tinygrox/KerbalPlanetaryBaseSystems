using UnityEngine;
using KSP.Localization;

namespace PlanetarySurfaceStructures
{
    class ModuleKPBSConverter : ModuleResourceConverter, IModuleInfo
    {



        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Conversion Speed", guiUnits = "%"), UI_FloatRange(minValue = 10f, maxValue = 100f, stepIncrement = 10f)]
        public float productionSpeed = 100;

        public Callback<Rect> GetDrawModulePanelCallback()
        {
            return null;
        }

        public string GetModuleTitle()
        {
            return Localizer.GetStringByTag("#LOC_KPBS.resourceconverter.name");//"Resource Converter";
        }

        public string GetPrimaryField()
        {
            return null;
        }



        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "#LOC_KPBS.resourceconverter.dumpoxygen")]
        [UI_Toggle(scene = UI_Scene.All)]
        private bool dumpOxygen = false;

        [KSPField]
        public bool allowDumpExcessOxygen = false;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            Fields["productionSpeed"].guiName = Localizer.GetStringByTag("#LOC_KPBS.resourceconverter.speed");

            if (!allowDumpExcessOxygen)
            {
                Fields["dumpOxygen"].guiActive = false;
                Fields["dumpOxygen"].guiActiveEditor = false;
            }

            part.highlightColor.r = 0.33f;
            part.highlightColor.g = 0.635f;
            part.highlightColor.b = 0.87f;
            part.Highlight(true);
        }
        

        // Prepare the recipe with regard to the amount of crew in this module
        protected override ConversionRecipe PrepareRecipe(double deltatime)
        {
            ConversionRecipe recipe = base.PrepareRecipe(deltatime);

            if (recipe != null)
            {
                //change the rate of the inputs
                for (int i = 0; i < recipe.Inputs.Count; i++)
                {
                    ResourceRatio res = recipe.Inputs[i];
                    res.Ratio = inputList[i].Ratio * (productionSpeed / 100f);
                    recipe.Inputs[i] = res;
                }
                //change the rate of the outputs
                for (int i = 0; i < recipe.Outputs.Count; i++)
                {
                    ResourceRatio res = recipe.Outputs[i];
                    res.Ratio = outputList[i].Ratio * (productionSpeed / 100f);

                    //Dump oxygen when set to true
                    if (res.ResourceName == "Oxygen") 
                    {
                        if (dumpOxygen) {
                            res.DumpExcess = true;
                        }
                        else
                        {
                            res.DumpExcess = false;
                        }                        
                    }
          
                    recipe.Outputs[i] = res;
                }
                //change the value of the requirements
                for (int i = 0; i < recipe.Requirements.Count; i++)
                {
                    ResourceRatio res = recipe.Requirements[i];
                    res.Ratio = reqList[i].Ratio * (productionSpeed / 100f);
                    recipe.Requirements[i] = res;
                }
            }

            return recipe;
        }
    }
}
