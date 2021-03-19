using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OneDas.DataManagement.Explorer.Shared
{
    public partial class FormGenerator : ComponentBase
    {
        [Parameter]
        public object DataContext { get; set; }

        [Parameter]
        public EventCallback Changed { get; set; }

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

                var type = this.DataContext.GetType();
                var constant = Expression.Constant(this.DataContext, type);
                var expression = MemberExpression.Property(constant, property.Name);
                var value = property.GetValue(this.DataContext);

                if (property.PropertyType == typeof(string))
                {
                    builder.OpenComponent(sequence++, typeof(MatTextField<string>));
                    builder.AddAttribute(sequence++, "Value", value);

                    builder.AddAttribute(sequence++, "ValueChanged",
                        EventCallback.Factory.Create<string>(this, value =>
                        {
                            property.SetValue(this.DataContext, value);
                            this.OnChanged();
                        }));

                    builder.AddAttribute(sequence++, "Label", property.Name);
                    builder.CloseComponent();
                }
                else if (property.PropertyType == typeof(bool))
                {
                    builder.OpenComponent(sequence++, typeof(MatCheckbox<bool>));
                    builder.AddAttribute(sequence++, "Value", value);

                    builder.AddAttribute(sequence++, "ValueChanged",
                        EventCallback.Factory.Create<bool>(this, value => 
                        {
                            property.SetValue(this.DataContext, value);
                            this.OnChanged();
                        }));

                    builder.AddAttribute(sequence++, "Label", property.Name);
                    builder.CloseComponent();
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    builder.OpenComponent(sequence++, typeof(MatDatePicker<DateTime>));
                    builder.AddAttribute(sequence++, "Value", value);

                    builder.AddAttribute(sequence++, "ValueChanged",
                        EventCallback.Factory.Create<DateTime>(this, value => 
                        {
                            property.SetValue(this.DataContext, value);
                            this.OnChanged();
                        }));

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
                    var elementType = list.GetType().GetGenericArguments().Single();

                    builder.OpenElement(sequence++, "div");
                    builder.AddAttribute(sequence++, "class", "form-generator-list");

                    builder.OpenElement(sequence++, "div");
                    builder.AddAttribute(sequence++, "class", "form-generator-list-header");

                    builder.OpenElement(sequence++, "span");
                    builder.AddAttribute(sequence++, "class", "form-generator-list-title");
                    builder.AddContent(sequence++, $"{property.Name} ({list.Count})");
                    builder.CloseComponent();

                    builder.OpenElement(sequence++, "span");
                    builder.AddAttribute(sequence++, "class", "form-generator-list-add-button");
                    builder.AddAttribute(sequence++, "onclick",
                       EventCallback.Factory.Create<MouseEventArgs>(this, value =>
                       {
                           var element = Activator.CreateInstance(elementType);
                           list.Add(element);
                           this.OnChanged();
                       }));
                    builder.OpenElement(sequence++, "i");
                    builder.AddAttribute(sequence++, "class", "fas fa-plus");
                    builder.CloseElement();
                    builder.CloseElement();

                    builder.CloseElement();

                    builder.OpenElement(sequence++, "div");
                    builder.AddAttribute(sequence++, "class", "form-generator-list-content");

                    if (list.Count > 0)
                    {
                        var index = 0;

                        foreach (var item in list)
                        {
                            builder.OpenElement(sequence++, "div");
                            builder.AddAttribute(sequence++, "class", "form-generator-list-item");

                            if (elementType.IsPrimitive || elementType.IsEnum || elementType == typeof(string) || elementType == typeof(decimal))
                            {
                                sequence = (int)OneDasUtilities.InvokeGenericMethod(
                                    this.GetType(),
                                    this,
                                    nameof(AddGeneric),
                                    BindingFlags.NonPublic | BindingFlags.Instance,
                                    elementType,
                                    new object[] { builder, sequence, list, index, item });
                            }
                            else
                            {
                                builder.OpenComponent(sequence++, typeof(FormGenerator));
                                builder.AddAttribute(sequence++, "DataContext", item);
                                builder.AddAttribute(sequence++, "Changed",
                                    EventCallback.Factory.Create(this, value => this.OnChanged()));
                                builder.CloseComponent();

                                var localIndex = index;
                                builder.OpenElement(sequence++, "span");
                                builder.AddAttribute(sequence++, "class", "form-generator-list-item-remove-button");
                                builder.AddAttribute(sequence++, "onclick",
                                   EventCallback.Factory.Create<MouseEventArgs>(this, value =>
                                   {
                                       var element = Activator.CreateInstance(elementType);
                                       list.RemoveAt(localIndex);
                                       this.OnChanged();
                                   }));
                                builder.OpenElement(sequence++, "i");
                                builder.AddAttribute(sequence++, "class", "fas fa-trash-alt");
                                builder.CloseElement();
                                builder.CloseElement();
                            }

                            builder.CloseElement();

                            index++;
                        }
                    }

                    builder.CloseElement();
                    builder.CloseElement();
                }

                builder.CloseElement();
            }

            builder.CloseComponent();
        };

        private int AddGeneric<T>(RenderTreeBuilder builder, int sequence, IList list, int index, object value)
        {
            builder.OpenComponent(sequence++, typeof(MatTextField<T>));
            builder.AddAttribute(sequence++, "Value", value);

            builder.AddAttribute(sequence++, "ValueChanged",
                EventCallback.Factory.Create<T>(this, value =>
                {
                    list[index] = value;
                    this.OnChanged();
                }));

            builder.AddAttribute(sequence++, "OnKeyDown",
               EventCallback.Factory.Create<KeyboardEventArgs>(this, e =>
               {
                   if (e.Key == "Delete")
                   {
                       list.RemoveAt(index);
                       this.OnChanged();
                   }
               }));

            builder.CloseComponent();

            return sequence;
        }

        private void OnChanged()
        {
            this.Changed.InvokeAsync();
        }
    }
}
