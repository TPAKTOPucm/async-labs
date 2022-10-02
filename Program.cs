using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace matrix {
	class Program {
		static void Main(string[] args) {
			int firstHight = int.Parse(Console.ReadLine());
			int firstLength_secondHeigth = int.Parse(Console.ReadLine());
			int secondlength = int.Parse(Console.ReadLine());
			int[,] matrixA = new int[firstHight, firstLength_secondHeigth];
			int[,] matrixB = new int[firstLength_secondHeigth, secondlength];
			RandomMatrix(matrixA);
			RandomMatrix(matrixB);
			Console.WriteLine("Матрица А:");
			printMatrix(matrixA);
			Console.WriteLine("Матрица B:");
			printMatrix(matrixB);
			Console.WriteLine("Синхронное перемножение:");
			Stopwatch watch = new Stopwatch();
			watch.Start();
			var result = matrixMultiplication(matrixA, matrixB);
			watch.Stop();
			printMatrix(result);
			Console.WriteLine("Затраченное время: " + watch.ElapsedMilliseconds + " ms\n" +
				"Асинхронное перемножение:");
			watch.Restart();
			result = matrixMultiplicationParallel(matrixA, matrixB, 6);
			watch.Stop();
			printMatrix(result);
			Console.WriteLine("Затраченное время: " + watch.ElapsedMilliseconds + " ms");
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
			int stringsPerThread = (int)(result.GetLength(0)/(double)numOfThreads+0.9);
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
	}
}
