    [Test]
    public void ExecCommand__WhenRunCalledWithDialogHandleArg__ThenShouldHandleDialog()
    {
        // Given
        const string texts = "a,b";

        this.ReinitializeUnit(null, out var runnerEngineMock, out var plugin);
        
        var innerMessageBox = default(IExtendedMessageBoxService);
        runnerEngineMock
          .Setup(x => x.Run(...,...,...))
          .Callback((_,mesageBox,_) => innerMessageBox = messageBox);
        var receivedArgs = default(PluginArgs);
        plugin.Action += (plugin, args) =>
        {
            receivedArgs = args.Args;
            return null;
        };

        const string givenHanlderCmd = "cmdA:arg=val";
        const string givenHandlerSn = "2402054243MX";

        var validRunCmdWithDialogHandler = $"run: {MakeRunArgs(
            TestIdArgValue,
            TestCfgDataArgValue,
            MakeDialogHandlersArgValue((Texts: texts, Sn: givenHandlerSn, Cmd: givenHanlderCmd)))}";

        this.unit.ExecCommand(ValidInitCmd);

        // When
        this.unit.ExecCommand(validRunCmdWithDialogHandler);
        innerMessageBox.ShowInfo(
          ...,texts
        ); // TODO : Validate all methods (eg ShowYesNo,...)

        // Then
        Assert.That(receivedArgs, Is.Not.Null);
        Assert.That(
            receivedArgs,
            Has.One.Matches<KeyValuePair<string, object>>(x => x.Key == PluginArgsKeys.Action && (string)x.Value == PluginArgsValues.ExecRemote));
        Assert.That(
            receivedArgs,
            Has.One.Matches<KeyValuePair<string, object>>(x => x.Key == PluginArgsKeys.PluginSystem && (bool)x.Value == true));
        Assert.That(
            receivedArgs,
            Has.One.Matches<KeyValuePair<string, object>>(x => x.Key == PluginArgsKeys.DeviceId && (string)x.Value == givenHandlerSn));
        Assert.That(
            receivedArgs,
            Has.One.Matches<KeyValuePair<string, object>>(x => x.Key == PluginArgsKeys.Command && (string)x.Value == givenHanlderCmd));
    } 