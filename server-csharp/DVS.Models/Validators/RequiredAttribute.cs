using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DVS.Models.Validators
{
    public class RequiredAttribute : ValidationAttribute
    {
        public RequiredAttribute()
        {
            base.ErrorMessage = "必填项";
        }
        public RequiredAttribute(string ErrorMessage) {
            base.ErrorMessage = ErrorMessage;
        }
        public override bool IsValid(object value)
        {
            // return base.IsValid(value);

            if (value == null)
            {
                return false;
            }

            if (value.GetType() == typeof(string) && string.IsNullOrEmpty(value.ToString()))
            {
                return false;
            }
            return true;
        }
    }
}
