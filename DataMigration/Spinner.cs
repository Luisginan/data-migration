namespace OneLonDataMigration;

public class Spinner : ISpinner
{
    private readonly string[] _sequence = { "/", "-", "\\", "|" };
    private int _counter;
    private bool _active;
    private Thread _thread;

    public void Start()
    {
        if (_active) return;
        _active = true;
        _thread = new Thread(Spin);
        _thread.Start();
    }

    public void Stop()
    {
        _active = false;
        if (_thread != null && _thread.IsAlive)
        {
            _thread.Join();
        }
        Console.Write("\b");
    }

    private void Spin()
    {
        while (_active)
        {
            Turn();
            Thread.Sleep(100);
        }
    }

    private void Turn()
    {
        _counter++;
        Console.Write(_sequence[_counter % _sequence.Length]);
        Console.Write("\b");
    }
}