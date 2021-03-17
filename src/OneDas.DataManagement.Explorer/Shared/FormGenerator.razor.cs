using MatBlazor;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class FormGenerator : ComponentBase
    {
        [Parameter]
        public object DataContext { get; set; }

        private RenderFragment CreateComponent() => builder =>
        {
            var properties = this.DataContext.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var sequence = 0; // Autoincrement is not recommended for performance reasons (https://docs.microsoft.com/en-us/aspnet/core/blazor/advanced-scenarios?view=aspnetcore-5.0#the-problem-with-generating-sequence-numbers-programmatically)

            builder.OpenElement(sequence++, "div");
            builder.AddAttribute(sequence++, "class", "form-generator");

            foreach (var property in properties)
            {
                builder.OpenElement(sequence++, "div");
                builder.AddAttribute(sequence++, "class", "form-generator-property");

                var constant = Expression.Constant(this.DataContext, this.DataContext.GetType());
                var expression = MemberExpression.Property(constant, property.Name);
                var value = property.GetValue(this.DataContext);

                if (property.PropertyType == typeof(string))
                {
                    builder.OpenComponent(sequence++, typeof(MatTextField<string>));
                    builder.AddAttribute(sequence++, "Value", value);

                    builder.AddAttribute(sequence++, "ValueChanged",
                        EventCallback.Factory.CreateInferred(this, _value => property.SetValue(this.DataContext, _value), (string)value));

                    builder.AddAttribute(sequence++, "ValueExpression",
                        Expression.Lambda<Func<string>>(expression));

                    builder.AddAttribute(sequence++, "Label", property.Name);

                    builder.CloseComponent();
                }
                else if (property.PropertyType == typeof(bool))
                {
                    builder.OpenComponent(sequence++, typeof(MatCheckbox<bool>));
                    builder.AddAttribute(sequence++, "Value", value);

                    builder.AddAttribute(sequence++, "ValueChanged",
                        EventCallback.Factory.CreateInferred(this, _value => property.SetValue(this.DataContext, _value), (bool)value));

                    builder.AddAttribute(sequence++, "ValueExpression",
                        Expression.Lambda<Func<bool>>(expression));

                    builder.AddAttribute(sequence++, "Label", property.Name);

                    builder.CloseComponent();
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    builder.OpenComponent(sequence++, typeof(MatDatePicker<DateTime>));
                    builder.AddAttribute(sequence++, "Value", value);

                    builder.AddAttribute(sequence++, "ValueChanged",
                        EventCallback.Factory.CreateInferred(this, _value => property.SetValue(this.DataContext, _value), (DateTime)value));

                    builder.AddAttribute(sequence++, "ValueExpression",
                        Expression.Lambda<Func<DateTime>>(expression));

                    builder.AddAttribute(sequence++, "Label", property.Name);

                    builder.AddAttribute(sequence++, "Format", "dd.MM.yyyy HH:mm");
                    builder.AddAttribute(sequence++, "EnableTime", true);
                    builder.AddAttribute(sequence++, "EnableSeconds", false);
                    builder.AddAttribute(sequence++, "Enable24hours", true);
                    builder.AddAttribute(sequence++, "AllowInput", true);

                    builder.CloseComponent();
                }
                // List
                else if (value is IList list)
                {
                    builder.OpenElement(sequence++, "div");
                    builder.AddAttribute(sequence++, "class", "form-generator-list");

                    if (list.Count == 0)
                    {
                        builder.OpenElement(sequence++, "span");
                        builder.AddContent(sequence++, $"There are no {property.Name} yet.");
                        builder.CloseComponent();
                    }
                    else
                    {
                        foreach (var item in list)
                        {
                            builder.OpenElement(sequence++, "div");
                            builder.AddAttribute(sequence++, "class", "form-generator-list-item");

                            builder.OpenComponent(sequence++, typeof(FormGenerator));
                            builder.AddAttribute(sequence++, "DataContext", item);
                            builder.CloseComponent();

                            builder.CloseElement();
                        }
                    }

                    builder.OpenComponent(sequence++, typeof(MatIconButton));
                    builder.AddAttribute(sequence++, "Icon", "add");
                    builder.CloseComponent();

                    builder.CloseElement();
                }

                builder.CloseElement();
            }

            builder.CloseComponent();
        };
    }
}
