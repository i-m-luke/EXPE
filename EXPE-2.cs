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
            var valueDeserializer = new ValueDeserializer(...); // TODO: Create deserialized (+ extend the Ioc with it?)
            var configObjectInstanceLazy = new Lazy(() => valueDeserializer.Deserialize(this.Model.ConfigObject));
            var userConfigEditorService = new UserConfigEditorService(
                configObjectInstanceLazy,
                []); // TODO: exctend with driver configs instances 
            var editorsServices = new EditorsServices
                userConfigEditorServices: userConfigEditorService,
                packageManifestEditorServices: null);
            
            // TODO: Extend ioc with EditorsServices
            
            var configObject = new TestConfigObject(
                this.Model.ConfigObject, configObjectInstanceLazy, ioc));
            
            return configObject;
        }
}

...

public class EditorServiceBase(
    Lazy<object> configInstanceLazy,
    ConcurrentDictionary<Type, object> driverConfigInstanceLaziesMap) : IEditorService
{
    public object GetConfigObject()
        => configInstanceLazy.Value;
        
    public object[] GetDriverConfigs()
        => [..driverConfigLaziesMap.Select(x => x.Value.Value)];
            
    public object? GetDriverConfig(Type type)
        => driverConfigInstanceLaziesMap.TryGetItem(type, out var configInstanceLazy) 
            ? configInstanceLazy.Value
            : null
}
                                
public class UserConfigEditorService(
    Lazy<object> configInstanceLazy)
    : EditorServiceBase(configInstanceLazy), IUserConfigEditorService;
