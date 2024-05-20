using System;

namespace app
{
    internal class Program
    {
        //Обработка введённых данных в массив
        static string[] GetChipsArr(string inp_v)
        {
            if (inp_v.StartsWith("chips:"))
            {
                inp_v = inp_v.Substring(6);
            }
            string[] chips = inp_v.Replace(" ", "").
            Trim(new char[] { '[', ']' }).
            Split(new char[] { ',' });
            return chips;
        }

        //Получаем индексы при итерации по массиву
        static int[] GetIndexes(int len, int position, int step)
        {
            int ind1, ind2;
            ind1 = position + step;
            if (ind1 >= len)
            {
                ind1 -= len;
            }
            ind2 = position - step;
            if (ind2 < 0)
            {
                ind2 += len;
            }
            return new int[2] { ind1, ind2 };
        }

        //Оптимизируем массив, чтобы первым был индекс меньшего значения
        static int[] UpdateIndexes(int[] chips, int mean, int[] ind)
        {

            if (Math.Abs(chips[ind[1]] - mean) > Math.Abs(chips[ind[0]] - mean))
            {
                return new int[2] { ind[1], ind[0] };
            }
            else
            {
                return ind;
            }
        }

        //Считаем сумму отклонений элементов от среднего значения в диапазоне 
        static int[] GetRangeValue(int[] chips, int mean, int first_range_v, int second_range_v)
        {
            int len;
            if (first_range_v > second_range_v)
            {
                len = chips.Length - first_range_v + second_range_v;
            }
            else
            {
                len = second_range_v - first_range_v;
            }
            int sum_v = 0;
            int sum_abs_v = 0;
            for (int i = 0; i < len; i++)
            {
                if (i + first_range_v < chips.Length)
                {
                    sum_v += chips[i + first_range_v] - mean;
                    sum_abs_v += Math.Abs(chips[i + first_range_v] - mean);
                }
                else
                {
                    sum_v += chips[i + first_range_v - chips.Length] - mean;
                    sum_abs_v += Math.Abs(chips[i + first_range_v - chips.Length] - mean);
                }
            }
            return new int[2] { sum_abs_v, Math.Abs(sum_v) };   // Возвращаем сумму_модулей_отклонений и модуль_суммы_отклонений
        }

        //Находим диапазон с максимальной суммой модулей отклонений элементов от среднего значения в диапазоне
        static int[] GetMaxRange(int[] chips, int range_v, int mean)
        {
            int first_range_v = 0, second_range_v = 0, max_range_value = 0, operate_mean = 0;
            int operate_firs, operate_second;
            int[] operate_range_value;
            for (int i = 0; i < chips.Length; i++)
            {
                if (i + range_v >= chips.Length)
                {
                    operate_firs = i;
                    operate_second = i + range_v - chips.Length;
                }
                else
                {
                    operate_firs = i;
                    operate_second = i + range_v;
                }
                operate_range_value = GetRangeValue(chips: chips, mean: mean, first_range_v: operate_firs, second_range_v: operate_second);
                if ((operate_range_value[0] > max_range_value) || ((operate_range_value[0] == max_range_value) && (operate_range_value[1] < operate_mean)))
                {
                    max_range_value = operate_range_value[0];
                    first_range_v = operate_firs;
                    second_range_v = operate_second;
                    operate_mean = operate_range_value[1];
                }
            }
            return new int[4] { max_range_value, first_range_v, second_range_v, operate_mean };   // { сумма_модулей_отклонений, начало_диапазона, конец_диапазона, сумма_отклонений}
        }

        //Индекс максиимального элемента
        static int GetMaxIndex(int[] chips, int mean)
        {
            int test_range = chips.Length / 2;
            int[] range_data = GetMaxRange(chips: chips, range_v: test_range, mean: mean);
            int m_el = chips[range_data[1]];
            int i_m = range_data[1];
            for (int i = 0; i < test_range; i++)
            {
                int j, n;
                if (i + range_data[1] < chips.Length)
                {
                    j = i + range_data[1];
                }
                else
                {
                    j = i + range_data[1] - chips.Length;
                }
                if (range_data[2] - i >= 0)
                {
                    n = range_data[2] - i;
                }
                else
                {
                    n = range_data[2] - i + chips.Length;
                }
                if (Math.Abs(chips[j] - mean) > Math.Abs(m_el - mean))
                {
                    m_el = chips[j];
                    i_m = j;
                }
                if (Math.Abs(chips[n] - mean) > Math.Abs(m_el - mean))
                {
                    m_el = chips[n];
                    i_m = n;
                }
            }
            return i_m;
        }

        //Вычисляем изменение значения неровности при перемещении
        static int GetSome(int value, int mean, int asperity)
        {
            int some = value - mean;    //Если + то есть излишек, - нехватка
            if ((some < 0 & asperity > 0) || (some > 0 & asperity < 0))
            {
                return 0;   //В обеих позициях нехватка или излишек
            }

            if (Math.Abs(asperity) <= Math.Abs(some))
            {
                return asperity;
            }
            else
            {
                return some;
            }

        }

        static int MakeChipsAction(int[] chips, int mean, int steps, int i)
        {
            int[] indexes;
            int some_chip = 0;
            int little_step = 0;
            int asperity = mean - chips[i];    //Неровность если +: надо добавить до средней, если -: то отнять лишнее
            while ((asperity != 0) && (some_chip == 0))
            {
                little_step++;
                asperity = asperity / Math.Abs(asperity);
                indexes = GetIndexes(len: chips.Length, position: i, step: little_step);
                indexes = UpdateIndexes(chips: chips, mean: mean, ind: indexes);
                for (int j = 0; j < 2; j++)
                {
                    if ((chips[i] != mean) && (chips[indexes[j]] != mean))
                    {
                        some_chip = GetSome(value: chips[indexes[j]], mean: mean, asperity: asperity);
                        chips[i] += some_chip;
                        chips[indexes[j]] -= some_chip;
                        steps += little_step * Math.Abs(some_chip);
                        asperity -= some_chip;

                        Console.WriteLine($"{string.Join("-", chips)}");
                        if (some_chip != 0)
                        {
                            return steps;
                        }
                    }
                }
            }
            return steps;
        }

        //Считаем колличество шагов для выравнивания. Проверка среднего значения происходит до вызова метода!
        static int CountSteps(int[] chips, int mean)
        {
            int steps = 0;
            int i = GetMaxIndex(chips: chips, mean: mean);
            while (chips[i] != mean)
            {
                steps = MakeChipsAction(chips: chips, mean: mean, steps: steps, i: i);
                i = GetMaxIndex(chips: chips, mean: mean);
            }
            return steps;
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Input:");
            string inp_v = Console.ReadLine() ?? "";
            string[] text_chips = GetChipsArr(inp_v: inp_v);
            byte[] chips;
            try
            {
                chips = Array.ConvertAll(text_chips, byte.Parse);
            }
            catch // (FormatException e)
            {
                Console.WriteLine("Invalid input value, program terminated.");
                Console.ReadLine();
                return;
            }
            double mean = chips.Average(x => x);
            if (mean % 1 > 0)
            {
                Console.WriteLine("The average value of the given values is not a whole number.");
                Console.ReadLine();
                return;
            }
            int int_mean = Convert.ToInt32(mean);
            int[] int_chips = Array.ConvertAll(chips, Convert.ToInt32);
            int steps = CountSteps(chips: int_chips, mean: int_mean);
            Console.WriteLine($"{steps}");
            // Console.ReadLine();
        }
    }
}
