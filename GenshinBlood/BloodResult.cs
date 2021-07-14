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

        public string Assessment
        {
            get
            {
                return RatioRank switch
                {
                    < 0.001349898031630 => "ŷ��",
                    < 0.022750131948179 => "��ŷ",
                    < 0.158655253931457 => "��ŷ",
                    < 0.841344746068543 => "����",
                    < 0.977249868051821 => "�Ϸ�",
                    < 0.998650101968370 => "�ܷ�",
                    _ => "����"
                };

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
