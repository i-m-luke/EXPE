

/// <inheritdoc />
    protected override void Initialize()
    {
        SystemTest.Common.DevLogger.Instance.LogInfo($"{LogPrefix}: Initializing ...");

        base.Initialize();
        this.RegisterIndependentView(true);

        ThreadHelper.ThrowIfNotOnUIThread();

        this.ProjectActions = new ProjectActions(this.ProjectInfoProvider);

        var mainAssemblyPath = Path.Combine(
            this.ProjectInfoProvider.OutputDirPath,
            this.ProjectInfoProvider.OutputFileName);
        if (!File.Exists(mainAssemblyPath))
        {
            var dte = this.GetDte();
            if (dte is not null)
            {
                this.messageBoxService.ShowInfo(Resources.MainAssemblyNotFoundInfoMessage);
                dte.Solution.SolutionBuild.BuildProject(
                    dte.Solution.SolutionBuild.ActiveConfiguration.Name,
                    this.ProjectInfoProvider.UniqueName,
                    WaitForBuildToFinish: true);
            }
        }

        var editorsRemoting = this.GetRequiredService<IEditorsRemoting>();
        if (!editorsRemoting.ColorsThemeInitialized)
        {
            this.GetColorsTheme().Visit(editorsRemoting.InitializeColorsTheme);
        }

        // Init test buffer manager
        this.TextBufferManager = new TextBufferManager(
            textBufferLines,
            (IComponentModel)this.GetRequiredService<SComponentModel>());
        this.TextBufferManager.UndoRedoHappened += this.OnUndoRedoHappened;
        this.currentValidTextBufferSnapshot = this.TextBufferManager.GetCurrentSnapshot();
        
        // Init remote editor
        var remoteEditorResult = this.GetRemoteEditor(this.editorFactory); 
        if (remoteEditorResult.Editor is null)
        {
            if (remoteEditorResult.Error is not null)
            {
                // Show error dialog with remoteEditorResult.Error;
            }
           
            // TATO ŘÁDKA SE MOŽNÁ BUDE MUSET PŘESUNOUT DOLŮ POD InitWindowFrame a podmínit 'if remote EditorResult.Editor is null'
            this.WindowFrame.CloseFrame((uint)__FRAMECLOSE.FRAMECLOSE_PromptSave); 
            return;
        }
        else
        {
            this.RemoteEditor = remoteEditorResult.Editor;
            try
            {
                this.RemoteEditor.Initialize(
                    new RemoteEditorVisitor(new WeakReference<IRemoteEditorVisitor>(this)),
                this.TextBufferManager.BufferText);
            }
            catch (Exception ex)
            {
                this.messageBoxService.ShowError(
                    ex.Message, Resources.UnhandledException_Dialog_Title);
                // ??? KDYŽ INICIALIZACE SELŽE TAK SE TIŠE POTLAČÍ?! TAKOVÝ EDITOR BY SE MĚL UZAVŘÍT!
        }

        this.InitWindowFrame();
        this.InitDteEvents();

        SystemTest.Common.DevLogger.Instance.LogInfo($"{LogPrefix}: Initialized");
    }
