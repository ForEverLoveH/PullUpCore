using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamerADCore.GameSystem.GameModel
{
    public class SerialMessageTran
    {
        public byte[] btAryTranData;  //完整数据包
        public int btAryTranDataLen; //数据包长度
        public byte[] btCheckAryData = new byte[7];//收到结束成绩后的校验码
        public byte strCmd = 0x00;
        public List<string> ints0 = new List<string>();
        public List<string> ints1 = new List<string>();
        public int strTime = 0;
        public TimeSpan timeSpan;

        public StringBuilder handleData = new StringBuilder();

        //记录开始设备
        public List<int> ints = new List<int>();

        public bool RecEndFlag = true;

        public SerialMessageTran(byte[] btTranData, int status = 0)
        {
            this.btAryTranData = btTranData;
            this.btAryTranDataLen = btTranData.Length;
            strCmd = btTranData[0];
            if (status == 0 && btAryTranData[0] == 0xFE)
            {
                qStartMessageTran();
            }
            else if (status == 1 && btAryTranData[0] == 0xFF)
            {
                qEndMessageTran();
            }
        }

        public void qStartMessageTran()
        {
            /*
            FE 00 06 00 1F F1 00 00 20 F1 01 FF
            // fe 头码
            第2，3位   00 06  时间
            第4，5位   00 1f  分数1
            第6，7位   F1 00  道号01
            第8，9位   00 20  分数2
            第10，11位 F1 01  道号02
            结束位 ：ff
            */
            ints0.Clear();
            ints.Clear();
            if (btAryTranDataLen == 0) return;
            int bt0 = btAryTranData[1];
            int bt1 = btAryTranData[2];
            if (bt0 > 16)
            {
                bt0 -= 16;
                strTime += 16 << 8;
                strTime += bt0 << 12;
            }
            else
            {
                strTime += bt0 << 8;
            }
            strTime += bt1;
            timeSpan = new TimeSpan(0, 0, strTime);
            int loop = 3;
            while (btAryTranDataLen - loop > 4)
            {
                byte[] temp = new byte[4];
                Array.Copy(btAryTranData, loop, temp, 0, 4);
                int score16 = temp[0];
                int score = score16 * 256 + temp[1];
                if (score > 0)
                {
                    ints.Add(1);
                    ints0.Add(score + "");
                }
                else
                {
                    ints.Add(0);
                    ints0.Add("0");
                }
                loop += 4;
            }
        }

        public void qEndMessageTran()
        {
            ints1.Clear();
            //主机发送成绩：FF  00 07 94 05 33   00 10     01         00 02      00 01   00 07 93 64 44    00 11      01         00 02      00 02   FF
            //////////////头码     ID卡号5位    分数两位  01固定模式  时间两位   手柄号       ID卡号5位     分数两位   01固定模式    时间两位    手柄号  结尾码

            /*  for (int i = 1; i < btAryTranDataLen; i += 12)
              {
                 int iis = i + 5;
                 if (iis > btAryTranDataLen) { break; }
                 int score16 = Convert.ToInt32(btAryTranData[iis].ToString("X2"));
                 //int score160 = btAryTranData[iis];
                 iis++;
                 int score0 = Convert.ToInt32(btAryTranData[iis].ToString("X2"));
                 //int score0 = btAryTranData[iis];
                 int score = score16 * 100 + score0;

                 ints1.Add(score + "");
              }*/

            for (int i = 1; i < btAryTranDataLen; i += 12)
            {
                byte[] temp = new byte[12];
                if (i + 12 > btAryTranDataLen) { break; }
                Array.Copy(btAryTranData, i, temp, 0, 12);
                //秒
                int secScore = temp[5] * 256 + temp[6];
                if (secScore > 0)
                {
                    ints1.Add(secScore + "");
                }
                else
                {
                    ints1.Add(0 + "");
                }

            }
        }

        public static byte CheckSum(byte[] btAryBuffer, int nStartPos, int nLen)
        {
            byte btSum = 0x00;

            for (int nloop = nStartPos; nloop < nStartPos + nLen; nloop++)
            {
                btSum ^= btAryBuffer[nloop];
            }
            return btSum;
        }
    }
}

