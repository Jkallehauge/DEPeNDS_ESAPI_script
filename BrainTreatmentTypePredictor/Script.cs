using BrainTreatmentTypePredictor.ViewModels;
using BrainTreatmentTypePredictor.Views;
using BrainTreatmentTypePredictor.Services;
using EsapiDVHhelper;
using EsapiEssentials.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace VMS.TPS
{
    public class Script /*: ScriptBase*/
    {
        public Script()
        {

        }

        public void Execute(ScriptContext context)
        {
            var esapiService = new EsapiService(context); //very important to initialize on the Esapi thread

            using (var ui = new UiRunner()) //kills the service when it is done using it
            {
                //Starts a new thread 
                ui.Run(() =>
                {
                    //Runs the UI on the new thread
                    MainWindow window = new MainWindow();

                    ICPRService cPRService = new CPRService();

                    IPosibilityPredicter posibilityPredicter = new PosibilityPredicter();

                    var viewModel = new MainWindowViewModel(esapiService, cPRService, posibilityPredicter);

                    window.DataContext = viewModel;
                    window.ShowDialog();
                });
            }
        }

        //public override void Run(PluginScriptContext context)
        //{
        //    var esapiService = new EsapiService(context); //very important to initialize on the Esapi thread

        //    using (var ui = new UiRunner()) //kills the service when it is done using it
        //    {
        //        //Starts a new thread 
        //        ui.Run(() =>
        //        {
        //            //Runs the UI on the new thread
        //            MainWindow window = new MainWindow();

        //            ICPRService cPRService = new CPRService();

        //            IPosibilityPredicter posibilityPredicter = new PosibilityPredicter();

        //            var viewModel = new MainWindowViewModel(esapiService, cPRService, posibilityPredicter);

        //            window.DataContext = viewModel;
        //            window.ShowDialog();
        //        });
        //    }
        //}
    }
}
