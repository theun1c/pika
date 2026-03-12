
using Avalonia.Controls;
using Pika.Desktop.Controls;

namespace Pika.Desktop.Views;

public partial class MainWindow : Window
{

    public MainWindow()
    {
        InitializeComponent();
        
        var player = this.FindControl<VideoGlControl>("PlayerControl");
        
        player?.SetSource("/home/theun1c/Downloads/output.mp4");
    }
}
