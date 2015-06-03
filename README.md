# MVC-Custom-Validations
This is a collection of custom MVC validation types that can be used in combination with each other as well as the native MVC validations.


#### Why are these so special?
These validations use "expression walking". The expression argument used is parsed into LINQ, which is then converted into C# for the server-side validation and JavaScript for the client-side validation. By keeping things loose and DRY, your conditions stay in one place and allow for great flexibility.


##### Expressions
Expressions define the condition that must be true in order for the validation to occur. The expressions follow C# syntax, with one exception: strings should be represented by single quotes (') instead of double quotes ("). You can use the C# operators in your expression to help define your logic.
Let's say you have a view model with some properties: Name (string), ID (int), and IsNew (bool).
Here are some sample expression strings you can pass in to these arguments:
```csharp
IsNew
IsNew == true
IsNew != true
ID == 1
ID > 1
Name == 'Joe'
Name != 'Joe'
!IsNew && ID <= 100
IsNew || ID == 0
```

### RequiredIf
```csharp
[RequiredIf(string expression)]
```
This tag will optionally require a field, based on the provided expression.

##### Examples
```csharp
// Will be required if the boolean IsNew is true
[RequiredIf("IsNew")]

// Will be required if the string Name equals 'Joe'
[RequiredIf("Name == 'Joe'")]

// Will be required if either IsNew is true or ID equals 0
[RequiredIf("IsNew || ID == 0")]
```

### RegularExpressionIf
```csharp
[RegularExpressionIf(params string[] expressions)]
```
This tag will optionally perform a regular expression if the condition applies and a value is provided. If not value is provided, the cases will not attempt to validate.

##### Defining Multiple Cases
This validator allows you to define multiple expressions, or 'cases'. Because you cannot pass complex objects into an attribute, this one uses a pattern to help you define your expression and regular expression. For each 'case' you want to match, you will pass two strings: The first is your expression, and the second is your regular expression that will be used if the expression matches.

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
