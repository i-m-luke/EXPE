protected static async Task<DataAnalysisCreateResult<TDocumentViewModel>> CreateWithDataAnalysis<
        TDocumentModel, TDocumentViewModel, TAnalyzer, TResolver, TAnalyzeResult>(
        TDocumentModel possibleModel,
        Func<TDocumentModel, TDocumentViewModel> viewModelFactory,
        Func<TAnalyzer> analyzerFactory,
        Func<TAnalyzeResult, TResolver> resolverFactory,
        Action<TDocumentModel>? postHook = null,
        Func<string,bool> onSevereConflicts = null,
        Func<string,bool> onCriticalConflicts = null)
        where TDocumentModel : class, new()
        where TAnalyzer : IAnalyzer<TDocumentModel, TAnalyzeResult>
        where TResolver : DataAnalysisResolverBase<TDocumentModel, TAnalyzeResult>
        where TAnalyzeResult : AnalyzeResultBase
    {
        onSevereConflicts ??= logText => true; // true means terminate
        onCriticalConflicts ??= logText => true;
        
        var analyzeResult = await AnalyzeAsync<TDocumentModel, TAnalyzer, TAnalyzeResult>(
            possibleModel, analyzerFactory);
        var logText = string.Join(Environment.NewLine, analyzeResult.ToLogLines()); 
        
        if (analyzeResult.HasCriticalConflicts)
        {
           if (onCriticalConflicts(logText))
           {
               return;
           }
        }
        
        if (analyzeResult.HasSevereConflicts)
        {
            if (onSevereConflicts(logText))
            {
                return;
            }
        }

        var dataAnalysisResultResolver = resolverFactory(analyzeResult);
        var resolvedModel = await dataAnalysisResultResolver.ResolveAsync(possibleModel)
                            ?? new TDocumentModel();
        return new DataAnalysisCreateResult<TDocumentViewModel>(
            Create(resolvedModel, viewModelFactory, postHook),
            conflictsFound);
    }

 
            // RemoteEditorVisitor bude mít metodu bool OnDataHasCriticalConflicts(string analyzeLogText)
                MessageBoxService.ShowError(
                    TextUtils.SafeFormat(Properties.Errors.LoadedDataHasCriticalConflicts, logText),
                    Resources.DataAnalysis_Dialog_Title);
                return new DataAnalysisCreateResult<TDocumentViewModel>(Maybe.None, conflictsFound);
            }

            // RemoteEditorVisitor bude mít metodu bool OnDataHasSevereConflicts(string analyzeLogText)
            if (!MessageBoxService.ShowQuestion(
                    TextUtils.SafeFormat(Properties.Errors.LoadedDataHasSevereConflicts, logText),
                    Resources.DataAnalysis_Dialog_Title))
            {
                return new DataAnalysisCreateResult<TDocumentViewModel>(Maybe.None, conflictsFound);
            }