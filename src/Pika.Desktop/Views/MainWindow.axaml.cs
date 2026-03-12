
using Avalonia.Controls;
using Pika.Desktop.Controls;

namespace Pika.Desktop.Views;

public partial class MainWindow : Window
{

    public MainWindow()
    {
        InitializeComponent();
        
        var player = this.FindControl<VideoGlControl>("PlayerControl");
        
        // player?.SetSource("/home/theun1c/Downloads/output.mp4");
        player?.SetSource("https://p56.kodik.info/s/m/aHR0cHM6Ly9jbG91ZC5rb2Rpay1zdG9yYWdlLmNvbS91c2VydXBsb2Fkcy8wMWY2MjE1Yi0wYTg5LTQ4NDgtOGYxYi0zZGRjMGVkZDhkYWI/a1330804fba0792cf3c4d40103ef5ebbc2f0a54e989da820f981f24f83ecf91b:2026031208/360.mp4:hls:manifest.m3u8");
    }
}
