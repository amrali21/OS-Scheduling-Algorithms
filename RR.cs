using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR_FULL                    //FULL ROUND ROBIN CODE.
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
                    // Console.Write(values[r, 0]);
                    string id = values[r, 0];
                    int arrival_time = Int32.Parse((values[r, 1]));
                    int burst = Int32.Parse((values[r, 2]));

                    my_processes.Add(new process(id, arrival_time, burst));
                }
            }



            Queue<process> runningQ = new Queue<process>();
            Queue<process> readyQ = new Queue<process>();


            //int simulation_time = 30;


            int simulation_time = 100;


            int quanta = 1;
            int x = quanta;

            for (int j = 0; j <= simulation_time; j++)
            {       //SIMULATOR


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
                                readyQ.Peek().isReady = false;    // move ready process to running queue
                                runningQ.Enqueue(readyQ.Peek()); // if no process is running, run next ready process
                                readyQ.Dequeue();
                                Console.WriteLine("time: " + j + " process " + runningQ.Peek().id + " is Running");
                                readyQ.Enqueue(my_processes[i]); // THIS LINE WAS ADDED
                                runningQ.Peek().start_time = j;
                            }
                            else
                            {         // run first process
                                runningQ.Enqueue(my_processes[i]);
                                Console.WriteLine("time: " + j + " process " + my_processes[i].id + " is Running");
                                my_processes[i].start_time = j;
                            }
                        }
                        else
                        {
                            my_processes[i].isReady = true;
                            readyQ.Enqueue(my_processes[i]); // else put it in ready queue
                        }

                    }

                }

                // ANOTHER FOR ON READY PROCESSES
                // IF ARRIVE TIME > J THEN INCREASE WAITING TIME BY 1.
                for (int i = 0; i < my_processes.Count; i++)
                {
                    if ((j > my_processes[i].arrival_time) && (my_processes[i].isReady == true))
                    {
                        my_processes[i].waiting_time += 1;
                    }
                }



                if (runningQ.Count != 0 && j > 0) // if a process is running execute this.
                {
                    runningQ.Peek().burst -= 1;
                    x -= 1;

                    if (runningQ.Peek().burst == 0) // if the serve time is over remove process //
                    {
                        Console.WriteLine("time: " + j + " process " + runningQ.Peek().id + " exited");
                        runningQ.Dequeue();
                        x = quanta;
                        // if another process is ready add it to queue.
                        if (readyQ.Count != 0)
                        {
                            readyQ.Peek().isReady = false;
                            runningQ.Enqueue(readyQ.Peek()); // if no process is running, run next ready process
                            readyQ.Dequeue();
                            Console.WriteLine("time: " + j + " process " + runningQ.Peek().id + " is Running");
                            runningQ.Peek().start_time = j;
                        }
                    }


                    if (x == 0)  // if the quanta time if over for this process
                    {
                        x = quanta;

                        runningQ.Peek().isReady = true;
                        readyQ.Enqueue(runningQ.Peek());  // move it to readyQ
                        runningQ.Dequeue();
                        readyQ.Peek().isReady = false;    // EDITED
                        runningQ.Enqueue(readyQ.Peek());
                        Console.WriteLine("time: " + j + " process " + runningQ.Peek().id + " is Running");
                        readyQ.Dequeue();

                    }
                }




            }

            for (int i = 0; i < my_processes.Count; i++)
            {
                int turn_around = my_processes[i].waiting_time + my_processes[i].serve_time;
                Console.WriteLine("process " + my_processes[i].id + " turn around time " + turn_around);
            }
            Console.ReadLine();

        }
    }

    class process
    {
        public int serve_time;
        public string id;
        public int arrival_time;
        public int burst;
        public int start_time;
        public int waiting_time = 0;
        public bool isReady = false; // set this to true if process is in ready queue. else set it to false.

        public process(string id, int arrival_time, int burst)
        {
            this.id = id;
            this.arrival_time = arrival_time;
            this.burst = burst;
            serve_time = burst;


        }
    }
}
