
namespace DJRM.Common.Infrastructure.Validation
{
    /// <summary>
    /// Interface that must be implemented by all the validators which
    /// validate model entities.
    /// </summary>
    /// <typeparam name="T">Type of the entity that will be validated.</typeparam>
    public interface IValidator<T> where T : EntityBase
    {
        /// <summary>
        /// Checks if an entity satisfies the business rules defined.
        /// </summary>
        /// <param name="entity">Entity to be validated.</param>
        /// <returns>Object that encapsulates the result of the validation process.</returns>
        ValidationResult Validate(T entity);


        /// <summary>
        /// Checks if an entity satisfies the business rules defined to be deleted.
        /// </summary>
        /// <param name="entity">Entity to be validated.</param>
        /// <returns>Object that encapsulates the result of the validation process.</returns>
        ValidationResult CanBeDeleted(T entity);

    }
}
