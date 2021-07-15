namespace GenshinBlood
{
    internal class BloodResult
    {
        public int TotalCount { get; set; }

        public int Star5Count { get; set; }

        public double RatioRank { get; set; }

        public PointEx[] PlotData { get; set; }

        public double AverageCount => (double)TotalCount / Star5Count;

        public double AverageRatio => (double)Star5Count / TotalCount;

        public double RatioRankShow
        {
            get
            {
                if (Star5Count == 0)
                {
                    return double.NaN;
                }
                else
                {
                    return RatioRank;
                }
            }
        }

        public string Assessment
        {
            get
            {
                if (Star5Count == 0)
                {
                    return "NaN";
                }
                else
                {
                    return RatioRank switch
                    {
                        < 0.001349898031630 => "欧皇",
                        < 0.022750131948179 => "很欧",
                        < 0.158655253931457 => "较欧",
                        < 0.841344746068543 => "正常",
                        < 0.977249868051821 => "较非",
                        < 0.998650101968370 => "很非",
                        _ => "非酋"
                    };
                }
            }
        }
    }

    class PointEx
    {
        public string X { get; set; }

        public double Y { get; set; }

        public string Type { get; set; }

    }


}
