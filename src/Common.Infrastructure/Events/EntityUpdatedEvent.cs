using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;

namespace DJRM.Common.Infrastructure.Events
{
    /// <summary>
    /// Event raised when a entity has been updated.
    /// </summary>
    /// <typeparam name="T">Type of the entity that has been updated.</typeparam>
    public class EntityUpdatedEvent<T> : IDomainEvent
        where T : EntityBase
    {
        public T Entity { get; set; }
        public List<String> UpdatedProperties { get; set; }
        public Dictionary<String, Object> OriginalValues { get; set; }

        /// <summary>
        /// Allows to cast this as an event of a different entity type
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public EntityUpdatedEvent<TEntity> OfType<TEntity>()
            where TEntity : EntityBase
        {
            return new EntityUpdatedEvent<TEntity>
            {
                UpdatedProperties = this.UpdatedProperties,
                OriginalValues = this.OriginalValues,
                Entity = this.Entity as TEntity
            };
        }

        /// <summary>
        /// Allows to check if a property of given isntance is modified by passing 
        /// a lambda function that acess the desired property
        /// </summary>
        /// <example>HasPropertyBeenUpdated(entity => entity.MyField)</example>
        public bool HasPropertyBeenUpdated<TValue>(Expression<Func<T, TValue>> entityExpression)
        {
            bool modified = false;
            string propertyAccessExpression = GetPropertyAccessExpression(entityExpression);

            if (propertyAccessExpression != null && propertyAccessExpression.Contains("."))
            {
                string propertyName = propertyAccessExpression.Substring(propertyAccessExpression.IndexOf(".") + 1);
                modified = UpdatedProperties.Contains(propertyName);
            }

            return modified;
        }

        /// <summary>
        /// Allows to get the value of a changed property by passing 
        /// a lambda function that acess the desired property
        /// </summary>
        /// <example>GetOriginalValue(entity => entity.MyField)</example>
        public TValue GetOriginalValue<TValue>(Expression<Func<T, TValue>> entityExpression)
        {
            TValue value = default(TValue);
            string propertyAccessExpression = GetPropertyAccessExpression(entityExpression);

            if (propertyAccessExpression != null && propertyAccessExpression.Contains("."))
            {
                string propertyName = propertyAccessExpression.Substring(propertyAccessExpression.IndexOf(".") + 1);

                value = (TValue)OriginalValues[propertyName];
            }
            return value;
        }

        private static string GetPropertyAccessExpression<TValue>(Expression<Func<T, TValue>> entityExpression)
        {
            string propertyAccessExpression = null;

            if (entityExpression.Body is UnaryExpression)
            {
                var exprBody = entityExpression.Body as UnaryExpression;
                //get the name of the property from the lambda expression
                propertyAccessExpression = exprBody.Operand.ToString();
            }
            else if (entityExpression.Body is MemberExpression)
            {
                var exprBody = entityExpression.Body as MemberExpression;
                //get the name of the property from the lambda expression
                propertyAccessExpression = exprBody.ToString();

            }
            return propertyAccessExpression;
        }
    }
}