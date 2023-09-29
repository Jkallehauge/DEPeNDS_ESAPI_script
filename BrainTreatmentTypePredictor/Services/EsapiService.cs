using EsapiEssentials.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace BrainTreatmentTypePredictor.Services
{
    public class EsapiService : EsapiServiceBase<PluginScriptContext>, IEsapiService
    {
        public EsapiService(PluginScriptContext context) : base(context)
        {

        }

        public Task<List<string>> GetPlans()
        {
            return RunAsync(context =>
            {
                List<string> plans = new List<string>();
                foreach (var plan in context.IonPlansInScope)
                {
                    plans.Add(plan.Id);
                }
                foreach (var plan in context.ExternalPlansInScope)
                {
                    plans.Add(plan.Id);
                }
                return plans;
            });

        }


        private PlanSetup GetPlan(string PlanId, IEnumerable<IonPlanSetup> IonPlansInScope, IEnumerable<ExternalPlanSetup> ExternalPlansInScope)
        {
            foreach (var item in IonPlansInScope)
            {
                if (item.Id == PlanId)
                {
                    return item;
                }
            }
            foreach (var item in ExternalPlansInScope)
            {
                if (item.Id == PlanId)
                {
                    return item;
                }
            }
            return null;

        }

        public Task<List<string>> GetStructures(string PlanId)
        {
            return RunAsync(context =>
            {
                List<string> Structures = new List<string>();
                PlanSetup plan = GetPlan(PlanId, context.IonPlansInScope, context.ExternalPlansInScope);
                if (plan != null)
                {
                    foreach (var structure in plan.StructureSet.Structures)
                    {
                        Structures.Add(structure.Id);
                    }
                }
                return Structures;
            });

        }

        public Task<string> GetPatientId()
        {
            return RunAsync(context =>
            {
                return context.Patient.Id;
            });
        }

        private Structure GetStructureById(IEnumerable<IonPlanSetup> ionPlansInScope, IEnumerable<ExternalPlanSetup> externalPlansInScope, string planId, string structureId)
        {
            return GetPlan(planId, ionPlansInScope, externalPlansInScope)?.StructureSet?.Structures?.FirstOrDefault(s => s.Id == structureId);
        }

        private DVHData GetDVHData(IEnumerable<IonPlanSetup> ionPlansInScope, IEnumerable<ExternalPlanSetup> externalPlansInScope, string planId, Structure structure, DoseValuePresentation doseValuePresentation, VolumePresentation volumePresentation)
        {
            return GetPlan(planId, ionPlansInScope, externalPlansInScope)?.GetDVHCumulativeData(structure, doseValuePresentation, volumePresentation, 0.001);
        }

        public Task<double> GetMeanDoseForStructure(string structureId, string planId)
        {
            return RunAsync(context =>
            {
                double result = Double.NaN;

                Structure structure = GetStructureById(context.IonPlansInScope, context.ExternalPlansInScope, planId, structureId);

                if (structure != null)
                {
                    DVHData data = GetDVHData(context.IonPlansInScope, context.ExternalPlansInScope, planId, structure, DoseValuePresentation.Absolute, VolumePresentation.Relative);
                    if (data != null)
                    {
                        result = data.MeanDose.Dose;
                    }
                }
                return result;
            });
        }
    }
}
