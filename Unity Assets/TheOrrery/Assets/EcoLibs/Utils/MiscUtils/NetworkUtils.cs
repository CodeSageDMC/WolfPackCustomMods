// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Client.Utils
{
    using UnityEngine;

    public static class NetworkUtils
    {
        /*
         * Simple float2byte convertion section - pack 4byte float to a single byte
         */

        const float dividerUnsigned = 1f / 255f; //We use a 255 step float values to pack unsigned [0..1] float into 8 bits. Actually only 100 steps is enough already

        /// <summary>Pack unsigned normilized float into 1 byte (values [0..1])</summary>
        public static byte  FloatToByteUnsigned(float value) => (byte)(Mathf.Clamp(value / dividerUnsigned, 0f, 255f));
        /// <summary>Unpack unsigned normilized float from 1 byte (values [0..1])</summary>
        public static float ByteToFloatUnsigned(byte value)  => (float)((float)value * dividerUnsigned);

        const float dividerSigned = 1f / 127f; //We use a 127 step float values to pack signed float [-1..1] into 8 bits: 7 bits + 1 bit sign. (sbyte is [-128, 127])

        /// <summary>Pack signed normilized float into 1 byte (values [-1..1]). Less precision than unsigned.</summary>
        public static byte  FloatToByteSigned(float value)   => (byte)(Mathf.Clamp(value / dividerSigned, -127f, 127f));

        /// <summary>Unpack signed normilized float from 1 byte (values [-1..1]). Less precision than unsigned.</summary>
        public static float ByteToFloatSigned(byte value)    => (float)((sbyte)value * dividerSigned);

        //Quaternion compression to 32 bits section
        const int bitsPerChannel = 10;     //Bits per channel + 2 bits for fourth channel
        const int bitMask = 0x3FF;         //Bit mask for given number of bits per channel
        const float bound = 0.707107f;     //Lowest possible value for non-max component
        const float range = bound * 2;     //Range between max and min
        const float steps = 1024;          //10 bits per channel
        const float divider = range / steps; //Divider for math

        /// <summary>Pack Unity quaternion into 4 bytes in integer form. 10 bits per each channel.</summary>
        public static int QuaternionToInt(Quaternion q)
        {
            //Quaternion compression to 32 bits. Algorithm described here: https://gafferongames.com/post/snapshot_compression/
            //1. Get the index [0..3] of largest absolute value component and pack it with 2 bits
            //2. 2nd largest component cannot be bigger than 0.707107, so we quantize in range [-0.707, +0.707]
            //3. Instead of adding sign bit, just add 0.707 and then substract it when unpacking
            //4. Pack 3 non-largest components in order with 10 bits by stepping range [0, +1.414] (as we got rid of negative part)
            //5. We don't need to pack 4th component as it value will always be following, i.e. if w is largest then: w = sqrt(1 - x^2 - y^2 - z^2)

            //Find max component - we will not send it as it could be restored from other 3 components
            byte maxIndex = 0;
            var maxValue = Mathf.Abs(q[0]);
            for (byte i = 1; i < 4; i++)
                if (Mathf.Abs(q[i]) > maxValue)
                {
                    maxValue = Mathf.Abs(q[i]);
                    maxIndex = i;
                }

            //Negate whole quaternion if max abs value is negative (to get rid of 4th conponent sign)
            if (q[maxIndex] < 0)
                q = new Quaternion(-q.x, -q.y, -q.z, -q.w);

            int result = 0;

            for (byte i = 0, cIndex = 0; i < 4; i++) //cIndex is a component index
            {
                if (i == maxIndex) continue;

                var comp = (int)((q[i] + bound) / divider); //Do not keep sign, just add lower bound

                result |= comp << (bitsPerChannel * cIndex++);
            }

            result |= maxIndex << (bitsPerChannel * 3); //Max index is a 2-bit value

            return result;
        }

        /// <summary>Unpack Unity quaternion from 4 bytes in integer form (previously packed by QuaternionToInt). 10 bits per each channel.</summary>
        public static Quaternion IntToQuaternion(int q)
        {
            var result = new Quaternion();

            //Read largest component index to skip it when reading
            var maxIndex = (q >> (bitsPerChannel * 3)) & 0b11; //Take two bits

            var sqrSum = 1f;

            for (byte i = 0, cIndex = 0; i < 4; i++)
            {
                if (i == maxIndex) continue;

                //Get needed bits and convert back to float
                var rawValue = (q >> (bitsPerChannel * cIndex++) & bitMask);
                var c = rawValue * divider - bound;
                result[i] = c;

                sqrSum -= c * c; //Substract component's sqr to get 4th component later
            }

            //Finally, get the last component
            result[maxIndex] = Mathf.Sqrt(sqrSum);

            return result.normalized; //Normalize as we cound have errors while quantizing
        }
    }
}
