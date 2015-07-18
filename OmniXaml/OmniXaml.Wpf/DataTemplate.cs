﻿namespace OmniXaml.Wpf
{
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;

    [ContentProperty("AlternateTemplateContent")]
    public class DataTemplate : System.Windows.DataTemplate
    {
        private TemplateContent alternateTemplateContent;

        // ReSharper disable once EmptyConstructor
        public DataTemplate()
        {
        }

        public TemplateContent AlternateTemplateContent
        {
            get { return alternateTemplateContent; }
            set
            {
                alternateTemplateContent = value;
                VisualTree = ConvertTemplateContentIntoElementFactory(value);
            }
        }

        private static FrameworkElementFactory ConvertTemplateContentIntoElementFactory(TemplateContent templateContent)
        {
            var visualTree = (DependencyObject)templateContent.Load();
            var frameworkElementFactory = new FrameworkElementFactory(visualTree.GetType());

            foreach (var dependencyProperty in DependencyObjectHelper.GetDependencyProperties(visualTree))
            {
                var binding = BindingOperations.GetBinding(visualTree, dependencyProperty);

                var value = visualTree.GetValue(dependencyProperty);
                if (binding != null)
                {
                    value = binding;
                }
                
                frameworkElementFactory.SetValue(dependencyProperty, value);
            }

            return frameworkElementFactory;
        }
    }
}