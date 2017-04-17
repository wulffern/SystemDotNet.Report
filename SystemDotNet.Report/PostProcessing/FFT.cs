using System;
using NextGenLab.Chart;
using System.Collections.Generic;

namespace SystemDotNet.PostProcessing
{
    /// <summary>
    /// Summary description for FFT.
    /// </summary>
    public class FFT
    {
        public FFT()
        {
        }

        public Complex[] Run(double[] f)
        {
            if (f != null)
            {
                int val = 0;
                int i = 0;
                while ((val = (int)Math.Pow(2, i++)) < f.Length) ;

                if (val != f.Length)
                {
                    double[] fa = new double[val];
                    Array.Copy(f, fa, f.Length);
                    f = fa;
                }

            }

            double[] f1 = new double[2 * f.Length];
            for (int i = 0; i < f.Length; i++)
            {
                f1[2 * i] = f[i];
                f1[2 * i + 1] = 0.0;
            }
            return Run(f1, f.Length, 1, 1);
        }

        Complex[] Run(double[] f, int N, int skip, int forward)
        {


            int b, index1, index2, trans_size, trans;
            double pi2 = 4.0 * Math.Asin(1.0);
            double pi2n, cospi2n, sinpi2n; /* Used in recursive formulas for Re(W^b) and Im(W^b) */

            Complex wb;
            Complex temp1, temp2;

            Complex[] c = new Complex[f.Length / 2];
            for (int i = 0; i < c.Length; i++)
            {
                c[i] = new Complex(f[2 * i], f[2 * i + 1]);
            }

            for (index1 = 1, index2 = 0; index1 < N; index1++)
            {
                for (b = N / 2; index2 >= b; b /= 2)
                    index2 -= b;

                index2 += b; /* Next replace the first 0 in index2 with a 1 and this gives the correct next value */
                if (index2 > index1)
                {
                    temp1 = c[index2 * skip];
                    c[index2 * skip] = c[index1 * skip];
                    c[index1 * skip] = temp1;
                }
            }


            for (trans_size = 2; trans_size <= N; trans_size *= 2)
            {
                pi2n = forward * pi2 / (double)trans_size; /* +- 2 pi/trans_size */
                cospi2n = Math.Cos(pi2n); /* Used to calculate W^k in D-L formula */
                sinpi2n = Math.Sin(pi2n);
                wb.Real = 1.0; /* Initialize W^b for b=0 */
                wb.Imag = 0.0;
                for (b = 0; b < trans_size / 2; b++)
                {
                    for (trans = 0; trans < N / trans_size; trans++)
                    {
                        index1 = (trans * trans_size + b) * skip; /* Index of element in first half of transform being computed */
                        index2 = index1 + trans_size / 2 * skip; /* Index of element in second half of transform being computed */
                        temp1 = c[index1];
                        temp2 = c[index2];
                        c[index1].Real = temp1.Real + wb.Real * temp2.Real - wb.Imag * temp2.Imag; /* Implement D-L formula */
                        c[index1].Imag = temp1.Imag + wb.Real * temp2.Imag + wb.Imag * temp2.Real;
                        c[index2].Real = temp1.Real - wb.Real * temp2.Real + wb.Imag * temp2.Imag;
                        c[index2].Imag = temp1.Imag - wb.Real * temp2.Imag - wb.Imag * temp2.Real;
                    }
                    temp1 = wb;
                    wb.Real = cospi2n * temp1.Real - sinpi2n * temp1.Imag; /* Real part of e^(2 pi i b/trans_size) used in D-L formula */
                    wb.Imag = cospi2n * temp1.Imag + sinpi2n * temp1.Real; /* Imaginary part of e^(2 pi i b/trans_size) used in D-L formula */
                }
            }
            if (forward < 0.0)
                for (index1 = 0; index1 < skip * N; index1 += skip)
                {
                    c[index1].Real /= N;
                    c[index1].Imag /= N;
                }

            return c;
        }

        public Complex[] DoFFT(double[] d, WindowBase window)
        {
            double M = d.Length;
            double[] dwindow = window.Calc((int)M);

            ArrayMath.Multiply(ref d, ref dwindow);
            Complex[] fft = Run(d);
            Complex[] spec = new Complex[fft.Length / 2];

            for (int i = 1; i < fft.Length / 2 + 1; i++)
            {
                spec[i - 1].Real = fft[i].Real * 2 / M;
                spec[i - 1].Imag = fft[i].Imag * 2 / M;
            }
            return spec;
        }

        public Complex[] DoFFT(List<double> d, WindowBase window)
        {
            double[] dd = d.ToArray();
            return DoFFT(dd,window);
        }

        public void PowerSpectralDensity(double[] d, out ChartData cd, WindowBase window, double fs)
        {
            PowerSpectralDensity(d, out cd, window, fs, double.NaN);
           
        }

        public void PowerSpectralDensity(double[] d, out ChartData cd, WindowBase window, double fs,double maxSignalAmplitude)
        {
            int M = d.Length;
            double[] y = PowerSpectralDensity(d, window, fs,maxSignalAmplitude);

            double[] freq = new double[y.Length];
            for (int i = 0; i < freq.Length; i++)
            {
                freq[i] = (double)i * fs / (double)M;
            }

            //PowerSpectralDensity(d, out x, window);
            cd = ChartData.GetInstance();
            cd.AutoScale = true;
            cd.AxisLabelX = "[Hz]";
            cd.AxisLabelY = "[dB]";
            cd.AxisTypeX = AxisType.LIN;
            cd.AxisTypeY = AxisType.LIN;
            cd.ChartType = ChartType.Curve;
            cd.ShowZero = false;
            cd.Title = "Power Spectral Density";
            cd.TitlesY = new string[] { "Magnitude" };
            cd.TitleX = "Frequency";
            cd.X = freq;
            cd.Y = new double[][] { y };

        }

        public double[] PowerSpectralDensity(double[] d, WindowBase window, double fs)
        {
         
            return PowerSpectralDensity(d,window,fs,double.NaN);
        }

        public double[] PowerSpectralDensity(double[] d, WindowBase window, double fs,double maxSignalAmpl )
        {
            Complex[] spec = DoFFT(d, window);

            double[] y = new double[spec.Length];
            double max = Double.MinValue;
            for (int i = 0; i < spec.Length; i++)
            {
                y[i] = 20 * Math.Log10(spec[i].Abs);
                if (y[i] > max)
                    max = y[i];
            }


            if (double.NaN.CompareTo(maxSignalAmpl) == 0)
            {
                ArrayMath.Add(ref y, -max);
            }
            else
            {
                max = 20 * Math.Log10(maxSignalAmpl / 2);
                ArrayMath.Add(ref y, -max);
            }

           
            return y;
        }

        public double Sndr(double[] d, WindowBase window, double fs, double f0,double fmin, double fmax)
        {
            return SignalNoise(d, window, fs, f0, 0, 3, 1,fmin,fmax);
        }

        public double Sndr(double[] d, WindowBase window, double fs, double f0, int OSR)
        {

            double fmax = fs / (2 * OSR);
            return SignalNoise(d, window, fs, f0, 0, 3, 1, -1, fmax);
        }

        public double Sndr(double[] d, WindowBase window, double fs, double f0)
        {
            return SignalNoise(d, window, fs, f0, 0, 3, 1);
        }

        public double Snr(double[] d, WindowBase window, double fs, double f0)
        {
            return SignalNoise(d, window, fs, f0, 6, 3, 1);
        }

        public double Snr(double[] d, WindowBase window, double fs, double f0,double fmin, double fmax)
        {
            return SignalNoise(d, window, fs, f0, 6, 3, 1,fmin,fmax);
        }

        public double Snr(double[] d, WindowBase window, double fs, double f0, int OSR)
        {
            double fmax = fs / (2 * OSR);
            return SignalNoise(d, window, fs, f0, 6, 3, 1, -1, fmax);
        }


        public void DynamicParameters(List<double> d, WindowBase window, 
            double fs, double f0, double fmin, double fmax, bool remove_dc,out double snr,out double sndr, out double enob)
        {

            double[] dd = new double[d.Count];

            double dc_offset = 0;
            if (remove_dc)
                dc_offset = ArrayMath.Sum(d.ToArray()) / d.Count;

            if (dc_offset != 0)
            {
                for (int i = 0; i < d.Count; i++)
                {
                    dd[i] = d[i] - dc_offset;
                }
            }

            Complex[] spec = DoFFT(dd, window);
            snr = SignalNoise(spec,dd.Length, fs, f0, 6, 3, fmin, fmax);
            sndr = SignalNoise(spec,dd.Length, fs, f0, 0, 3, fmin, fmax);
            enob = (sndr - 1.76) / 6.02;

        }

        double SignalNoise(double[] d, WindowBase window, double fs, double f0, int excludeharmoics, int filterwidth, double signalamplitude)
        {
            return SignalNoise(d, window, fs, f0, excludeharmoics, filterwidth, signalamplitude, -1, -1);
        }

        double SignalNoise(Complex[] spec,int samples, double fs, double f0, int excludeharmoics, int filterwidth, double fmin, double fmax)
        {
            int M = samples;

            //Find signal index
            int signal = 0;
            signal = (int)Math.Floor(f0 / fs * M);


            //Find  fmin an fmax index
            int fmini = 1;
            if (fmin >= 0)
                fmini = (int)Math.Floor(fmin / fs * M);
            int fmaxi = spec.Length;
            if (fmax > 0)
                fmaxi = (int)Math.Floor(fmax / fs * M);

            //Find harmonics index
            int[] harmonics = new int[excludeharmoics];
            for (int i = 2; i < harmonics.Length; i++)
            {
                harmonics[i] = (int)Math.Floor((f0 * i) / fs * M);
            }

            //Calculate noise power
            double noise = 0;
            bool isharmonic;
            for (int i = fmini; i < fmaxi; i++)
            {
                if (i >= (signal - filterwidth) && i <= (signal + filterwidth))
                    continue;

                isharmonic = false;
                for (int j = 0; j < harmonics.Length; j++)
                {
                    if (i >= (harmonics[j] - filterwidth) && i <= (harmonics[j] + filterwidth))
                    {
                        isharmonic = true;
                        break;
                    }
                }
                if (isharmonic)
                    continue;

                noise += Math.Pow(spec[i].Abs, 2);
            }

            //Calculate signal power
            double dsig = 0;
            for (int i = signal - filterwidth; i < signal + filterwidth + 1; i++)
            {
                if (i > 0 && i < spec.Length)
                {
                    //amp /= signalamplitude;
                    dsig += Math.Pow(spec[i].Abs, 2);

                }
            }

            double bw = f0 / fs;
            double noisetopower = 10 * Math.Log10(dsig / noise);
            return noisetopower;
        }

        double SignalNoise(double[] d, WindowBase window, double fs, double f0, 
            int excludeharmoics, int filterwidth, double signalamplitude, double fmin, double fmax)
        {
            Complex[] spec = DoFFT(d, window);
            
            return SignalNoise(spec,d.Length,fs,f0,excludeharmoics,filterwidth,fmin,fmax);
        }


    }
}
