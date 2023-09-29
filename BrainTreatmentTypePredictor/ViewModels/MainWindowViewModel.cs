using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using BrainTreatmentTypePredictor.Services;
using Prism.Commands;
using EsapiDVHhelper;

namespace BrainTreatmentTypePredictor.ViewModels
{
    //https://www.c-sharpcorner.com/article/getting-started-mvvm-pattern-using-prism-library-in-wpf/
    public class MainWindowViewModel : BindableBase
    {
        private IEsapiService _esapiService;
        private ICPRService _cPRService;
        private IPosibilityPredicter _posibilityPredictor;

        #region Foton
        private string _selectedFotonPlan = String.Empty;
        public string SelectedFotonPlan
        {
            get => _selectedFotonPlan;
            set => SetProperty(ref _selectedFotonPlan, value);
        }

        private string _selectedBrainCTVstructureFoton;
        public string SelectedBrainCTVstructureFoton
        {
            get => _selectedBrainCTVstructureFoton;
            set => SetProperty(ref _selectedBrainCTVstructureFoton, value);
        }

        private List<string> _structuresFoton;
        public List<string> StructuresFoton
        {
            get => _structuresFoton;
            set => SetProperty(ref _structuresFoton, value);
        }

        private double _meanDoseFoton = Double.NaN;
        public double MeanDoseFoton
        {
            get => _meanDoseFoton;
            set => SetProperty(ref _meanDoseFoton, value);
        }
        #endregion

        #region Proton
        private string _selectedProtonPlan = String.Empty;
        public string SelectedProtonPlan
        {
            get => _selectedProtonPlan;
            set => SetProperty(ref _selectedProtonPlan, value);
        }

        private string _selectedBrainCTVstructureProton;
        public string SelectedBrainCTVstructureProton
        {
            get => _selectedBrainCTVstructureProton;
            set => SetProperty(ref _selectedBrainCTVstructureProton, value);
        }

        private List<string> _structuresProton;
        public List<string> StructuresProton
        {
            get => _structuresProton;
            set => SetProperty(ref _structuresProton, value);
        }

        private double _meanDoseProton = Double.NaN;
        public double MeanDoseProton
        {
            get => _meanDoseProton;
            set => SetProperty(ref _meanDoseProton, value);
        }
        #endregion

        #region Common
        private List<string> _plans;
        public List<string> Plans
        {
            get => _plans;
            set => SetProperty(ref _plans, value);
        }

        private int _age;
        public int Age
        {
            get => _age;
            set => SetProperty(ref _age, value);
        }

        private double _predictedPosibility = Double.NaN;
        public double PredictedPosibility
        {
            get => _predictedPosibility;
            set => SetProperty(ref _predictedPosibility, value);
        }
        #endregion

        #region Commands
        public DelegateCommand StartCommand { get; private set; }
        public DelegateCommand LoadStructuresFotonCommand { get; private set; }
        public DelegateCommand LoadStructuresProtonCommand { get; private set; }
        public DelegateCommand CalculateCommand { get; private set; }
        #endregion
        public MainWindowViewModel(IEsapiService esapiService, ICPRService cPRService, IPosibilityPredicter posibilityPredicter)
        {
            _esapiService = esapiService;
            _cPRService = cPRService;
            _posibilityPredictor = posibilityPredicter;
            SetUpCommands();
        }

        private void SetUpCommands()
        {
            StartCommand = new DelegateCommand(Start);
            LoadStructuresFotonCommand = new DelegateCommand(LoadStructuresFotonAsync);
            LoadStructuresProtonCommand = new DelegateCommand(LoadStructuresProton);
            CalculateCommand = new DelegateCommand(Calculate);
        }

        private async void Calculate()
        {
            if(CanCalculate())
            {
                MeanDoseFoton = await _esapiService.GetMeanDoseForStructure(SelectedBrainCTVstructureFoton, SelectedFotonPlan);
                MeanDoseProton = await _esapiService.GetMeanDoseForStructure(SelectedBrainCTVstructureProton, SelectedProtonPlan);
                PredictedPosibility = _posibilityPredictor.Predict(MeanDoseFoton, MeanDoseProton, Age);
            }
        }

        public bool CanCalculate()
        {
            if (SelectedBrainCTVstructureFoton == "N/A" )
            {
                System.Windows.MessageBox.Show("En eller flere strukturer er ikke valgt for Foton.");
                return false;
            }
            else if (SelectedBrainCTVstructureProton == "N/A" )
            {
                System.Windows.MessageBox.Show("En eller flere strukturer er ikke valgt for Proton.");
                return false;
            }
            else if (Age <= 0)
            {
                System.Windows.MessageBox.Show("Patientens alder kan ikke være negativ eller nul.");
                return false;
            }
            else
            {
                return true;
            }
        }

        private async void LoadStructuresProton()
        {
            PredictedPosibility = Double.NaN;
            MeanDoseProton = Double.NaN;
            StructuresProton = await _esapiService.GetStructures(SelectedProtonPlan);
            SelectedBrainCTVstructureProton = SetChosenStructure(new List<string> { "Brain-CTV-BS" }, StructuresProton);
        }

        private async void LoadStructuresFotonAsync()
        {
            PredictedPosibility = Double.NaN;
            MeanDoseFoton = Double.NaN;
            StructuresFoton = await _esapiService.GetStructures(SelectedFotonPlan);
            SelectedBrainCTVstructureFoton = SetChosenStructure(new List<string> { "Brain-CTV-BS" }, StructuresFoton);
        }

        private async void Start()
        {
            Plans = await _esapiService.GetPlans();
            SetAge();
        }

        private async void SetAge()
        {
            string patientID = await _esapiService.GetPatientId();
            try
            {
                if (_cPRService.IsCPRNumber(patientID))
                {
                    Age = _cPRService.GetAge(patientID);
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            
            
        }

        /// <summary>
        /// Returns the name of the structure if the naming convention has been followed or else returns N/A and then the user need to choose the correct structure.
        /// </summary>
        /// <param name="searchString">Name of structure as by the naming convention</param>
        /// <param name="structurList">List of all the structures</param>
        /// <returns>Name of structure or N/A</returns>
        private string SetChosenStructure(List<string> searchStrings, List<string> structurList)
        {
            string result = "N/A";
            string search = structurList.Where(s => searchStrings.Any(st => s.ToUpper().Contains(st.ToUpper()))).FirstOrDefault();
            if (search != null)
            {
                result = search;
            }
            return result;
        }


    }
}
