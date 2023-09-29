using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainTreatmentTypePredictor.Services
{
    public class PosibilityPredicter : IPosibilityPredicter
    {
        private const double b0 = 0.9219;
        private const double b1 = -0.0422;
        private const double b2 = 0.1591;

        private const double a1 = 0.254829592;
        private const double a2 = -0.284496736;
        private const double a3 = 1.421413741;
        private const double a4 = -1.453152027;
        private const double a5 = 1.061405429;
        private const double p1 = 0.3275911;

        public double Predict(double meanDoseModalitet1, double meanDoseModalitet2, int patientAge)
        {
            double x = b0 + patientAge * b1 + (meanDoseModalitet1 - meanDoseModalitet2) * b2;
            x = x / Math.Sqrt(2.0);
            double t = 1.0 / (1.0 + p1 * x);
            double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);
            double Probability = 0.5 * (1.0 + y);
            Probability = Probability * 100.0;

            return Probability;
        }
    }
}
