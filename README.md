# MVC Custom Validations
This is a collection of custom MVC validation types that can be used in combination with each other as well as the native MVC validations.


#### Why are these so special?
These validations use "expression walking" to define the conditions that the validator must meet in order to be invoked. The expressions you provide are parsed into LINQ, which are then converted into C# for the server-side validation and JavaScript for the client-side validation. By keeping things loose and DRY, your conditions stay in one place, allowing for greater flexibility and the benefit of client- and server-side conditional validation.


##### What are expressions?
Expressions are strings that define the condition that must be true in order for the validation to occur. The expressions follow C# syntax, with one exception: strings should be represented by single quotes (') instead of double quotes ("). You can use the C# operators in your expressions to define your logic.
Let's say you have a view model with some properties: Name (string), ID (int), and IsNew (bool).
Here are some sample expression strings you can pass in to these arguments:
```csharp
IsNew == true
IsNew                     // Equivolent to IsNew == true
!IsNew                    // Equivolent to IsNew == false
ID == 1
ID > 1
Name == 'Joe'             // Note the single quotes
Name != 'Joe'             // Note the single quotes
!IsNew && ID <= 100
IsNew || ID == 0
```



## Validations

### RequiredIfAttribute
```csharp
[RequiredIf(string expression)]
```
This attribute will conditionally require a field, based on the provided expression.

##### Examples
```csharp
// Will be required if the boolean IsNew is true
[RequiredIf("IsNew")]

// Will be required if the string Name equals 'Joe'
[RequiredIf("Name == 'Joe'")]

// Will be required if either IsNew is true or ID equals 0
[RequiredIf("IsNew || ID == 0")]
```

### RegularExpressionIfAttribute
```csharp
[RegularExpressionIf(params string[] expressions)]
```
This attribute will conditionally perform a regular expression validation if the condition is true and a value is provided. If no value is provided, the validation will not occur.

##### Defining Multiple Cases
This attribute allows you to define multiple expressions, or 'cases', that will be checked in the order they are defined. However, because C# does not allow us to pass complex objects into an attribute, we must provide two strings per 'case': The first is your condition expression, and the second is the regular expression that will be used if the condition is true.

Remember that you must provide two strings per case. This means that you will always pass at least two strings for every case, and their order is important.

If more than one case has a valid condition, only the first case defined will be validated. It is recommended that you define your cases starting with your most specific conditions first.

##### Examples
```csharp
// Will validate for numbers if a boolean property named RestrictToNumber is true
[RegularExpressionIf("RestrictToNumber", @"^\d+$")]

// Will validate the value as a Tax ID based on the value of an int property named TaxIDType.
// If TaxIDType == 1, then validate the value as an SSN.
// If TaxIDType == 2, then validate the value as an EIN.
[RegularExpressionIf(
  "TaxIDType == 1", @"^(^\d{3}-?\d{2}-?\d{4}$|^XXX-XX-XXXX$)$",
  "TaxIDType == 2", @"^[1-9]\d?-\d{7}$"
)]
```
 
### Samples
Simply run the MVC application found in this repository to see a sample form with these validators in action.


### Getting Started
The files required to include these validators in your own project are:
* ~/Content/jquery.validate.unobtrusive.custom.js
* ~/Helpers/Expressions/JavascriptExpressionVisitor.cs
* ~/Filters/Validations/ConditionalValidationAttribute.cs
* ~/Filters/Validations/RequiredIf.cs
* ~/Filters/Validations/RegularExpressionIf.cs

#### Dependencies
To evaluate strings as expressions, we use the [DynamicQuery library](https://www.nuget.org/packages/DynamicQuery/).
