﻿namespace Zat.SystemTest.RunnerApp.Entities.TestExplorer;

using System.Windows;

using DevKit.Wpf.Extensions;

using Zat.SystemTest.RunnerApp.Entities.ConfigEditor;
using Zat.SystemTest.RunnerApp.Views;
using Zat.SystemTest.RunnerApp.Views.Main;
using Zat.SystemTest.Wpf.Controls;
using Zat.SystemTest.Wpf.UIStateTracking;

// TODO: Rename ExitState to CurrentState?
// TODO: ExitState class - have inner separate states for controls in own properties (eg. ExitState.TestExplorer, ExitState.ConfigEditor)

/// <summary>
/// Interaction logic for TestExplorer.xaml.
/// </summary>
public partial class TestExplorerControl : IUIStateTrackable
{
    private const string TestModuleComboBoxStateKey = nameof(TestModuleComboBoxStateKey);
    private const string ExplorationTreeViewItemStateKey = nameof(ExplorationTreeViewItemStateKey);

    /// <summary>
    /// Initializes a new instance of the <see cref="TestExplorerControl"/> class.
    /// </summary>
    public TestExplorerControl()
    {
        this.InitializeComponent();
        this.Loaded += this.OnLoaded;
    }

    /// <inheritdoc />
    public void InitState()
    {
        this.TestModuleComboBox.SelectedIndex = AppStateManager
            .Instance.ExitState.TestExplorerComboBoxItemIndex;
        this.ExplorationTreeView.TreeViewNodeStates = AppStateManager
            .Instance.ExitState.TestExplorerTreeViewNodeStates;
        this.ExplorationTreeView
                .GetChild(AppStateManager
            .Instance.ExitState.TestExplorerTreeViewIndexedPath)?
                .Button
                .PerformClick(); 
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        this.Loaded -= this.OnLoaded;
        
        var configEditor = this
            .FindFirstParent<MainView>()?
            .FindFirDescendant<ConfigEditorControl>();

        this.TestModuleComboBox.SelectionChanged +=
            (_, _) =>``
            {
                var selectedIndex = this.TestModuleComboBox.SelectedIndex;
                AppStateManager.Instance.ExitState.TestExplorerComboBoxItemIndex = selectedIndex;
                AppStateManager.Instance.SetState(
                    StateKeys.DeriveCurrentKey(TestModuleComboBoxStateKey),
                    new ComboBoxState(selectedIndex, ExitStateKey));

                //// TODO: Event handler pro button click získá state s tímto klíčem a na základě něho sestaví klíč pro získání stavu NodeStates
            };

        AppStateManager.Instance.StateChanged += (key, state) =>
        {
            if (!(key == TestModuleComboBoxStateKey && state is ComboBoxState comboBoxState))
            {
                return;
            }

            var treeViewItemStateKey = MakeExplorationTreeViewItemStateKey(comboBoxState.SelectedItemIndex);
            var treeViewItemState = AppStateManager.Instance.GetState<TreeViewItemState>(treeViewItemStateKey);
            if (treeViewItemState is null)
            {
                return;
            }

            // TODO: Set treeViewItemState as exit state

            this.ExplorationTreeView
                .GetChild(treeViewItemState.IndexedPath)?
                .Button
                .PerformClick();
        };

        // TODO:
        // Attach event hanlder to all nodes of testExplorer.ExplorationTreeView to track their selection state
        // But we have to attach the event every time when the tree rerenders + dispose the old handlers
        // For every button it should be possible to get indexed path of its tree view item parent
        // Maybe there could be an event ItemButtonClicked on the TreeView control?
        // WE HAVE TO ATTACH HANDLERS TO BUTTONS AFTER THE TREE RERENDERS!!!
        foreach (var treeViewItem in this.ExplorationTreeView.GetDescendants<TreeViewItem>())
        {
            void OnClick(object sender, RoutedEventArgs args)
            {
                var indexedPath = treeViewItem.GetIndexedPath(); // TODO: Or take the indexedPath fron AppStateManager.Instance.ExitState.TestExplorer.ExplorationTreeViewIndexedPath;
                AppStateManager.Instance.ExitState.TestExplorerTreeViewNodeStates = new TreeViewNodeStates(indexedPath);
                // TODO: We have to set state for indexedPath for selectedItemIndex using MakeExplorationTreeViewItemStateKey
                configEditor.UpdateState(); // TODO: UpdateState will set NodeStates to property tree view ... Or tigger state update by StateChanged event?
            }

            var selectedIndex = this.TestModuleComboBox.SelectedIndex;

            treeViewItem.Button.Click += OnClick;
            treeViewItem.Unloaded += (_, _) => treeViewItem.Button.Click -= OnClick;
        }
    }

    private static string MakeExplorationTreeViewItemStateKey(
        int testModuleComboBoxSelectedIndex)
        => $"{ExplorationTreeViewItemStateKey}|{testModuleComboBoxSelectedIndex}";
}