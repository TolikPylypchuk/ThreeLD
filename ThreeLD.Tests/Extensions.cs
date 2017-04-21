using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace ThreeLD.Tests
{
    internal static class Extensions
    {
        public static void Validate(this Controller controller, object model)
        {
            var validationContext = new ValidationContext(model, null, null);
            var validationResults = new List<ValidationResult>();

            bool res = Validator.TryValidateObject(
                model, validationContext, validationResults, true);

            if (!res)
            {
                foreach (var validationResult in validationResults)
                {
                    controller.ModelState.AddModelError(
                        validationResult.MemberNames.FirstOrDefault()
                        ?? String.Empty,
                        validationResult.ErrorMessage);
                }
            }
        }
    }
}
