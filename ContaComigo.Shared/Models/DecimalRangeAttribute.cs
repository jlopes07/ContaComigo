using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ContaComigo.Shared.Models
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class DecimalRangeAttribute : RangeAttribute
    {
        public DecimalRangeAttribute(double minimum, double maximum) : base(minimum, maximum)
        {
        }

        public DecimalRangeAttribute(Type type, string minimum, string maximum) : base(type, minimum, maximum)
        {
        }

        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                return true; // Requer um RequiredAttribute para nulos
            }

            if (value is decimal decimalValue)
            {
                return decimalValue >= Convert.ToDecimal(Minimum) && decimalValue <= Convert.ToDecimal(Maximum);
            }

            // Fallback para tipos de base ou outros tipos, se necessário.
            // O RangeAttribute padrão já lida com double, int, etc.
            return base.IsValid(value);
        }

        public override string FormatErrorMessage(string name)
        {
            // Isso pode ser útil para mensagens de erro personalizadas que incluam os valores min/max.
            // Para manter a compatibilidade com a mensagem padrão de RangeAttribute, podemos usar base.
            return base.FormatErrorMessage(name);
        }
    }
}