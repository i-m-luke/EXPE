namespace Zat.SystemTest.RunnerApp.Views.Main;

using System.Windows;

using DevKit.Wpf.Extensions;

using Zat.SystemTest.RunnerApp.Entities.ConfigEditor;
using Zat.SystemTest.RunnerApp.Entities.TestExplorer;
using Zat.SystemTest.Wpf.Controls;
using Zat.SystemTest.Wpf.UIStateTracking;

/// <summary>
/// Interaction logic for MainWindow.xaml.
/// </summary>
public partial class MainView
{
    private const string ExitStateKey = nameof(ExitStateKey);

    private const string TestModuleComboBoxStateKey = nameof(TestModuleComboBoxStateKey);
    private const string ExplorationTreeViewItemStateKey = nameof(ExplorationTreeViewItemStateKey);

    private readonly AppState.ExitState exitState;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainView"/> class.
    /// </summary>
    public MainView()
    {
        this.InitializeComponent();

        this.exitState = AppState.Instance.GetStateOrDefault(
            ExitStateKey, () => new AppState.ExitState(0, ExitStateKey));

        this.Loaded += this.OnLoaded;
        Application.Current.Exit += this.OnAppExited;
    }

    private static string MakeExplorationTreeViewItemStateKey(
        int testModuleComboBoxSelectedIndex)
        => $"{ExplorationTreeViewItemStateKey}|{testModuleComboBoxSelectedIndex}";

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        this.Loaded -= this.OnLoaded;

        this.LoadControlStates<TestExplorerControl>(this.LoadTestExplorerStates);
        this.LoadControlStates<ConfigEditorControl>(this.LoadConfigEditorStates);
    }

    private void OnAppExited(object sender, ExitEventArgs e)
    {
        // TODO : Nebude nutne
        AppState.Instance.SetState(ExitStateKey, this.exitState);
        AppState.Instance.Save();
    }

    private void LoadTestExplorerStates(TestExplorerControl testExplorer)
    {
        var testModuleComboBox = testExplorer.TestModuleComboBox;
        testModuleComboBox.SelectedIndex = this.exitState.TestExplorerComboBoxItemIndex;
        testModuleComboBox.SelectionChanged +=
            (_, _) =>
            {
                var selectedIndex = testModuleComboBox.SelectedIndex;
                this.exitState.TestExplorerComboBoxItemIndex = selectedIndex;

                var treeViewItemStateKey = MakeExplorationTreeViewItemStateKey(selectedIndex);
                var treeViewItemState = AppState.Instance.GetState<TreeViewItemState>(treeViewItemStateKey);
                if (treeViewItemState?.IndexedPath is { Length: > 0 } indexedPath)
                {
                    testExplorer
                        .ExplorationTreeView
                        .GetChild(indexedPath)?
                        .Button
                        .PerformClick();
                }

                // TODO: Event handler pro button click získá state s tímto klíčem a na základě něho sestaví klíč prp získání stavu NodeStates
                AppState.Instance.SetState(
                    StateKeys.DeriveCurrentKey(TestModuleComboBoxStateKey),
                    new ComboBoxState(selectedIndex, ExitStateKey));
            };

        // TODO:
        // Attach event hanlder to all nodes of testExplorer.ExplorationTreeView to track their selection state
        // But we have to attach the event every time when the tree rerenders + dispose the old handlers
        // For every button it should be possible to get indexed path of its tree view item parent
        // Maybe there could be an event ItemButtonClicked on the TreeView control?
    }

    private void LoadConfigEditorStates(ConfigEditorControl configEditor)
    {
        // ,,,
        AppState.Instance.StateChanged +=
            (key, state) =>
            {
                if (key != TestModuleComboBoxStateKey || state is not ComboBoxState comboBoxState)
                {
                    return;
                }
            };
    }

    private void LoadControlStates<TControl>(Action<TControl> initer)
        where TControl : DependencyObject
    {
        var control = this.FindFirstDescendant<TControl>();
        if (control is null)
        {
            DevLogger.Instance.LogError(
                $"Unable to init states of control '{nameof(TControl)}': " +
                "Failed to find the control in the visual tree");
            return;
        }

        initer(control);
    }
}