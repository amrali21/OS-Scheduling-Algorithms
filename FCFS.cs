using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp11         // THIS IS FULL FCFS CODE.
{
    class Program
    {
        static void Main(string[] args)
        {

            string whole_file = System.IO.File.ReadAllText("D:\\Porcesses.csv");

            // Split into lines.
            whole_file = whole_file.Replace('\n', '\r');
            string[] lines = whole_file.Split(new char[] { '\r' },
                StringSplitOptions.RemoveEmptyEntries);

            // See how many rows and columns there are.
            int num_rows = lines.Length;
            int num_cols = lines[0].Split(',').Length;

            // Allocate the data array.
            string[,] values = new string[num_rows, num_cols];

            List<process> my_processes = new List<process>();

            // Load the array.
            for (int r = 1; r < num_rows; r++)
            {
                string[] line_r = lines[r].Split(',');
                for (int c = 0; c < num_cols; c++)
                {
                    values[r, c] = line_r[c];
                }
                if (!String.IsNullOrEmpty(values[r, 0]))
                {
                    string id = values[r, 0];
                    int arrival_time = Int32.Parse((values[r, 1]));
                    int burst = Int32.Parse((values[r, 2]));
                    my_processes.Add(new process(id, arrival_time, burst));
                }
            }
            Console.WriteLine("Processes Table =");
            for (int i = 1; i < 6; i++)
            {
                for (int j = 0; j < num_cols; j++)
                {
                    Console.Write(string.Format("{0} ", values[i, j]));

                }
                Console.WriteLine("");

            }
            string[] process_name = new string[] { values[1, 0], values[2, 0], values[3, 0], values[4, 0], values[5, 0] };
            int[] arrival_time1 = new int[] { Int32.Parse((values[1, 1])), Int32.Parse((values[2, 1])), Int32.Parse((values[3, 1])), Int32.Parse((values[4, 1])), Int32.Parse((values[5, 1])) };
            int[] Service_Time1 = new int[] { Int32.Parse((values[1, 2])), Int32.Parse((values[2, 2])), Int32.Parse((values[3, 2])), Int32.Parse((values[4, 2])), Int32.Parse((values[5, 2])) };


            Queue<process> runningQ = new Queue<process>();
            Queue<process> readyQ = new Queue<process>();

            int simulation_time = 100;


            for (int j = 0; j <= simulation_time; j++)
            {       //SIMULATOR

                if (runningQ.Count != 0) // if the queue is not empty decrease running process's time by 1
                {

                    if (j - runningQ.Peek().start_time >= runningQ.Peek().burst)
                    {
                        Console.WriteLine("time: " + j + " process " + runningQ.Peek().id + " exited");
                        runningQ.Dequeue();       //OBJECT EXITED THE QUEUE.
                        if (readyQ.Count != 0)  // when a process exits, enter the next ready process.
                        {

                            runningQ.Enqueue(readyQ.Peek());
                            Console.WriteLine("time: " + j + " process " + readyQ.Peek().id + " is Running");
                            readyQ.Peek().start_time = j;
                            readyQ.Dequeue();
                        }
                    }

                }

                for (int i = 0; i < my_processes.Count; i++)
                {  //loop over processes each time

                    if (j == my_processes[i].arrival_time) // if the process came now
                    {
                        //PROCESS ARRIVED.
                        Console.WriteLine("time: " + j + " process " + my_processes[i].id + " arrived");
                        if (runningQ.Count == 0)
                        {
                            if (readyQ.Count != 0)
                            {
                                runningQ.Enqueue(readyQ.Peek()); // if no process is running, run next ready process
                                readyQ.Dequeue();
                                Console.WriteLine("time: " + j + " process " + runningQ.Peek().id + " is Running");
                                readyQ.Enqueue(my_processes[i]); // THIS LINE WAS ADDED
                                runningQ.Peek().start_time = j;
                            }
                            else
                            {
                                runningQ.Enqueue(my_processes[i]);
                                Console.WriteLine("time: " + j + " process " + my_processes[i].id + " is Running");
                                my_processes[i].start_time = j;
                            }

                        }
                        else
                        {
                            readyQ.Enqueue(my_processes[i]); // else put it in ready queue
                        }

                    }

                }

            }

            for (int i = 0; i < my_processes.Count; i++)
            {  //loop over processes each time
                int waiting_time = my_processes[i].start_time - my_processes[i].arrival_time;
                int turnaround_time = waiting_time + my_processes[i].burst;
                Console.WriteLine("Process: " + my_processes[i].id + " turn around time " + turnaround_time + " waiting time " + waiting_time);
            }

            Console.ReadLine();
        }
    }

    class process
    {
        public string id;
        public int arrival_time;
        public int burst;
        public int start_time;
        public int waiting_time;

        public process(string id, int arrival_time, int burst)
        {
            this.id = id;
            this.arrival_time = arrival_time;
            this.burst = burst;

        }
    }
}
