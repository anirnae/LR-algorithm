using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SocketTcpServer
{
    class Matrix : ICloneable
    {
        int n;
        decimal[,] data;

        public Matrix() {
            n = 3;
            data = new decimal[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    data[i, j] = 0;
        }
        public Matrix(decimal num) {
            n = 3;
            data = new decimal[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    if (i == j)
                        data[i, j] = num;
                    else data[i, j] = 0;
        }
        public Matrix(decimal[,] data) {
            //проверка?
            n = 3;
            this.data = new decimal[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    this.data[i, j] = data[i, j];
        }
        public int Dimension {
            get { return n; }
        }
        public decimal Determinant {
            get {
                return this[0, 0] * this[1, 1] * this[2, 2] +
                    this[2, 0] * this[0, 1] * this[1, 2] +
                    this[1, 0] * this[2, 1] * this[0, 2] -
                    this[2, 0] * this[1, 1] * this[0, 2] -
                    this[0, 0] * this[2, 1] * this[1, 2] -
                    this[1, 0] * this[0, 1] * this[2, 2];
            }
        }

        public object Clone() {
            decimal[,] data = new decimal[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    data[i, j] = this.data[i, j];
            return new Matrix(data);
        }

        public decimal this[int i, int j] {
            get { return data[i, j]; }
            set { data[i, j] = value; }
        }

        public Dictionary<char, Matrix> GetLU() {
            Matrix L = new Matrix();
            Matrix U = (Matrix)this.Clone();

            for (int i = 0; i < n; i++)
                L[i, i] = 1;

            for (int k = 0; k < n - 1; k++) {
                for (int i = k + 1; i < n; i++) {
                    L[i, k] = U[i, k] / U[k, k];
                    for (int j = k; j < n; j++)
                        U[i, j] -= L[i, k] * U[k, j];
                }
            }
            return new Dictionary<char, Matrix> {
                {'L', L },
                {'U', U }
            };
        }
        public Matrix Multiply(Matrix M) {
            decimal[,] data = new decimal[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    for (int k = 0; k < n; k++)
                        data[i, j] += this[i, k] * M[k, j];

            return new Matrix(data);
        }
        public decimal[] Multiply(decimal[] v) {
            decimal[] res = new decimal[n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    res[i] += Math.Round(this[i, j] * v[j], 2);
            return res;
        }
        public Matrix Minus(Matrix M) {
            Matrix res = (Matrix)this.Clone();
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    res[i, j] -= M[i, j];
            return res;
        }
        public List<Tuple<decimal, decimal[]>> GetEigenVectors() {
            //Matrix res = new Matrix();
            var resList = new List<Tuple<decimal, decimal[]>>();
            decimal x1, x2, x3;

            var ev = this[2, 2];
            Matrix M = this.Minus(new Matrix(ev));
            x3 = 1;

            if (M[1, 2] == 0)
                x2 = 0;
            else if (M[1, 1] == 0) {
                x3 = 0;
                x2 = 1;
            }
            else
                x2 = -M[1, 2] * x3 / M[1, 1];

            if (M[0, 0] == 0) {
                x1 = 1;
                x2 = 0;
            }
            else
                x1 = (-M[0, 2] * x3 - M[0, 1] * x2) / M[0, 0];

            decimal[] v = new decimal[] { x1, x2, x3 };
            var tuple = new Tuple<decimal, decimal[]>(ev, v);
            resList.Add(tuple);

            ev = this[1, 1];
            M = this.Minus(new Matrix(ev));

            x3 = 0;
            x2 = 1;
            if (M[0, 0] == 0 && M[0, 1] != 0) {
                x2 = 0;
                x1 = 1;
            }
            else if (M[0, 0] == 0) {
                x1 = 0;
            }
            else x1 = -M[0, 1] * x2 / M[0, 0];

            v = new decimal[] { x1, x2, x3 };
            tuple = new Tuple<decimal, decimal[]>(ev, v);
            resList.Add(tuple);

            ev = this[0, 0];
            M = this.Minus(new Matrix(ev));

            x3 = 0;
            x2 = 0;
            x1 = 1;

            v = new decimal[] { x1, x2, x3 };
            tuple = new Tuple<decimal, decimal[]>(ev, v);
            resList.Add(tuple);

            return resList;
        }
    }
}