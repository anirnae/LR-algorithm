using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Collections.Generic;

namespace SocketTcpServer
{
    class Program
    {
        static decimal pogr = 0.01M;
        static int port = 8005; // порт для приема входящих запросов
        static string errorMessage = "";
        static string sAnswer;
        static void Main(string[] args) {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try {
                listenSocket.Bind(ipPoint);
                listenSocket.Listen(10);

                while (true) {
                    errorMessage = "";
                    Console.WriteLine("Сервер запущен. Ожидание подключений...");
                    Socket handler = listenSocket.Accept();
                    Console.WriteLine("Подключение " + handler.LocalEndPoint);

                    byte[] bdata = new byte[1024];
                    int bytesRec = handler.Receive(bdata);
                    string sdata = Encoding.UTF8.GetString(bdata, 0, bytesRec);

                    // делаем вычисления

                    string[] sdecimals = sdata.Split(';');
                    decimal[,] data = {
                        {Decimal.Parse(sdecimals[0]), Decimal.Parse(sdecimals[1]), Decimal.Parse(sdecimals[2]) },
                        {Decimal.Parse(sdecimals[3]), Decimal.Parse(sdecimals[4]), Decimal.Parse(sdecimals[5]) },
                        {Decimal.Parse(sdecimals[6]), Decimal.Parse(sdecimals[7]), Decimal.Parse(sdecimals[8]) }
                    };

                    try {
                        Matrix A = new Matrix(data);
                        if (A.Determinant == 0)
                            throw new Exception("Ошибка: определитель равен 0.");
                        Matrix P = new Matrix(1);

                        int count = 0;
                    
                        while (Math.Abs(A[1, 0]) + Math.Abs(A[2, 0]) + Math.Abs(A[2, 1]) > pogr && count < 20) {
                            var LU = A.GetLU();
                            P = P.Multiply(LU['L']);
                            A = LU['U'].Multiply(LU['L']);
                            count++;
                        }
                        if (count == 20)
                            errorMessage = "Результаты могут быть получены с большой погрешностью.";

                        for (int i = 0; i < A.Dimension; i++)
                            for (int j = 0; j < A.Dimension; j++)
                                A[i, j] = Math.Round(A[i, j], 2); //округляем матрицу

                        decimal[] eigenValues = new decimal[] { A[0, 0], A[1, 1], A[2, 2] };
                        eigenValues = eigenValues.Distinct().ToArray(); //получили собственные значения

                        var vecList = A.GetEigenVectors();
                        var initVecList = new List<Tuple<decimal, decimal[]>>();
                        foreach (Tuple<decimal, decimal[]> tuple in vecList) {
                            var newTuple = new Tuple<decimal, decimal[]>(tuple.Item1, P.Multiply(tuple.Item2));
                            initVecList.Add(newTuple);
                        } // получили собственные вектора

                        // отправляем ответ

                        sAnswer = "";
                        sAnswer += errorMessage + '_';

                        foreach (decimal eigenValue in eigenValues)
                            sAnswer += eigenValue + Environment.NewLine;

                        sAnswer += '_';

                        foreach (Tuple<decimal, decimal[]> tuple in initVecList) {
                            sAnswer += tuple.Item1 + ": ( ";
                            foreach (decimal num in tuple.Item2)
                                sAnswer += num.ToString() + "  ";
                            sAnswer += ")" + Environment.NewLine;
                        }
                    }
                    catch (DivideByZeroException) {
                        sAnswer = "Ошибка: попытка деления на нуль в процессе расчетов. Попробуйте не оставлять нулей на главной диагонали._error_error";
                    }
                    catch (Exception ex) {
                        sAnswer = ex.Message + "_error_error";
                    }
                    bdata = Encoding.UTF8.GetBytes(sAnswer);
                    handler.Send(bdata);

                    // закрываем сокет

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    Console.WriteLine("Подключение закрыто" + Environment.NewLine);
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
