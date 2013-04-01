using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Elmah;
using DJRM.Common.Infrastructure.Validation;

namespace DJRM.Common.Controllers
{
    public class ControllerBase : Controller
    {

        /// <summary>
        /// Returns a standard JSON object for AJAX calls
        /// </summary>
        protected JsonResult StandardJson(object Data, IEnumerable<ValidationError> Errors)
        {
            var errorList = Errors.ToList();
            errorList.AddRange(GetModelStateErrors());

            return Json(new { Content = Data, 
                              Successful = Errors.Count() == 0, 
                              Errors = Errors });
        }


        protected IEnumerable<ValidationError> GetModelStateErrors()
        {
            List<ValidationError> errors = new List<ValidationError>();

            foreach (string key in ModelState.Keys)
            {
                foreach (ModelError error in ModelState[key].Errors.Where(e => !string.IsNullOrEmpty(e.ErrorMessage)))
                {
                    errors.Add(new ValidationError { PropertyName = key, ErrorMessage = error.ErrorMessage });
                }
            }

            return errors;
        }

        /// <summary>
        /// Logs exception using Elmah
        /// </summary>
        protected void LogException(Exception e)
        {
            ErrorSignal.FromCurrentContext().Raise(e);
        }

        protected void AddErrorsToModelState(IEnumerable<ValidationError> errors)
        {
            if (errors != null)
            {
                //Add validation errors
                foreach (ValidationError error in errors)
                {
                    string propertyName = error.PropertyName;

                    if (!String.IsNullOrEmpty(propertyName))
                    {
                        //If the viewModel is contained by another ViewModel as PageDetails, 
                        //then the propertyName in the view won't be error.PropertyName, it'll be PageDetails.PropertyName
                        propertyName = ModelState.Keys.FirstOrDefault(p => p.Contains("." + error.PropertyName));

                        if (propertyName == null)
                        {
                            propertyName = error.PropertyName;
                        }
                    }

                    AddErrorToModelState(propertyName, error.ErrorMessage);
                }
            }
        }

        private void AddErrorToModelState(string propertyName, string errorMessage)
        {
            //A null value for the property name will be replaced with an empty string:
            propertyName = propertyName ?? "";

            bool errorAlreadyInState = (ModelState[propertyName] != null
                                        && ModelState[propertyName].Errors.Count(e => e.ErrorMessage == errorMessage) > 0);

            if (!errorAlreadyInState)
            {
                ModelState.AddModelError(propertyName, errorMessage);
            }
        }
    }
}