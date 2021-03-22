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
                    builder.AddAttribute(sequence++, "class", "form-generator-header");

                    builder.OpenElement(sequence++, "span");
                    builder.AddAttribute(sequence++, "class", "form-generator-title");
                    builder.AddContent(sequence++, $"{property.Name} ({list.Count})");
                    builder.CloseComponent();

                    builder.OpenElement(sequence++, "span");
                    builder.AddAttribute(sequence++, "class", "form-generator-add-button");
                    builder.AddAttribute(sequence++, "onclick",
                       EventCallback.Factory.Create<MouseEventArgs>(this, value =>
                       {
                           var element = this.Instantiate(elementType);
                           list.Add(element);
                           this.OnChanged();
                       }));
                    builder.OpenElement(sequence++, "i");
                    builder.AddAttribute(sequence++, "class", "fas fa-plus");
                    builder.CloseElement();
                    builder.CloseElement();

                    builder.CloseElement();

                    builder.OpenElement(sequence++, "div");
                    builder.AddAttribute(sequence++, "class", "form-generator-content");

                    if (list.Count > 0)
                    {
                        var index = 0;

                        foreach (var item in list)
                        {
                            builder.OpenElement(sequence++, "div");
                            builder.AddAttribute(sequence++, "class", "form-generator-list-item");

                            if (elementType.IsPrimitive || elementType.IsEnum || elementType == typeof(string) || elementType == typeof(decimal))
                            {
                                var localIndex = index;

                                Action<object> valueChanged = newValue =>
                                {
                                    list[localIndex] = newValue;
                                    this.OnChanged();
                                };

                                var onKeyDown = EventCallback.Factory.Create<KeyboardEventArgs>(this, e =>
                                {
                                    if (e.Key == "Delete")
                                    {
                                        list.RemoveAt(localIndex);
                                        this.OnChanged();
                                    }
                                });

                                sequence = (int)OneDasUtilities.InvokeGenericMethod(
                                    this.GetType(),
                                    this,
                                    nameof(AddGenericPrimitive),
                                    BindingFlags.NonPublic | BindingFlags.Instance,
                                    elementType,
                                    new object[] { builder, sequence, "Value", item, valueChanged, onKeyDown });
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
                                builder.AddAttribute(sequence++, "class", "form-generator-remove-button");
                                builder.AddAttribute(sequence++, "onclick",
                                   EventCallback.Factory.Create<MouseEventArgs>(this, e =>
                                   {
                                       var element = this.Instantiate(elementType);
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
                // Dictionary
                else if (value is IDictionary dict)
                {
                    var elementTypes = dict.GetType().GetGenericArguments();

                    if (!(elementTypes[0].IsPrimitive || elementTypes[0].IsEnum || elementTypes[0] == typeof(string) || elementTypes[0] == typeof(decimal)))
                        throw new Exception("Only primitive keys are supported.");

                    builder.OpenElement(sequence++, "div");
                    builder.AddAttribute(sequence++, "class", "form-generator-dict");

                    builder.OpenElement(sequence++, "div");
                    builder.AddAttribute(sequence++, "class", "form-generator-header");

                    builder.OpenElement(sequence++, "span");
                    builder.AddAttribute(sequence++, "class", "form-generator-title");
                    builder.AddContent(sequence++, $"{property.Name} ({dict.Count})");
                    builder.CloseComponent();

                    builder.OpenElement(sequence++, "span");
                    builder.AddAttribute(sequence++, "class", "form-generator-add-button");
                    builder.AddAttribute(sequence++, "onclick",
                       EventCallback.Factory.Create<MouseEventArgs>(this, value =>
                       {
                           var dictKey = this.Instantiate(elementTypes[0]);
                           var dictValue = this.Instantiate(elementTypes[1]);
                           dict[dictKey] = dictValue;
                           this.OnChanged();
                       }));
                    builder.OpenElement(sequence++, "i");
                    builder.AddAttribute(sequence++, "class", "fas fa-plus");
                    builder.CloseElement();
                    builder.CloseElement();

                    builder.CloseElement();

                    builder.OpenElement(sequence++, "div");
                    builder.AddAttribute(sequence++, "class", "form-generator-content");

                    if (dict.Count > 0)
                    {
                        foreach (var key in dict.Keys)
                        {
                            builder.OpenElement(sequence++, "div");
                            builder.AddAttribute(sequence++, "class", "form-generator-dict-entry");

                            // key
                            Action<object> valueChanged = newKey =>
                            {
                                dict.Remove(key);
                                dict[newKey] = this.Instantiate(elementTypes[1]);
                                this.OnChanged();
                            };

                            var onKeyDown = EventCallback.Factory.Create<KeyboardEventArgs>(this, e =>
                            {
                                if (e.Key == "Delete")
                                {
                                    dict.Remove(key);
                                    this.OnChanged();
                                }
                            });

                            sequence = (int)OneDasUtilities.InvokeGenericMethod(
                                   this.GetType(),
                                   this,
                                   nameof(AddGenericPrimitive),
                                   BindingFlags.NonPublic | BindingFlags.Instance,
                                   elementTypes[0],
                                   new object[] { builder, sequence, "Key", key, valueChanged, onKeyDown });

                            // space
                            builder.OpenElement(sequence++, "span");
                            builder.AddContent(sequence++, " ");
                            builder.CloseElement();

                            // value
                            if (elementTypes[1].IsPrimitive || elementTypes[1].IsEnum || elementTypes[1] == typeof(string) || elementTypes[1] == typeof(decimal))
                            {
                                Action<object> valueChanged2 = newValue =>
                                {
                                    dict[key] = newValue;
                                    this.OnChanged();
                                };

                                sequence = (int)OneDasUtilities.InvokeGenericMethod(
                                    this.GetType(),
                                    this,
                                    nameof(AddGenericPrimitive),
                                    BindingFlags.NonPublic | BindingFlags.Instance,
                                    elementTypes[1],
                                    new object[] { builder, sequence, "Value", dict[key], valueChanged2, null });
                            }
                            else
                            {
                                builder.OpenComponent(sequence++, typeof(FormGenerator));
                                builder.AddAttribute(sequence++, "DataContext", dict[key]);
                                builder.AddAttribute(sequence++, "Changed",
                                    EventCallback.Factory.Create(this, value => this.OnChanged()));
                                builder.CloseComponent();
                            }

                            builder.CloseElement();
                        }
                    }

                    builder.CloseElement();
                    builder.CloseElement();
                }

                builder.CloseElement();
            }

            builder.CloseComponent();
        };

        private int AddGenericPrimitive<T>(RenderTreeBuilder builder, int sequence, string label, T value, Action<object> valueChanged, EventCallback<KeyboardEventArgs> onKeyDown)
        {
            if (typeof(T).IsEnum)
            {
                builder.OpenComponent(sequence++, typeof(MatSelectItem<T>));
                builder.AddAttribute(sequence++, "Items", Enum.GetValues(typeof(T)));
            }
            else
            {
                builder.OpenComponent(sequence++, typeof(MatTextField<T>));
            }

            builder.AddAttribute(sequence++, "Label", label);
            builder.AddAttribute(sequence++, "Value", value);

            builder.AddAttribute(sequence++, "ValueChanged", EventCallback.Factory.Create<T>(this, newValue =>
            {
                valueChanged?.Invoke(newValue);
            }));

            if (!onKeyDown.Equals(default))
                builder.AddAttribute(sequence++, "OnKeyDown", onKeyDown);

            builder.CloseComponent();

            return sequence;
        }

        private object Instantiate(Type type)
        {
            if (type == typeof(string))
                return string.Empty;

            else
                return Activator.CreateInstance(type);
        }

        private void OnChanged()
        {
            this.Changed.InvokeAsync();
        }
    }
}
