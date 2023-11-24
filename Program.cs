using Data;
using Services;

var taskData = new TaskData();
var taskService = new TaskService(taskData);

await taskService.Run();


// using System;
// using System.Threading;
// using System.Threading.Tasks;

// namespace TaskBasedAsynchronousProgramming
// {
//     class Program
//     {
//         static void Main(string[] args)
//         {
//             Console.WriteLine($"Main Thread : {Thread.CurrentThread.ManagedThreadId} Statred");
//             _ = Task.Run(() => { PrintCounter(); });
//             Console.WriteLine($"Main Thread : {Thread.CurrentThread.ManagedThreadId} Completed");
//             Console.ReadKey();
//         }

//         static void PrintCounter()
//         {
//             Console.WriteLine($"Child Thread : {Thread.CurrentThread.ManagedThreadId} Started");
//             for (int count = 1; count <= 5; count++)
//             {
//                 Console.WriteLine($"count value: {count}");
//             }
//             Console.WriteLine($"Child Thread : {Thread.CurrentThread.ManagedThreadId} Completed");
//         }
//     }
// }