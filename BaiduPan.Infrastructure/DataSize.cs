using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Infrastructure
{
    /// <summary>
    ///  A data Structure is used to represent the size, and can be converted to be the
    ///  appropriate size for display.
    /// </summary>
    public struct DataSize : IEquatable<DataSize>
    {

        public const double ConversionFactor = 1024;

        private SizeUnitEnum _unitl;

        public long BaseBValue { get; }

        public double Value { get; private set; }

        public SizeUnitEnum Unit
        {
            get { return _unitl; }

            set
            {
                if (value == _unitl)
                    return;

                var loopNumber = value - _unitl;
                if (loopNumber > 0)
                {
                    for (int i = 0; i < loopNumber; i++)
                        Value /= ConversionFactor;
                }
                else
                {
                    loopNumber = -loopNumber;
                    for (int i = 0; i < loopNumber; i++)
                        Value *= ConversionFactor;
                }

                _unitl = value;
                
            }
        }


        public DataSize(long size)
        {

            BaseBValue = size;
            Value = size;
            _unitl = SizeUnitEnum.B;

            while (Value > ConversionFactor)
            {
                Value /= ConversionFactor;
                _unitl++;
            }

       
        }

        public DataSize(double size, SizeUnitEnum sizeUnit = SizeUnitEnum.B)
        {
            var temp = size;
            for (int i = 0;  i < sizeUnit - SizeUnitEnum.B; i++)
            {
                temp *= ConversionFactor;
            }

            BaseBValue = (long)temp;
            while (size >= ConversionFactor && sizeUnit < SizeUnitEnum.P)
            {
                size /= ConversionFactor;
                sizeUnit++;
            }

            Value = size;

         

            _unitl = sizeUnit;

        }


        public bool Equals(DataSize other)
        {
            return this == other;
        }


        public override string ToString()
        {
            return ($"{ Value:N2}")
                 + (Unit == SizeUnitEnum.B ? $"{Unit}" : $"{Unit}B");
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            
            return obj is DataSize && Equals((DataSize)obj);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)_unitl *397) ^ Value.GetHashCode();
            }
        }

        #region  Operators


        public static DataSize operator +(DataSize left, DataSize right)
        {
            return new DataSize(left.BaseBValue + right.BaseBValue);
        }


        public static DataSize operator +(DataSize left, long right)
        {
            return new DataSize(left.BaseBValue + right);
        }


        public static DataSize operator -(DataSize value)
        {
            return new DataSize(-value.BaseBValue);
        }

        public static DataSize operator -(DataSize left, DataSize right)
        {
            return left + -right;
        }


        public static DataSize operator -(DataSize left, long right)
        {
            return left + -right;
        }

        public static DataSize operator *(DataSize left, double right)
        {
            return new DataSize(left.BaseBValue * right);
        }


        public static DataSize operator /(DataSize left, double right)
        {
            return new DataSize(left.BaseBValue / right);
        }

        public static double operator /(DataSize left, DataSize right)
        {
            return 1.0 * left.BaseBValue / right.BaseBValue;
        }


        public static bool operator ==(DataSize left, DataSize right)
        {
            return left.BaseBValue == right.BaseBValue;
        }


        public static bool operator !=(DataSize left, DataSize right)
        {
            return !(left == right);
        }
        #endregion



    }
}
