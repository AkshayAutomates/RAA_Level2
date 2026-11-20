#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;


#endregion

namespace RAA_Level2
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

          
            // open form
            MyForm currentForm = new MyForm()
            {
                Width = 650,
                Height = 500,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Topmost = true,
            };

            currentForm.ShowDialog();

            //t form data and do something
            if (currentForm.DialogResult == false)
            {
                return Result.Cancelled;
            }

            // Get form data and do something
            string filePath = currentForm.GetFilePath();

            if (filePath == null)
            {
                return Result.Cancelled;
            }

            // Read CSV File
            string[] levelArray = System.IO.File.ReadAllLines(filePath);
            List<string[]> levelData = new List<string[]>();
           

            foreach(string level in levelArray)
            {
                string[] cellData = level.Split(',');
                levelData.Add(cellData);
            }
            // remove the header row
            levelData.RemoveAt(0);

            Transaction t = new Transaction(doc);
            t.Start("Project Setup");
            //Create Levels and Views
            foreach (string[] currentLevelData in levelData)
            {
                //set parameters
                string levelName = currentLevelData[0];
                string elevImperialString = currentLevelData[1];
                string elevMetricString = currentLevelData[2];

                double levelElevation = 0;

                if(currentForm.GetUnits() == "imperial")
                {
                    double.TryParse(elevImperialString, out levelElevation);
                }
                else
                {
                    double metricConvert = 0;
                    double.TryParse(elevMetricString, out metricConvert);
                    levelElevation = metricConvert * 3.28084;
                }
                
                // Create level
                
                Level currentLevel = Level.Create(doc, levelElevation);
                currentLevel.Name = levelName;

                //Create the views
                if(currentForm.CreateFloorPlan() == true)
                {
                    ViewFamilyType planvft = new FilteredElementCollector(doc)
                        .OfClass(typeof(ViewFamilyType))
                        .Cast<ViewFamilyType>()
                        .FirstOrDefault(v => v.ViewFamily == ViewFamily.FloorPlan);

                    //ViewFamilyType curViewFamilyType = ViewFamily.FloorPlan as ViewFamilyType;
                    ViewPlan currentViewPlan = ViewPlan.Create(doc, planvft.Id, currentLevel.Id);
                }

                if(currentForm.CreateCeilingPlan() == true)
                {
                    ViewFamilyType cpvft = new FilteredElementCollector(doc)
                        .OfClass(typeof(ViewFamilyType))
                        .Cast<ViewFamilyType>()
                        .FirstOrDefault(v => v.ViewFamily == ViewFamily.CeilingPlan);
                    ViewPlan currentCeilingPlan = ViewPlan.Create(doc, cpvft.Id, currentLevel.Id);
                }
            }

            t.Commit();
            TaskDialog.Show("Complete", "Created " + levelData.Count + " Levels and Views");
            return Result.Succeeded;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
