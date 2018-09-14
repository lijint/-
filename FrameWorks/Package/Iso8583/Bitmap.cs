using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Landi.FrameWorks.Iso8583
{
    [Serializable]
    public class Bitmap
    {
        //ISO 8583 �������������128
        internal const int FieldCount = 128;//128;
        private BitArray array;

        public Bitmap()
        {
            this.array = new BitArray(Bitmap.FieldCount);
        }

        public Bitmap(byte[] map)
        {
            Array.Reverse(map);
            this.array = new BitArray(map);
            Array.Reverse(map);
        }

        /// <summary>
        /// ��ȡһ��ֵ��ָʾλͼ�Ƿ���ȫ128�ֶεġ�
        /// </summary>
        public bool IsFull
        {
            get
            {
                byte[] thisMap = this.GetBytes(); //��ͨ��thisMap�ĳ������ж�
                for (int i = 0; i < thisMap.Length; i++)
                {
                    if (thisMap[i] != 255)
                        return false;
                }
                return true;
            }
        }

        public byte[] GetBytes()
        {
            byte[] map = new byte[16];
            this.array.CopyTo(map, 0);
            //���ֻ��64��
            if (!this.Get(1))
            {
                map = new byte[8];

                BitArray tmp = new BitArray(64);
                for (int i = 0; i < tmp.Count; i++)
                {
                    tmp.Set(i, this.array.Get(64 + i));
                }
                tmp.CopyTo(map, 0);
            }

            Array.Reverse(map);
            return map;
        }

        public bool Get(int bitNum)
        {
            //��0���޸�λͼ
            if (bitNum == 0)
            {
                return true;
            }

            return this.array.Get(FieldCount - bitNum);
        }

        public void Set(int bitNum, bool value)
        {
            //��0���޸�λͼ
            if (bitNum == 0)
            {
                return;
            }

            this.array.Set(FieldCount - bitNum, value);
        }

        public void CopyTo(Array array, int index)
        {
            byte[] map = this.GetBytes();
            map.CopyTo(array, index);
        }

        public bool IsEqual(byte[] map)
        {
            byte[] thisMap = this.GetBytes();
            if (thisMap.Length != map.Length)
                return false;
            for (int i = 0; i < map.Length; i++)
                if (map[i] != thisMap[i]) return false;

            return true;
        }
    }
}
