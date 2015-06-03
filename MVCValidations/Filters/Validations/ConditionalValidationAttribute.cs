using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Web.Mvc;
using MVCValidations.Helpers.Expressions;
using DynamicExpression = System.Linq.Dynamic.DynamicExpression;

namespace MVCValidations.Filters.Validations
{
    public abstract class ConditionalValidationAttribute : ValidationAttribute, IClientValidatable
    {
        protected readonly string Condition;
        protected string ValidationType { get; set; }


        protected ConditionalValidationAttribute()
        {
        }
        protected ConditionalValidationAttribute(string condition)
        {
            Condition = condition;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var conditionFunction = CreateExpressionDelegate(validationContext.ObjectType, Condition);
            var conditionMet = (bool)conditionFunction.DynamicInvoke(validationContext.ObjectInstance);
            if (conditionMet && value == null)
            {
                return new ValidationResult(FormatErrorMessage(null));
            }
            return null;
        }

        protected Delegate CreateExpressionDelegate(Type objectType, string expression)
        {
            var lambdaExpression = CreateExpression(objectType, expression);
            var func = lambdaExpression.Compile();
            return func;
        }

        protected LambdaExpression CreateExpression(Type objectType, string expression)
        {
            var lambdaExpression = DynamicExpression.ParseLambda(objectType, typeof(bool), expression.Replace("'", "\""));
            return lambdaExpression;
        }

        public virtual IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var errorMessage = FormatErrorMessage(metadata.GetDisplayName());

            var expression = CreateExpression(metadata.ContainerType, Condition);
            var visitor = new JavascriptExpressionVisitor();
            var javascriptExpression = visitor.Translate(expression);

            var clientRule = new ModelClientValidationRule
            {
                ErrorMessage = errorMessage,
                ValidationType = ValidationType,
                ValidationParameters =
                                         {
                                             { "expression", javascriptExpression }
                                         }
            };

            return new[] { clientRule };
        }
    }

}
