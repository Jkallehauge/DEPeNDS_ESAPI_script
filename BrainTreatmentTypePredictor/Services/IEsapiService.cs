using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainTreatmentTypePredictor.Services
{
    public interface IEsapiService
    {
        Task<List<string>> GetPlans();

        Task<List<string>> GetStructures(string PlanId);

        Task<string> GetPatientId();

        Task<double> GetMeanDoseForStructure(string structureId, string PlanId);
    }
}
