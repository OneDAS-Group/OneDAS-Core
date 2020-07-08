using System;
using System.Text.RegularExpressions;

namespace OneDas.Infrastructure
{
#warning Replace everything with TimeSpan ("SamplePeriod")? Timespan bases on 100 ns = 10 MHz. Better would be a wrapper, which would allow to lower this limit.

    public class SampleRateContainer
    {
        #region Fields

        ulong _samplesPerDay;

        #endregion

        #region Constructors

        public SampleRateContainer(ulong samplesPerDay, bool ensureNonZeroIntegerHz = false)
        {
            this.SamplesPerDay = samplesPerDay;

            if (ensureNonZeroIntegerHz)
                this.CheckNonZeroIntegerHz();
        }

        public SampleRateContainer(SampleRate sampleRate)
        {
            this.SamplesPerDay = 86400UL * (100 / (ulong)sampleRate);
            this.IsNonZeroIntegerHz = true;
        }

        public SampleRateContainer(string sampleRateWithUnit, bool ensureNonZeroIntegerHz = false)
        {
            // Hz
            var matchHz = Regex.Match(sampleRateWithUnit, @"([0-9|\.]+)\sHz");

            if (matchHz.Success)
            {
                this.SamplesPerDay = 86400UL * ulong.Parse(matchHz.Groups[1].Value);

                if (ensureNonZeroIntegerHz)
                    this.CheckNonZeroIntegerHz();

                return;
            }

            // s
            var matchT = Regex.Match(sampleRateWithUnit, @"([0-9|\.]+)\ss");

            if (matchT.Success)
            {
                this.SamplesPerDay = 86400UL / ulong.Parse(matchT.Groups[1].Value);

                if (ensureNonZeroIntegerHz)
                    this.CheckNonZeroIntegerHz();

                return;
            }

            // else
            throw new ArgumentException(nameof(sampleRateWithUnit));
        }

        #endregion

        #region Properties

        public ulong SamplesPerDay 
        {
            get
            {
                return _samplesPerDay;
            }
            private set
            {
                if (value == 0)
                    throw new Exception("A sample rate of '0' is not allowed.");

                _samplesPerDay = value;
            }
        }

        public decimal SamplesPerSecond => _samplesPerDay / 86400m;

        public ulong SamplesPerSecondAsUInt64
        {
            get
            {
                if (!this.IsNonZeroIntegerHz)
                    throw new Exception($"Only positive non-zero integer frequencies are supported, but the actual frequeny is '{this.SamplesPerSecond}' samples per second.");
                else
                    return _samplesPerDay / 86400;
            }
        }

        public TimeSpan Period => TimeSpan.FromSeconds((double)(1/this.SamplesPerSecond));

        public ulong NativeSampleRateFactor => 100 / (ulong)this.SamplesPerSecond;

        public bool IsNonZeroIntegerHz { get; private set; }

        #endregion

        #region Methods

        private void CheckNonZeroIntegerHz()
        {
            var value = _samplesPerDay / (double)86400;
            this.IsNonZeroIntegerHz = (value % 1 == 0) && (value >= 1);

            if (!this.IsNonZeroIntegerHz)
            {
                throw new Exception("The provided sample rate is not a non-zero integer frequency.");
            }
        }

        public override bool Equals(object obj)
        {
            var other = (SampleRateContainer)obj;

            if (other == null)
                return false;

            return this.SamplesPerDay == other.SamplesPerDay;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SamplesPerDay);
        }

        public string ToUnitString(bool underscore = false)
        {
            var frequency = this.SamplesPerDay / 86400.0;
            var period = 86400.0 / this.SamplesPerDay;
            var fillChar = underscore ? '_' : ' ';

            if (frequency > 1)
                return $"{frequency:0.##}{fillChar}Hz";
            else
                return $"{period:0.##}{fillChar}s";
        }

        #endregion
    }
}