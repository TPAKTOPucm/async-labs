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
			PrintMatrix(matrixA);
			Console.WriteLine("Матрица B:");
			PrintMatrix(matrixB);
			Console.WriteLine("Синхронное перемножение:");
			Stopwatch watch = new Stopwatch();
			watch.Start();
			var result = MatrixMultiplication(matrixA, matrixB);
			watch.Stop();
			PrintMatrix(result);
			Console.WriteLine("Затраченное время: " + watch.ElapsedMilliseconds + " ms\n" +
				"Асинхронное перемножение:");
			watch.Restart();
			result = MatrixMultiplicationParallel(matrixA, matrixB, 6);
			watch.Stop();
			PrintMatrix(result);
			Console.WriteLine("Затраченное время: " + watch.ElapsedMilliseconds + " ms\n" +
				"Асинхронное перемножение(конвейер):");
			watch.Restart();
			result = MatrixMultiplicationParallel(matrixA, matrixB);
			watch.Stop();
			PrintMatrix(result);
			Console.WriteLine("Затраченное время: " + watch.ElapsedMilliseconds + " ms");
		}
		static void RandomMatrix(int[,] matrix) {
			Random randomInt = new Random();
			for (int i = 0; i < matrix.GetLength(1); i++)
				for (int j = 0; j < matrix.GetLength(0); j++)
					matrix[j, i] = randomInt.Next(10);
		}
		static int[,] MatrixMultiplication(int[,] A, int[,] B) {
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
		static void PrintMatrix(int[,] matrix) {
			for (int i = 0; i < matrix.GetLength(0); i++) {
				for (int j = 0; j < matrix.GetLength(1); j++)
					Console.Write(matrix[i, j] + " ");
				Console.WriteLine();
			}
		}
		static int[,] MatrixMultiplicationParallel(int[,] A, int[,] B, int numOfThreads) {
			int[,] result = new int[A.GetLength(0), B.GetLength(1)];
			var threads = new Thread[numOfThreads];
			int stringsPerThread = (int)(result.GetLength(0)/(double)numOfThreads+0.9);
			for (int threadIndex = 0; threadIndex < numOfThreads; threadIndex++) {
				int lastNum = (threadIndex + 1) * stringsPerThread;
				if (threadIndex == numOfThreads - 1) {
					lastNum = result.GetLength(0);
				}
				threads[threadIndex] = new Thread(() => {
					StringMultiplication(threadIndex * stringsPerThread, lastNum, A, B, result);
				});
				threads[threadIndex].Start();
				Thread.Sleep(2);
			}
			foreach (var thread in threads)
				thread.Join();
			return result;
		}
		static int[,] MatrixMultiplicationParallel(int[,] A, int[,] B) {
			return MatrixMultiplicationParallel(A, B, A.GetLength(0));
		}
		static void StringMultiplication(int firstString, int numOfStrings, int[,] A, int[,] B, int[,] result) {
			for (int i = firstString; i < numOfStrings; i++) {
				for (int j = 0; j < result.GetLength(1); j++) {
					result[i, j] = 0;
					for (int k = 0; k < A.GetLength(1); k++)
						result[i, j] += A[i, k] * B[k, j];
				}
			}
		}
	}
}
