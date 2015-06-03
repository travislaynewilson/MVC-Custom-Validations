(function ($) {
    // The following functions are taken from jquery.validate.unobtrusive: getModelPrefix, appendModelPrefix
    function getModelPrefix(fieldName) {
        return fieldName.substr(0, fieldName.lastIndexOf(".") + 1);
    }
    function appendModelPrefix(value, prefix) {
        if (value.indexOf("*.") === 0) {
            value = value.replace("*.", prefix);
        }
        return value;
    }
    function getPropertyValue(propertyName, dataType, prefix) {
        var name = appendModelPrefix(propertyName, prefix);

        // get the actual value of the target control
        // note - this probably needs to cater for more 
        // control types, e.g. radios
        var control = $('*[name=\'' + name + '\']');
        var controlType = control.attr('type');

        var actualValue;
        if (controlType === 'checkbox') {
            actualValue = control.prop('checked');
        }
        else if (controlType === 'radio') {
            var checkedControl = control.filter(':checked');
            if (checkedControl.length) {
                actualValue = checkedControl.val();
            }
        } else {
            actualValue = control.val();
        }

        // test for date format and handle (via jQuery UI)
        var dateFormat = control.data('dateformat');
        if (dateFormat != undefined) {
            actualValue = $.datepicker.parseDate(dateFormat, actualValue); // consider testing for parseDate and throwing a helpful message :-)
        }
        return actualValue;
    }



    // RequiredIf
    $.validator.addMethod('requiredif',
        function (value, element, parameters) {
            var validatorFunc = parameters.validatorFunc;
            var result = validatorFunc();
            if (result) {
                return $.validator.methods.required.call(this, value, element, parameters);
            }
            return true;
        }
    );
    $.validator.unobtrusive.adapters.add('requiredif', ['expression'],
        function (options) {
            var prefix = getModelPrefix(options.element.name);
            var expression = options.params['expression'];
            var gv = function (propertyName, dataType) { return getPropertyValue(propertyName, dataType, prefix); };
            eval('function theValidator(gv) { return ' + expression + ';}');
            options.rules['requiredif'] = {
                validatorFunc: function () {
                    return theValidator(gv);
                }
            };
            options.messages['requiredif'] = options.message;
        }
    );


    // RegularExpressionIf
    $.validator.addMethod('regexif',
        function (value, element, parameters) {
            var cases = parameters.cases;
            for (var i = 0; i < cases.length; i++) {
                var result = cases[i].validate();
                if (result) {
                    return $.validator.methods.regex.call(this, value, element, cases[i].RegularExpression);
                }
            }
            return true;
        }
    );
    $.validator.unobtrusive.adapters.add('regexif', ['cases'],
        function (options) {
            var cases = JSON.parse(options.params['cases']);
            var prefix = getModelPrefix(options.element.name);
            var gv = function (propertyName, dataType) { return getPropertyValue(propertyName, dataType, prefix); };

            for (var i = 0; i < cases.length; i++) {
                (function (gv, expression) {
                    eval('function theValidator(gv) { return ' + expression + ';}');
                    cases[i].validate = function () {
                        return theValidator(gv);
                    };
                })(gv, cases[i].Condition);
            }
            options.rules['regexif'] = {
                cases: cases
            };
            options.messages['regexif'] = options.message;
        }
    );

})(jQuery);