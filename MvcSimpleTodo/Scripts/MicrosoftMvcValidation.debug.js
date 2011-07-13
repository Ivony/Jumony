//!----------------------------------------------------------
//! Copyright (C) Microsoft Corporation. All rights reserved.
//!----------------------------------------------------------
//! MicrosoftMvcValidation.js


Type.registerNamespace('Sys.Mvc');

////////////////////////////////////////////////////////////////////////////////
// Sys.Mvc.Validation

Sys.Mvc.$create_Validation = function Sys_Mvc_Validation() { return {}; }


////////////////////////////////////////////////////////////////////////////////
// Sys.Mvc.JsonValidationField

Sys.Mvc.$create_JsonValidationField = function Sys_Mvc_JsonValidationField() { return {}; }


////////////////////////////////////////////////////////////////////////////////
// Sys.Mvc.JsonValidationOptions

Sys.Mvc.$create_JsonValidationOptions = function Sys_Mvc_JsonValidationOptions() { return {}; }


////////////////////////////////////////////////////////////////////////////////
// Sys.Mvc.JsonValidationRule

Sys.Mvc.$create_JsonValidationRule = function Sys_Mvc_JsonValidationRule() { return {}; }


////////////////////////////////////////////////////////////////////////////////
// Sys.Mvc.ValidationContext

Sys.Mvc.$create_ValidationContext = function Sys_Mvc_ValidationContext() { return {}; }


////////////////////////////////////////////////////////////////////////////////
// Sys.Mvc.NumberValidator

Sys.Mvc.NumberValidator = function Sys_Mvc_NumberValidator() {
}
Sys.Mvc.NumberValidator.create = function Sys_Mvc_NumberValidator$create(rule) {
    /// <param name="rule" type="Sys.Mvc.JsonValidationRule">
    /// </param>
    /// <returns type="Sys.Mvc.Validator"></returns>
    return Function.createDelegate(new Sys.Mvc.NumberValidator(), new Sys.Mvc.NumberValidator().validate);
}
Sys.Mvc.NumberValidator.prototype = {
    
    validate: function Sys_Mvc_NumberValidator$validate(value, context) {
        /// <param name="value" type="String">
        /// </param>
        /// <param name="context" type="Sys.Mvc.ValidationContext">
        /// </param>
        /// <returns type="Object"></returns>
        if (Sys.Mvc._validationUtil.stringIsNullOrEmpty(value)) {
            return true;
        }
        var n = Number.parseLocale(value);
        return (!isNaN(n));
    }
}


////////////////////////////////////////////////////////////////////////////////
// Sys.Mvc.FormContext

Sys.Mvc.FormContext = function Sys_Mvc_FormContext(formElement, validationSummaryElement) {
    /// <param name="formElement" type="Object" domElement="true">
    /// </param>
    /// <param name="validationSummaryElement" type="Object" domElement="true">
    /// </param>
    /// <field name="_validationSummaryErrorCss" type="String" static="true">
    /// </field>
    /// <field name="_validationSummaryValidCss" type="String" static="true">
    /// </field>
    /// <field name="_formValidationTag" type="String" static="true">
    /// </field>
    /// <field name="_onClickHandler" type="Sys.UI.DomEventHandler">
    /// </field>
    /// <field name="_onSubmitHandler" type="Sys.UI.DomEventHandler">
    /// </field>
    /// <field name="_errors" type="Array">
    /// </field>
    /// <field name="_submitButtonClicked" type="Object" domElement="true">
    /// </field>
    /// <field name="_validationSummaryElement" type="Object" domElement="true">
    /// </field>
    /// <field name="_validationSummaryULElement" type="Object" domElement="true">
    /// </field>
    /// <field name="fields" type="Array" elementType="FieldContext">
    /// </field>
    /// <field name="_formElement" type="Object" domElement="true">
    /// </field>
    /// <field name="replaceValidationSummary" type="Boolean">
    /// </field>
    this._errors = [];
    this.fields = new Array(0);
    this._formElement = formElement;
    this._validationSummaryElement = validationSummaryElement;
    formElement[Sys.Mvc.FormContext._formValidationTag] = this;
    if (validationSummaryElement) {
        var ulElements = validationSummaryElement.getElementsByTagName('ul');
        if (ulElements.length > 0) {
            this._validationSummaryULElement = ulElements[0];
        }
    }
    this._onClickHandler = Function.createDelegate(this, this._form_OnClick);
    this._onSubmitHandler = Function.createDelegate(this, this._form_OnSubmit);
}
Sys.Mvc.FormContext._Application_Load = function Sys_Mvc_FormContext$_Application_Load() {
    var allFormOptions = window.mvcClientValidationMetadata;
    if (allFormOptions) {
        while (allFormOptions.length > 0) {
            var thisFormOptions = allFormOptions.pop();
            Sys.Mvc.FormContext._parseJsonOptions(thisFormOptions);
        }
    }
}
Sys.Mvc.FormContext._getFormElementsWithName = function Sys_Mvc_FormContext$_getFormElementsWithName(formElement, name) {
    /// <param name="formElement" type="Object" domElement="true">
    /// </param>
    /// <param name="name" type="String">
    /// </param>
    /// <returns type="Array" elementType="Object" elementDomElement="true"></returns>
    var allElementsWithNameInForm = [];
    var allElementsWithName = document.getElementsByName(name);
    for (var i = 0; i < allElementsWithName.length; i++) {
        var thisElement = allElementsWithName[i];
        if (Sys.Mvc.FormContext._isElementInHierarchy(formElement, thisElement)) {
            Array.add(allElementsWithNameInForm, thisElement);
        }
    }
    return allElementsWithNameInForm;
}
Sys.Mvc.FormContext.getValidationForForm = function Sys_Mvc_FormContext$getValidationForForm(formElement) {
    /// <param name="formElement" type="Object" domElement="true">
    /// </param>
    /// <returns type="Sys.Mvc.FormContext"></returns>
    return formElement[Sys.Mvc.FormContext._formValidationTag];
}
Sys.Mvc.FormContext._isElementInHierarchy = function Sys_Mvc_FormContext$_isElementInHierarchy(parent, child) {
    /// <param name="parent" type="Object" domElement="true">
    /// </param>
    /// <param name="child" type="Object" domElement="true">
    /// </param>
    /// <returns type="Boolean"></returns>
    while (child) {
        if (parent === child) {
            return true;
        }
        child = child.parentNode;
    }
    return false;
}
Sys.Mvc.FormContext._parseJsonOptions = function Sys_Mvc_FormContext$_parseJsonOptions(options) {
    /// <param name="options" type="Sys.Mvc.JsonValidationOptions">
    /// </param>
    /// <returns type="Sys.Mvc.FormContext"></returns>
    var formElement = $get(options.FormId);
    var validationSummaryElement = (!Sys.Mvc._validationUtil.stringIsNullOrEmpty(options.ValidationSummaryId)) ? $get(options.ValidationSummaryId) : null;
    var formContext = new Sys.Mvc.FormContext(formElement, validationSummaryElement);
    formContext.enableDynamicValidation();
    formContext.replaceValidationSummary = options.ReplaceValidationSummary;
    for (var i = 0; i < options.Fields.length; i++) {
        var field = options.Fields[i];
        var fieldElements = Sys.Mvc.FormContext._getFormElementsWithName(formElement, field.FieldName);
        var validationMessageElement = (!Sys.Mvc._validationUtil.stringIsNullOrEmpty(field.ValidationMessageId)) ? $get(field.ValidationMessageId) : null;
        var fieldContext = new Sys.Mvc.FieldContext(formContext);
        Array.addRange(fieldContext.elements, fieldElements);
        fieldContext.validationMessageElement = validationMessageElement;
        fieldContext.replaceValidationMessageContents = field.ReplaceValidationMessageContents;
        for (var j = 0; j < field.ValidationRules.length; j++) {
            var rule = field.ValidationRules[j];
            var validator = Sys.Mvc.ValidatorRegistry.getValidator(rule);
            if (validator) {
                var validation = Sys.Mvc.$create_Validation();
                validation.fieldErrorMessage = rule.ErrorMessage;
                validation.validator = validator;
                Array.add(fieldContext.validations, validation);
            }
        }
        fieldContext.enableDynamicValidation();
        Array.add(formContext.fields, fieldContext);
    }
    var registeredValidatorCallbacks = formElement.validationCallbacks;
    if (!registeredValidatorCallbacks) {
        registeredValidatorCallbacks = [];
        formElement.validationCallbacks = registeredValidatorCallbacks;
    }
    registeredValidatorCallbacks.push(Function.createDelegate(null, function() {
        return Sys.Mvc._validationUtil.arrayIsNullOrEmpty(formContext.validate('submit'));
    }));
    return formContext;
}
Sys.Mvc.FormContext.prototype = {
    _onClickHandler: null,
    _onSubmitHandler: null,
    _submitButtonClicked: null,
    _validationSummaryElement: null,
    _validationSummaryULElement: null,
    _formElement: null,
    replaceValidationSummary: false,
    
    addError: function Sys_Mvc_FormContext$addError(message) {
        /// <param name="message" type="String">
        /// </param>
        this.addErrors([ message ]);
    },
    
    addErrors: function Sys_Mvc_FormContext$addErrors(messages) {
        /// <param name="messages" type="Array" elementType="String">
        /// </param>
        if (!Sys.Mvc._validationUtil.arrayIsNullOrEmpty(messages)) {
            Array.addRange(this._errors, messages);
            this._onErrorCountChanged();
        }
    },
    
    clearErrors: function Sys_Mvc_FormContext$clearErrors() {
        Array.clear(this._errors);
        this._onErrorCountChanged();
    },
    
    _displayError: function Sys_Mvc_FormContext$_displayError() {
        if (this._validationSummaryElement) {
            if (this._validationSummaryULElement) {
                Sys.Mvc._validationUtil.removeAllChildren(this._validationSummaryULElement);
                for (var i = 0; i < this._errors.length; i++) {
                    var liElement = document.createElement('li');
                    Sys.Mvc._validationUtil.setInnerText(liElement, this._errors[i]);
                    this._validationSummaryULElement.appendChild(liElement);
                }
            }
            Sys.UI.DomElement.removeCssClass(this._validationSummaryElement, Sys.Mvc.FormContext._validationSummaryValidCss);
            Sys.UI.DomElement.addCssClass(this._validationSummaryElement, Sys.Mvc.FormContext._validationSummaryErrorCss);
        }
    },
    
    _displaySuccess: function Sys_Mvc_FormContext$_displaySuccess() {
        var validationSummaryElement = this._validationSummaryElement;
        if (validationSummaryElement) {
            var validationSummaryULElement = this._validationSummaryULElement;
            if (validationSummaryULElement) {
                validationSummaryULElement.innerHTML = '';
            }
            Sys.UI.DomElement.removeCssClass(validationSummaryElement, Sys.Mvc.FormContext._validationSummaryErrorCss);
            Sys.UI.DomElement.addCssClass(validationSummaryElement, Sys.Mvc.FormContext._validationSummaryValidCss);
        }
    },
    
    enableDynamicValidation: function Sys_Mvc_FormContext$enableDynamicValidation() {
        Sys.UI.DomEvent.addHandler(this._formElement, 'click', this._onClickHandler);
        Sys.UI.DomEvent.addHandler(this._formElement, 'submit', this._onSubmitHandler);
    },
    
    _findSubmitButton: function Sys_Mvc_FormContext$_findSubmitButton(element) {
        /// <param name="element" type="Object" domElement="true">
        /// </param>
        /// <returns type="Object" domElement="true"></returns>
        if (element.disabled) {
            return null;
        }
        var tagName = element.tagName.toUpperCase();
        var inputElement = element;
        if (tagName === 'INPUT') {
            var type = inputElement.type;
            if (type === 'submit' || type === 'image') {
                return inputElement;
            }
        }
        else if ((tagName === 'BUTTON') && (inputElement.type === 'submit')) {
            return inputElement;
        }
        return null;
    },
    
    _form_OnClick: function Sys_Mvc_FormContext$_form_OnClick(e) {
        /// <param name="e" type="Sys.UI.DomEvent">
        /// </param>
        this._submitButtonClicked = this._findSubmitButton(e.target);
    },
    
    _form_OnSubmit: function Sys_Mvc_FormContext$_form_OnSubmit(e) {
        /// <param name="e" type="Sys.UI.DomEvent">
        /// </param>
        var form = e.target;
        var submitButton = this._submitButtonClicked;
        if (submitButton && submitButton.disableValidation) {
            return;
        }
        var errorMessages = this.validate('submit');
        if (!Sys.Mvc._validationUtil.arrayIsNullOrEmpty(errorMessages)) {
            e.preventDefault();
        }
    },
    
    _onErrorCountChanged: function Sys_Mvc_FormContext$_onErrorCountChanged() {
        if (!this._errors.length) {
            this._displaySuccess();
        }
        else {
            this._displayError();
        }
    },
    
    validate: function Sys_Mvc_FormContext$validate(eventName) {
        /// <param name="eventName" type="String">
        /// </param>
        /// <returns type="Array" elementType="String"></returns>
        var fields = this.fields;
        var errors = [];
        for (var i = 0; i < fields.length; i++) {
            var field = fields[i];
            var thisErrors = field.validate(eventName);
            if (thisErrors) {
                Array.addRange(errors, thisErrors);
            }
        }
        if (this.replaceValidationSummary) {
            this.clearErrors();
            this.addErrors(errors);
        }
        return errors;
    }
}


////////////////////////////////////////////////////////////////////////////////
// Sys.Mvc.FieldContext

Sys.Mvc.FieldContext = function Sys_Mvc_FieldContext(formContext) {
    /// <param name="formContext" type="Sys.Mvc.FormContext">
    /// </param>
    /// <field name="_hasTextChangedTag" type="String" static="true">
    /// </field>
    /// <field name="_hasValidationFiredTag" type="String" static="true">
    /// </field>
    /// <field name="_inputElementErrorCss" type="String" static="true">
    /// </field>
    /// <field name="_inputElementValidCss" type="String" static="true">
    /// </field>
    /// <field name="_validationMessageErrorCss" type="String" static="true">
    /// </field>
    /// <field name="_validationMessageValidCss" type="String" static="true">
    /// </field>
    /// <field name="_onBlurHandler" type="Sys.UI.DomEventHandler">
    /// </field>
    /// <field name="_onChangeHandler" type="Sys.UI.DomEventHandler">
    /// </field>
    /// <field name="_onInputHandler" type="Sys.UI.DomEventHandler">
    /// </field>
    /// <field name="_onPropertyChangeHandler" type="Sys.UI.DomEventHandler">
    /// </field>
    /// <field name="_errors" type="Array">
    /// </field>
    /// <field name="defaultErrorMessage" type="String">
    /// </field>
    /// <field name="elements" type="Array" elementType="Object" elementDomElement="true">
    /// </field>
    /// <field name="formContext" type="Sys.Mvc.FormContext">
    /// </field>
    /// <field name="replaceValidationMessageContents" type="Boolean">
    /// </field>
    /// <field name="validationMessageElement" type="Object" domElement="true">
    /// </field>
    /// <field name="validations" type="Array" elementType="Validation">
    /// </field>
    this._errors = [];
    this.elements = new Array(0);
    this.validations = new Array(0);
    this.formContext = formContext;
    this._onBlurHandler = Function.createDelegate(this, this._element_OnBlur);
    this._onChangeHandler = Function.createDelegate(this, this._element_OnChange);
    this._onInputHandler = Function.createDelegate(this, this._element_OnInput);
    this._onPropertyChangeHandler = Function.createDelegate(this, this._element_OnPropertyChange);
}
Sys.Mvc.FieldContext.prototype = {
    _onBlurHandler: null,
    _onChangeHandler: null,
    _onInputHandler: null,
    _onPropertyChangeHandler: null,
    defaultErrorMessage: null,
    formContext: null,
    replaceValidationMessageContents: false,
    validationMessageElement: null,
    
    addError: function Sys_Mvc_FieldContext$addError(message) {
        /// <param name="message" type="String">
        /// </param>
        this.addErrors([ message ]);
    },
    
    addErrors: function Sys_Mvc_FieldContext$addErrors(messages) {
        /// <param name="messages" type="Array" elementType="String">
        /// </param>
        if (!Sys.Mvc._validationUtil.arrayIsNullOrEmpty(messages)) {
            Array.addRange(this._errors, messages);
            this._onErrorCountChanged();
        }
    },
    
    clearErrors: function Sys_Mvc_FieldContext$clearErrors() {
        Array.clear(this._errors);
        this._onErrorCountChanged();
    },
    
    _displayError: function Sys_Mvc_FieldContext$_displayError() {
        var validationMessageElement = this.validationMessageElement;
        if (validationMessageElement) {
            if (this.replaceValidationMessageContents) {
                Sys.Mvc._validationUtil.setInnerText(validationMessageElement, this._errors[0]);
            }
            Sys.UI.DomElement.removeCssClass(validationMessageElement, Sys.Mvc.FieldContext._validationMessageValidCss);
            Sys.UI.DomElement.addCssClass(validationMessageElement, Sys.Mvc.FieldContext._validationMessageErrorCss);
        }
        var elements = this.elements;
        for (var i = 0; i < elements.length; i++) {
            var element = elements[i];
            Sys.UI.DomElement.removeCssClass(element, Sys.Mvc.FieldContext._inputElementValidCss);
            Sys.UI.DomElement.addCssClass(element, Sys.Mvc.FieldContext._inputElementErrorCss);
        }
    },
    
    _displaySuccess: function Sys_Mvc_FieldContext$_displaySuccess() {
        var validationMessageElement = this.validationMessageElement;
        if (validationMessageElement) {
            if (this.replaceValidationMessageContents) {
                Sys.Mvc._validationUtil.setInnerText(validationMessageElement, '');
            }
            Sys.UI.DomElement.removeCssClass(validationMessageElement, Sys.Mvc.FieldContext._validationMessageErrorCss);
            Sys.UI.DomElement.addCssClass(validationMessageElement, Sys.Mvc.FieldContext._validationMessageValidCss);
        }
        var elements = this.elements;
        for (var i = 0; i < elements.length; i++) {
            var element = elements[i];
            Sys.UI.DomElement.removeCssClass(element, Sys.Mvc.FieldContext._inputElementErrorCss);
            Sys.UI.DomElement.addCssClass(element, Sys.Mvc.FieldContext._inputElementValidCss);
        }
    },
    
    _element_OnBlur: function Sys_Mvc_FieldContext$_element_OnBlur(e) {
        /// <param name="e" type="Sys.UI.DomEvent">
        /// </param>
        if (e.target[Sys.Mvc.FieldContext._hasTextChangedTag] || e.target[Sys.Mvc.FieldContext._hasValidationFiredTag]) {
            this.validate('blur');
        }
    },
    
    _element_OnChange: function Sys_Mvc_FieldContext$_element_OnChange(e) {
        /// <param name="e" type="Sys.UI.DomEvent">
        /// </param>
        e.target[Sys.Mvc.FieldContext._hasTextChangedTag] = true;
    },
    
    _element_OnInput: function Sys_Mvc_FieldContext$_element_OnInput(e) {
        /// <param name="e" type="Sys.UI.DomEvent">
        /// </param>
        e.target[Sys.Mvc.FieldContext._hasTextChangedTag] = true;
        if (e.target[Sys.Mvc.FieldContext._hasValidationFiredTag]) {
            this.validate('input');
        }
    },
    
    _element_OnPropertyChange: function Sys_Mvc_FieldContext$_element_OnPropertyChange(e) {
        /// <param name="e" type="Sys.UI.DomEvent">
        /// </param>
        if (e.rawEvent.propertyName === 'value') {
            e.target[Sys.Mvc.FieldContext._hasTextChangedTag] = true;
            if (e.target[Sys.Mvc.FieldContext._hasValidationFiredTag]) {
                this.validate('input');
            }
        }
    },
    
    enableDynamicValidation: function Sys_Mvc_FieldContext$enableDynamicValidation() {
        var elements = this.elements;
        for (var i = 0; i < elements.length; i++) {
            var element = elements[i];
            if (Sys.Mvc._validationUtil.elementSupportsEvent(element, 'onpropertychange')) {
                Sys.UI.DomEvent.addHandler(element, 'propertychange', this._onPropertyChangeHandler);
            }
            else {
                Sys.UI.DomEvent.addHandler(element, 'input', this._onInputHandler);
            }
            Sys.UI.DomEvent.addHandler(element, 'change', this._onChangeHandler);
            Sys.UI.DomEvent.addHandler(element, 'blur', this._onBlurHandler);
        }
    },
    
    _getErrorString: function Sys_Mvc_FieldContext$_getErrorString(validatorReturnValue, fieldErrorMessage) {
        /// <param name="validatorReturnValue" type="Object">
        /// </param>
        /// <param name="fieldErrorMessage" type="String">
        /// </param>
        /// <returns type="String"></returns>
        var fallbackErrorMessage = fieldErrorMessage || this.defaultErrorMessage;
        if (Boolean.isInstanceOfType(validatorReturnValue)) {
            return (validatorReturnValue) ? null : fallbackErrorMessage;
        }
        if (String.isInstanceOfType(validatorReturnValue)) {
            return ((validatorReturnValue).length) ? validatorReturnValue : fallbackErrorMessage;
        }
        return null;
    },
    
    _getStringValue: function Sys_Mvc_FieldContext$_getStringValue() {
        /// <returns type="String"></returns>
        var elements = this.elements;
        return (elements.length > 0) ? elements[0].value : null;
    },
    
    _markValidationFired: function Sys_Mvc_FieldContext$_markValidationFired() {
        var elements = this.elements;
        for (var i = 0; i < elements.length; i++) {
            var element = elements[i];
            element[Sys.Mvc.FieldContext._hasValidationFiredTag] = true;
        }
    },
    
    _onErrorCountChanged: function Sys_Mvc_FieldContext$_onErrorCountChanged() {
        if (!this._errors.length) {
            this._displaySuccess();
        }
        else {
            this._displayError();
        }
    },
    
    validate: function Sys_Mvc_FieldContext$validate(eventName) {
        /// <param name="eventName" type="String">
        /// </param>
        /// <returns type="Array" elementType="String"></returns>
        var validations = this.validations;
        var errors = [];
        var value = this._getStringValue();
        for (var i = 0; i < validations.length; i++) {
            var validation = validations[i];
            var context = Sys.Mvc.$create_ValidationContext();
            context.eventName = eventName;
            context.fieldContext = this;
            context.validation = validation;
            var retVal = validation.validator(value, context);
            var errorMessage = this._getErrorString(retVal, validation.fieldErrorMessage);
            if (!Sys.Mvc._validationUtil.stringIsNullOrEmpty(errorMessage)) {
                Array.add(errors, errorMessage);
            }
        }
        this._markValidationFired();
        this.clearErrors();
        this.addErrors(errors);
        return errors;
    }
}


////////////////////////////////////////////////////////////////////////////////
// Sys.Mvc.RangeValidator

Sys.Mvc.RangeValidator = function Sys_Mvc_RangeValidator(minimum, maximum) {
    /// <param name="minimum" type="Number">
    /// </param>
    /// <param name="maximum" type="Number">
    /// </param>
    /// <field name="_minimum" type="Number">
    /// </field>
    /// <field name="_maximum" type="Number">
    /// </field>
    this._minimum = minimum;
    this._maximum = maximum;
}
Sys.Mvc.RangeValidator.create = function Sys_Mvc_RangeValidator$create(rule) {
    /// <param name="rule" type="Sys.Mvc.JsonValidationRule">
    /// </param>
    /// <returns type="Sys.Mvc.Validator"></returns>
    var min = rule.ValidationParameters['minimum'];
    var max = rule.ValidationParameters['maximum'];
    return Function.createDelegate(new Sys.Mvc.RangeValidator(min, max), new Sys.Mvc.RangeValidator(min, max).validate);
}
Sys.Mvc.RangeValidator.prototype = {
    _minimum: null,
    _maximum: null,
    
    validate: function Sys_Mvc_RangeValidator$validate(value, context) {
        /// <param name="value" type="String">
        /// </param>
        /// <param name="context" type="Sys.Mvc.ValidationContext">
        /// </param>
        /// <returns type="Object"></returns>
        if (Sys.Mvc._validationUtil.stringIsNullOrEmpty(value)) {
            return true;
        }
        var n = Number.parseLocale(value);
        return (!isNaN(n) && this._minimum <= n && n <= this._maximum);
    }
}


////////////////////////////////////////////////////////////////////////////////
// Sys.Mvc.RegularExpressionValidator

Sys.Mvc.RegularExpressionValidator = function Sys_Mvc_RegularExpressionValidator(pattern) {
    /// <param name="pattern" type="String">
    /// </param>
    /// <field name="_pattern" type="String">
    /// </field>
    this._pattern = pattern;
}
Sys.Mvc.RegularExpressionValidator.create = function Sys_Mvc_RegularExpressionValidator$create(rule) {
    /// <param name="rule" type="Sys.Mvc.JsonValidationRule">
    /// </param>
    /// <returns type="Sys.Mvc.Validator"></returns>
    var pattern = rule.ValidationParameters['pattern'];
    return Function.createDelegate(new Sys.Mvc.RegularExpressionValidator(pattern), new Sys.Mvc.RegularExpressionValidator(pattern).validate);
}
Sys.Mvc.RegularExpressionValidator.prototype = {
    _pattern: null,
    
    validate: function Sys_Mvc_RegularExpressionValidator$validate(value, context) {
        /// <param name="value" type="String">
        /// </param>
        /// <param name="context" type="Sys.Mvc.ValidationContext">
        /// </param>
        /// <returns type="Object"></returns>
        if (Sys.Mvc._validationUtil.stringIsNullOrEmpty(value)) {
            return true;
        }
        var regExp = new RegExp(this._pattern);
        var matches = regExp.exec(value);
        return (!Sys.Mvc._validationUtil.arrayIsNullOrEmpty(matches) && matches[0].length === value.length);
    }
}


////////////////////////////////////////////////////////////////////////////////
// Sys.Mvc.RequiredValidator

Sys.Mvc.RequiredValidator = function Sys_Mvc_RequiredValidator() {
}
Sys.Mvc.RequiredValidator.create = function Sys_Mvc_RequiredValidator$create(rule) {
    /// <param name="rule" type="Sys.Mvc.JsonValidationRule">
    /// </param>
    /// <returns type="Sys.Mvc.Validator"></returns>
    return Function.createDelegate(new Sys.Mvc.RequiredValidator(), new Sys.Mvc.RequiredValidator().validate);
}
Sys.Mvc.RequiredValidator._isRadioInputElement = function Sys_Mvc_RequiredValidator$_isRadioInputElement(element) {
    /// <param name="element" type="Object" domElement="true">
    /// </param>
    /// <returns type="Boolean"></returns>
    if (element.tagName.toUpperCase() === 'INPUT') {
        var inputType = (element.type).toUpperCase();
        if (inputType === 'RADIO') {
            return true;
        }
    }
    return false;
}
Sys.Mvc.RequiredValidator._isSelectInputElement = function Sys_Mvc_RequiredValidator$_isSelectInputElement(element) {
    /// <param name="element" type="Object" domElement="true">
    /// </param>
    /// <returns type="Boolean"></returns>
    if (element.tagName.toUpperCase() === 'SELECT') {
        return true;
    }
    return false;
}
Sys.Mvc.RequiredValidator._isTextualInputElement = function Sys_Mvc_RequiredValidator$_isTextualInputElement(element) {
    /// <param name="element" type="Object" domElement="true">
    /// </param>
    /// <returns type="Boolean"></returns>
    if (element.tagName.toUpperCase() === 'INPUT') {
        var inputType = (element.type).toUpperCase();
        switch (inputType) {
            case 'TEXT':
            case 'PASSWORD':
            case 'FILE':
                return true;
        }
    }
    if (element.tagName.toUpperCase() === 'TEXTAREA') {
        return true;
    }
    return false;
}
Sys.Mvc.RequiredValidator._validateRadioInput = function Sys_Mvc_RequiredValidator$_validateRadioInput(elements) {
    /// <param name="elements" type="Array" elementType="Object" elementDomElement="true">
    /// </param>
    /// <returns type="Object"></returns>
    for (var i = 0; i < elements.length; i++) {
        var element = elements[i];
        if (element.checked) {
            return true;
        }
    }
    return false;
}
Sys.Mvc.RequiredValidator._validateSelectInput = function Sys_Mvc_RequiredValidator$_validateSelectInput(optionElements) {
    /// <param name="optionElements" type="DOMElementCollection">
    /// </param>
    /// <returns type="Object"></returns>
    for (var i = 0; i < optionElements.length; i++) {
        var element = optionElements[i];
        if (element.selected) {
            if (!Sys.Mvc._validationUtil.stringIsNullOrEmpty(element.value)) {
                return true;
            }
        }
    }
    return false;
}
Sys.Mvc.RequiredValidator._validateTextualInput = function Sys_Mvc_RequiredValidator$_validateTextualInput(element) {
    /// <param name="element" type="Object" domElement="true">
    /// </param>
    /// <returns type="Object"></returns>
    return (!Sys.Mvc._validationUtil.stringIsNullOrEmpty(element.value));
}
Sys.Mvc.RequiredValidator.prototype = {
    
    validate: function Sys_Mvc_RequiredValidator$validate(value, context) {
        /// <param name="value" type="String">
        /// </param>
        /// <param name="context" type="Sys.Mvc.ValidationContext">
        /// </param>
        /// <returns type="Object"></returns>
        var elements = context.fieldContext.elements;
        if (!elements.length) {
            return true;
        }
        var sampleElement = elements[0];
        if (Sys.Mvc.RequiredValidator._isTextualInputElement(sampleElement)) {
            return Sys.Mvc.RequiredValidator._validateTextualInput(sampleElement);
        }
        if (Sys.Mvc.RequiredValidator._isRadioInputElement(sampleElement)) {
            return Sys.Mvc.RequiredValidator._validateRadioInput(elements);
        }
        if (Sys.Mvc.RequiredValidator._isSelectInputElement(sampleElement)) {
            return Sys.Mvc.RequiredValidator._validateSelectInput((sampleElement).options);
        }
        return true;
    }
}


////////////////////////////////////////////////////////////////////////////////
// Sys.Mvc.StringLengthValidator

Sys.Mvc.StringLengthValidator = function Sys_Mvc_StringLengthValidator(minLength, maxLength) {
    /// <param name="minLength" type="Number" integer="true">
    /// </param>
    /// <param name="maxLength" type="Number" integer="true">
    /// </param>
    /// <field name="_maxLength" type="Number" integer="true">
    /// </field>
    /// <field name="_minLength" type="Number" integer="true">
    /// </field>
    this._minLength = minLength;
    this._maxLength = maxLength;
}
Sys.Mvc.StringLengthValidator.create = function Sys_Mvc_StringLengthValidator$create(rule) {
    /// <param name="rule" type="Sys.Mvc.JsonValidationRule">
    /// </param>
    /// <returns type="Sys.Mvc.Validator"></returns>
    var minLength = rule.ValidationParameters['minimumLength'];
    var maxLength = rule.ValidationParameters['maximumLength'];
    return Function.createDelegate(new Sys.Mvc.StringLengthValidator(minLength, maxLength), new Sys.Mvc.StringLengthValidator(minLength, maxLength).validate);
}
Sys.Mvc.StringLengthValidator.prototype = {
    _maxLength: 0,
    _minLength: 0,
    
    validate: function Sys_Mvc_StringLengthValidator$validate(value, context) {
        /// <param name="value" type="String">
        /// </param>
        /// <param name="context" type="Sys.Mvc.ValidationContext">
        /// </param>
        /// <returns type="Object"></returns>
        if (Sys.Mvc._validationUtil.stringIsNullOrEmpty(value)) {
            return true;
        }
        return (this._minLength <= value.length && value.length <= this._maxLength);
    }
}


////////////////////////////////////////////////////////////////////////////////
// Sys.Mvc._validationUtil

Sys.Mvc._validationUtil = function Sys_Mvc__validationUtil() {
}
Sys.Mvc._validationUtil.arrayIsNullOrEmpty = function Sys_Mvc__validationUtil$arrayIsNullOrEmpty(array) {
    /// <param name="array" type="Array" elementType="Object">
    /// </param>
    /// <returns type="Boolean"></returns>
    return (!array || !array.length);
}
Sys.Mvc._validationUtil.stringIsNullOrEmpty = function Sys_Mvc__validationUtil$stringIsNullOrEmpty(value) {
    /// <param name="value" type="String">
    /// </param>
    /// <returns type="Boolean"></returns>
    return (!value || !value.length);
}
Sys.Mvc._validationUtil.elementSupportsEvent = function Sys_Mvc__validationUtil$elementSupportsEvent(element, eventAttributeName) {
    /// <param name="element" type="Object" domElement="true">
    /// </param>
    /// <param name="eventAttributeName" type="String">
    /// </param>
    /// <returns type="Boolean"></returns>
    return (eventAttributeName in element);
}
Sys.Mvc._validationUtil.removeAllChildren = function Sys_Mvc__validationUtil$removeAllChildren(element) {
    /// <param name="element" type="Object" domElement="true">
    /// </param>
    while (element.firstChild) {
        element.removeChild(element.firstChild);
    }
}
Sys.Mvc._validationUtil.setInnerText = function Sys_Mvc__validationUtil$setInnerText(element, innerText) {
    /// <param name="element" type="Object" domElement="true">
    /// </param>
    /// <param name="innerText" type="String">
    /// </param>
    var textNode = document.createTextNode(innerText);
    Sys.Mvc._validationUtil.removeAllChildren(element);
    element.appendChild(textNode);
}


////////////////////////////////////////////////////////////////////////////////
// Sys.Mvc.ValidatorRegistry

Sys.Mvc.ValidatorRegistry = function Sys_Mvc_ValidatorRegistry() {
    /// <field name="validators" type="Object" static="true">
    /// </field>
}
Sys.Mvc.ValidatorRegistry.getValidator = function Sys_Mvc_ValidatorRegistry$getValidator(rule) {
    /// <param name="rule" type="Sys.Mvc.JsonValidationRule">
    /// </param>
    /// <returns type="Sys.Mvc.Validator"></returns>
    var creator = Sys.Mvc.ValidatorRegistry.validators[rule.ValidationType];
    return (creator) ? creator(rule) : null;
}
Sys.Mvc.ValidatorRegistry._getDefaultValidators = function Sys_Mvc_ValidatorRegistry$_getDefaultValidators() {
    /// <returns type="Object"></returns>
    return { required: Function.createDelegate(null, Sys.Mvc.RequiredValidator.create), stringLength: Function.createDelegate(null, Sys.Mvc.StringLengthValidator.create), regularExpression: Function.createDelegate(null, Sys.Mvc.RegularExpressionValidator.create), range: Function.createDelegate(null, Sys.Mvc.RangeValidator.create), number: Function.createDelegate(null, Sys.Mvc.NumberValidator.create) };
}


Sys.Mvc.NumberValidator.registerClass('Sys.Mvc.NumberValidator');
Sys.Mvc.FormContext.registerClass('Sys.Mvc.FormContext');
Sys.Mvc.FieldContext.registerClass('Sys.Mvc.FieldContext');
Sys.Mvc.RangeValidator.registerClass('Sys.Mvc.RangeValidator');
Sys.Mvc.RegularExpressionValidator.registerClass('Sys.Mvc.RegularExpressionValidator');
Sys.Mvc.RequiredValidator.registerClass('Sys.Mvc.RequiredValidator');
Sys.Mvc.StringLengthValidator.registerClass('Sys.Mvc.StringLengthValidator');
Sys.Mvc._validationUtil.registerClass('Sys.Mvc._validationUtil');
Sys.Mvc.ValidatorRegistry.registerClass('Sys.Mvc.ValidatorRegistry');
Sys.Mvc.FormContext._validationSummaryErrorCss = 'validation-summary-errors';
Sys.Mvc.FormContext._validationSummaryValidCss = 'validation-summary-valid';
Sys.Mvc.FormContext._formValidationTag = '__MVC_FormValidation';
Sys.Mvc.FieldContext._hasTextChangedTag = '__MVC_HasTextChanged';
Sys.Mvc.FieldContext._hasValidationFiredTag = '__MVC_HasValidationFired';
Sys.Mvc.FieldContext._inputElementErrorCss = 'input-validation-error';
Sys.Mvc.FieldContext._inputElementValidCss = 'input-validation-valid';
Sys.Mvc.FieldContext._validationMessageErrorCss = 'field-validation-error';
Sys.Mvc.FieldContext._validationMessageValidCss = 'field-validation-valid';
Sys.Mvc.ValidatorRegistry.validators = Sys.Mvc.ValidatorRegistry._getDefaultValidators();

// ---- Do not remove this footer ----
// Generated using Script# v0.5.0.0 (http://projects.nikhilk.net)
// -----------------------------------

// register validation
Sys.Application.add_load(function() {
  Sys.Application.remove_load(arguments.callee);
  Sys.Mvc.FormContext._Application_Load();
});

// SIG // Begin signature block
// SIG // MIIQTAYJKoZIhvcNAQcCoIIQPTCCEDkCAQExCzAJBgUr
// SIG // DgMCGgUAMGcGCisGAQQBgjcCAQSgWTBXMDIGCisGAQQB
// SIG // gjcCAR4wJAIBAQQQEODJBs441BGiowAQS9NQkAIBAAIB
// SIG // AAIBAAIBAAIBADAhMAkGBSsOAwIaBQAEFMRs9xIdlsvt
// SIG // Y+pC/wMHLOQJBwN+oIIODzCCBBMwggNAoAMCAQICEGoL
// SIG // mU/AACKrEdsCQnwC074wCQYFKw4DAh0FADB1MSswKQYD
// SIG // VQQLEyJDb3B5cmlnaHQgKGMpIDE5OTkgTWljcm9zb2Z0
// SIG // IENvcnAuMR4wHAYDVQQLExVNaWNyb3NvZnQgQ29ycG9y
// SIG // YXRpb24xJjAkBgNVBAMTHU1pY3Jvc29mdCBUZXN0IFJv
// SIG // b3QgQXV0aG9yaXR5MB4XDTA2MDYyMjIyNTczMVoXDTEx
// SIG // MDYyMTA3MDAwMFowcTELMAkGA1UEBhMCVVMxEzARBgNV
// SIG // BAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQx
// SIG // HjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEb
// SIG // MBkGA1UEAxMSTWljcm9zb2Z0IFRlc3QgUENBMIIBIjAN
// SIG // BgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAj/Pz33qn
// SIG // cihhfpDzgWdPPEKAs8NyTe9/EGW4StfGTaxnm6+j/cTt
// SIG // fDRsVXNecQkcoKI69WVT1NzP8zOjWjMsV81IIbelJDAx
// SIG // UzWp2tnbdH9MLnhnzdvJ7bGPt67/eW+sIwZUDiNDN3jd
// SIG // Pk4KbdAq9sZ+W5J0DbMTD1yxcbQQ/LEgCAgueW5f0nI0
// SIG // rpI6gbAyrM5DWTCmwfyu+MzofYZrXK7r3pX6Kjl1BlxB
// SIG // OlHcVzVOksssnXuk3Jrp/iGcYR87pEx/UrGFOWR9kYlv
// SIG // nhRCs7yi2moXhyTmG9V8fY+q3ALJoV7d/YEqnybDNkHT
// SIG // z/xzDRx0KDjypQrF0Q+7077QkwIDAQABo4HrMIHoMIGo
// SIG // BgNVHQEEgaAwgZ2AEMBjRdejAX15xXp6XyjbQ9ahdzB1
// SIG // MSswKQYDVQQLEyJDb3B5cmlnaHQgKGMpIDE5OTkgTWlj
// SIG // cm9zb2Z0IENvcnAuMR4wHAYDVQQLExVNaWNyb3NvZnQg
// SIG // Q29ycG9yYXRpb24xJjAkBgNVBAMTHU1pY3Jvc29mdCBU
// SIG // ZXN0IFJvb3QgQXV0aG9yaXR5ghBf6k/S8h1DELboVD7Y
// SIG // lSYYMA8GA1UdEwEB/wQFMAMBAf8wHQYDVR0OBBYEFFSl
// SIG // IUygrm+cYE4Pzt1G1ddh1hesMAsGA1UdDwQEAwIBhjAJ
// SIG // BgUrDgMCHQUAA4HBACzODwWw7h9lGeKjJ7yc936jJard
// SIG // LMfrxQKBMZfJTb9MWDDIJ9WniM6epQ7vmTWM9Q4cLMy2
// SIG // kMGgdc3mffQLETF6g/v+aEzFG5tUqingK125JFP57MGc
// SIG // JYMlQGO3KUIcedPC8cyj+oYwi6tbSpDLRCCQ7MAFS15r
// SIG // 4Dnxn783pZ5nSXh1o+NrSz5mbGusDIj0ujHBCqblI96+
// SIG // Rk7oVQ2DI3oQkSmGQf+BrmRXoJfB3YuXXFc+F88beLHS
// SIG // F0S8oJhPjzCCBKgwggOQoAMCAQICCmEBi3MAAAAAABMw
// SIG // DQYJKoZIhvcNAQEFBQAwcTELMAkGA1UEBhMCVVMxEzAR
// SIG // BgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1v
// SIG // bmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlv
// SIG // bjEbMBkGA1UEAxMSTWljcm9zb2Z0IFRlc3QgUENBMB4X
// SIG // DTA5MDgxNzIzMjAxN1oXDTExMDYyMTA3MDAwMFowgYEx
// SIG // EzARBgoJkiaJk/IsZAEZFgNjb20xGTAXBgoJkiaJk/Is
// SIG // ZAEZFgltaWNyb3NvZnQxFDASBgoJkiaJk/IsZAEZFgRj
// SIG // b3JwMRcwFQYKCZImiZPyLGQBGRYHcmVkbW9uZDEgMB4G
// SIG // A1UEAxMXTVNJVCBUZXN0IENvZGVTaWduIENBIDEwggEi
// SIG // MA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDKz+fW
// SIG // ilZnvB1mb2XQEkuK0GeO6we7n8RfXKMFTp9ifiOnD0v5
// SIG // FFYrPjAKGMrOxroVu8rPTOPukz6hlYdMzIkV68iyS4FU
// SIG // ZjQGz5wQNnLKbUN1PFlP+NsWJZjzvRuZv9WWweCKnUeE
// SIG // Fxur+rzMtvz50aVAechNt36xI6rIxVXRv5xvDKzkKTGv
// SIG // BmaP0YsqkNcUe3GJy17yWoEWX+kKGX69xNezEai06On2
// SIG // cpKToU0ibyRNhgs2Ygzb5U/9hISMYt7YFdEYggL0zTNp
// SIG // 59hmfaB5FT0yMor1iUcSFVtTGObPmB1dsD4EPcYSTZtp
// SIG // 5R4hzYecLp8kSV78s1ycVDt5pQY1AgMBAAGjggEvMIIB
// SIG // KzAQBgkrBgEEAYI3FQEEAwIBADAdBgNVHQ4EFgQUhOTQ
// SIG // p5jIj+9WN5bdvfFGrMW5xZ4wGQYJKwYBBAGCNxQCBAwe
// SIG // CgBTAHUAYgBDAEEwCwYDVR0PBAQDAgGGMA8GA1UdEwEB
// SIG // /wQFMAMBAf8wHwYDVR0jBBgwFoAUVKUhTKCub5xgTg/O
// SIG // 3UbV12HWF6wwTAYDVR0fBEUwQzBBoD+gPYY7aHR0cDov
// SIG // L2NybC5taWNyb3NvZnQuY29tL3BraS9jcmwvcHJvZHVj
// SIG // dHMvbGVnYWN5dGVzdHBjYS5jcmwwUAYIKwYBBQUHAQEE
// SIG // RDBCMEAGCCsGAQUFBzAChjRodHRwOi8vd3d3Lm1pY3Jv
// SIG // c29mdC5jb20vcGtpL2NlcnRzL0xlZ2FjeVRlc3RQQ0Eu
// SIG // Y3J0MA0GCSqGSIb3DQEBBQUAA4IBAQA4GSVNJtByD1os
// SIG // xEzGCLI18ykM+RrR02D1DyopRstCY+OoOeX5WX5BVknd
// SIG // j0w6P1Ea4TD450ozSN7q1yWQcgIT2K8DbwyKTnDn5enx
// SIG // josg2n+ljxnLputPDiFAdfNP+XHew9x/gB+JR7oSK/Ps
// SIG // LzXbuVITIRDkPogIJUFQMrwKI9o0bv2sLWV+fSk+fEXB
// SIG // OaHysKBGU+EIhjrfHx4QP38jQUi2yJZQ85klqVVuSL21
// SIG // dwIP5QZiYplN6zicK6ez3r+yozQLOg6mc5MBgrUPBTsV
// SIG // sSbHM2+BGVaorOyI7JMr0sHBl6IGFbqRIPqtWY4rimD8
// SIG // uNi6hHfLJFmTMDstbNJEMIIFSDCCBDCgAwIBAgIKa4DO
// SIG // qQAAAACmmDANBgkqhkiG9w0BAQUFADCBgTETMBEGCgmS
// SIG // JomT8ixkARkWA2NvbTEZMBcGCgmSJomT8ixkARkWCW1p
// SIG // Y3Jvc29mdDEUMBIGCgmSJomT8ixkARkWBGNvcnAxFzAV
// SIG // BgoJkiaJk/IsZAEZFgdyZWRtb25kMSAwHgYDVQQDExdN
// SIG // U0lUIFRlc3QgQ29kZVNpZ24gQ0EgMTAeFw0wOTExMDYx
// SIG // ODE3MDdaFw0xMDExMDYxODE3MDdaMBUxEzARBgNVBAMT
// SIG // ClZTIEJsZCBMYWIwgZ8wDQYJKoZIhvcNAQEBBQADgY0A
// SIG // MIGJAoGBAJMiPNeJy8vp5oeABJLebUDw5LUKy+N3pOFp
// SIG // h5QGJmE4b4JgN2LEXNVLh6lOle35xLCbQOJCVs1eDOgq
// SIG // puOWq5EvFYOugrxGcS4wfHNt4/Rwjigo/UQDYU755puL
// SIG // RBqLVtGqlcMYwLhzAWV0R7HWtmBDfhqAH19O3P3foI2X
// SIG // zrLrAgMBAAGjggKvMIICqzALBgNVHQ8EBAMCB4AwHQYD
// SIG // VR0OBBYEFAjpDmzPyPih2x+qdItA5Ul2ZAe3MD0GCSsG
// SIG // AQQBgjcVBwQwMC4GJisGAQQBgjcVCIPPiU2t8gKFoZ8M
// SIG // gvrKfYHh+3SBT4KusGqH9P0yAgFkAgEMMB8GA1UdIwQY
// SIG // MBaAFITk0KeYyI/vVjeW3b3xRqzFucWeMIHoBgNVHR8E
// SIG // geAwgd0wgdqggdeggdSGNmh0dHA6Ly9jb3JwcGtpL2Ny
// SIG // bC9NU0lUJTIwVGVzdCUyMENvZGVTaWduJTIwQ0ElMjAx
// SIG // LmNybIZNaHR0cDovL21zY3JsLm1pY3Jvc29mdC5jb20v
// SIG // cGtpL21zY29ycC9jcmwvTVNJVCUyMFRlc3QlMjBDb2Rl
// SIG // U2lnbiUyMENBJTIwMS5jcmyGS2h0dHA6Ly9jcmwubWlj
// SIG // cm9zb2Z0LmNvbS9wa2kvbXNjb3JwL2NybC9NU0lUJTIw
// SIG // VGVzdCUyMENvZGVTaWduJTIwQ0ElMjAxLmNybDCBqQYI
// SIG // KwYBBQUHAQEEgZwwgZkwQgYIKwYBBQUHMAKGNmh0dHA6
// SIG // Ly9jb3JwcGtpL2FpYS9NU0lUJTIwVGVzdCUyMENvZGVT
// SIG // aWduJTIwQ0ElMjAxLmNydDBTBggrBgEFBQcwAoZHaHR0
// SIG // cDovL3d3dy5taWNyb3NvZnQuY29tL3BraS9tc2NvcnAv
// SIG // TVNJVCUyMFRlc3QlMjBDb2RlU2lnbiUyMENBJTIwMS5j
// SIG // cnQwHwYDVR0lBBgwFgYKKwYBBAGCNwoDBgYIKwYBBQUH
// SIG // AwMwKQYJKwYBBAGCNxUKBBwwGjAMBgorBgEEAYI3CgMG
// SIG // MAoGCCsGAQUFBwMDMDoGA1UdEQQzMDGgLwYKKwYBBAGC
// SIG // NxQCA6AhDB9kbGFiQHJlZG1vbmQuY29ycC5taWNyb3Nv
// SIG // ZnQuY29tMA0GCSqGSIb3DQEBBQUAA4IBAQBqcp669vuu
// SIG // QzcKv0NTjeY2jhqSYRlwon/Q83ON8GCb1vf3AEFmwPNI
// SIG // 5hxSmGpqr4JrfuJFFa6SxO8praB4oaZeTKt7bAH/uRpb
// SIG // HP3U8Y6tuJAzfWaYUiNoF02lpgFEa44pw3sGJ3XA6uj0
// SIG // cG4jo1U5b81pkFblA4WRIuU1VHUDmARJbinQVt3JAFyU
// SIG // /J4SuAMUxraGUS8voUpk/Jyy8A7dhNepQQmc8BlY6lIQ
// SIG // fyU6WYQhOSuuQO5mfZhJaFGA53gqWzJfVBD32i7O6lAt
// SIG // /SXE7oV+Fwo5FHC8dOMzIn4bITvDQxgfO0M530uBmnCY
// SIG // qsRRYNNgYql6JvUjP/DSy6ZfMYIBqTCCAaUCAQEwgZAw
// SIG // gYExEzARBgoJkiaJk/IsZAEZFgNjb20xGTAXBgoJkiaJ
// SIG // k/IsZAEZFgltaWNyb3NvZnQxFDASBgoJkiaJk/IsZAEZ
// SIG // FgRjb3JwMRcwFQYKCZImiZPyLGQBGRYHcmVkbW9uZDEg
// SIG // MB4GA1UEAxMXTVNJVCBUZXN0IENvZGVTaWduIENBIDEC
// SIG // CmuAzqkAAAAAppgwCQYFKw4DAhoFAKBwMBAGCisGAQQB
// SIG // gjcCAQwxAjAAMBkGCSqGSIb3DQEJAzEMBgorBgEEAYI3
// SIG // AgEEMBwGCisGAQQBgjcCAQsxDjAMBgorBgEEAYI3AgEV
// SIG // MCMGCSqGSIb3DQEJBDEWBBQkLSwhHGzDhiC1AXY6E0vX
// SIG // xiN88TANBgkqhkiG9w0BAQEFAASBgAmKfmzzpF8qnH2r
// SIG // o5yfgetrkSv201rtCyr9iECGVCfaAZa8CAOyI59SyGb8
// SIG // 5mE/kGWplK0ZQGZKD99oUXC/KZ+Mug4Wn9XJuOOJN632
// SIG // PE0vLIcGr4lsi5TL4KVOTGWNOOH6dOecGYjEfykwp7Oe
// SIG // 7PyCLYxF0zfGnNgCpN0eY6to
// SIG // End signature block
