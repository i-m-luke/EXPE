﻿namespace Zat.SystemTest.Common.Net.MVVM.ViewModel.UserConfig;

using CommunityToolkit.Mvvm.DependencyInjection;

using Zat.SystemTest.Common.Net.MVVM.ViewModel;

/// <summary>
/// Represents a test user config data container.
/// </summary>
/// <param name="model">The model.</param>
/// <param name="ioc">Shared IoC.</param>
public class TestUserConfig(
    Model.UserConfig.TestUserConfig model,
    Ioc ioc)
    : ValidatableViewModelBase<Model.UserConfig.TestUserConfig>(model)
{
    /// <summary>
    /// Gets the ID of the test the user config is associated with.
    /// </summary>
    public Guid TestId
        => this.Model.TestId;

    /// <summary>
    /// Gets the test user config object.
    /// </summary>
    [field: AllowNull]
    public TestConfigObject ConfigObject
        => this.GetOrInitialize(ref field, () => 
        {
            var userConfigEditorService = new UserConfigEditorService();
            var editorsServices = new EditorsServices
                userConfigEditorServices: userConfigEditorService,
                packageManifestEditorServices: null);
                
            // TODO: Extend ioc with EditorsServices
            var configObject = new TestConfigObject(this.Model.ConfigObject, ioc));
            
            // TODO: Instantiate the configObject and provided it to the userConfigEditorService
            
            return configObject;
        }
}

...

public class UserConfigEditorService(Lazy<object> configInstanceLazy) : IUserConfigEditorService
{
    public object GetConfigObject()
        => configInstanceLazy.Value;
        
    public object[] GetDriverConfigs()
        => [];
}
