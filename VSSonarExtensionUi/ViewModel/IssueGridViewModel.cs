﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IssueGridViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The issue grid view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VSSonarExtensionUi.ViewModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;
    using System.Windows.Data;
    using System.Windows.Input;

    using ExtensionTypes;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;

    using PropertyChanged;

    using SqaleUi.helpers;

    using VSSonarExtensionUi.Helpers;
    using VSSonarExtensionUi.Menu;

    using VSSonarPlugins;

    /// <summary>
    ///     The issue grid view model.
    /// </summary>
    [ImplementPropertyChanged]
    public class IssueGridViewModel : ViewModelBase, IViewModelBase, IDataModel, IFilterCommand, IFilterOption 
    {
        #region Constants

        private static object _lock = new object();
        
        /// <summary>
        ///     The data grid options key.
        /// </summary>
        private const string DataGridOptionsKey = "DataGridOptions";

        #endregion

        #region Fields

        /// <summary>
        ///     The filter.
        /// </summary>
        private readonly IFilter filter;

        /// <summary>
        ///     The model base.
        /// </summary>
        private readonly IViewModelBase modelBase;

        /// <summary>
        ///     The vsenvironmenthelper.
        /// </summary>
        private readonly IVsEnvironmentHelper vsenvironmenthelper;

        private readonly SonarQubeViewModel model;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="IssueGridViewModel" /> class.
        /// </summary>
        public IssueGridViewModel()
        {
            this.Issues = new ItemsChangeObservableCollection<Issue>(this);
            this.IssuesInView = new CollectionViewSource { Source = this.Issues }.View;

            BindingOperations.EnableCollectionSynchronization(this.IssuesInView, _lock);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IssueGridViewModel"/> class.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="modelBase">
        /// The model Base.
        /// </param>
        /// <param name="rowContextMenu">
        /// The row Context Menu.
        /// </param>
        public IssueGridViewModel(SonarQubeViewModel model, IViewModelBase modelBase, bool rowContextMenu)
        {
            this.model = model;
            this.modelBase = modelBase;
            this.vsenvironmenthelper = model.VsHelper;
            this.Issues = new ItemsChangeObservableCollection<Issue>(this);
            this.IssuesInView = new CollectionViewSource { Source = this.Issues }.View;
            BindingOperations.EnableCollectionSynchronization(this.IssuesInView, _lock);
            this.Issues.Add(new Issue { Message = "component", Component = "message" });
            this.Issues.Add(new Issue { Message = "message", Component = "component" });

            this.MouseEventCommand = new RelayCommand(this.OnMouseEventCommand);

            this.SelectionChangedCommand = new RelayCommand<IList>(
                items =>
                    {
                        this.SelectedItems = items;
                        this.IssuesCounter = this.Issues.Count.ToString(CultureInfo.InvariantCulture);

                        // SendItemToWorkAreaMenu.RefreshMenuItemsStatus(this.ContextMenuItems, items != null);
                        // SelectKeyMenuItem.RefreshMenuItemsStatus(this.ContextMenuItems, this.SelectedItems.Count == 1);
                        // CreateTagMenuItem.RefreshMenuItemsStatus(
                        // this.ContextMenuItems,
                        // this.SelectedItems.Count == 1 && this.mainModel.ConnectedToServer);
                    });
            this.ColumnHeaderChangedCommand = new RelayCommand(this.OnColumnHeaderChangedCommand);
            this.RestoreUserSettingsInIssuesDataGrid();

            this.ShowHeaderContextMenu = true;
            this.ContextVisibilityMenuItems = this.CreateColumnVisibiltyMenu();
            this.ShowContextMenu = rowContextMenu;
            this.ContextMenuItems = this.CreateRowContextMenu();

            this.filter = new IssueFilter(this);
            this.InitFilterCommanding();
        }

        public void UpdateIssues(List<Issue> listOfIssues)
        {
            this.IssuesInView = new CollectionViewSource { Source = null }.View;
            this.IssuesInView = new CollectionViewSource { Source = listOfIssues }.View;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the assignee index.
        /// </summary>
        public int AssigneeIndex { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether assignee visible.
        /// </summary>
        public bool AssigneeVisible { get; set; }

        /// <summary>
        ///     Gets or sets the busy indicator tooltip.
        /// </summary>
        public string BusyIndicatorTooltip { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether can hide coloumn command.
        /// </summary>
        public bool CanHideColoumnCommand { get; set; }

        /// <summary>
        ///     Gets or sets the clear clear filter term rule command.
        /// </summary>
        public ICommand ClearClearFilterTermRuleCommand { get; set; }

        /// <summary>
        ///     Gets or sets the clear filter term assignee command.
        /// </summary>
        public ICommand ClearFilterTermAssigneeCommand { get; set; }

        /// <summary>
        ///     Gets or sets the clear filter term component command.
        /// </summary>
        public ICommand ClearFilterTermComponentCommand { get; set; }

        /// <summary>
        ///     Gets or sets the clear filter term is new command.
        /// </summary>
        public ICommand ClearFilterTermIsNewCommand { get; set; }

        /// <summary>
        ///     Gets or sets the clear filter term message command.
        /// </summary>
        public ICommand ClearFilterTermMessageCommand { get; set; }

        /// <summary>
        ///     Gets or sets the clear filter term project command.
        /// </summary>
        public ICommand ClearFilterTermProjectCommand { get; set; }

        /// <summary>
        ///     Gets or sets the clear filter term resolution command.
        /// </summary>
        public ICommand ClearFilterTermResolutionCommand { get; set; }

        /// <summary>
        ///     Gets or sets the clear filter term severity command.
        /// </summary>
        public ICommand ClearFilterTermSeverityCommand { get; set; }

        /// <summary>
        ///     Gets or sets the clear filter term status command.
        /// </summary>
        public ICommand ClearFilterTermStatusCommand { get; set; }

        /// <summary>
        ///     Gets or sets the close date index.
        /// </summary>
        public int CloseDateIndex { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether close date visible.
        /// </summary>
        public bool CloseDateVisible { get; set; }

        /// <summary>
        ///     Gets or sets the column header changed command.
        /// </summary>
        public ICommand ColumnHeaderChangedCommand { get; set; }

        /// <summary>
        ///     Gets or sets the component index.
        /// </summary>
        public int ComponentIndex { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether component visible.
        /// </summary>
        public bool ComponentVisible { get; set; }

        /// <summary>
        ///     Gets or sets the context menu items.
        /// </summary>
        public ObservableCollection<IMenuItem> ContextMenuItems { get; set; }

        /// <summary>
        ///     Gets or sets the context visibility menu items.
        /// </summary>
        public ObservableCollection<IMenuItem> ContextVisibilityMenuItems { get; set; }

        /// <summary>
        ///     Gets or sets the creation date index.
        /// </summary>
        public int CreationDateIndex { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether Creation Date visible.
        /// </summary>
        public bool CreationDateVisible { get; set; }

        /// <summary>
        ///     Gets or sets the effort to fix index.
        /// </summary>
        public int EffortToFixIndex { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether effort to fix is visible.
        /// </summary>
        public bool EffortToFixVisible { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether extension is busy.
        /// </summary>
        public bool ExtensionIsBusy { get; set; }

        /// <summary>
        ///     Gets or sets the filter apply command.
        /// </summary>
        public ICommand FilterApplyCommand { get; set; }

        /// <summary>
        ///     Gets or sets the filter clear all command.
        /// </summary>
        public ICommand FilterClearAllCommand { get; set; }

        /// <summary>
        ///     Gets or sets the filter term assignee.
        /// </summary>
        public string FilterTermAssignee { get; set; }

        /// <summary>
        ///     Gets or sets the filter term component.
        /// </summary>
        public string FilterTermComponent { get; set; }

        /// <summary>
        ///     Gets or sets the filter term is new.
        /// </summary>
        public string FilterTermIsNew { get; set; }

        /// <summary>
        ///     Gets or sets the filter term message.
        /// </summary>
        public string FilterTermMessage { get; set; }

        /// <summary>
        ///     Gets or sets the filter term project.
        /// </summary>
        public string FilterTermProject { get; set; }

        /// <summary>
        ///     Gets or sets the filter term resolution.
        /// </summary>
        public Resolution? FilterTermResolution { get; set; }

        /// <summary>
        ///     Gets or sets the filter term rule.
        /// </summary>
        public string FilterTermRule { get; set; }

        /// <summary>
        ///     Gets or sets the filter term severity.
        /// </summary>
        public Severity? FilterTermSeverity { get; set; }

        /// <summary>
        ///     Gets or sets the filter term status.
        /// </summary>
        public IssueStatus? FilterTermStatus { get; set; }

        /// <summary>
        ///     Gets or sets the header.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        ///     Gets or sets the id index.
        /// </summary>
        public int IdIndex { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether id visible.
        /// </summary>
        public bool IdVisible { get; set; }

        /// <summary>
        ///     Gets or sets the id index.
        /// </summary>
        public int IsNewIndex { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether id visible.
        /// </summary>
        public bool IsNewVisible { get; set; }

        /// <summary>
        ///     Gets or sets the issues.
        /// </summary>
        [AlsoNotifyFor("IssuesCounter")]
        public ObservableCollection<Issue> Issues { get; set; }

        /// <summary>
        ///     Gets or sets the issues counter.
        /// </summary>
        public string IssuesCounter { get; set; }

        /// <summary>
        ///     Gets or sets the issues in view.
        /// </summary>
        public ICollectionView IssuesInView { get; set; }

        /// <summary>
        ///     Gets or sets the key index.
        /// </summary>
        public int KeyIndex { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether key visible.
        /// </summary>
        public bool KeyVisible { get; set; }

        /// <summary>
        ///     Gets or sets the line index.
        /// </summary>
        public int LineIndex { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether line visible.
        /// </summary>
        public bool LineVisible { get; set; }

        /// <summary>
        ///     Gets or sets the message index.
        /// </summary>
        public int MessageIndex { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether message visible.
        /// </summary>
        public bool MessageVisible { get; set; }

        /// <summary>
        ///     Gets or sets the mouse event command.
        /// </summary>
        public ICommand MouseEventCommand { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the project index.
        /// </summary>
        public int ProjectIndex { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether project visible.
        /// </summary>
        public bool ProjectVisible { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether query for issues is running.
        /// </summary>
        public bool QueryForIssuesIsRunning { get; set; }

        /// <summary>
        ///     Gets or sets the resolution index.
        /// </summary>
        public int ResolutionIndex { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether resolution visible.
        /// </summary>
        public bool ResolutionVisible { get; set; }

        /// <summary>
        ///     Gets or sets the rule index.
        /// </summary>
        public int RuleIndex { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether rule visible.
        /// </summary>
        public bool RuleVisible { get; set; }

        /// <summary>
        ///     Gets or sets the selected items.
        /// </summary>
        public IList SelectedItems { get; set; }

        /// <summary>
        ///     Gets or sets the selection changed command.
        /// </summary>
        public ICommand SelectionChangedCommand { get; set; }

        /// <summary>
        ///     Gets or sets the severity index.
        /// </summary>
        public int SeverityIndex { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether severity visible.
        /// </summary>
        public bool SeverityVisible { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether show context menu.
        /// </summary>
        public bool ShowContextMenu { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether show header context menu.
        /// </summary>
        public bool ShowHeaderContextMenu { get; set; }

        /// <summary>
        ///     Gets or sets the status index.
        /// </summary>
        public int StatusIndex { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether status visible.
        /// </summary>
        public bool StatusVisible { get; set; }

        /// <summary>
        ///     Gets or sets the update date index.
        /// </summary>
        public int UpdateDateIndex { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether update date visible.
        /// </summary>
        public bool UpdateDateVisible { get; set; }

        /// <summary>
        ///     Gets or sets the violation id index.
        /// </summary>
        public int ViolationIdIndex { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether violation id visible.
        /// </summary>
        public bool ViolationIdVisible { get; set; }


        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The process changes.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="propertyChangedEventArgs">
        /// The property changed event args.
        /// </param>
        public void ProcessChanges(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
        }

        /// <summary>
        ///     The restore user settings.
        /// </summary>
        public void RestoreUserSettingsInIssuesDataGrid()
        {
            if (this.vsenvironmenthelper == null)
            {
                this.ResetWindowDefaults();
                return;
            }

            Dictionary<string, string> options = this.vsenvironmenthelper.ReadAllAvailableOptionsInSettings(DataGridOptionsKey);
            if (options != null && options.Count > 0)
            {
                this.ReadWindowOptions(options);
            }
            else
            {
                this.WriteWindowOptions();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The clear filter.
        /// </summary>
        private void ClearFilter()
        {
            this.IssuesInView.Filter = this.filter.FilterFunction;
        }

        /// <summary>
        ///     The create column visibilty menu.
        /// </summary>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>ObservableCollection</cref>
        ///     </see>
        ///     .
        /// </returns>
        private ObservableCollection<IMenuItem> CreateColumnVisibiltyMenu()
        {
            PropertyInfo[] props = typeof(Issue).GetProperties();
            var submenus = new List<string>();
            foreach (PropertyInfo propertyInfo in props)
            {
                submenus.Add(propertyInfo.Name);
            }

            var menu = new ObservableCollection<IMenuItem> { ShowHideIssueColumn.MakeMenu(this, this.vsenvironmenthelper, submenus) };

            return menu;
        }

        /// <summary>
        /// The create row context menu.
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>ObservableCollection</cref>
        ///     </see>
        ///     .
        /// </returns>
        private ObservableCollection<IMenuItem> CreateRowContextMenu()
        {
            var menu = new ObservableCollection<IMenuItem> { ChangeStatusHandler.MakeMenu(this.model.SonarCubeConfiguration, this.model.SonarRestConnector, this) };

            return menu;
        }

        /// <summary>
        ///     The init filter commanding.
        /// </summary>
        private void InitFilterCommanding()
        {
            this.FilterTermMessage = string.Empty;
            this.FilterTermComponent = string.Empty;
            this.FilterTermProject = string.Empty;
            this.FilterTermRule = string.Empty;
            this.FilterTermAssignee = string.Empty;
            this.FilterTermStatus = null;
            this.FilterTermSeverity = null;
            this.FilterTermResolution = null;
            this.FilterTermIsNew = null;

            this.ClearClearFilterTermRuleCommand = new RelayCommand<object>(this.OnClearClearFilterTermRuleCommand);
            this.ClearFilterTermAssigneeCommand = new RelayCommand<object>(this.OnClearFilterTermAssigneeCommand);
            this.ClearFilterTermComponentCommand = new RelayCommand<object>(this.OnClearFilterTermComponentCommand);
            this.ClearFilterTermIsNewCommand = new RelayCommand<object>(this.OnClearFilterTermIsNewCommand);
            this.ClearFilterTermMessageCommand = new RelayCommand<object>(this.OnClearFilterTermMessageCommand);
            this.ClearFilterTermProjectCommand = new RelayCommand<object>(this.OnClearFilterTermProjectCommand);
            this.ClearFilterTermResolutionCommand = new RelayCommand<object>(this.OnClearFilterTermResolutionCommand);
            this.ClearFilterTermSeverityCommand = new RelayCommand<object>(this.OnClearFilterTermSeverityCommand);
            this.ClearFilterTermStatusCommand = new RelayCommand<object>(this.OnClearFilterTermStatusCommand);
            this.FilterApplyCommand = new RelayCommand<object>(this.OnFilterApplyCommand);
            this.FilterClearAllCommand = new RelayCommand<object>(this.OnFilterClearAllCommand);
        }

        /// <summary>
        /// The on clear clear filter term rule command.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        private void OnClearClearFilterTermRuleCommand(object obj)
        {
            this.FilterTermRule = string.Empty;
            this.ClearFilter();
        }

        /// <summary>
        /// The on clear filter term assignee command.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        private void OnClearFilterTermAssigneeCommand(object obj)
        {
            this.FilterTermAssignee = string.Empty;
            this.ClearFilter();
        }

        /// <summary>
        /// The on clear filter term component command.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        private void OnClearFilterTermComponentCommand(object obj)
        {
            this.FilterTermComponent = string.Empty;
            this.ClearFilter();
        }

        /// <summary>
        /// The on clear filter term is new command.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        private void OnClearFilterTermIsNewCommand(object obj)
        {
            this.FilterTermIsNew = null;
            this.ClearFilter();
        }

        /// <summary>
        /// The on clear filter term message command.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        private void OnClearFilterTermMessageCommand(object obj)
        {
            this.FilterTermMessage = string.Empty;
            this.ClearFilter();
        }

        /// <summary>
        /// The on clear filter term project command.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        private void OnClearFilterTermProjectCommand(object obj)
        {
            this.FilterTermProject = string.Empty;
            this.ClearFilter();
        }

        /// <summary>
        /// The on clear filter term resolution command.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        private void OnClearFilterTermResolutionCommand(object obj)
        {
            this.FilterTermResolution = null;
            this.ClearFilter();
        }

        /// <summary>
        /// The on clear filter term severity command.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        private void OnClearFilterTermSeverityCommand(object obj)
        {
            this.FilterTermSeverity = null;
            this.ClearFilter();
        }

        /// <summary>
        /// The on clear filter term status command.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        private void OnClearFilterTermStatusCommand(object obj)
        {
            this.FilterTermStatus = null;
            this.ClearFilter();
        }

        /// <summary>
        ///     The on column header changed command.
        /// </summary>
        private void OnColumnHeaderChangedCommand()
        {
            this.SaveWindowOptions();
        }

        /// <summary>
        /// The on filter apply command.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        private void OnFilterApplyCommand(object obj)
        {
            this.IssuesInView.Filter = this.filter.FilterFunction;
        }

        /// <summary>
        /// The on filter clear all command.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        private void OnFilterClearAllCommand(object obj)
        {
            this.FilterTermStatus = null;
            this.FilterTermSeverity = null;
            this.FilterTermResolution = null;
            this.FilterTermIsNew = null;

            this.FilterTermProject = string.Empty;
            this.FilterTermMessage = string.Empty;
            this.FilterTermComponent = string.Empty;
            this.FilterTermAssignee = string.Empty;
            this.FilterTermRule = string.Empty;

            this.ClearFilter();
        }

        /// <summary>
        ///     The on mouse event command.
        /// </summary>
        private void OnMouseEventCommand()
        {
        }

        /// <summary>
        /// The read window options.
        /// </summary>
        /// <param name="options">
        /// The options.
        /// </param>
        private void ReadWindowOptions(Dictionary<string, string> options)
        {
            try
            {
                this.ComponentIndex = int.Parse(options["ComponentIndex"], CultureInfo.InvariantCulture);
                this.LineIndex = int.Parse(options["LineIndex"], CultureInfo.InvariantCulture);
                this.AssigneeIndex = int.Parse(options["AssigneeIndex"], CultureInfo.InvariantCulture);
                this.MessageIndex = int.Parse(options["MessageIndex"], CultureInfo.InvariantCulture);
                this.StatusIndex = int.Parse(options["StatusIndex"], CultureInfo.InvariantCulture);
                this.SeverityIndex = int.Parse(options["SeverityIndex"], CultureInfo.InvariantCulture);
                this.RuleIndex = int.Parse(options["RuleIndex"], CultureInfo.InvariantCulture);
                this.CreationDateIndex = int.Parse(options["CreationDateIndex"], CultureInfo.InvariantCulture);
                this.ProjectIndex = int.Parse(options["ProjectIndex"], CultureInfo.InvariantCulture);
                this.ResolutionIndex = int.Parse(options["ResolutionIndex"], CultureInfo.InvariantCulture);
                this.EffortToFixIndex = int.Parse(options["EffortToFixIndex"], CultureInfo.InvariantCulture);
                this.UpdateDateIndex = int.Parse(options["UpdateDateIndex"], CultureInfo.InvariantCulture);
                this.CloseDateIndex = int.Parse(options["CloseDateIndex"], CultureInfo.InvariantCulture);
                this.KeyIndex = int.Parse(options["KeyIndex"], CultureInfo.InvariantCulture);
                this.IdIndex = int.Parse(options["IdIndex"], CultureInfo.InvariantCulture);

                this.ComponentVisible = bool.Parse(options["ComponentVisible"]);
                this.LineVisible = bool.Parse(options["LineVisible"]);
                this.AssigneeVisible = bool.Parse(options["AssigneeVisible"]);
                this.MessageVisible = bool.Parse(options["MessageVisible"]);
                this.StatusVisible = bool.Parse(options["StatusVisible"]);
                this.SeverityVisible = bool.Parse(options["SeverityVisible"]);
                this.RuleVisible = bool.Parse(options["RuleVisible"]);
                this.CreationDateVisible = bool.Parse(options["CreationDateVisible"]);
                this.ProjectVisible = bool.Parse(options["ProjectVisible"]);
                this.ResolutionVisible = bool.Parse(options["ResolutionVisible"]);
                this.EffortToFixVisible = bool.Parse(options["EffortToFixVisible"]);
                this.UpdateDateVisible = bool.Parse(options["UpdateDateVisible"]);
                this.CloseDateVisible = bool.Parse(options["CloseDateVisible"]);
                this.KeyVisible = bool.Parse(options["KeyVisible"]);
                this.IdVisible = bool.Parse(options["IdVisible"]);
                this.IsNewIndex = int.Parse(options["IsNewIndex"], CultureInfo.InvariantCulture);
                this.IsNewVisible = bool.Parse(options["IsNewVisible"]);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        ///     The reset window defaults.
        /// </summary>
        private void ResetWindowDefaults()
        {
            this.MessageIndex = 0;
            this.CreationDateIndex = 1;
            this.CloseDateIndex = 2;
            this.ComponentIndex = 3;
            this.EffortToFixIndex = 4;
            this.LineIndex = 5;
            this.ProjectIndex = 6;
            this.UpdateDateIndex = 7;
            this.StatusIndex = 8;
            this.SeverityIndex = 9;
            this.RuleIndex = 10;
            this.ResolutionIndex = 11;
            this.AssigneeIndex = 12;
            this.IsNewIndex = 13;
            this.KeyIndex = 14;
            this.IdIndex = 15;
            this.ViolationIdIndex = 16;

            this.ComponentVisible = true;
            this.LineVisible = true;
            this.AssigneeVisible = false;
            this.MessageVisible = true;
            this.StatusVisible = true;
            this.SeverityVisible = true;
            this.RuleVisible = false;
            this.CreationDateVisible = false;
            this.ProjectVisible = false;
            this.ResolutionVisible = false;
            this.EffortToFixVisible = true;
            this.UpdateDateVisible = false;
            this.CloseDateVisible = false;
            this.KeyVisible = false;
            this.IdVisible = false;
        }

        /// <summary>
        ///     The write window options.
        /// </summary>
        private void SaveWindowOptions()
        {
            this.vsenvironmenthelper.WriteOptionInApplicationData(
                DataGridOptionsKey, 
                "ComponentIndex", 
                this.ComponentIndex.ToString(CultureInfo.InvariantCulture));
            this.vsenvironmenthelper.WriteOptionInApplicationData(
                DataGridOptionsKey, 
                "LineIndex", 
                this.LineIndex.ToString(CultureInfo.InvariantCulture));
            this.vsenvironmenthelper.WriteOptionInApplicationData(
                DataGridOptionsKey, 
                "AssigneeIndex", 
                this.AssigneeIndex.ToString(CultureInfo.InvariantCulture));
            this.vsenvironmenthelper.WriteOptionInApplicationData(
                DataGridOptionsKey, 
                "MessageIndex", 
                this.MessageIndex.ToString(CultureInfo.InvariantCulture));
            this.vsenvironmenthelper.WriteOptionInApplicationData(
                DataGridOptionsKey, 
                "StatusIndex", 
                this.StatusIndex.ToString(CultureInfo.InvariantCulture));
            this.vsenvironmenthelper.WriteOptionInApplicationData(
                DataGridOptionsKey, 
                "SeverityIndex", 
                this.SeverityIndex.ToString(CultureInfo.InvariantCulture));
            this.vsenvironmenthelper.WriteOptionInApplicationData(
                DataGridOptionsKey, 
                "RuleIndex", 
                this.RuleIndex.ToString(CultureInfo.InvariantCulture));
            this.vsenvironmenthelper.WriteOptionInApplicationData(
                DataGridOptionsKey, 
                "CreationDateIndex", 
                this.CreationDateIndex.ToString(CultureInfo.InvariantCulture));
            this.vsenvironmenthelper.WriteOptionInApplicationData(
                DataGridOptionsKey, 
                "ProjectIndex", 
                this.ProjectIndex.ToString(CultureInfo.InvariantCulture));
            this.vsenvironmenthelper.WriteOptionInApplicationData(
                DataGridOptionsKey, 
                "ResolutionIndex", 
                this.ResolutionIndex.ToString(CultureInfo.InvariantCulture));
            this.vsenvironmenthelper.WriteOptionInApplicationData(
                DataGridOptionsKey, 
                "EffortToFixIndex", 
                this.EffortToFixIndex.ToString(CultureInfo.InvariantCulture));
            this.vsenvironmenthelper.WriteOptionInApplicationData(
                DataGridOptionsKey, 
                "UpdateDateIndex", 
                this.UpdateDateIndex.ToString(CultureInfo.InvariantCulture));
            this.vsenvironmenthelper.WriteOptionInApplicationData(
                DataGridOptionsKey, 
                "CloseDateIndex", 
                this.CloseDateIndex.ToString(CultureInfo.InvariantCulture));
            this.vsenvironmenthelper.WriteOptionInApplicationData(
                DataGridOptionsKey, 
                "KeyIndex", 
                this.KeyIndex.ToString(CultureInfo.InvariantCulture));
            this.vsenvironmenthelper.WriteOptionInApplicationData(DataGridOptionsKey, "IdIndex", this.IdIndex.ToString(CultureInfo.InvariantCulture));
            this.vsenvironmenthelper.WriteOptionInApplicationData(
                DataGridOptionsKey, 
                "IsNewIndex", 
                this.IsNewIndex.ToString(CultureInfo.InvariantCulture));
            this.vsenvironmenthelper.WriteOptionInApplicationData(
                DataGridOptionsKey, 
                "ViolationIdIndex", 
                this.ViolationIdIndex.ToString(CultureInfo.InvariantCulture));

            this.vsenvironmenthelper.WriteOptionInApplicationData(DataGridOptionsKey, "MessageVisible", this.MessageVisible.ToString());
            this.vsenvironmenthelper.WriteOptionInApplicationData(DataGridOptionsKey, "CreationDateVisible", this.CreationDateVisible.ToString());
            this.vsenvironmenthelper.WriteOptionInApplicationData(DataGridOptionsKey, "CloseDateVisible", this.CloseDateVisible.ToString());
            this.vsenvironmenthelper.WriteOptionInApplicationData(DataGridOptionsKey, "ComponentVisible", this.ComponentVisible.ToString());
            this.vsenvironmenthelper.WriteOptionInApplicationData(DataGridOptionsKey, "EffortToFixVisible", this.EffortToFixVisible.ToString());
            this.vsenvironmenthelper.WriteOptionInApplicationData(DataGridOptionsKey, "LineVisible", this.LineVisible.ToString());
            this.vsenvironmenthelper.WriteOptionInApplicationData(DataGridOptionsKey, "ProjectVisible", this.ProjectVisible.ToString());
            this.vsenvironmenthelper.WriteOptionInApplicationData(DataGridOptionsKey, "UpdateDateVisible", this.UpdateDateVisible.ToString());
            this.vsenvironmenthelper.WriteOptionInApplicationData(DataGridOptionsKey, "StatusVisible", this.StatusVisible.ToString());
            this.vsenvironmenthelper.WriteOptionInApplicationData(DataGridOptionsKey, "SeverityVisible", this.SeverityVisible.ToString());
            this.vsenvironmenthelper.WriteOptionInApplicationData(DataGridOptionsKey, "RuleVisible", this.RuleVisible.ToString());
            this.vsenvironmenthelper.WriteOptionInApplicationData(DataGridOptionsKey, "ResolutionVisible", this.ResolutionVisible.ToString());
            this.vsenvironmenthelper.WriteOptionInApplicationData(DataGridOptionsKey, "AssigneeVisible", this.AssigneeVisible.ToString());
            this.vsenvironmenthelper.WriteOptionInApplicationData(DataGridOptionsKey, "IsNewVisible", this.IsNewVisible.ToString());
            this.vsenvironmenthelper.WriteOptionInApplicationData(DataGridOptionsKey, "KeyVisible", this.KeyVisible.ToString());
            this.vsenvironmenthelper.WriteOptionInApplicationData(DataGridOptionsKey, "IdVisible", this.IdVisible.ToString());
            this.vsenvironmenthelper.WriteOptionInApplicationData(DataGridOptionsKey, "ViolationIdVisible", this.ViolationIdVisible.ToString());
        }

        /// <summary>
        ///     The set filter active.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private bool SetFilterActive()
        {
            return !this.FilterTermMessage.Equals(string.Empty) || !this.FilterTermComponent.Equals(string.Empty)
                   || !this.FilterTermProject.Equals(string.Empty) || !this.FilterTermRule.Equals(string.Empty)
                   || !this.FilterTermAssignee.Equals(string.Empty) || this.FilterTermStatus != null || this.FilterTermSeverity != null
                   || this.FilterTermResolution != null || this.FilterTermIsNew != null;
        }

        /// <summary>
        ///     The write window options.
        /// </summary>
        private void WriteWindowOptions()
        {
            int i = 0;
            foreach (PropertyInfo propertyInfo in typeof(Issue).GetProperties())
            {
                this.vsenvironmenthelper.WriteOptionInApplicationData(
                    DataGridOptionsKey, 
                    propertyInfo.Name + "Index", 
                    i.ToString(CultureInfo.InvariantCulture));
                this.vsenvironmenthelper.WriteOptionInApplicationData(DataGridOptionsKey, propertyInfo.Name + "Visible", "true");
                i++;
            }

            Dictionary<string, string> options = this.vsenvironmenthelper.ReadAllAvailableOptionsInSettings(DataGridOptionsKey);
            if (options != null && options.Count > 0)
            {
                this.ReadWindowOptions(options);
            }
        }

        #endregion
    }
}