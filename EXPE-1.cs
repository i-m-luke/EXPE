public void OnKeyDown(object sender, KeyEventArgs args)
    {
        if (args.OriginalSource is not TextBox textBox)
        {
            return;
        }
        
        // This will block TreeViewItem collapse/expand behaviour when numeric '-' or '+' keys are pressed
        var insertText = args.Key switch 
        {
            Key.Subtract => "-",
            Key.Add => "+",
            Key.Multiply => "*",
            _ => null, 
        }
        
        if (insertText is null))
        {
            return;
        }

        var changedText = textBox.Text.Insert(textBox.SelectionStart, insertText);
        if (this.TryPreviewTextInput(changedText, out var finalText))
        {
            textBox.Text = finalText;
            textBox.SelectionStart += insertText.Length;
        }

        args.Handled = true;
    }