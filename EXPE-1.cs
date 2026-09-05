protected static async Task<DataAnalysisCreateResult<TDocumentViewModel>> CreateWithDataAnalysis<
        TDocumentModel, TDocumentViewModel, TAnalyzer, TResolver, TAnalyzeResult>(
        TDocumentModel possibleModel,
        Func<TDocumentModel, TDocumentViewModel> viewModelFactory,
        Func<TAnalyzer> analyzerFactory,
        Func<TAnalyzeResult, TResolver> resolverFactory,
        Action<TDocumentModel>? postHook = null,
        Func<string,string> onSevereConflictsMessage = null,
        Func<string,string> onCriticalConflictsMessage = null)
        where TDocumentModel : class, new()
        where TAnalyzer : IAnalyzer<TDocumentModel, TAnalyzeResult>
        where TResolver : DataAnalysisResolverBase<TDocumentModel, TAnalyzeResult>
        where TAnalyzeResult : AnalyzeResultBase
    {
        onSevereConflicts ??= logText => TextUtils.SafeFormat(Properties.Errors.LoadedDataHasCriticalConflicts, logText); // TODO: Change resources text 
        onCriticalConflicts ??= logText => TextUtils.SafeFormat(Properties.Errors.LoadedDataHasSevereConflicts, logText);
        
        var analyzeResult = await AnalyzeAsync<TDocumentModel, TAnalyzer, TAnalyzeResult>(
            possibleModel, analyzerFactory);
        var logText = string.Join(Environment.NewLine, analyzeResult.ToLogLines()); 
        
        if (analyzeResult.HasCriticalConflicts)
        {
           var message = onCriticalConflictsMessage(logText); 
           MessageBoxService.ShowError(
               message, Resources.DataAnalysis_Dialog_Title);
            return new DataAnalysisCreateResult<TDocumentViewModel>(Maybe.None, false);
        }
        
        if (analyzeResult.HasSevereConflicts)
        {
            var message = onSevereConflictsMessage(logText);
            if (!MessageBoxService.ShowQuestion(
                    message, Resources.DataAnalysis_Dialog_Title))
            {
                return new DataAnalysisCreateResult<TDocumentViewModel>(Maybe.None, false);
            }
        }

        var dataAnalysisResultResolver = resolverFactory(analyzeResult);
        var resolvedModel = await dataAnalysisResultResolver.ResolveAsync(possibleModel)
                            ?? new TDocumentModel();
        return new DataAnalysisCreateResult<TDocumentViewModel>(
            Create(resolvedModel, viewModelFactory, postHook),
            analyzeResult.HasConflicts); // If had conflicts then the model was changed at this point
    }

 
            // RemoteEditorVisitor bude mít metodu bool OnDataHasCriticalConflicts(string analyzeLogText)
                
            }

            // RemoteEditorVisitor bude mít metodu bool OnDataHasSevereConflicts(string analyzeLogText)
            