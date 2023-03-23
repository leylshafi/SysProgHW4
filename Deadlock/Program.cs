namespace Deadlock;

internal class Program
{
    static object object1 = new object();
    static object object2 = new object();

    public static void ObliviousFunction()
    {
        lock (object1)
        {
            Thread.Sleep(1000); 
            lock (object2)
            {
            }
        }
    }

    public static void BlindFunction()
    {
        lock (object2)
        {
            Thread.Sleep(1000); 
            lock (object1)
            {
            }
        }
    }
    static void Main(string[] args)
    {
        Thread thread1 = new Thread((ThreadStart)ObliviousFunction);
        Thread thread2 = new Thread((ThreadStart)BlindFunction);

        thread1.Start();
        thread2.Start();

        while (true)
        {
            
        }
    }

    // Numunede 2 dene thread var ve her iki thread de ishlemek uchun bir birlerini gozleyirler.
    // thread1 in ishlemesi uchun thread2, 2 nin ishlemesi uchun ise 1 in ishlemesi lazimdir.
    // Threadlerin bu kimi chixilmaz veziyyete dushmesi deadlock adlanir. 
}