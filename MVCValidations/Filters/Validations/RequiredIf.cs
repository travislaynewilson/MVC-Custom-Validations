namespace MVCValidations.Filters.Validations
{
    public class RequiredIfAttribute : ConditionalValidationAttribute
    {
        public RequiredIfAttribute(string condition) : base(condition)
        {
            ValidationType = "requiredif";
        }
    }
}
