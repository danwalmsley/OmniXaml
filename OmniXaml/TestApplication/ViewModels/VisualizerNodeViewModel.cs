namespace TestApplication.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using OmniXaml;
    using OmniXaml.Tests;
    using OmniXaml.Typing;

    public class VisualizerNodeViewModel : ViewModel
    {
        private readonly Node model;
        private bool isExpanded;
        private readonly List<VisualizerNodeViewModel> children;

        public VisualizerNodeViewModel(Node model)
        {
            this.model = model;
            this.IsExpanded = true;
            children = model.Children.Select(node => new VisualizerNodeViewModel(node)).ToList();

            CollapseBranchCommand = new RelayCommand(o => CollapseChildren());
            ExpandBranchCommand = new RelayCommand(o => ExpandChildren());
        }

        private void ExpandChildren()
        {
            SetExpanded(true);
        }

        private void CollapseChildren()
        {
            SetExpanded(false);
        }

        private void SetExpanded(bool value)
        {
            IsExpanded = value;
            foreach (var visualizerNodeViewModel in Children)
            {
                visualizerNodeViewModel.SetExpanded(value);
            }
        }

        public string Name => GetName(model);

        private string GetName(Node node)
        {
            switch (NodeType)
            {
                case NodeType.Member:
                    return GetMemberName(node);

               case NodeType.Object:
                    return node.XamlNode.XamlType.Name;

                case NodeType.GetObject:
                    return "Collection";

                case NodeType.NamespaceDeclaration:
                    return "Namespace Declaration: Mapping " + node.XamlNode.NamespaceDeclaration;

                case NodeType.Value:
                    return $"\"{node.XamlNode.Value}\"";

                case NodeType.Root:
                    return "Root";
            }

            throw new InvalidOperationException("The node type {NodeType} cannot be handled.");
        }

        private static string GetMemberName(Node node)
        {
            var xamlMember = node.XamlNode.Member;

            if (!xamlMember.IsAttachable)
            {
                return xamlMember.Name;
            }

            return xamlMember.DeclaringType.Name + "." + xamlMember.Name;
        }

        public IEnumerable<VisualizerNodeViewModel> Children => children;

        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                OnPropertyChanged();
            }
        }

        private static NodeType GetNodeType(XamlNode current)
        {
            switch (current.NodeType)
            {
                case XamlNodeType.StartMember:
                case XamlNodeType.EndMember:
                    return NodeType.Member;

                case XamlNodeType.StartObject:
                case XamlNodeType.EndObject:
                    return NodeType.Object;

                case XamlNodeType.GetObject:
                    return NodeType.GetObject;

                case XamlNodeType.NamespaceDeclaration:
                    return NodeType.NamespaceDeclaration;

                case XamlNodeType.Value:
                    return NodeType.Value;

                case XamlNodeType.None:
                    return NodeType.Root;
            }

            throw new InvalidOperationException("Cannont translate the type");
        }

        public NodeType NodeType => GetNodeType(model.XamlNode);

        public ICommand CollapseBranchCommand { get; private set; }
        public ICommand ExpandBranchCommand { get; private set; }
    }
}