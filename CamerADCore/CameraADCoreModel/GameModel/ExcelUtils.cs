using MiniExcelLibs;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace CameraADCoreModel.GameModel
{
    public class ExcelUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="value"></param>
        public static void MiniExcel_OutPutExcel(string path, object value)
        {
            MiniExcel.SaveAs(path, value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ldic"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool OutPutExcel(List<Dictionary<string, string>> ldic, string path)
        {
            bool result = false;
            if (ldic.Count == 0) return result;

            HSSFWorkbook workbook = new HSSFWorkbook();
            HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet("Sheet1");//����һ������ΪSheet0�ı�;
            HSSFRow row = (HSSFRow)sheet.CreateRow(0);//����һ��д����)
            Dictionary<string, string> dict = ldic[0];
            int coloum = 0;
            foreach (var key in dict.Keys)
            {
                row.CreateCell(coloum).SetCellValue(key);//��һ�б��⣬
                coloum++;
            }
            for (int i = 0; i < ldic.Count; i++)
            {
                row = (HSSFRow)sheet.CreateRow(i + 1);
                coloum = 0;
                dict = ldic[i];
                foreach (var key in dict.Keys)
                {
                    row.CreateCell(coloum).SetCellValue(dict[key]);//��һ�б��⣬
                    coloum++;
                }
            }
            //�ļ�д���λ��
            using (FileStream fs = File.OpenWrite(path))
            {
                workbook.Write(fs);//��򿪵����xls�ļ���д������  
                result = true;
            }
            return result;
        }
    }
}