using System;
using System.Collections.Generic;
using System.Linq;

namespace PriorityScheduling
{
    class Program
    {
        public class Process
        {
            public int Id { get; set; }
            public int ArrivalTime { get; set; }
            public int BurstTime { get; set; }
            public int Priority { get; set; }
            public int WaitingTime { get; set; }
            public int TurnaroundTime { get; set; }
            public int ResponseTime { get; set; }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Enter number of processes:");
            int n = int.Parse(Console.ReadLine());

            List<Process> processes = new List<Process>();

            for (int i = 0; i < n; i++)
            {
                Console.WriteLine($"Enter Arrival Time, Burst Time, and Priority for Process {i + 1}:");
                int arrival = int.Parse(Console.ReadLine());
                int burst = int.Parse(Console.ReadLine());
                int priority = int.Parse(Console.ReadLine());

                processes.Add(new Process
                {
                    Id = i + 1,
                    ArrivalTime = arrival,
                    BurstTime = burst,
                    Priority = priority
                });
            }

            Console.WriteLine("Choose Scheduling Type: \n1. Non-Preemptive \n2. Preemptive");
            int choice = int.Parse(Console.ReadLine());

            if (choice == 1)
                NonPreemptivePriorityScheduling(processes);
            else if (choice == 2)
                PreemptivePriorityScheduling(processes);
            else
                Console.WriteLine("Invalid choice");

            Console.ReadLine();
        }

        static void NonPreemptivePriorityScheduling(List<Process> processes)
        {
            processes = processes.OrderBy(p => p.ArrivalTime).ToList(); // ترتيب العمليات بناءً على وقت الوصول
            int time = 0;
            int completed = 0;
            int n = processes.Count;
            List<Process> readyQueue = new List<Process>();

            while (completed < n)
            {
                // العمليات التي وصلت وتكون جاهزة للتنفيذ
                readyQueue = processes.Where(p => p.ArrivalTime <= time && p.TurnaroundTime == 0).OrderBy(p => p.Priority).ToList();

                if (readyQueue.Count > 0)
                {
                    Process current = readyQueue.First(); // اختيار العملية ذات الأولوية الأعلى
                    current.WaitingTime = time - current.ArrivalTime; // حساب وقت الانتظار
                    time += current.BurstTime; // تحديث الزمن بناءً على زمن تشغيل العملية
                    current.TurnaroundTime = time - current.ArrivalTime; // حساب وقت الاستجابة الكلي
                    current.ResponseTime = current.WaitingTime; // حساب وقت الاستجابة
                    Console.WriteLine($"Process {current.Id}: Arrival Time = {current.ArrivalTime}, Burst Time = {current.BurstTime}, Waiting Time = {current.WaitingTime}, Turnaround Time = {current.TurnaroundTime}, Response Time = {current.ResponseTime}");
                    completed++;
                }
                else
                {
                    time++; // إذا لم تكن هناك عملية جاهزة، يتم زيادة الزمن
                }
            }
        }

        static void PreemptivePriorityScheduling(List<Process> processes)
        {
            processes = processes.OrderBy(p => p.ArrivalTime).ToList();
            int time = 0; // وقت التنفيذ الحالي
            int completed = 0; // عدد العمليات المكتملة
            int n = processes.Count;
            List<Process> readyQueue = new List<Process>();
            int[] remainingBurst = new int[n]; // تخزين زمن التشغيل المتبقي لكل عملية
            bool[] isCompleted = new bool[n]; // لتتبع العمليات المكتملة
            List<Tuple<int, int, int>> ganttChart = new List<Tuple<int, int, int>>(); // تخزين معلومات مخطط جانت

            for (int i = 0; i < n; i++) remainingBurst[i] = processes[i].BurstTime;

            while (completed < n)
            {
                // تحديث قائمة العمليات الجاهزة بناءً على وقت الوصول
                readyQueue = processes.Where(p => p.ArrivalTime <= time && !isCompleted[processes.IndexOf(p)]).OrderBy(p => p.Priority).ToList();

                if (readyQueue.Count > 0)
                {
                    Process current = readyQueue.First();
                    int index = processes.IndexOf(current);

                    // أول وقت استجابة
                    if (remainingBurst[index] == current.BurstTime)
                    {
                        current.ResponseTime = time - current.ArrivalTime;
                    }

                    // تقليل زمن التشغيل المتبقي
                    remainingBurst[index]--;

                    // إذا انتهت العملية
                    if (remainingBurst[index] == 0)
                    {
                        completed++; // زيادة عدد العمليات المكتملة
                        isCompleted[index] = true; // تعيين العملية كمكتملة
                        current.TurnaroundTime = time + 1 - current.ArrivalTime;
                        current.WaitingTime = current.TurnaroundTime - current.BurstTime;
                        ganttChart.Add(Tuple.Create(current.Id, time + 1 - current.BurstTime, time + 1));
                        Console.WriteLine($"Process {current.Id}: Arrival Time = {current.ArrivalTime}, Burst Time = {current.BurstTime}, Waiting Time = {current.WaitingTime}, Turnaround Time = {current.TurnaroundTime}, Response Time = {current.ResponseTime}");
                    }
                }

                time++;
            }

            // حساب وعرض متوسطات الوقت
            double avgWaitingTime = processes.Average(p => p.WaitingTime);
            double avgTurnaroundTime = processes.Average(p => p.TurnaroundTime);
            double avgResponseTime = processes.Average(p => p.ResponseTime);

            Console.WriteLine($"Average Waiting Time: {avgWaitingTime}");
            Console.WriteLine($"Average Turnaround Time: {avgTurnaroundTime}");
            Console.WriteLine($"Average Response Time: {avgResponseTime}");

            // عرض مخطط جانت
            Console.WriteLine("Gantt Chart:");
            foreach (var entry in ganttChart)
            {
                Console.WriteLine($"Process {entry.Item1}: Start Time = {entry.Item2}, End Time = {entry.Item3}");
            }
        }
    }
}
