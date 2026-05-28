private readonly Event<Action<string>> changed = new();

public event Action<string> Changed
{
    add { 
        changed.Add(value); }
    remove { 
        changed.Remove(value);}
}

public void DoSome()
{
    changed.Notify("A")
}

public void Dispose()
{
    changedEvent.Dispose();
}

public Event<Action<T>> : IDisposable
    where T : delegate
{
    List<T> handler = [];

    public void Add(Action<T> handler)
        => this.handlers.Add
        (handler); //lock 
        
    public void Notify(T arg1)
    {
        // TODO : Lock 
        foreach (var handler in handlers)
        {
            handler();
        }
    }
    
    public void Release()
        => this.handlers.Clear();
        
    public void Dispose()
        => this.Release();
}