namespace BrainTreatmentTypePredictor.Services
{
    public interface IPosibilityPredicter
    {
        double Predict(double meanDoseModalitet1, double meanDoseModalitet2, int patientAge);
    }
}