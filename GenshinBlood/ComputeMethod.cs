using System;
using System.Threading.Tasks;

namespace GenshinBlood
{
    internal static class ComputeMethod
    {

        private static double[] CharacterInitDensity;

        private static double[] WeaponInitDensity;

        static ComputeMethod()
        {
            CharacterInitDensity = GetInitCharacterDensity();
            WeaponInitDensity = GetInitWeaponDensity();
        }

        private static double GetCharacterProbability(int n)
        {
            if (n < 1 || n > 90)
            {
                throw new ArgumentOutOfRangeException();
            }
            return n switch
            {
                < 74 => 0.006,
                < 90 => 0.06 * (n - 73) + 0.006,
                _ => 1
            };
        }

        private static double GetWeaponProbability(int n)
        {
            if (n < 1 || n > 80)
            {
                throw new ArgumentOutOfRangeException();
            }
            return n switch
            {
                < 63 => 0.007,
                < 74 => 0.07 * (n - 62) + 0.007,
                < 80 => 0.035 * (n - 73) + 0.777,
                _ => 1
            };
        }


        private static (double[] density, double[] distribution) GetCharacterDensityAndDistribution(int star5Count)
        {
            if (star5Count < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            // 样本空间概率密度
            var curve = new double[90];
            // 临时概率密度
            var tmp = new double[90 * star5Count];
            // 概率密度
            var den = new double[90 * star5Count];
            // 分布函数
            var dis = new double[90 * star5Count];

            curve[0] = 0.006;
            for (int i = 1; i < curve.Length; i++)
            {
                curve[i] = curve[i - 1] / GetCharacterProbability(i) * (1 - GetCharacterProbability(i)) * GetCharacterProbability(i + 1);
            }
            curve.CopyTo(den, 0);
            curve.CopyTo(tmp, 0);

            //progress += 1 / total;
            for (int n = 2; n <= star5Count; n++)
            {
                // progress += 1 / total;
                Array.Clear(den, 0, den.Length);
                Parallel.For(n, 90 * n + 1, i =>
                {
                    for (int j = 1; j <= 90 && i - j > 0; j++)
                    {
                        den[i - 1] += tmp[i - j - 1] * curve[j - 1];
                    }
                });
                den.CopyTo(tmp, 0);
            }

            dis[0] = den[0];
            for (int i = 1; i < den.Length; i++)
            {
                dis[i] = dis[i - 1] + den[i];
            }
            return (den, dis);
        }


        private static (double[] density, double[] distribution) GetWeaponDensityAndDistribution(int star5Count)
        {
            if (star5Count < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            var curve = new double[80];
            var tmp = new double[80 * star5Count];
            var den = new double[80 * star5Count];
            var dis = new double[80 * star5Count];

            curve[0] = 0.007;
            for (int i = 1; i < curve.Length; i++)
            {
                curve[i] = curve[i - 1] / GetWeaponProbability(i) * (1 - GetWeaponProbability(i)) * GetWeaponProbability(i + 1);
            }
            curve.CopyTo(den, 0);
            curve.CopyTo(tmp, 0);

            // progress += 1 / total;
            for (int n = 2; n <= star5Count; n++)
            {
                // progress += 1 / total;
                Array.Clear(den, 0, den.Length);
                Parallel.For(n, 80 * n + 1, i =>
                {
                    for (int j = 1; j <= 80 && i - j > 0; j++)
                    {
                        den[i - 1] += tmp[i - j - 1] * curve[j - 1];
                    }
                });
                den.CopyTo(tmp, 0);
            }

            dis[0] = den[0];
            for (int i = 1; i < den.Length; i++)
            {
                dis[i] = dis[i - 1] + den[i];
            }
            return (den, dis);
        }


        private static double[] GetInitCharacterDensity()
        {
            var result = new double[90];
            result[0] = 0.006;
            for (int i = 1; i < result.Length; i++)
            {
                result[i] = result[i - 1] / GetCharacterProbability(i) * (1 - GetCharacterProbability(i)) * GetCharacterProbability(i + 1);
            }
            return result;
        }

        private static double[] GetInitWeaponDensity()
        {
            var result = new double[80];
            result[0] = 0.007;
            for (int i = 1; i < result.Length; i++)
            {
                result[i] = result[i - 1] / GetWeaponProbability(i) * (1 - GetWeaponProbability(i)) * GetWeaponProbability(i + 1);
            }
            return result;
        }


        public static void IterateCharacterDensity(ref double[] density, int n)
        {
            if (n == 1)
            {
                CharacterInitDensity.CopyTo(density, 0);
                return;
            }
            var tmp = new double[density.Length];
            density.CopyTo(tmp, 0);
            Array.Clear(density, 0, density.Length);
            for (int i = 1; i <= 90 * n; i++)
            {
                for (int j = 1; j <= 90 && i - j > 0; j++)
                {
                    density[i - 1] += tmp[i - j - 1] * CharacterInitDensity[j - 1];
                }
            }
        }


        public static void IterateWeaponDensity(ref double[] density, int n)
        {
            if (n <= 1)
            {
                WeaponInitDensity.CopyTo(density, 0);
                return;
            }
            var tmp = new double[density.Length];
            density.CopyTo(tmp, 0);
            Array.Clear(density, 0, density.Length);
            for (int i = 1; i <= 80 * n; i++)
            {
                for (int j = 1; j <= 80 && i - j > 0; j++)
                {
                    density[i - 1] += tmp[i - j - 1] * WeaponInitDensity[j - 1];
                }
            }
        }




        public static void IterateDensity(int wishType, ref double[] density, int n)
        {
            if (wishType == 0)
            {
                IterateCharacterDensity(ref density, n);
            }
            else
            {
                IterateWeaponDensity(ref density, n);
            }
        }

        public static double[] GetDistribution(in double[] density)
        {
            var dis = new double[density.Length];
            dis[0] = density[0];
            for (int i = 1; i < density.Length; i++)
            {
                dis[i] = dis[i - 1] + density[i];
            }
            return dis;
        }
    }
}
