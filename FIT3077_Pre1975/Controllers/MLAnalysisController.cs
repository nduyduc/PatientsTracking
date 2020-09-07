using System.Linq;
using System.Threading.Tasks;
using FIT3077_Pre1975.Helpers;
using FIT3077_Pre1975.Models;
using FIT3077_Pre1975.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace FIT3077_Pre1975.Controllers
{
    /// <summary>
    /// Controller class for MLAnalysis Views
    /// </summary>
    public class MLAnalysisController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Result()
        {
            return View();
        }

        /// <summary>
        /// Handle BuildModel event from View
        /// </summary>
        /// <returns> Result View showing F1Score and Accuracy </returns>
        public async Task<ActionResult> TrainModel()
        {
            // Read data from CSV file
            IDataView data = MLHelpers.ReadFromCsv();

            data = AppContext.MlContext.Data.Cache(data);

            // Pre-processing data before training
            IDataView cleanData = MLHelpers.PrepareData(data);

            // use FastTree Model
            var modelEstimator = AppContext.MlContext.BinaryClassification.Trainers.FastTree();

            // cross-validation
            var cvResults = AppContext.MlContext.BinaryClassification.CrossValidateNonCalibrated(cleanData, modelEstimator, numberOfFolds: 4);

            // get the best model from 4 fold models
            double[] f1Score = cvResults
                .OrderByDescending(fold => fold.Metrics.F1Score)
                .Select(fold => fold.Metrics.F1Score)
                .ToArray();

            double[] accuracy = cvResults
                .OrderByDescending(fold => fold.Metrics.Accuracy)
                .Select(fold => fold.Metrics.Accuracy)
                .ToArray();

            ITransformer[] models = cvResults
                .OrderByDescending(fold => fold.Metrics.Accuracy)
                .Select(fold => fold.Model)
                .ToArray();

            BinaryClassificationMetrics[] metrics = cvResults
                .OrderByDescending(fold => fold.Metrics.Accuracy)
                .Select(fold => fold.Metrics)
                .ToArray();

            // Get Top Model
            ITransformer topModel = models[0];

            MLResultViewModel resultView = new MLResultViewModel
            {
                accuracy = f1Score[0],
                f1score = accuracy[0]
            };

            return View("Result", resultView);
        }

        /// <summary>
        /// Handle Get More Data event in View
        /// </summary>
        /// <returns> Index View </returns>
        public async Task<ActionResult> GetData()
        {
            // execute async task with callback to avoid blocking the web app
            _ = FhirService.GetData().ContinueWith((data) =>
              {
                  // Write fetched data to csv file
                  MLHelpers.WriteToCsv(data.Result);

                  // Save new fetched data to AppContext
                  foreach (Patient p in data.Result)
                  {
                      AppContext.AnalysisData.AddPatient(p);
                  }
              });

            return View("Index");
        }
    }
}