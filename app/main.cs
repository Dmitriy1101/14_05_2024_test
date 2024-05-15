using System;

namespace app // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        //Обработка введённых данных в массив
        static string[] get_chips_arr(string inp_v)
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
        static int[] get_indexes(int len, int position, int step)
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
        static int[] update_indexes(int[] chips, int[] ind)
        {

            if (chips[ind[1]] <= chips[ind[0]])
            {
                return new int[2] { ind[1], ind[0] };
            }
            else
            {
                return ind;
            }
        }

        //Индекс максиимального элемента
        static int get_m_index(int[] chips)
        {
            int m_el = 0;
            int i_m = 0;
            for (int i = 0; i < chips.Length; i++)
            {
                if (chips[i] > m_el)
                {
                    m_el = chips[i];
                    i_m = i;
                }
            }
            return i_m;
        }

        //Вычисляем изменение значения неровности при перемещении
        static int get_some(int value, int mean, int asperity)
        {
            int some = value - mean;    //Если + то есть излишек, - нехватка
            if ((some < 0 && asperity > 0) || (some > 0 && asperity < 0))
            {
                return 0;   //В обеих позициях нехватка или излишек
            }

            if (Math.Abs(asperity) < Math.Abs(some))
            {
                return asperity;
            }
            else
            {
                return some;
            }

        }

        //Считаем колличество шагов для выравнивания. Проверка среднего значения происходит до вызова метода!
        static int count_steps(int[] chips, int mean)
        {
            int steps = 0;
            int little_step;
            int[] indexes;
            int i = get_m_index(chips: chips);
            while (chips[i] != mean)
            {
                if (chips[i] != mean)
                {
                    little_step = 0;
                    int asperity = mean - chips[i];    //Неровность если +: надо добавить до средней, если -: то отнять лишнее
                    while (asperity != 0)
                    {
                        little_step++;
                        indexes = get_indexes(len: chips.Length, position: i, step: little_step);
                        indexes = update_indexes(chips: chips, ind: indexes);
                        for (int j = 0; j < 2; j++)
                        {
                            if ((chips[i] != mean) && (chips[indexes[j]] != mean))
                            {
                                int some_chip = get_some(value: chips[indexes[j]], mean: mean, asperity: asperity);
                                chips[i] += some_chip;
                                chips[indexes[j]] -= some_chip;
                                steps += little_step * Math.Abs(some_chip);
                                asperity -= some_chip;
                            }
                        }
                    }
                }
                i = get_m_index(chips: chips);
            }
            return steps;
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Input:");
            string inp_v = Console.ReadLine() ?? "";
            string[] text_chips = get_chips_arr(inp_v: inp_v);
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
            int steps = count_steps(chips: int_chips, mean: int_mean);
            Console.WriteLine($"{steps}");
            Console.ReadLine();
        }
    }
}
