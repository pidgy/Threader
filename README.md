# Threader

Threader is a nuget package for running Tasks in parallel like they are System.Threading.Threads.

This package is optimized for long running operations. The UWP 10.0 framework does not allow the usage of Threads, but rather Tasks.
Tasks are not treated like Threads and a user will not get a desired result using them in that manner.

Threader allows users to execute long and short running operations in the same manner as Threads.

## Example

```C#
  using NolanHodge.Threader;
  
  // A Class that satisfies the IThreader interface.
  class Button : IThreader
  {
      Action<object> Draw;
      public ButtonData(Canvas canvas, Action<object> action)
      {
          Canvas = canvas;
          Label = label;
      }

      public string Name()
      {
          return Canvas.Name;
      }

      public void Action()
      {
          Draw(this);
      }
  }

  // The Application requiring long running operations.
  class Application
  {
    Threader T = new Threader(); 
    void Init()
    {
        foreach (string name in canvases)
        {
            T.Spawn(new Button(name, Diagnose))
        }
    }

    // Kill the Threaders pool.
    void OnClose()
    {
      T.KillAll();
    }

    // Use the IThreader object in parallel, handling stops.
    void Diagnose(Button b)
    {
        while (true)
        {
            if (T.Killed(b))
            {
                return;
            }

            OnMainThread(Draw(b));
        }
    }
}

```
