using System.ComponentModel.DataAnnotations;
using MVCValidations.Filters.Validations;

namespace MVCValidations.ViewModels
{
    public class FormViewModel
    {
        public bool OptIn { get; set; }

        [RequiredIf("OptIn")]
        public string Email { get; set; }

        [RequiredIf("Email == 'joe@aol.com'")]
        public string EmailExtended { get; set; }

        [RequiredIf("OptIn && Email == 'joe@aol.com'")]
        public string EmailExtended2 { get; set; }


        public bool ValidateAge { get; set; }

        [RegularExpressionIf("ValidateAge", @"^\d+$")]
        public string Age { get; set; }


        public int NameRule { get; set; }

        [RegularExpressionIf(
            "NameRule == 1", @"^joe$",
            "NameRule == 2", @"^mike$"
        )]
        public string Name { get; set; }


        public bool RequireTaxID { get; set; }
        public int TaxIDType { get; set; }

        [RequiredIf("RequireTaxID == true"), RegularExpressionIf(
            "TaxIDType == 1", @"^(^\d{3}-?\d{2}-?\d{4}$|^XXX-XX-XXXX$)$",
            "TaxIDType == 2", @"^[1-9]\d?-\d{7}$"
        )]
        public string TaxID { get; set; }
    }
}
