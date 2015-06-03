using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Newtonsoft.Json;
using MVCValidations.Helpers.Expressions;

namespace MVCValidations.Filters.Validations
{
    public class RegularExpressionCase
    {
        public RegularExpressionCase(string condition, string regularExpression)
        {
            Condition = condition;
            RegularExpression = regularExpression;
        }

        public string Condition { get; set; }
        public string RegularExpression { get; set; }
    }

    /// <summary>
    /// A conditional regular expression validation filter that will fire any of the matching conditions provided. You can provide multiple 'cases' by providing strings in groups of two: the first string would be your condition, and the second is your regular expression.
    /// </summary>
    /// <example>
    /// [RegularExpressionIf("SomeField == \"Some value\"", @"^/d+$")]
    /// [RegularExpressionIf(
    ///     "SomeField == \"Some value\"", @"^/d+$",
    ///     "SomeField != \"Some value\" &amp;&amp; SomeOtherField == true", @"^/w+$",
    ///     "SomeField != \"Some value\" || AnotherField &lt; 1", @"^/w+$"
    /// )]
    /// </example>

    public class RegularExpressionIfAttribute : ConditionalValidationAttribute
    {
        protected readonly List<RegularExpressionCase> Rules;

        public RegularExpressionIfAttribute(params string[] rules)
        {
            ValidationType = "regexif";

            Rules = new List<RegularExpressionCase>();

            for (var i = 0; i < rules.Length; i+=2)
            {
                Rules.Add(new RegularExpressionCase(rules[i], rules[i + 1]));
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return null;

            foreach (var rule in Rules)
            {
                var conditionFunction = CreateExpressionDelegate(validationContext.ObjectType, rule.Condition);
                var conditionMet = (bool)conditionFunction.DynamicInvoke(validationContext.ObjectInstance);
                if (conditionMet)
                {
                    var regex = new Regex(rule.RegularExpression);
                    if (!regex.IsMatch((value ?? "").ToString()))
                    {
                        return new ValidationResult(FormatErrorMessage(null));
                    }
                }
            }
            return null;
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var errorMessage = FormatErrorMessage(metadata.GetDisplayName());

            var cases = new List<RegularExpressionCase>();
            foreach (var rule in Rules)
            {
                var expression = CreateExpression(metadata.ContainerType, rule.Condition);
                var visitor = new JavascriptExpressionVisitor();
                var javascriptExpression = visitor.Translate(expression);

                cases.Add(new RegularExpressionCase(javascriptExpression, rule.RegularExpression));
            }
            var clientRule = new ModelClientValidationRule
            {
                ErrorMessage = errorMessage,
                ValidationType = ValidationType,
                ValidationParameters =
                                         {
                                             { "cases", JsonConvert.SerializeObject(cases) }
                                         }
            };

            return new[] { clientRule };
        }
    }
}
