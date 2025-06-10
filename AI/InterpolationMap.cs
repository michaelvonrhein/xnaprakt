using System;
using System.Collections.Generic;
using System.Text;

namespace PraktWS0708.AI
{
    public class InterpolationMap
    {
        #region Fields

        private float maxInput;
        private float updateRate = 0.05f;      

        private float[] values;
        private int updateContext;

        #endregion

        #region Constructors

        public InterpolationMap(int length, float maxInput)
            : this(length, maxInput, 100.0f)
        {
        }

        public InterpolationMap(int length, float maxInput, float initialValue)
            : this(length, maxInput, initialValue, 0.05f, (int)(0.1f * length))
        {
        }

        public InterpolationMap(int length, float maxInput, float initialValue,
            float updateRate, int updateContext)
        {
            values = new float[length];

            this.maxInput = maxInput;
            this.updateRate = updateRate;
            this.updateContext = updateContext;

            for (int i = 0; i < length; ++i)
                values[i] = initialValue;
        }

        #endregion

        #region Properties

        public float this[float input]
        {
            get { return values[GetIndex(input)]; }
            set { values[GetIndex(input)] = value; }
        }

        public int Length
        {
            get { return values.Length; }
        }

        /// <summary>
        /// Gets or sets the maximum input value the InterpolationMap can hold.
        /// Changing this property may corrupt all learned values!
        /// </summary>
        public float MaxInput
        {
            get { return maxInput; }
            set { maxInput = value; }
        }

        /// <summary>
        /// Gets or sets the update rate. Setting this to zero will effectively
        /// disable the update capability.
        /// </summary>
        public float UpdateRate
        {
            get { return updateRate; }
            set { updateRate = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the InterpolationMap for a given input value.
        /// </summary>
        /// <param name="input">Input value</param>
        /// <param name="output">Output value</param>
        public void Update(float input, float output)
        {
            if (updateRate <= 0.0f)
                return;

            int index = GetIndex(input);

            int min = Math.Max(0, index - updateContext);
            int max = Math.Min(index + updateContext, values.Length - 1);

            for (int i = min; i <= max; ++i)
            {
                float rate = updateRate * (1.0f - Math.Abs(index - i) / (updateContext + 1));
                values[i] = (values[i] + rate * output) / (1.0f + rate);
            }
        }

        #endregion

        #region Private Methods

        private int GetIndex(float input)
        {
            int index = (int)((input / maxInput) * (values.Length - 1));
            return Math.Min(values.Length - 1, Math.Max(0, index));
        }

        #endregion
    }
}
