using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trawick.Common.Filters
{
	public class RequiredIfAttribute : ValidationAttribute
	{
		RequiredAttribute _innerAttribute = new RequiredAttribute();
		public string _dependentProperty { get; set; }
		public object _targetValue { get; set; }
		public string _message { get; set; }

		public RequiredIfAttribute(string dependentProperty, object targetValue, string message = null)
		{
			this._dependentProperty = dependentProperty;
			this._targetValue = targetValue;
			this._message = message;
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var field = validationContext.ObjectType.GetProperty(_dependentProperty);
			if (field != null)
			{
				var dependentValue = field.GetValue(validationContext.ObjectInstance, null);
				if ((dependentValue == null && _targetValue == null) || (dependentValue.Equals(_targetValue)))
				{
					if (!_innerAttribute.IsValid(value))
					{
						if (!string.IsNullOrEmpty(this._message))
						{
							return new ValidationResult(ErrorMessage = this._message);
						}
						string name = validationContext.DisplayName;
						return new ValidationResult(ErrorMessage = name + " Is required.");
					}
				}
				return ValidationResult.Success;
			}
			else
			{
				return new ValidationResult(FormatErrorMessage(_dependentProperty));
			}
		}
	}
}
