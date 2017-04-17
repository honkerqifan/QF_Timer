using System.Collections.Generic;

namespace QF_Timer
{
    public class ConfigInfo
    {
        public int ServerPort { get; set; }
        public List<int> TimerList { get; set; }
        public List<string> TimerStrArr { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
        public int FontSize { get; set; }
        public int LastFontSize { get; set; }
        public string BackgroundColor { get; set; }
        public string FontColor { get; set; }
        public string LastBackgroundColor { get; set; }
        public string LastFontColor { get; set; }
        public double Opacity { get; set; }
    }
}
