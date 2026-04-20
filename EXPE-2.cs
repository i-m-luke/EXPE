namespace Zat.SystemTest.Funtest.Plugin.Devices;

using Zat.SystemTest.Common.Services;
using Zat.SystemTest.Funtest.Plugin.UtilityObjects;

public class RunnerMessageBox(
    IExtendedMessageBoxService messageBox,
    RunnerMessageBox.AutoHandler dialogAutoHandler,
    DialogAutoHandlerInfo[] autoHandlesInfo) : IExtendedMessageBoxService
{
    public delegate void AutoHandler(DialogAutoHandlerInfo autoHandleInfo);

    /// <summary>
    /// Shows the information message box or handles the dialog automatically.
    /// </summary>
    /// <param name="text">Message box text.</param>
    /// <param name="title">Message box title.</param>
    public void ShowInfo(string text, string? title = null)
    {
        if (this.HandleDialog(text))
        {
            return;
        }

        messageBox.ShowInfo(text, title);
    }

    /// <summary>
    /// Shows the information message box or handles the dialog automatically.
    /// </summary>
    /// <param name="text">Message box text.</param>
    /// <param name="title">Message box title.</param>
    public void ShowWarning(string text, string? title = null)
    {
        if (this.HandleDialog(text))
        {
            return;
        }

        messageBox.ShowWarning(text, title);
    }

    /// <summary>
    /// Shows the information message box or handles the dialog automatically.
    /// </summary>
    /// <param name="text">Message box text.</param>
    /// <param name="title">Message box title.</param>
    public void ShowError(string text, string? title = null)
    {
        if (this.HandleDialog(text))
        {
            return;
        }

        messageBox.ShowError(text, title);
    }

    /// <summary>
    /// Shows the information message box or handles the dialog automatically.
    /// </summary>
    /// <param name="text">Message box text.</param>
    /// <param name="title">Message box title.</param>
    /// <returns><see langword="true"/> if the question is answered positively, <see langword="false"/> otherwise.</returns>
    public bool ShowQuestion(string text, string? title = null)
        => this.HandleDialog(text) || messageBox.ShowQuestion(text, title);

    /// <summary>
    /// Shows the message box or handles the dialog automatically.
    /// </summary>
    /// <param name="type">Message box type.</param>
    /// <param name="text">Message box text.</param>
    /// <param name="title">Message box title.</param>
    /// <returns><see langword="true"/> if the question is answered positively, <see langword="false"/> otherwise.</returns>
    public MessageBoxResult Show(MessageBoxType type, string text, string? title = null)
        => this.HandleDialog(text) ? MessageBoxResult.Confirmed : messageBox.Show(type, text, title);

    namespace Zat.SystemTest.Funtest.Plugin.Devices;

using Zat.SystemTest.Common.Services;
using Zat.SystemTest.Funtest.Plugin.UtilityObjects;

public class RunnerMessageBox(
    IExtendedMessageBoxService messageBox,
    RunnerMessageBox.AutoHandler dialogAutoHandler,
    DialogAutoHandlerInfo[] autoHandlesInfo) : IExtendedMessageBoxService
{
    public delegate void AutoHandler(DialogAutoHandlerInfo autoHandleInfo);

    /// <summary>
    /// Shows the information message box or handles the dialog automatically.
    /// </summary>
    /// <param name="text">Message box text.</param>
    /// <param name="title">Message box title.</param>
    public void ShowInfo(string text, string? title = null)
    {
        if (this.HandleDialog(text))
        {
            return;
        }

        messageBox.ShowInfo(text, title);
    }

    /// <summary>
    /// Shows the information message box or handles the dialog automatically.
    /// </summary>
    /// <param name="text">Message box text.</param>
    /// <param name="title">Message box title.</param>
    public void ShowWarning(string text, string? title = null)
    {
        if (this.HandleDialog(text))
        {
            return;
        }

        messageBox.ShowWarning(text, title);
    }

    /// <summary>
    /// Shows the information message box or handles the dialog automatically.
    /// </summary>
    /// <param name="text">Message box text.</param>
    /// <param name="title">Message box title.</param>
    public void ShowError(string text, string? title = null)
    {
        if (this.HandleDialog(text))
        {
            return;
        }

        messageBox.ShowError(text, title);
    }

    /// <summary>
    /// Shows the information message box or handles the dialog automatically.
    /// </summary>
    /// <param name="text">Message box text.</param>
    /// <param name="title">Message box title.</param>
    /// <returns><see langword="true"/> if the question is answered positively, <see langword="false"/> otherwise.</returns>
    public bool ShowQuestion(string text, string? title = null)
        => this.HandleDialog(text) || messageBox.ShowQuestion(text, title);

    /// <summary>
    /// Shows the message box or handles the dialog automatically.
    /// </summary>
    /// <param name="type">Message box type.</param>
    /// <param name="text">Message box text.</param>
    /// <param name="title">Message box title.</param>
    /// <returns><see langword="true"/> if the question is answered positively, <see langword="false"/> otherwise.</returns>
    public MessageBoxResult Show(MessageBoxType type, string text, string? title = null)
        => this.HandleDialog(text) ? MessageBoxResult.Confirmed : messageBox.Show(type, text, title);

    private bool HandleDialog(string text)
{
    var handled = false;
    var normalizedText = text; // TODO: Use this to show dialog
    
    foreach (var dialogHandleInfo in autoHandlesInfo)
    {
        if (!dialogHandleInfo.Texts.All(t => text.Contains(t, StringComparison.OrdinalIgnoreCase)))
        {
            continue;
        }

        dialogAutoHandler(dialogHandleInfo);
        
        foreach (var dialogText in dialogHandleInfo.Texts)
        {
            normalizedText = normalizedText.Replace(dialogText, string.Empty, StringComparison.OrdinalIgnoreCase);
        }
 

        handled = true;
    }

    return handled;
  }
}
}