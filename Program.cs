using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace matrix {
    class Program {
        static int[,] A;
        static int[,] B;
        static int[,] result;
        static void Main(string[] args) {
            for (int i = 6; i < 100; i++) {
                int firstHight = i;
                int firstLength_secondHeigth = i;
                int secondlength = i;
                int[,] matrixA = new int[firstHight, firstLength_secondHeigth];
                int[,] matrixB = new int[firstLength_secondHeigth, secondlength];
                RandomMatrix(matrixA);
                RandomMatrix(matrixB);
                A = matrixA;
                B = matrixB;
                //Console.WriteLine("Синхронное перемножение:");
                Stopwatch watch = new Stopwatch();
                watch.Start();
                result = matrixMultiplication(matrixA, matrixB);
                watch.Stop();
                Console.Write(watch.ElapsedMilliseconds + ";");
                //Async 6 threads
                watch.Restart();
                result = matrixMultiplicationParallel(matrixA, matrixB, 6);
                watch.Stop();
                Console.Write(watch.ElapsedMilliseconds + ";");
                watch.Restart();
                //async strings
                result = matrixMultiplicationParallel();
                watch.Stop();
                Console.Write(watch.ElapsedMilliseconds + ";");
                watch.Restart();
                //async strings task
                result = matrixMultiplicationParallelTask();
                watch.Stop();
                Console.WriteLine(watch.ElapsedMilliseconds);
            }
        }
        static void RandomMatrix(int[,] matrix) {
            Random randomInt = new Random();
            for (int i = 0; i < matrix.GetLength(1); i++)
                for (int j = 0; j < matrix.GetLength(0); j++)
                    matrix[j, i] = randomInt.Next(10);
        }
        static int[,] matrixMultiplication(int[,] A, int[,] B) {
            int[,] result = new int[A.GetLength(0), B.GetLength(1)];
            for (int i = 0; i < result.GetLength(0); i++) {
                for (int j = 0; j < result.GetLength(1); j++) {
                    result[i, j] = 0;
                    for (int k = 0; k < A.GetLength(1); k++)
                        result[i, j] += A[i, k] * B[k, j];
                }
            }
            return result;
        }
        static void printMatrix(int[,] matrix) {
            for (int i = 0; i < matrix.GetLength(0); i++) {
                for (int j = 0; j < matrix.GetLength(1); j++)
                    Console.Write(matrix[i, j] + " ");
                Console.WriteLine();
            }
        }
        static int[,] matrixMultiplicationParallel(int[,] A, int[,] B, int numOfThreads) {
            int[,] result = new int[A.GetLength(0), B.GetLength(1)];
            var tasks = new Task[numOfThreads];
            int stringsPerThread = (int)(result.GetLength(0) / (double)numOfThreads + 0.9);
            for (int i = 0; i < numOfThreads; i++) {
                int lastNum = (i + 1) * stringsPerThread;
                if (i == numOfThreads - 1) {
                    lastNum = result.GetLength(0);
                }
                tasks[i] = Task.Run(() => {
                    for (int j = i * stringsPerThread; j < lastNum; j++) {
                        for (int k = 0; k < result.GetLength(1); k++) {
                            result[j, k] = 0;
                            for (int n = 0; n < A.GetLength(1); n++)
                                result[j, k] += A[j, n] * B[n, k];
                        }
                    }
                });
                Thread.Sleep(2);
            }
            Task.WaitAll(tasks);
            return result;
        }
        static int[,] matrixMultiplicationParallel() {
            result = new int[A.GetLength(0), B.GetLength(1)];
            Thread[] threads = new Thread[result.GetLength(0)];
            for (int i = 0; i < result.GetLength(0); i++) {
                threads[i] = new Thread(new ParameterizedThreadStart(stringMultiplication));
                threads[i].Start(i);
            }
            foreach (Thread thread in threads) {
                thread.Join();
            }
            return result;
        }
        static int[,] matrixMultiplicationParallelTask() {
            result = new int[A.GetLength(0), B.GetLength(1)];
            Task[] threads = new Task[result.GetLength(0)];
            for (int i = 0; i < result.GetLength(0); i++) {
                threads[i] = new Task(stringMultiplication, i);
                threads[i].Start();
            }
            Task.WaitAll(threads);
            return result;
        }
        static void stringMultiplication(object numOfString) {
            int i = (int)numOfString;
            for (int j = 0; j < result.GetLength(1); j++) {
                int resultInt = 0;
                for (int k = 0; k < A.GetLength(1); k++)
                    resultInt += A[i, k] * B[k, j];
                result[i, j] = resultInt;
            }
        }
    }
}
