// Umístěno v Shared.Stores? 
[FeatureState]
record TestConfigurationState(
  Version IdeVersion,
  Version RuntimeVersion)

# TestConfigurationActions.cs (records)
record ChangedAction(
  Version? NewIdeVersion,
  Version? NewRuntimeVersion)

...

# TestConfiguration (feat)

// Reducers.cs
static class Reducers
{
  [ReducerMethod]
  public static TestConfigurationState OnChanged(
    TestConfigurationState current, ChangedAction action)
  {
    var updated = default(TestConfigurationState);
    
    if (action.NewRuntimeVersion)
    {
      updated = current with { RuntimeVersion = action.NewRuntimeVersion};
    }
    
    // TODO 
    
    return updated;
  }
}

...

# TestExecution (feat)

ctor(IState<testConfigurationState> testConfigurationState)
{
  
}

// testConfigurationState poskytne stav (RuntimeVersion, IdeVersion, ...)