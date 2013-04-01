using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace DJRM.Common.Infrastructure.Validation
{
    public class ValidatorBase<T> : IValidator<T> where T : EntityBase
    {
        #region IValidator<T>

        public virtual ValidationResult Validate(T entity)
        {
            var validationResult = new ValidationResult();
            validationResult.Errors.AddRange(ValidateDataAnnotations(entity));

            return validationResult;
        }

        public virtual ValidationResult CanBeDeleted(T entity)
        {
            /// Template Method
            /// Step 1. Generic check for every entity: new ones can be deleted
            /// Step 2. Perform custom checks
            var result = new ValidationResult();

            if (entity.IsNew)
            {
                return result;
            }

            OnCanBeDeleted(entity, result);

            return result;
        }

        #endregion

        #region protected and private methods
        protected IEnumerable<ValidationError> ValidateDataAnnotations(T entity)
        {
            List<ValidationError> errors = new List<ValidationError>();

            var context = new ValidationContext(entity, serviceProvider: null, items: null);
            var dataAnnotationValidationResults = new List<DataAnnotationsValidationResult>();

            if (!Validator.TryValidateObject(entity, context, dataAnnotationValidationResults, validateAllProperties: true))
            {
                errors.AddRange(TranslateToValidationError(entity, dataAnnotationValidationResults));
            }

            return errors;
        }

        private IEnumerable<ValidationError> TranslateToValidationError(T entity, ICollection<DataAnnotationsValidationResult> dataAnnotationValidationResults)
        {
            var errors = new List<ValidationError>();

            foreach (var dataAnnotationsValidationResult in dataAnnotationValidationResults.Where(r => r != DataAnnotationsValidationResult.Success))
            {
                errors.Add(GetValidationErrorFor(entity, dataAnnotationsValidationResult.MemberNames.FirstOrDefault(), dataAnnotationsValidationResult.ErrorMessage));
            }
            return errors;
        }

        protected ValidationError GetValidationErrorFor(T entity, string errorMessage)
        {
            return GetValidationErrorFor(entity, "", errorMessage);
        }

        protected ValidationError GetValidationErrorFor(T entity, string propertyName, string errorMessage)
        {
            return new ValidationError() { EntityName = entity.GetType().Name, EntityId = entity.Id, PropertyName = propertyName, ErrorMessage = errorMessage };
        }

        /// <summary>
        /// Checks if an entity satisfies the business rules defined to be deleted.
        /// These custom rules are verified only for non new entities. This validation is called from CanBeDelete        
        /// </summary>
        /// <param name="entity">Entity to be validated</param>
        /// <param name="result">Object that encapsulates the result of the validation process.</param>
        protected virtual void OnCanBeDeleted(T entity, ValidationResult result) { }

        #endregion
    }
}