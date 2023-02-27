using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SysProgHW4;


public partial class MainWindow : Window
{
    public ObservableCollection<Car> Cars { get; set; }
    private CancellationTokenSource? cts = new();
    Stopwatch watch = null;
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        Cars = new();
        watch = new();
        
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (tgButton.IsChecked==true) {
            AddCarsMulti(cts.Token);
        }
        else
        {
            AddCarsSingle(cts.Token);
        }
    }

    private void AddCarsMulti(CancellationToken token)
    {
        Cars?.Clear();
        var directory = new DirectoryInfo(@"..\..\..\FakeData");
        var sync = new object();
        watch = Stopwatch.StartNew();
        foreach (var file in directory.GetFiles())
        {
            if (file.Extension == ".json")
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    var jsonTxt = File.ReadAllText(file.FullName);

                    var carlist = JsonSerializer.Deserialize<List<Car>>(jsonTxt);

                    if (carlist is not null)
                    {
                        foreach (var car in carlist)
                        {

                            if (token.IsCancellationRequested)
                            {
                                watch.Stop();
                                Dispatcher.Invoke(() => timeTxt.Text= watch.Elapsed.ToString());
                                break;
                            }

                            lock (sync)
                                Dispatcher.Invoke(() => Cars?.Add(car));

                            Dispatcher.Invoke(() => timeTxt.Text = watch.Elapsed.ToString());
                            Thread.Sleep(100);
                        }
                    }
                });
            }
        }
    }
    private void AddCarsSingle(CancellationToken token)
    {

        Cars?.Clear();
        new Thread(() =>
        {
            watch = Stopwatch.StartNew();
            var directory = new DirectoryInfo(@"..\..\..\FakeData");
            foreach (var file in directory.GetFiles())
            {

                if (file.Extension == ".json")
                {
                    var jsonTxt = File.ReadAllText(file.FullName);

                    var carlist = JsonSerializer.Deserialize<List<Car>>(jsonTxt);

                    if (carlist is not null)
                        foreach (var car in carlist)
                        {
                            if (token.IsCancellationRequested)
                            {
                                watch.Stop();
                                Dispatcher.Invoke(() => timeTxt.Text = watch.Elapsed.ToString());
                                break;
                            }

                            Dispatcher.Invoke(() => Cars?.Add(car));
                            Dispatcher.Invoke(() => timeTxt.Text = watch.Elapsed.ToString());
                            Thread.Sleep(100);
                        }
                }
            }
            watch.Stop();
            Dispatcher.Invoke(() => timeTxt.Text = watch.Elapsed.ToString());
        }).Start();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        cts?.Cancel();
    }
}
